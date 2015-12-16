/*! rangeslider.js, based off of 1.2.1, remove jquery requirement, add comments */
var rangeslider;
(function(){
"use strict";

// Constructor. Takes an HTML 'input' element, creates a replacement
// rangeslider, and plugs it in to the DOM in its place.
// 
// element: The element that represents the HTML 'input' element that is to
// be "converted" into this rangeslider class.
// options: An object containing options for this rangeslider instance. Read
// the documentation for information on options.
rangeslider = function(element, options) {
  // A handle to the main window object.
  this._window = window;
  // A handle to the main document.
  this._document = document;
  // A reference to the HTML 'input' element that was passed in.
  this._htmlInputElement = element;
  // All options, including the defaults plus any from the 'options' parameter
  // that override any defaults.
  this._options = {
    "polyfill": true,
    "rangeClass": "rangeslider",
    "disabledClass": "rangeslider--disabled",
    "extraSelectClass": "rangeslider__extraselect",
    "fillClass": "rangeslider__fill",
    "handleClass": "rangeslider__handle",
    "startEvent": ["mousedown", "touchstart", "pointerdown"],
    "moveEvent": ["mousemove", "touchmove", "pointermove"],
    "endEvent": ["mouseup", "touchend", "pointerup"]
  };
  for (var k in options) {
    if (!options.hasOwnProperty(k)) continue;
    this._options[k] = options[k];
  }
  
  // If this instance is supposed to be used as a polyfill, then do nothing
  // and simply return right now. (polyfill == replacement if not available)
  if (this._options["polyfill"] && rangeslider.supportsRange()) {
    return false;
  }
  
  // An identifier for this unique rangeslider instance.
  this._rangesliderIdentifier = "js-rangeslider-" + (++rangeslider._identifiers);
  
  // Values attached to the HTML input element, or defaults if they are not
  // attached. Or, values related to these values. (Note that these || short-
  // circuit selectors do work, but only because element.getAttribute() is
  // returning strings, so "0" || 100 yields "0" and not 100.)
  // Note that values set here may be modified before being used to set the
  // values of the rangeslider instance. For example, the width of the
  // rangeslider needs to be calculated before the position value can be set.
  // Other notes: 'position' is the X coordinate of the handle that represents
  // the 'value' of the rangeslider. When 'value==min', 'position==0', and
  // when 'value==max', 'position==maxHandleX'. 'handleLeft' is a slight
  // addition to the 'position' value to take into account half the width of the
  // handle, and 'maxHandleX' takes into account the full width of the handle.
  //   min       value                   max
  //    |          |                      |
  // |..|----------|----------------------|..|
  //    |          |                      |
  //    0       position             maxHandleX
  // |--| handleLeft
  this._values = {};
  this._loadBaseInputAttributes(); // min, max, value, step
  this._values["position"] = 0;
  this._values["handleLeft"] = 0;
  this._values["maxHandleX"] = 0;
  this._values["toFixed"] = (this._values["step"] + '').replace('.', '').length - 1;
  
  // Div elements that are used to place and handle the actual rangeslider
  // instance.
  this._divExtraSelect = document.createElement("div");
  this._divExtraSelect.setAttribute("class", this._options["extraSelectClass"]);
  this._divFill = document.createElement("div");
  this._divFill.setAttribute("class", this._options["fillClass"]);
  this._divHandle = document.createElement("div");
  this._divHandle.setAttribute("class", this._options["handleClass"]);
  this._divRange = document.createElement("div");
  this._divRange.setAttribute("class", this._options["rangeClass"]);
  this._divRange.setAttribute("id", this._rangesliderIdentifier);
  this._divRange.appendChild(this._divExtraSelect);
  this._divRange.appendChild(this._divFill);
  this._divRange.appendChild(this._divHandle);
  rangeslider._insertAfter(this._divRange, element);
  
  // Hide the existing element, since we are replacing it with our own
  // rangeslider. (Should this be at the end, after the positioning of the
  // new rangeslider element?)
  element.style.position = "absolute";
  element.style.width = "1px";
  element.style.height = "1px";
  element.style.overflow = "hidden";
  element.style.opacity = "0";
  element.style.visibility = "hidden";
  
  // Function bindings, binding function declarations to 'this' state so they
  // can interact with the correct rangeslider instance.
  var _this = this;
  this._functionBindings = {
    "down": function() {_this._handleDown.apply(_this, arguments);},
    "move": function() {_this._handleMove.apply(_this, arguments);},
    "end": function() {_this._handleEnd.apply(_this, arguments);}
  };
  
  // If there was a custom 'onInit' function in the passed options, then
  // call it.
  if (this._options["onInit"]) {
    this._options["onInit"]();
  }
  
  // First update, initialize dimensions, position, value, left, maxHandleX,
  // etc.
  this.update();
  
  // Attach main mouse-down event.
  for (var i = 0; i < this._options["startEvent"].length; ++i) {
    rangeslider._addEvent(this._divRange, this._options["startEvent"][i], this._functionBindings["down"]);
  }
  
  // Attach resize event
  var _this = this;
  rangeslider._addEvent(this._window, "resize", rangeslider._debounce(
    function() {
      _this.update();
    }, 200
  ));
};

// Loads the base input attributes (min, max, value, step) from the input
// element attached to this rangeslider instance.
rangeslider.prototype._loadBaseInputAttributes = function() {
  this._values["min"] = parseFloat(this._htmlInputElement.getAttribute("min") || 0);
  this._values["max"] = parseFloat(this._htmlInputElement.getAttribute("max") || 100);
  if (isNaN(this._values["max"])) this._values["max"] = 100;
  this._values["value"] = parseFloat(this._htmlInputElement.value || (this._values["min"] + (this._values["max"] - this._values["min"]) / 2));
  if (this._values["value"] < this._values["min"]) this._values["value"] = this._values["min"];
  if (this._values["value"] > this._values["max"]) this._values["value"] = this._values["max"];
  this._values["step"] = parseFloat(this._htmlInputElement.getAttribute("step") || 1);
};

// Performs a positional and configuration update on the rangeslider instance.
// This includes getting its bounding box positions and setting its size
// accordingly.
// 
// The current 'value' of the rangeslider, stored internally, is used to
// update the display and position of the rangeslider. Dimensions are
// recalculated if necessary, in order to account for situations where
// the size of the rangeslider has changed.
// 
// updateAttributes: Default false. If true, then update the base attributes
// from the input element, including: min, max, value, step.
rangeslider.prototype.update = function(updateAttributes) {
  if (updateAttributes) {
    this._loadBaseInputAttributes();
  }
  // set up/update instance-local properties
  var handleWidth = rangeslider._getDimension(this._divHandle, "offsetWidth");
  var rangeWidth = rangeslider._getDimension(this._divRange, "offsetWidth");
  this._values["maxHandleX"] = rangeWidth - handleWidth;
  this._values["handleLeft"] = handleWidth / 2;
  this._values["position"] = this._getPositionFromValue(this._values["value"]);
  // add/remove 'disabled' class state
  if (this._htmlInputElement.disabled) {
    rangeslider._addClass(this._divRange, this._options["disabledClass"]);
  } else {
    rangeslider._removeClass(this._divRange, this._options["disabledClass"]);
  }
  // set final position
  this.setPosition(this._values["position"]);
};

// Gets the position of the range slider instance's handle from its current
// value. This can be used in particular, to determine where the position of the
// handle needs to go, when the rangeslider is adjusted (eg size) and the
// previous calculation of the handle position needs to be re-done.
// 
// We don't need to worry about 'step' here, as stepping only applies to the
// value, not the returned position. Other rangeslider values (such as
// min/max) are used to calculate the position.
// 
// value: The current "value" of the rangeslider instance, eg between min/max,
// etc.
// 
// Returns: The "position" that the handle should be at on the rangeslider in
// order for it to equal the indicated value.
rangeslider.prototype._getPositionFromValue = function(value) {
  var percent = (value - this._values["min"]) / (this._values["max"] - this._values["min"]);
  return percent * this._values["maxHandleX"];
};

// Gets the value of the range slider at the given handle position. This is
// really just an inverse function. Note that the original version of this
// function had a bunch of odd match and toFixed code that didn't seem to
// really do all that much, for any particular reason.
// 
// We need to account for 'step' here, as the value needs to be snapped to the
// nearest step value. Other rangeslider values (such as min/max) are used
// to calculate the value.
// 
// position: The handle position.
// 
// Returns: The "value" that the handle indicates at the desired position.
rangeslider.prototype._getValueFromPosition = function(position) {
  // percentage of the position, where 0 == 0% and maxHandleX = 100%
  var percentage = 0;
  if (this._values["maxHandleX"] != 0) {
    percentage = position / (this._values["maxHandleX"]);
  }
  // the number of 'steps', which the rounding is for, so we can get
  // an exact value. if == 0, then just don't take that into account
  var steps = percentage * (this._values["max"] - this._values["min"]);
  if (this._values["step"] != 0) {
    steps = Math.round(steps / this._values["step"]) * this._values["step"];
  }
  // final value. toFixed makes the output have the same number of significant
  // digits as the 'steps' value, so for steps of "0.01", the output will have
  // two decimal digits, etc
  var value = this._values["min"] + steps;
  return Number((value).toFixed(this._values["toFixed"]));
};

// Sets the current position of the range slider instance's handle. Also updates
// the fill and handle positions/widths. The position is capped at the min/max
// position (which is just 0 and maxHandleX), so any position value less than
// the minimum is just set to the minimum, etc. The position value is also
// "snapped" to its nearest 'value' value, taking into account any nearby 'step'
// points.
// 
// Note that the input position value does not have the "handleLeft" adjustment
// added to it. That adjustment is only applied to specific elements in specific
// circumstances as the layout requires.
// 
// position: The position to set.
// alsoValue: Defaults false. If true, then also set the value based on the
// calculations from the position.
rangeslider.prototype.setPosition = function(position, alsoValue) {
  position = rangeslider._cap(position, 0, this._values["maxHandleX"]);
  // calculate the value, then re-calculate the position from that value, so
  // that we "snap" the position to the nearest allowable step
  var value = this._getValueFromPosition(position);
  position = this._getPositionFromValue(value);
  // update ui
  this._divFill.style.width = (position + this._values["handleLeft"]) + "px";
  this._divHandle.style.left = position + "px";
  // update instance values and send any necessary events
  this._values["position"] = position;
  if (alsoValue) {
    this._values["value"] = value;
  }
  // trigger 'onSlide' constructor function
  if (this._options["onSlide"]) {
    this._options["onSlide"](this._values["position"], this._values["value"]);
  }
};

// Sets the value of the rangeslider to the indicated value, then executes any
// attached "input" events. The value is capped to the min/max values of the
// rangeslider instance. The value is also "snapped" to its nearest 'value'
// value, taking into account any nearby 'step' points.
// 
// value: The value to set to the rangeslider element.
// alsoPosition: Defaults false. If true, then also set the position based on
// the calculations from the value.
rangeslider.prototype.setValue = function(value, alsoPosition) {
  value = rangeslider._cap(value, this._values["min"], this._values["max"]);
  // snap value position to the nearest allowable step
  var position = this._getPositionFromValue(value);
  value = this._getValueFromPosition(position);
  // set value and send any necessary events
  this._values["value"] = value;
  this._htmlInputElement.value = value;
  if (alsoPosition) {
    this._values["position"] = position;
    this._divFill.style.width = (position + this._values["handleLeft"]) + "px";
    this._divHandle.style.left = position + "px";
  }
  // execute "input" events here, origin:this.identifier
  // this.$element.val(value).trigger('input',{origin:this.identifier});
};

// Gets the relative position of the given event from the main rangeslider
// element.
// 
// event: A mouse click, or touch event, that has coordinates of where the
// event occurred.
// 
// Returns: An object of "x" and "y" properties that are the x and y coordinate
// of the relative position, to the main rangeslider div element.
rangeslider.prototype._getRelativePosition = function(event) {
  var range = {
    "x": this._divRange.getBoundingClientRect().left,
    "y": this._divRange.getBoundingClientRect().top
  };
  var page = {"x":0, "y":0};
  if (typeof event.pageX !== "undefined") {
    page = {
      "x": event.pageX,
      "y": event.pageY
    };
  } else if (event.originalEvent && typeof event.originalEvent.clientX !== "undefined") {
    page = {
      "x": event.originalEvent.clientX,
      "y": event.originalEvent.clientY
    };
  } else if (event.originalEvent && event.originalEvent.touches && event.originalEvent.touches[0] && typeof event.originalEvent.touches[0].clientX !== "undefined") {
    page = {
      "x": event.originalEvent.touches[0].clientX,
      "y": event.originalEvent.touches[0].clientY
    };
  } else if (event.currentPoint && typeof event.currentPoint.x !== "undefined") {
    page = {
      "x": event.currentPoint.x,
      "y": event.currentPoint.y
    };
  } else if (event.clientX && event.clientY) {
    page = {
      "x": event.clientX,
      "y": event.clientY
    };
  }
  return {
    "x": page["x"] - range["x"],
    "y": page["y"] - range["y"]
  };
};

// Function that is called when the handle is "grabbed" by an event. It doesn't
// immediately trigger anything, but sets up the handleEnd event capture.
// 
// event: An event, usually either mouse or touch related, that at least has
// coordinates for where the event occurred.
rangeslider.prototype._handleDown = function(event) {
  if (event.preventDefault) event.preventDefault(); else event.returnValue = false;
  // add doc events
  for (var i = 0; i < this._options["moveEvent"].length; ++i) {
    rangeslider._addEvent(this._document, this._options["moveEvent"][i], this._functionBindings["move"]);
  }
  for (var i = 0; i < this._options["endEvent"].length; ++i) {
    rangeslider._addEvent(this._document, this._options["endEvent"][i], this._functionBindings["end"]);
  }
  // if it was the handle that was clicked on, don't immediately set the
  // position, instead wait for the next movement to set the position. just
  // a slightly better ux
  //if ((' ' + event.target.className + ' ').replace(/[\n\t]/g, ' ').indexOf(this._options["handleClass"]) > -1) {
    //return;
  //}
  var relativePosition = this._getRelativePosition(event);
  this.setPosition(relativePosition["x"] - this._values["handleLeft"], true);
};

// Function that is called when the handle is moved by an event. It triggers
// the updating of the position and value of the rangeslider.
// 
// event: An event, usually either mouse or touch related, that at least has
// coordinates for where the event occurred.
rangeslider.prototype._handleMove = function(event) {
  if (event.preventDefault) event.preventDefault(); else event.returnValue = false;
  var relativePosition = this._getRelativePosition(event);
  this.setPosition(relativePosition["x"] - this._values["handleLeft"], true);
};

// Function that is called when the handle is "let go" by an event. It triggers
// the 'onSlideEnd' event passed in to the original constructor to this
// rangeslider.
// 
// event: An event, usually either mouse or touch related, that at least has
// coordinates for where the event occurred.
rangeslider.prototype._handleEnd = function(event) {
  if (event.preventDefault) event.preventDefault(); else event.returnValue = false;
  // remove doc events
  for (var i = 0; i < this._options["moveEvent"].length; ++i) {
    rangeslider._removeEvent(this._document, this._options["moveEvent"][i], this._functionBindings["move"]);
  }
  for (var i = 0; i < this._options["endEvent"].length; ++i) {
    rangeslider._removeEvent(this._document, this._options["endEvent"][i], this._functionBindings["end"]);
  }
  // call options-defined function if available
  if (this._options["onSlideEnd"]) {
    this._options["onSlideEnd"](this._values["position"], this._values["value"]);
  }
};

// Just adds the given event to the given object.
// 
// obj: Object to add event to.
// event: Event to add.
// f: Function to call on event.
rangeslider._addEvent = function(obj, event, f) {
  if (obj.addEventListener) {
    obj.addEventListener(event, f);
  } else if (obj.attachEvent) {
    obj.attachEvent("on" + event, f);
  }
};

// Just removes the given event from the given object.
// 
// obj: Object to remove event from.
// event: Event to remove.
// f: Function to remove from event.
rangeslider._removeEvent = function(obj, event, f) {
  if (obj.removeEventListener) {
    obj.removeEventListener(event, f);
  } else if (obj.detachEvent) {
    obj.detachEvent("on" + event, f);
  }
};

// Determines if the current browser supports HTML 'input' elements with the
// 'range' type.
// 
// Returns: True if the browser supports range, false if it does not.
rangeslider.supportsRange = function() {
  if (rangeslider._supportsRangeValue === null) {
    rangeslider._supportsRangeValue = rangeslider._supportsRangeFunction();
  }
  return rangeslider._supportsRangeValue;
};

// The last calculated supportsRange() result, so that it does not need to be
// recalculated on each desired check.
rangeslider._supportsRangeValue = null;

// Determines if the current browser supports HTML 'input' elements with the
// 'range' type.
// 
// Returns: True if the browser supports range, false if it does not.
rangeslider._supportsRangeFunction = function() {
  var i = document.createElement("input");
  i.setAttribute("type", "range");
  return i.type !== "text";
};

// A unique identifier for the number of rangesliders that have been created
// by this class. Also doubles as the number of created rangesliders.
rangeslider._identifiers = 0;

// A helper function to insert one DOM element after a second DOM element, eg
// adding one element as the immediate sibling of another.
// 
// existingElement: The existing DOM element.
// newElement: The new DOM element to insert just after the 'existingElement'.
rangeslider._insertAfter = function(newElement, existingElement) {
  var parent = existingElement.parentNode;
  if (parent.lastchild == existingElement) {
    parent.appendChild(newElement);
  } else {
    parent.insertBefore(newElement, existingElement.nextSibling);
  }
};

// A helper for adding a class to an HTML DOM element. The class name is not
// added if already present.
// 
// element: The HTML element to modify the class of.
// name: The class to add to the element.
rangeslider._addClass = function(element, name) {
  var classes = element.getAttribute("class").split(" ");
  for (var i = 0; i < classes.length; ++i) {
    if (classes[i] == name) {
      return;
    }
  }
  classes.push(name);
  element.setAttribute("class", classes.join(" "));
};

// A helper for removing a class from an HTML DOM element.
// 
// element: The HTML element to modify the class of.
// name: The class to remove from the element.
rangeslider._removeClass = function(element, name) {
  var classes = element.getAttribute("class").split(" ");
  var removed = false;
  for (var i = 0; i < classes.length; ++i) {
    if (classes[i] == name) {
      classes.splice(i--, 1);
      removed = true;
    }
  }
  if (removed) {
    element.setAttribute("class", classes.join(" "));
  }
};

// Returns the dimensions for the given HTML element. This is used to determine
// the area the rangeslider needs to work in.
// 
// element: The HTML element to get the dimensions of.
// key: The key of the 'element' that represents its width dimension. It is
// used as element[key], and is usually one of offsetWidth or contentWidth, for
// example.
rangeslider._getDimension = function(element, key) {
  // find all hidden parent nodes
  var hiddenParentNodes = [];
  var node = element.parentNode;
  while (node.offsetWidth === 0 || node.offsetHeight === 0 || node.open === false) {
    hiddenParentNodes.push(node);
    node = node.parentNode;
  }
  // the dimension of the element, we will attempt to sort through the
  // hidden parent nodes to get a more accurate value
  var dimension = element[key];
  if (hiddenParentNodes.length > 0) {
    var styles = [];
    // temporarily hide parent content
    for (var i = 0; i < hiddenParentNodes.length; ++i) {
      styles.push(hiddenParentNodes[i].style.cssText);
      hiddenParentNodes[i].style.display = "block";
      hiddenParentNodes[i].style.height = "0";
      hiddenParentNodes[i].style.overflow = "hidden";
      hiddenParentNodes[i].style.visibility = "hidden";
      if (typeof hiddenParentNodes[i].open !== "undefined") {
        hiddenParentNodes[i].open = false;
      }
    }
    // capture dimension size
    dimension = element[key];
    // restore parent node css styles
    for (var i = 0; i < hiddenParentNodes.length; ++i) {
      hiddenParentNodes[i].style.cssText = styles[i];
      if (typeof hiddenParentNodes[i].open !== "undefined") {
        hiddenParentNodes[i].open = true;
      }
    }
  }
  return dimension;
};

// Simply returns the value capped to the given minimum/maximum values.
// 
// value: The value to cap.
// min: The minimum value.
// max: The maximum value.
// 
// Returns: Value, or min or max if value is less than the minimum or greater
// than the maximum.
rangeslider._cap = function(value, min, max) {
  if (value < min) return min;
  if (value > max) return max;
  return value;
};

// Delays a function call for a given number of milliseconds, then calls it with
// any additional arguments passed in to this function.
// 
// f: The function to call.
// ms: The time to wait to call the function.
// 
// Returns: The results of the setTimeout() call, which can be used to clear
// the timeout.
rangeslider._delay = function(f, ms) {
  var args = arguments.slice(2);
  return setTimeout(function(){ return f.call(args);}, ms);
};

// A debounce function. Prevents a function from being called too often.
// This function returns a debounce-function, which can be used normally, but
// will prevent execution if it is called too quickly. The function will be
// called, though it may not be called until 'ms' time has passed, and will
// only be called once, regardless of how many times it is executed. It will be
// called with the latest arguments.
// 
// f: The function to debounce.
// ms: The number of milliseconds to delay executing the debounce, also the
// maximum time between executions.
// early: Defaults false. If true, then instead of waiting for the "end" of
// the ms timer to execute, execute at the beginning of the ms timer as well.
// The function won't execute twice unless it is called twice within the ms
// time period.
rangeslider._debounce = function(f, ms, early) {
  var timeout = null;
  var args = null;
  if (early) {
    return function() {
      args = arguments;
      if (timeout != null) return;
      var timeoutFunc = function() {
        timeout = null;
        f.call(args);
        args = null;
      };
      timeout = setTimeout(timeoutFunc, ms);
    };
  } else {
    return function() {
      args = arguments;
      if (timeout != null) return;
      var timeoutFunc = function() {
        timeout = null;
        if (args != null) f.call(args);
        args = null;
      };
      args = null;
      timeout = setTimeout(timeoutFunc, ms);
      f.call(arguments);
    };
  }
};

// Retrieves the total sum of all offsetLeft and offsetTop properties from all
// offsetParent nodes from the given node. This results in a value that is the
// total left/top position of the given node, an HTML entity.
// 
// node: An HTML entity to get the total parent offset values from.
// 
// Returns: An object with properties "left" and "top", representing the total
// of those values.
rangeslider._getTotalOffsetsFromNode = function(node) {
  var ret = {"left":0, "top":0};
  while (node) {
    ret["left"] += node.offsetLeft;
    ret["top"] += node.offsetTop;
    node = node.offsetParent;
  }
  return ret;
};

}());