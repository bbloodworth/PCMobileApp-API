/*! Utilities.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// No constructor, this should prevent initialization. Also provides an object
// to attach static methods/properties to.
anzovinmod.Utilities = function() {
  throw "cannot instantiate anzovinmod.Utilities";
};

// Holds instance information about createjs objects, including default
// instances of objects and lists of non-object properties.
// In the case of createjs not being defined when this script executes, the
// object instances will just be null instead.
anzovinmod.Utilities.CreateJSObjects = {};

// CreateJS information about the createjs.MovieClip type.
anzovinmod.Utilities.CreateJSObjects.MovieClip = {
  // An instance of a brand new createjs.MovieClip object. This is used in
  // places where the properties of a default object need to be compared to
  // those of an in-use object to determine if properties are display object
  // instances or not.
  "defaultInstance": (typeof createjs !== "undefined" ? new createjs.MovieClip() : null),
  // These properties are attached to the MovieClip object, but are used
  // internally by createjs so should be skipped when iterating over other
  // properties of the MovieClip.
  "nonObjectAttributes": [
    "tweenjs_count",
    "_off",
    "_anzovinmod"
  ]
};

// CreateJS information about the createjs.Container type.
anzovinmod.Utilities.CreateJSObjects.Container = {
  "defaultInstance": (typeof createjs !== "undefined" ? new createjs.Container() : null),
  "nonObjectAttributes": [
    "tweenjs_count",
    "_off",
    "_anzovinmod"
  ]
};

// This function takes a movieclip or container object, and returns a list of
// all the display objects that are attached to the element. This includes
// those objects that are not actually children, but are attached as properties
// to the movieclip/container.
// 
// This function take a default created instance of the type of object passed
// in, and strips out any on-object properties that are a part of the default
// object. This effectively strips out any internally used variables, such as
// "x", "y", etc.
// 
// e: The element to search for children in.
// 
// Return: An array of properties that are child display objects.
anzovinmod.Utilities.listChildDisplayObjectProperties = function(e) {
  e = anzovinmod.Utilities.defaultParam(e, null);
  var ret = [];
  if (e == null) {
    return ret;
  }
  // determine information necessary for recursive calls
  var objectInformation = null;
  if (typeof createjs !== "undefined" && e instanceof createjs.MovieClip) {
    objectInformation = anzovinmod.Utilities.CreateJSObjects.MovieClip;
  } else if (typeof createjs !== "undefined" && e instanceof createjs.Container) {
    objectInformation = anzovinmod.Utilities.CreateJSObjects.Container;
  }
  // if a container type, then recursively call on properties
  if (objectInformation != null) {
    for (var eElement in e) {
      // initial check for default properties. only properties that are not
      // a part of the prototype could possible be display objects, so all the
      // rest can just be discarded
      if (!e.hasOwnProperty(eElement)) {
        continue;
      }
      // is a default property, so skip. these properties are a part of the
      // default object instance, assigned to the object instance during
      // creation, and no longer contain the default prototype value
      if (objectInformation.defaultInstance.hasOwnProperty(eElement)) {
        continue;
      }
      // enhanced search for default properties that have yet to be modified
      // from the prototype's initial value of it. anything that is a part of
      // the base element's properties are not going to be display objects that
      // we are interested in, so we can skip them
      if (eElement in objectInformation.defaultInstance) {
        continue;
      }
      // is a non-object attribute, so skip. these are manually defined
      // properties that may not show up in the default property checks above,
      // but should be skipped. maybe these are used by createjs, or maybe
      // these are used for our own purposes
      if (objectInformation.nonObjectAttributes.indexOf(eElement) != -1) {
        continue;
      }
      // is a function, so skip it. this takes into account all the "frame_#"
      // functions that maybe defined on the root movieclip object or others
      if (typeof e[eElement] === "function") {
        continue;
      }
      // is a valid property, add it
      ret.push(eElement);
    }
  }
  return ret;
};

// A helper function to re-scope function calls cleanly.
// 
// Executed like: (this == obj)
//  bind(obj, function() {console.log(this);})
// 
// scope: The object to "own" the function call.
// fn: The function to call under a bound scope.
// 
// Returns: A function that can be immediatelly called with any arguments as
// desired, that will execute under the desired scope.
anzovinmod.Utilities.bind = function(scope, fn) {
  return function () {
      fn.apply(scope, arguments);
  };
};

// A helper function to use default values in functions.
// 
// A parameter to a function, if left undefined, will be equal to 'undefined'
// type when the function executes. Using this function, you can set up a
// guarantee that parameters will be set to sensicle default values.
// 
// Example:
// var x = function(a) {console.log(a);};
// y();    // undefined
// y(123); // 123
// 
// 'undefined' is not actually a keyword (like null). As such, strange behaviors
// can occur with using the 'undefined' variable word. As such, it may be more
// appropriate to use 'null' as an indicator that a parameter should be a
// default value. For example, in a function(a,b,c), the parameter b might have
// a default of 12 and c might have a default of 50. If someone wished to use
// the default value for b but specify a value manually for c, one could call
// the function as: f(1,undefined,22).
// 
// If you wish to allow for f(1,null,22) to accomplish the same thing, then
// use the 'dOnNull' parameter to this utility function. It will use the default
// value in the case of undefined and null, so as to let you be a bit more
// secure in your use of 'undefined' property.
// 
// v: The parameter that may or may not be defined (usually function input).
// d: The default value to use if the parameter v is not defined.
// dOnNull: Also returns d if v is null, not just undefined.
// 
// Returns: If v is undefined, then d is returned. If v is null and dOnNull is
// true, then d is returned. Otherwise, v is returned.
anzovinmod.Utilities.defaultParam = function(v, d, dOnNull) {
  // optimize for more speed
  if (dOnNull !== true) {
    return (typeof(v) == "undefined" ? d : v);
  } else {
    return (typeof(v) == "undefined" ? d : (v === null ? d : v));
  }
};

// Deep clones an array.
// 
// source: The array to clone.
// deep: Whether to deep clone. Defaults false.
// arrayLength: If the cloned object is an array, then only clone this number
// of elements from the beginning of the array. If unset, then clone the
// entire array. If greater than the array length, then clones the entire array.
// If less than zero, then return an empty array.
// 
//
anzovinmod.Utilities.cloneArray = function(source, deep, arrayLength) {
  deep = anzovinmod.Utilities.defaultParam(deep, false, true);
  arrayLength = anzovinmod.Utilities.defaultParam(arrayLength, null);
  if (source instanceof Array) {
    var ret = [];
    var iter = source.length;
    if (arrayLength != null) {
      if (arrayLength >= 0 && arrayLength <= source.length) {
        iter = arrayLength;
      }
    }
    if (deep) {
      while (iter--) {
        ret[iter] = anzovinmod.Utilities.cloneObject(source[iter], deep);
      }
    } else {
      while (iter--) {
        ret[iter] = source[iter];
      }
    }
    return ret;
  }
  return null;
};

// Deep clones an object. Note that this is not tested for complex objects,
// but should work fine for basic types such as those being used as
// associative arrays created with {}.
// 
// see: http://stackoverflow.com/questions/728360/most-elegant-way-to-clone-a-javascript-object
// see: http://stackoverflow.com/questions/122102/what-is-the-most-efficient-way-to-clone-an-object
// 
// source: The object to clone.
// deep: Whether to deep clone. Defaults false.
// 
// Returns: A new cloned instance of the object, not a reference to any other
// existing objects.
anzovinmod.Utilities.cloneObject = function(source, deep) {
  deep = anzovinmod.Utilities.defaultParam(deep, false, true);
  if (source == null) {
    return source;
  }
  if (source instanceof Array) {
    return anzovinmod.Utilities.cloneArray(source, deep);
  }
  if (source instanceof Object) {
    var ret = {};
    if (deep) {
      for (var i in source) {
        if (!source.hasOwnProperty(i)) {
          continue;
        }
        ret[i] = anzovinmod.Utilities.cloneObject(source[i], deep);
      }
    } else {
      for (var i in source) {
        if (!source.hasOwnProperty(i)) {
          continue;
        }
        ret[i] = source[i];
      }
    }
    return ret;
  }
  return source;
};

// Just convert the given object into a string reproducible facsimile.
// Will safely convert any NULL objects into the string "NULL". This is
// really just so that we can say log(pnt(x)) instead of having to include
// any special code for nullability checks and the like, even though JS seems
// to be good about handling that sort of thing automatically and often has
// its own handling of things like expanding objects onto the DOM when logged.
// 
// Note that for non-simple Object types (eg objects not created with {} but
// with a function constructor) they are simply returned as .toString() and not
// iterated over all elements. This is to help ensure that recursion does not
// take place in custom objects acting as classes, though this may still occur
// in simple Object types.
// 
// The more root reason for this, is that many of the types used in this project
// have .parent nodes that themselves have .children nodes, which creates an
// easy recursion.
// 
// Additionally, in order to help prevent infinite recursion in standard
// Objects (and Arrays and anything else that may have a recursive element in
// it), a level-limit can be used to prevent this sort of output. The second
// parameter to this function can optionally be specified to control the number
// of recursive printing to perform.
// 
// x: The object to print.
// maxRecursion: The maximum number of levels to print on objects and arrays.
// Defaults to 5.
// 
// Returns:
// A non-null string instance that describes the object.
anzovinmod.Utilities.pnt = function(x, maxRecursion) {
  maxRecursion = anzovinmod.Utilities.defaultParam(maxRecursion, 5);
  if (maxRecursion == 0) {
    return "...";
  }
  if (typeof(x) === "undefined") {
    return "UNDEFINED";
  } else if (x === null) {
    return "NULL";
  } else if (x instanceof Array) {
    var s = "[";
    for (var i = 0; i < x.length; ++i) {
      if (i != 0) {
        s += ",";
      }
      s += anzovinmod.Utilities.pnt(x[i], maxRecursion-1);
    }
    s += "]";
    return s;
  } else if (x instanceof Object && typeof(x) == "object") {
    // if the object constructor is "Object", then this was created with {} and
    // has a generally lower liklihood of being recursive as it likely does not
    // have .parent and .children elements that could easily cause recursion
    if (!(x && x.constructor && x.constructor.name && x.constructor.name == "Object")) {
      return x.toString();
    }
    var s = (x && x.constructor ? x.constructor.name : "");
    if (s == null || s == "") {
      s = "unknown";
    }
    s += "{";
    var count = 0;
    for (var i in x) {
      if (!x.hasOwnProperty(i)) {
        continue;
      }
      if (count != 0) {
        s += ",";
      }
      s += i + ":" + anzovinmod.Utilities.pnt(x[i], maxRecursion-1);
      ++count;
    }
    s += "}";
    return s;
  }
  return x.toString();
};

// Convert the given Matrix2D object into a CSS transform "matrix(...)" string.
// 
// m: Matrix2D instance to convert
// 
// Returns: A string that can be used in CSS transform properties.
anzovinmod.Utilities.cssTransformMatrix2D = function(m) {
  return "matrix(" + m.a + "," + m.b + "," + m.c + "," +
    m.d + "," + m.tx + "px," + m.ty + "px)";
};

// This function attempts to get the window size of the current display.
// 
// The method of obtaining the window width/height is left up to this function.
// The width/height returned ideally will be the exact pixels of the screen area
// available under the current display.
// 
// Common ways to calculate the width/height include: directly use the
// window.innerWidth value; use the document.body.clientWidth value after
// hiding all other content and temporarily setting its styles manually to have
// 100% width, no borders, margins, or padding, etc; etc.
// 
// This function may temporarily apply CSS to the document or body or child
// DOM elements in order to get a good calculation. Whatever changes are made
// will be only temporary and should ideally cause as little interference in
// the page content as possible.
// 
// Returns: An object that contains the properties "width" and "height".
anzovinmod.Utilities.getWindowSize = function() {
  var originalStyles = {
    "visibility": document.body.style.visibility,
    "overflow": document.body.style.overflow
  };
  document.body.style.visibility = "hidden";
  document.body.style.overflow = "hidden";
  var ret = {
    //"width":  window.innerWidth,
    //"height": window.innerHeight
    "width":  document.documentElement.clientWidth,
    "height": document.documentElement.clientHeight
  };
  document.body.style.visibility = originalStyles["visibility"];
  document.body.style.overflow = originalStyles["overflow"];
  return ret;
};

// This function is used to help resize elements in this project. It takes in
// a desired width/height combination that may be keyworded in particular ways,
// such as by referring to the window size or the containing element size,
// and calculates the final pixel size of the element in question.
// 
// The arguments of width/height can take on several forms. The exact form of
// which determines how the element is resized.
// 
// "window" / "100%" / 800
// 
// "window": Resizes based on the window size.
// "100%": Resizes based on the containing parent element size.
// 800: Just resizes to the given pixel size.
// 
// parent: The parent of the object node that is being resized.
// width: The desired width of the object.
// height: The desired height of the object.
// 
// Returns: An object that contains "width"/"height" attributes that are the
// calculated width/height based on the inputs.
anzovinmod.Utilities.simplifyPotentialResizeValues = function(parent, width, height) {
  parent = anzovinmod.Utilities.defaultParam(parent, null);
  width = anzovinmod.Utilities.defaultParam(width, null);
  height = anzovinmod.Utilities.defaultParam(height, null);
  // return null on all, require all parameters
  if (parent == null || width == null || height == null) {
    return null;
  }
  // determine window size if that is desired
  if (width == "window" || height == "window") {
    var windowSize = anzovinmod.Utilities.getWindowSize();
    if (width == "window") {
      width = windowSize["width"];
    }
    if (height == "window") {
      height = windowSize["height"];
    }
  }
  // if size is a percent, determine pixel size
  var isPercentW = false;
  var isPercentH = false;
  if (typeof(width) == "string" && width.substr(width.length-1, 1) == "%") {
    isPercentW = true;
  }
  if (typeof(height) == "string" && height.substr(height.length-1, 1) == "%") {
    isPercentH = true;
  }
  if (isPercentW || isPercentH) {
    // calculate available space
    var originalDisplays = [];
    for (var i = 0; i < parent.children.length; ++i) {
      var child = parent.children[i];
      if (child && child.style && child.style.display) {
        originalDisplays.push({"i":i,"d":child.style.display});
        child.style.display = "none !important";
      }
    }
    var parentWidth = parent.clientWidth;
    var parentHeight = parent.clientHeight;
    for (var i = 0; i < originalDisplays.length; ++i) {
      var j = originalDisplays[i];
      parent.children[j["i"]].style.display = j["d"];
    }
    // simplify corresponding values
    if (isPercentW) {
      var percent = parseFloat(width);
      if (isNaN(percent)) {
        percent = 100.0;
      }
      width = parentWidth * percent / 100.0;
    }
    if (isPercentH) {
      var percent = parseFloat(height);
      if (isNaN(percent)) {
        percent = 100.0;
      }
      height = parentHeight * percent / 100.0;
    }
  }
  // return calculated values
  return {
    "width": width,
    "height": height
  };
};

// Determines the presentation scale of an object when given its original and
// max potential scaled dimensions. The final scale is limited to the
// smaller of the resulting horizontal and vertical scaling, to keep the
// presented object within a bounding box and maintain aspect ratio.
// 
// x: The current/desired width of the object, in px.
// y: The current/desired height of the object, in px.
// baseX: The original width of the object.
// baseY: The original height of the object.
// resizeBehaviors: An object contain properties relating to resize behaviors.
// 
// Returns: The final scale value. Will return 0.0 if there was a problem with
// inputs or calculations. Will return 0.0 if the base inputs are zero.
anzovinmod.Utilities.calculateBoundScale = function(x, y, baseX, baseY, resizeBehaviors) {
  x = anzovinmod.Utilities.defaultParam(x, null);
  y = anzovinmod.Utilities.defaultParam(y, null);
  baseX = anzovinmod.Utilities.defaultParam(baseX, null);
  baseY = anzovinmod.Utilities.defaultParam(baseY, null);
  resizeBehaviors = anzovinmod.Utilities.defaultParam(resizeBehaviors, {}, true);
  if (x == null || y == null || baseX == null || baseY == null) {
    return 0.0;
  }
  if (!resizeBehaviors.hasOwnProperty("canScaleUp"))
    resizeBehaviors["canScaleUp"] = true;
  if (!resizeBehaviors.hasOwnProperty("canScaleDown"))
    resizeBehaviors["canScaleDown"] = true;
  if (baseX == 0 || baseY == 0) {
    return 0.0;
  }
  var scaleWidth = x / baseX;
  var scaleHeight = y / baseY;
  var finalScale = (scaleHeight > scaleWidth ? scaleWidth : scaleHeight);
  if (!resizeBehaviors["canScaleUp"] && finalScale > 1.0) {
    finalScale = 1.0;
  }
  if (!resizeBehaviors["canScaleDown"] && finalScale < 1.0) {
    finalScale = 1.0;
  }
  return finalScale;
};

// This function removes a part of a string. This can be used for a number of
// purposes, but one specific purpose is to add and remove classes from class
// definitions easily.
// 
// Note that if the string part is not found in the whole string,
// then the original string should be returned. Also note that this will
// search for all instances of the definition, and will not stop at a single
// instance of the part.
// 
// whole: The current string to remove a part from.
// part: The string part to remove from the whole.
// 
// Returns: The new string. Will return an empty string if inputs were null.
anzovinmod.Utilities.removeStringComponent = function(whole, part, sep) {
  whole = anzovinmod.Utilities.defaultParam(whole, "", true);
  part = anzovinmod.Utilities.defaultParam(part, "", true);
  sep = anzovinmod.Utilities.defaultParam(sep, " ", true);
  if (whole == "") return "";
  if (part == "") return whole;
  var wholePieces = whole.split(sep);
  for (var i = 0; i < wholePieces.length; ++i) {
    if (wholePieces[i] == part) {
      wholePieces.splice(i, 1);
      --i;
    }
  }
  return wholePieces.join(sep);
};

// The opposite of the removeStringComponent() function, this one adds the
// string part to the whole class definition, if it is not already contained
// within it.
// 
// whole: The current string to add a part to.
// part: The string part to add.
// 
// Returns: The new string.
anzovinmod.Utilities.addStringComponent = function(whole, part, sep) {
  whole = anzovinmod.Utilities.defaultParam(whole, "", true);
  part = anzovinmod.Utilities.defaultParam(part, "", true);
  sep = anzovinmod.Utilities.defaultParam(sep, " ", true);
  if (part == "") {
    return whole;
  }
  var wholePieces = whole.split(sep);
  for (var i = 0; i < wholePieces.length; ++i) {
    if (wholePieces[i] == part) {
      return whole;
    }
  }
  wholePieces.push(part);
  return wholePieces.join(sep);
};

// Returns whether or not the whole string component contains the partial string
// component. One use of this, is to check whether a CSS class exists in a
// single definition.
// 
// whole: The current class definition string.
// part: The class definition to check for.
// 
// Returns: True if the string is a part of the whole, else false.
anzovinmod.Utilities.hasStringComponent = function(whole, part, sep) {
  whole = anzovinmod.Utilities.defaultParam(whole, "", true);
  part = anzovinmod.Utilities.defaultParam(part, "", true);
  sep = anzovinmod.Utilities.defaultParam(sep, " ", true);
  if (whole == "" || part == "") {
    return false;
  }
  var wholePieces = whole.split(sep);
  for (var i = 0; i < wholePieces.length; ++i) {
    if (wholePieces[i] == part) {
      return true;
    }
  }
  return false;
};

// This helper function sets a DOM element property. This is a helper function
// because there is specific behaviors that must be followed when setting this
// data on IE8 systems.
// 
// In short, when on >IE8 or another browser, setting a property on a DOM
// element is different than setting an attribute. Attributes are of the form
// 'class="some-css-class"', while properties are more intangible. IE8 just
// converts the property into an attribute, so it must be converted to a string
// instead.
// 
// However, this doesn't resolve the issue of references to other elements being
// resolved, as not all property values are simple JSON values: Some may have
// references to other objects or DOM elements that should be maintained. The
// solution of this Utilities file, is to create an array of
// elements/names/properties so as to not have the properties directly attached
// to the objects in question. This way, the properties can still be referenced
// outside of the DOM element in question.
// 
// The only big thing about this, is that memory leaks may occur unless you
// manually remove the element from this kept list. So, when a DOM element is
// removed, it should be removed from this list as well.
// 
// elem: The DOM element that is getting a property set.
// name: The name of the property.
// prop: The property itself (can be anything).
anzovinmod.Utilities.setDomElementProperty = function(elem, name, prop) {
  elem = anzovinmod.Utilities.defaultParam(elem, null);
  name = anzovinmod.Utilities.defaultParam(name, null);
  if (elem == null || name == null) return;
  if (!anzovinmod.Utilities.IsIE8()) {
    elem[name] = prop;
    return;
  }
  var deps = anzovinmod.Utilities._DomElementProperties;
  for (var i = 0; i < deps.length; ++i) {
    if (deps[i]["elem"] == elem) {
      deps[i]["props"][name] = prop;
      return;
    }
  }
  var o = {"elem":elem, "props":{}};
  o["props"][name] = prop;
  deps.push(o);
};

// Obtains the element property from the given DOM element.
// 
// elem: The DOM element to obtain a property of.
// name: The name of the property to get.
// 
// Returns: The value of the property, or UNDEFINED if not one set.
anzovinmod.Utilities.getDomElementProperty = function(elem, name) {
  elem = anzovinmod.Utilities.defaultParam(elem, null);
  name = anzovinmod.Utilities.defaultParam(name, null);
  if (elem == null || name == null) return;
  if (!anzovinmod.Utilities.IsIE8()) {
    return elem[name];
  }
  var deps = anzovinmod.Utilities._DomElementProperties;
  for (var i = 0; i < deps.length; ++i) {
    if (deps[i]["elem"] == elem) {
      return deps[i]["props"][name];
    }
  }
  return;
};

// Removes the given DOM element from the internal list of stored DOM element
// properties. This is called when the DOM element is to be removed from the
// DOM graph and should be destroyed/garbage collected/whatever.
// 
// elem: The DOM element to remove all listings for.
anzovinmod.Utilities.clearDomElementProperties = function(elem) {
  elem = anzovinmod.Utilities.defaultParam(elem, null);
  if (elem == null) return;
  if (!anzovinmod.Utilities.IsIE8()) {
    return;
  }
  var deps = anzovinmod.Utilities._DomElementProperties;
  for (var i = 0; i < deps.length; ++i) {
    if (deps[i]["elem"] == elem) {
      deps.splice(i, 1);
      return;
    }
  }
};

// A list of the DOM element properties that have been set through calls to
// setDomElementProperty(). Clear out a DOM element with a call to
// clearDomElementProperties() when done with a DOM element.
// 
// Each value of this array is an object with the following format:
// "elem": A reference to the DOM element.
// "props": An object that stores name:value pairs of properties.
anzovinmod.Utilities._DomElementProperties = [];

// An indicator of whether this is IE8 or not. This can easily be set in the
// calling HTML file through something like the following:
// 
// <!--[if IE 8]>
// <script type="text/javascript">
// var anzovinmod = anzovinmod || {};
// anzovinmod.instance = anzovinmod.instance || {};
// anzovinmod.instance.IsIE8 = true;
// </script>
// <![endif]-->
// 
// These functions just use the anzovinmod.instance properties to make the
// final determinations.
anzovinmod.Utilities.IsIE8 = function() {
  return anzovinmod && anzovinmod.instance && anzovinmod.instance.IsIE8;
};

// An indicator of whether this is IE9.
anzovinmod.Utilities.IsIE9 = function() {
  return anzovinmod && anzovinmod.instance && anzovinmod.instance.IsIE9;
};

}());
