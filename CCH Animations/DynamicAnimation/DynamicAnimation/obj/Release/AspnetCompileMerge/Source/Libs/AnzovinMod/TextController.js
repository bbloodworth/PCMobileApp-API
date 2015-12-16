/*! TextController.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// Constructor. This class is responsible for managing text fields in the
// animation. This primarily (currently) comprises of two things. The first is
// applying text fixing translations, as the default Canvas text output is
// apparently always some number of pixels off of the correct position. The
// second is to overlay HTML form input fields on top of the animation for
// text fields that are editable by the user.
anzovinmod.TextController = function() {
  // This reference object (like the next one) has a two-level approach:
  // The first index is the scene name. The difference ends there, as this
  // will contain all of the text elements in the scene in its entirety, so as
  // to not require re-parsing the entire scene in order to find them again.
  this._allTextElements = {};
  // The main reference object. This is a list of the createjs.Text elements
  // that are to contain html form inputs. It is an object because the first
  // level index is the scene name. This lets us more easily manage many
  // scenes worth of Text elements, and improve performance when dealing
  // with Text elements from multiple scenes by only requiring comparisons
  // and iterations on a subset of the Text elements instead of all of them
  // on the stage.
  // The Text elements themselves will have two additional on-object properties
  // that we define to help keep track of the references to the HTML input
  // entities and the DOMElement instance attached to each one:
  // {_textFormInputElement}: A reference to the actual HTML input element.
  // {_textFormInputElementDOM}: A reference to the DOMElement entity that may
  // or may not be attached to the Text's parent entity.
  this._textInputs = {};
  // This reference object contains a list of all of the createjs.Text nodes
  // on given scenes that have edited e.y coordinates. These will be looked at
  // on every frame of the animation for changed e.y coordinates, because they
  // have been modified by the CreateJS animations and need to be re-adjusted
  // in order to line up properly with the graphics.
  this._animatedTextNodes_y = {};
  // The HTML form element to use for managing text input in the canvas.
  this._textForm = null;
  // The HTML form will contain a wrapper DIV element that itself will contain
  // the inputs. This is a reference to that DIV element.
  this._textFormDiv = null;
  // The defined width of the stage.
  this._width = 1920;
  // The defined height of the stage.
  this._height = 1080;
  // The current scale of the stage.
  this._scale = 1;
};

// Sets the size of the form element. The size is different from the currently
// displayed scale: The size should always match the canvas element's HTML
// attribute width/height, and not its CSS. If the displayed width of the
// canvas element changes, call setScale() instead of this function. Only call
// this function once during initialization, and if the stage size changes.
// 
// width: The new desired width.
// height: The new desierd height.
anzovinmod.TextController.prototype.setSize = function(width, height) {
  width = anzovinmod.Utilities.defaultParam(width, null);
  height = anzovinmod.Utilities.defaultParam(height, null);
  if (width == null || height == null) {
    return;
  }
  this._width = width;
  this._height = height;
  this._writeSizesToCss();
};

// Sets the scale of the form element to be that of the value provided. This is
// used when resizing the display of the canvas element. The canvas element
// scales itself accordingly depending on the difference between its HTML
// attribute width and its CSS width, but the HTML form needs to have its scale
// set manually.
// 
// Since the form has an overflow style but also a transform, you don't need to
// change the size to change the transform scale, while still having the correct
// overflow behavior. Changing the scale alone is sufficient.
// 
// Note that this function will attempt to set the scale in multiple different
// ways, depending on if the browser engine supports one particular scaling
// method or another. Eg, -moz-transform.
// 
// scale: A floating point value that is the scale to set. A value less than
// one reduces the size of the form.
anzovinmod.TextController.prototype.setScale = function(scale) {
  scale = anzovinmod.Utilities.defaultParam(scale, null);
  if (scale == null) {
    return;
  }
  this._scale = scale;
  this._writeSizesToCss();
};

// Just like the individual setSize() and setScale() functions, this one just
// does them both at the same time. If width or height are null, then they are
// not changed. If scale is null then it is not changed.
// 
// width: The new desired width.
// height: The new desired height.
// scale: The new desired scale.
anzovinmod.TextController.prototype.setSizeScale = function(width, height, scale) {
  width = anzovinmod.Utilities.defaultParam(width, null);
  height = anzovinmod.Utilities.defaultParam(height, null);
  scale = anzovinmod.Utilities.defaultParam(scale, null);
  if (width != null && height != null) {
    this._width = width;
    this._height = height;
  }
  if (scale != null) {
    this._scale = scale;
  }
  this._writeSizesToCss();
};

// This function just takes the width/height/scale definitions and writes the
// appropriate CSS to the appropriate form element or wrappers. It only changes
// the CSS of the form element or children of it, not any parent elements.
anzovinmod.TextController.prototype._writeSizesToCss = function() {
  if (this._textFormDiv != null) {
    this._textFormDiv.style.transform = "scale(" + this._scale + ")";
    this._textFormDiv.style.MozTransform = "scale(" + this._scale + ")";
    this._textFormDiv.style.WebkitTransform = "scale(" + this._scale + ")";
    this._textFormDiv.style.OTransform = "scale(" + this._scale + ")";
    this._textFormDiv.style.MSTransform = "scale(" + this._scale + ")";
  }
};

// Initializes the "_textInputs" and "_allTextElements" reference objects with
// createjs.Text elements from the given timeline instance. All the scenes on
// the main timeline are parsed.
// 
// Note that this can be called after adding new scenes to the mtl and calling
// this function again. It will not parse the same scene name multiple times,
// as the text inputs on scenes are indepentent.
// 
// mtl: A reference to the main timeline instance object.
// scene: The specific scene that is being searched. This is a cjs movieclip
// instance.
anzovinmod.TextController.prototype.initTextNodesFromMtl = function(mtl, scene) {
  mtl = anzovinmod.Utilities.defaultParam(mtl, null);
  scene = anzovinmod.Utilities.defaultParam(scene, null);
  if (mtl == null) return;
  if (scene == null) return;
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.TextController.initTextNodesFromMtl", "initializing text nodes for scene ", scene.name);
  // skip this scene if it is already parsed
  if (this._textInputs.hasOwnProperty(scene.name)) return;
  // find all text nodes on the scene
  var nodes = this._findTextNodesOnScene(mtl, scene.name);
  this._textInputs[scene.name] = [];
  this._allTextElements[scene.name] = [];
  for (var j = 0; j < nodes.length; ++j) {
    var node = nodes[j];
    this._allTextElements[scene.name].push(node);
    var nodeName = node.name;
    if (nodeName != null && nodeName.length > 6 && nodeName.lastIndexOf("_input") == nodeName.length - 6) {
      this._textInputs[scene.name].push(node);
    }
  }
  this._createTextFormInputs(scene.name);
  this.hideSceneElements(scene.name);
};

// Attempts to find all of a timeline's createjs.Text elements that are to have
// HTML text inputs associated with them. Note that this may at first return
// all the createjs.Text elements from a stage, but would be extended to
// eventually return only those text elements that have a custom name or
// property or other identifier that indicates they are to have html text
// inputs associated with them.
// 
// Note that this will return all Text elements, not just those that are
// currently visible on the stage, but all of them. This is for several
// reasons. First of all, this allows all of the text elements to be found
// initially upon the first stage load, simplifying the search process so that
// we do not have to search through all elements all the time. Otherwise, we
// would have to search through every element both on and off the stage all
// the time.
// 
// mtl: A reference to the main timeline instance object.
// scene: The name of the scene to search for elements of.
// 
// Returns: An array of createjs.Text elements on the given scene, that are to
// be managed for HTML input entities.
anzovinmod.TextController.prototype._findTextNodesOnScene = function(mtl, scene) {
  mtl = anzovinmod.Utilities.defaultParam(mtl, null);
  scene = anzovinmod.Utilities.defaultParam(scene, null);
  var ret = [];
  if (mtl == null || scene == null) {
    return ret;
  }
  var elements = mtl.findChildren(scene + ".**.*", true);
  for (var i = 0; i < elements.length; ++i) {
    if (elements[i] instanceof createjs.Text) {
      ret.push(elements[i]);
    }
  }
  return ret;
};

// Creates the basic text input form for the timeline, if not already created.
// The form is referenced directly by this instance, so there is no need to
// keep track of IDs or names of the form.
// 
// Any necessary CSS or positioning of the form itself is initialized here.
// The input elements themselves are added elsewhere.
// 
// The new form should be placed as a child of the div wrapper. For best results
// the canvas element should be placed inside of this form wrapper. This is
// because of issues with z-indexing and positioning the canvas and form
// elements as siblings: Things work much better when the canvas element is
// a sibling of the text input elements.
// 
// In short, create a Div element, create a Form, create a Canvas.
// 
// Returns: The HTML Form element that should be added to the div wrapper.
anzovinmod.TextController.prototype.createGetTextForm = function() {
  if (this._textForm != null) {
    return this._textForm;
  }
  this._textForm = document.createElement("form");
  this._textForm.setAttribute("method", "post");
  this._textForm.setAttribute("action", "");
  this._textFormDiv = document.createElement("div");
  this._textFormDiv.setAttribute("class", "formInputs");
  this._textForm.appendChild(this._textFormDiv);
  this._writeSizesToCss();
  return this._textForm;
};

// Creates a text form input for each of the text fields present on the stage.
// They are not positioned or sized here. They are only created, given the
// appropriate justification, hidden by default, then placed on the page.
// 
// This can be called multiple times if the stage changes. As long as the
// canvas text elements still have their custom properties attached to them,
// this function will not create duplicate input elements.
// 
// A separate function, to handle positioning, sizing, and text values of the
// elements, will need to be used elsewhere and called frequently to ensure that
// the text elements are correctly handling frame-by-frame changes to the stage.
// 
// scene: The scene name to create html input elements form.
anzovinmod.TextController.prototype._createTextFormInputs = function(scene) {
  scene = anzovinmod.Utilities.defaultParam(scene, null);
  if (scene == null) {
    return;
  }
  if (this._textForm == null) {
    return;
  }
  if (this._textFormDiv == null) {
    return;
  }
  if (!this._textInputs.hasOwnProperty(scene)) {
    return;
  }
  for (var i = 0; i < this._textInputs[scene].length; ++i) {
    var e = this._textInputs[scene][i];
    // skip if there is an html input already created for this text node
    if (e.hasOwnProperty("_textFormInputElement")) {
      continue;
    }
    // create element
    var newInputElement = document.createElement("input");
    // set font
    newInputElement.style.font = e.font;
    // set color
    newInputElement.style.color = e.color;
    // text align
    switch (e.textAlign) {
      case "left":
      case "start":
        newInputElement.style.textAlign = "left";
        break;
      case "right":
      case "end":
        newInputElement.style.textAlign = "right";
        break;
      case "center":
        newInputElement.style.textAlign = "center";
        break;
    }
    // content
    newInputElement.value = e.text;
    // make invisible until added
    newInputElement.style.display = "none";
    // make canvas element hidden
    e.visible = false;
    // add to form-div
    this._textFormDiv.appendChild(newInputElement);
    // add as custom properties for tracking
    e["_textFormInputElement"] = newInputElement;
    e["_textFormInputElementDOM"] = new createjs.DOMElement(newInputElement);
  }
};

// This function hides all the HTML input elements from the given scene. This
// is important as a scene transition would have the html input elements still
// be visible after the transition unless they are manually made hidden.
// 
// Note that a possible optimization in this function, could be to make the
// html input elements be children of DIVs, and to just show/hide the Div
// element when a scene goes on/off the stage.
// 
// scene: The name of the scene to hide the elements of.
anzovinmod.TextController.prototype.hideSceneElements = function(scene) {
  scene = anzovinmod.Utilities.defaultParam(scene, null);
  if (scene == null) {
    return;
  }
  if (!this._textInputs.hasOwnProperty(scene)) {
    return;
  }
  for (var i = 0; i < this._textInputs[scene].length; ++i) {
    var e = this._textInputs[scene][i];
    if (!e.hasOwnProperty("_textFormInputElement")) {
      continue;
    }
    var htmlInput = e["_textFormInputElement"];
    if (htmlInput.style.display != "none") {
      htmlInput.style.display = "none";
    }
  }
};

// This function is called once per frame of the stage animation. It is
// responsible for handling any sort of logic that needs to be handled on
// createjs.Text elements per frame, such as ensuring that the text
// position fix is in-place.
// 
// Note that this method uses a different array than the all-text-elements
// array to parse through all the nodes to parse. This is because after the
// first pass, text nodes that are not animated do not need to be checked again,
// and only those that are animated need to be checked. Additionally, text
// nodes that are animated are reset on every frame, so this can be used to
// ensure that we do not waste processing cycles by only checking those
// text nodes that we know may change.
// 
// We might add something to the CreateJS library to make it not set the y
// translation on text nodes that have not changed since last, which would
// make this function only necessary on text nodes that actually animate
// when they need to change their transformation, instead of all the time.
// That is not for now though.
// 
// scene: The name of the scene to manage the elements of.
// removeUnmodifiedElements: Default True. If True, then unmodified text nodes
// are removed from the list of text nodes to keep track of. If False, then
// text nodes will remain in the list.
anzovinmod.TextController.prototype.allTextFrameUpdate = function(scene, removeUnmodifiedElements) {
  scene = anzovinmod.Utilities.defaultParam(scene, null);
  removeUnmodifiedElements = anzovinmod.Utilities.defaultParam(removeUnmodifiedElements, true, true);
  if (scene == null) {
    return;
  }
  // if we haven't done this yet, then just use a copy of the 'all elements'
  // array to initialize our list
  if (!this._animatedTextNodes_y.hasOwnProperty(scene)) {
    if (!this._allTextElements.hasOwnProperty(scene)) {
      return;
    }
    this._animatedTextNodes_y[scene] = anzovinmod.Utilities.cloneArray(this._allTextElements[scene], false);
  }
  var nodes = this._animatedTextNodes_y[scene];
  var numberOfTextNodes = nodes.length;
  for (var i = 0; i < numberOfTextNodes; ++i) {
    var e = nodes[i];
    if (!this._frameNodeFixCjsTextPosition(e) && removeUnmodifiedElements) {
      nodes[i--] = nodes[--numberOfTextNodes];
    }
  }
  if (numberOfTextNodes != nodes.length) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.TextController.allTextFrameUpdate", "reducing animated text nodes from ", nodes.length, " to ", numberOfTextNodes);
    this._animatedTextNodes_y[scene] = anzovinmod.Utilities.cloneArray(nodes, false, numberOfTextNodes);
  }
};

// This function is called once per frame of the stage animation. It does
// anything that needs or may need to be done on each frame for HTML input
// text elements, such as watching out for adjustments and changed
// transformation matrices to apply to the HTML form elements, etc.
// 
// The first thing about this function, is that it only operates on a single
// scene's worth of createjs.Text nodes at once. This is to avoid having to
// process nodes that are obviously not visible.
// 
// scene: The name of the scene to manage the elements of.
// onSceneParent: A MovieClip or Container object that, if a Text node's parent
// history contains this element, treate the node as being on the scene, and
// hence visible. Otherwise, the node will be treated as invisible.
anzovinmod.TextController.prototype.inputTextFrameUpdate = function(scene, onSceneParent) {
  scene = anzovinmod.Utilities.defaultParam(scene, null);
  onSceneParent = anzovinmod.Utilities.defaultParam(onSceneParent, null);
  if (scene == null) {
    return;
  }
  if (!this._textInputs.hasOwnProperty(scene)) {
    return;
  }
  var es = this._textInputs[scene];
  for (var i = 0; i < es.length; ++i) {
    var e = es[i];
    // Skip nodes that do not have an HTML form input element associated.
    if (!e.hasOwnProperty("_textFormInputElement") || e["_textFormInputElement"] == null) {
      continue;
    }
    // Ensure that the node is supposed to be visible. If it is supposed to be
    // hidden, then make sure it is hidden and return "true". Otherwise, return
    // "false" but do not yet make it visible. We'll do that at the end to help
    // prevent any amount of jitter.
    if (this._frameNodeHideIfNotOnStage(e, onSceneParent)) {
      continue;
    }
    // Ensure that the node's DOMElement is either attached to the stage or to
    // the node's parent.
    this._frameNodeEnsureDomeIsAttachedToParent(e);
    // Adjust HTML input size/position if necessary.
    this._frameNodeAdjustHtmlInputSizeAndPosition(e);
    // Make sure the HTML text node is visible.
    this._frameNodeEnsureIsVisible(e);
  }
};

// Checks the parent history of e and, if the given scene parent is not found,
// hide the text node and return true to signify that the node is indeed hidden,
// or was already hidden. The node is not made visible if it should be visible,
// it is only hidden if it should be hidden.
// 
// e: The text node to check.
// onSceneParent: The node to check in the history to indicate that e is
// supposed to be visible.
// 
// Returns: True if the node is is hidden or is supposed to be hidden. False if
// e is supposed to be visible.
anzovinmod.TextController.prototype._frameNodeHideIfNotOnStage = function(e, onSceneParent) {
  var textElement = e["_textFormInputElement"];
  var parent = e.parent;
  while (parent != null && parent != onSceneParent) {
    parent = parent.parent;
  }
  if (parent == null || parent != onSceneParent) {
    if (textElement.style.display != "none") {
      textElement.style.display = "none";
    }
    return true;
  }
  return false;
};

// Checks the parent of the text node e and, if it does not have the node e's
// associated DOMElement attached, then attach it to the parent. As this
// function would only need to be called if the text node e is attached to
// an object or on the stage, it assumes that the parent exists and is valid.
// 
// e: The text node to check.
anzovinmod.TextController.prototype._frameNodeEnsureDomeIsAttachedToParent = function(e) {
  var domElement = e["_textFormInputElementDOM"];
  if (e.parent.children.indexOf(domElement) == -1) {
    e.parent.addChild(domElement);
    // fix newly added HTML form input transformation matrices
    this._frameNodeManuallySetHtmlInputTransformationMatrix(e);
  }
};

// Fixes the text positions of the given createjs.Text element. This
// is necessary to obtain a more pixel-perfect representation of the Canvas
// project across devices and from the output of Flash Pro. The default text
// rendering is offset by some degree.
// 
// Note that this function can detect when the position is changed versus when
// it has already been fixed, so it will only re-fix a node if it has been
// itself re-adjusted by the stage or animation.
// 
// e: The text node to check for and fix position of.
// 
// Returns: True if the position of the given node is changed, false if it
// is not changed.
anzovinmod.TextController.prototype._frameNodeFixCjsTextPosition = function(e) {
  // "Number" type means that it hasn't changed since last being "fixed"
  if (e.y instanceof Number) {
    return false;
  }
  var eSize = e.font.split(" ")[0];
  var eSizeNum = 0;
  if (eSize.indexOf("px") > 0) {
    eSizeNum = eSize.slice(0, eSize.indexOf("px"));
    eSizeNum = eSizeNum / 10;
  } else if (eSize.indexOf("pt") > 0) {
    eSizeNum = eSize.slice(0, eSize.indexOf("pt"));
    eSizeNum = (eSizeNum * 4 / 3) / 10;
  }
  e.y = new Number(e.y + 2 + eSizeNum);
  return true;
};

// Adjust the HTML Input element's size (CSS width/height) and position
// (CSS top/left) to match that of the Text node e. This is necessary to
// take into account offsets or animations of the text node.
// 
// e: The text node to adjust associated HTML input size and position of.
anzovinmod.TextController.prototype._frameNodeAdjustHtmlInputSizeAndPosition = function(e) {
  // Notes on Sizes & Positions:
  // 
  // lineWidth and lineHeight are not necessarily appropriate to use in
  // all cases. They just control when the text is supposed to wrap. We also
  // can't just blindly use the lineWidth or lineHeight values without regard
  // to other potential values. There are also many other properties that
  // relate to sizing text elements. This section outlines some of the values
  // that could be used to determine sizes of elements.
  // 
  // --lineWidth: Can be used as a good default value for width. It is defined
  // as the width before text is split to fit across multiple lines, so it is
  // either equal to or larger than the size of text that is displayed in Text
  // elements. This is one of the most likely variables that, if set, is
  // set as an explicit value, eg most likely to be the actual width of the
  // Text element from Flash Pro. This value can be null, in which case text
  // will not wrap at all. Another value will need to be used in this case.
  // Of note, is that text that is longer than this may not wrap if there are
  // no spaces in the text: It will continue outside of these bounds until
  // a breaking character is found.
  // 
  // --maxWidth: The maximum width of a text element. If the text would be
  // wider than this, then the text is compressed horizontally to fit into
  // the defined size limits. This is analogous to After Effects' OpenSesame's
  // "WIDTH #" format specifier. This could be used in place of lineWidth
  // if it is specified, as both place a limit on the maximum horizontal size
  // of an element. Unlike lineWidth though, this value is more aggressive
  // and text will be compressed regardless of the presence of line-breaking
  // spaces or other characters. Of additional note, is that this compression
  // is done per-line. A two-line text element may have the first line
  // compressed while the second line is displayed normally.
  // 
  // --getMeasuredWidth(): The width in pixels that the original, non
  // transformed text, displays as. Unlike getMeasuredHeight(), this value
  // does not take into account line breaks, regardless of whether they are
  // explicitly included via "\n" or implicit via lineWidth values. For
  // single lines of text, then this value could be used to determine the
  // displayed width of text elements.
  // 
  // --lineHeight: The height difference between two lines of text. If set to
  // a number, then when text is wrapped, subsequent lines appear this
  // distance below the first line. If set to zero or null, then the value of
  // getMeasuredHeight() is used instead, which itself performs a calculation
  // based on the font face in use to determine an appropriate value.
  // 
  // --getMeasuredHeight(): The height in pixels that the original, non
  // transformed text, displays as. On standard, non transformed text fields,
  // this is either very closely equal to or exactly equal to the value of
  // lineHeight times the number of lines in the text. If lineHeight is not
  // available, then the value is approximated through calculations on the
  // font face. This value includes any inherit line breaks caused by
  // lineWidth wrapping text. This value can usually be used in determining
  // the displayed height of text elements.
  // 
  // --getMeasuredLineHeight(): An approximation of the height of the font
  // face in question. This is based on the ratio between em and height, and
  // can give a much more accurate definition for the height of a line of text.
  // It should be noted however, that for fixed-width fonts this value can be
  // grossly inaccurate, as it is a static calculation based on the value of em
  // and does not do any actual glyph rendering to determine the actual line
  // height size.
  // 
  // --getBounds(): Returns the width, height, x, and y positions of the
  // element. This can be used for many different drawn elements, but is
  // also supported by Text elements. The values returned by this are the
  // actual display bounds, and are not representative of any possible limits
  // on these positions. Eg, the width component may be smaller than the
  // lineWidth or maxWidth values, and represent only the actual physical
  // width of the actual text being displayed.
  // 
  // Note that the object returned by getBounds() is not persistant, and
  // either the object or its properties should be copied if persistance is
  // needed on these values.
  // 
  // Conclusions:
  // 
  // The most concrete width definition can be obtained via maxWidth or
  // lineWidth. Prefer maxWidth first and lineWidth second, as maxWidth (if
  // defined) is the "absolute" maximum width of an element, while lineWidth
  // is more of a guideline and can be breached. If both of these are
  // unavailable, then getBounds().width will need to be used to determine
  // the current displayed width of an element. getMeasuredWidth() should be
  // used last, as it does not take into account explicit line breaks in the
  // displayed text, though for single-line elements this is not a problem and
  // getMeasuredWidth() should be equivalent to getBounds().width.
  // 
  // The most concrete height definition can be obtained via
  // getMeasuredLineHeight(), as it takes into account the actual size of the
  // font and doesn't mess around with baseline height differences. However,
  // this does not work entirely well for fixed-width fonts, in which case
  // something like getBounds().height or getMeasuredHeight() should be used.
  // These should be equivalent, as they both take into account line breaks in
  // the text. Just use the value that is already easily obtainable, eg if you
  // are already using getBounds() for its width property, then you might as
  // well use it for its height property.
  // 
  // As for the X and Y coordinates of the text elements, the values to use
  // depends on the text alignment and which of the width/height values were
  // used.
  // 
  // Of note first, is that the properties x and y of a text element
  // represent the point of interest of the text element dependent on the
  // text alignment. If the horizontal alignment is left, then the x
  // coordinate is that of the left bounds of the text element. If the
  // alignment is centered, then the x coordinate is that of the midpoint
  // of the text element. And if the alignment is right justified, then the
  // x coordinate is that of the right side of the text element.
  // 
  // getBounds() also has an x and y coordinate value. These values, unlike
  // the text elements', are not affected by justification, and instead
  // mearly represent the offset from the text element's x and y coordinates
  // that the rendered text begins at. For left justified text, x is therefore
  // zero. For center justified text, x is negative one-half the width
  // property of getBounds(). And for right justified text, x is just
  // negative its width property.
  var textElement = e["_textFormInputElement"];
  var left, top, width, height;
  var bounds = null;
  if (e.maxWidth != null) {
    width = e.maxWidth;
  } else if (e.lineWidth != null) {
    width = e.lineWidth;
  } else {
    bounds = e.getBounds();
    if (bounds != null) {
      width = bounds.width;
    } else {
      width = e.getMeasuredWidth();
    }
  }
  if (bounds != null) {
    height = bounds.height;
  } else {
    height = e.getMeasuredLineHeight();
  }
  // left-right
  if (bounds != null) {
    left = e.x + bounds.x;
  } else {
    switch (e.textAlign) {
      case "right":
      case "end":
        left = e.x - width;
        break;
      case "center":
        left = e.x - width/2;
        break;
      case "left":
      case "start":
      default:
        left = e.x;
        break;
    }
  }
  // top-bottom
  if (bounds != null) {
    top = e.y + bounds.y;
  } else {
    switch (e.textBaseline) {
      case "bottom":
      case "ideographic":
      case "alphabetic":
        top = e.y - height;
        break;
      case "middle":
        top = e.y - height/2;
        break;
      case "top":
      case "hanging":
      default:
        top = e.y;
        break;
    }
  }
  // adjustments
  top = top - 2; // -2 ff, +0 ie, probably other browser-specific adjustments
  // finalize written values
  left = left + "px";
  top = top + "px";
  width = width + "px";
  height = height + "px";
  // only adjust if changed
  if (textElement.style.left != left) {
    textElement.style.left = left;
  }
  if (textElement.style.top != top) {
    textElement.style.top = top;
  }
  if (textElement.style.width != width) {
    textElement.style.width = width;
  }
  if (textElement.style.height != height) {
    textElement.style.height = height;
  }
};

// Simply makes sure that the Text node's associated HTML element has the
// necessary CSS in place to make it visible.
// 
// e: The text node to ensure is visible.
anzovinmod.TextController.prototype._frameNodeEnsureIsVisible = function(e) {
  var textElement = e["_textFormInputElement"];
  if (textElement.style.display != "") {
    textElement.style.display = "";
  }
};

// createjs.DOMElements that are just added to a parent have the
// appropriate matrices applied to them, but these are not forwarded to
// the corresponding HTML form input until the next frame of animation.
// This forces the transformation matrix to be applied to the HTML input
// until the next animation frame can do it itself manually from then on.
// 
// e: The text node that had its DOMElement attached to the parent.
anzovinmod.TextController.prototype._frameNodeManuallySetHtmlInputTransformationMatrix = function(e) {
  // Transform Matrix
  // The transform matrix (CSS transform:matrix(a,b,c,d,tx,ty);) is added to
  // each HTML form element node automatically by the stage on each frame of
  // changed animation. It can be retrieved from the corresponding DOMElement by
  // calling its getConcatenatedMatrix() function. This returns a matrix that
  // represents the transformation matrices all the way to the root stage.
  // This is different from getMatrix(), which just returns the transformation
  // matrix of the single element.
  // 
  // Note, that you do not need to clone the returned matrix object. That
  // is only for transform bounds. Not passing an argument to the getMatrix()
  // and getConcatenatedMatrix() functions just returns a new object instance.
  // Passing in an object instance will write values to the object.
  var textElement = e["_textFormInputElement"];
  var domElement = e["_textFormInputElementDOM"];
  var domMatrix = domElement.getConcatenatedMatrix();
  var matrixString = anzovinmod.Utilities.cssTransformMatrix2D(domMatrix);
  textElement.style.transform = matrixString;
  textElement.style.MozTransform = matrixString;
  textElement.style.WebkitTransform = matrixString;
  textElement.style.OTransform = matrixString;
  textElement.style.MsTransform = matrixString;
};

}());
