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
/*! Logging.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// Constructor, should prevent instantiation. Also provides an object to
// attach static methods and properties to.
anzovinmod.Logging = function() {
  throw "cannot instantiate anzovinmod.Logging";
};

// The current loglevel.
anzovinmod.Logging._LogLevel = 4;

anzovinmod.Logging.LOG_ERROR  = 0;
anzovinmod.Logging.LOG_WARN   = 1;
anzovinmod.Logging.LOG_NOTICE = 2;
anzovinmod.Logging.LOG_INFO   = 3;
anzovinmod.Logging.LOG_DEBUG  = 4;
anzovinmod.Logging.LOG_TRACE  = 5;
anzovinmod.Logging.LOG_TRACE_1  = 5;
anzovinmod.Logging.LOG_TRACE_2  = 6;
anzovinmod.Logging.LOG_TRACE_3  = 7;
anzovinmod.Logging.LOG_TRACE_4  = 8;
anzovinmod.Logging.LOG_TRACE_5  = 9;
anzovinmod.Logging.LOG_TRACE_6  = 10;
anzovinmod.Logging.LOG_TRACE_7  = 11;
anzovinmod.Logging.LOG_TRACE_8  = 12;
anzovinmod.Logging.LOG_TRACE_9  = 13;
anzovinmod.Logging.MAX_LOG_LEVEL = 13;

// Simply returns true/false if the current logging level supports the
// given log level. This is useful for when constructing even the pieces of
// a log message are tedious or long-winded, so you can wrap the creation
// of the log message in a condition.
// 
// l: The log level to test.
// 
// Returns:
// True if the current log level would write a message with level 'l',
// False if it would not write a message.
anzovinmod.Logging.canLog = function(l) {
  return !(l > anzovinmod.Logging._LogLevel);
};

// Just returns the string component of the log message that corresponds to the
// log level, like "-warn--".
// 
// In the case of an invalid log level, then "INVALID" is a code used to
// indicate as such.
// 
// l: The log level to get the string component of.
// 
// Returns: A string for use in logging with.
anzovinmod.Logging._getLogLevelString = function(l) {
  if (typeof(l) != "number") l = -1;
  if (l > anzovinmod.Logging.MAX_LOG_LEVEL) l = -1;
  if (l < 0) l = -1;
  l = parseInt(l);
  switch (l) {
    case -1:                            return "INVALID--- ";
    case anzovinmod.Logging.LOG_ERROR:  return "error----- ";
    case anzovinmod.Logging.LOG_WARN:   return "-warn----- ";
    case anzovinmod.Logging.LOG_NOTICE: return "--notice-- ";
    case anzovinmod.Logging.LOG_INFO:   return "---info--- ";
    case anzovinmod.Logging.LOG_DEBUG:  return "----debug- ";
    default:                            return "-----trace ";
  }
  return "INVALID--- ";
};

// Traces the given statement. Only traces if the current log level is
// equal to or greater than indicated.
// 
// l: The log level of this statement.
// s: The object to trace out as a string. This can be specified multiple times
// in the function call.
// 
// Returns:
// True if traced, false if not traced.
anzovinmod.Logging.logit = function(l) {
  if (arguments.length <= 1) return false;
  if (typeof console === "undefined") return false;
  if (l > anzovinmod.Logging._LogLevel) return false;
  var logout = anzovinmod.Logging._getLogLevelString(l);
  logout += ": ";
  for (var i = 1; i < arguments.length; ++i) {
    logout += anzovinmod.Utilities.pnt(arguments[i]);
  }
  console.log(logout);
  return true;
};

// Traces the given statement. Only traces if the current log level is
// equal to or greater than indicated. This version accepts an additional
// parameter, which is the method that called this log function. This is used
// to handle formatting of log messages.
// 
// l: The log level of this statement.
// m: The method that called this log level (or other identifier).
// s: The object to trace out as a string. This can be specified multiple times
// in the function call.
// 
// Returns:
// True if traced, false if not traced.
anzovinmod.Logging.logm = function(l, m) {
  if (arguments.length <= 2) return false;
  if (typeof console === "undefined") return false;
  if (l > anzovinmod.Logging._LogLevel) return false;
  var logout = anzovinmod.Logging._getLogLevelString(l);
  logout += ": " + m + "() ";
  for (var i = 2; i < arguments.length; ++i) {
    logout += anzovinmod.Utilities.pnt(arguments[i]);
  }
  console.log(logout);
  return true;
};

// Traces the given statement. Only traces if the current log level is
// equal to or greater than indicated. This version only traces once per
// argument passed in to this function, and each argument/object is on its own
// trace line with no other content. This is useful for tracing objects directly
// to the console, so you can interact with them. Additionally, no processing
// is performed on these arguments/objects being logged.
// 
// l: The log level of this statement.
// o: The object to trace out as a string. This can be specified multiple times
// in the function call.
// 
// Returns:
// True if traced, false if not traced.
anzovinmod.Logging.logo = function(l) {
  if (arguments.length <= 1) return false;
  if (typeof console === "undefined") return false;
  if (l > anzovinmod.Logging._LogLevel) return false;
  for (var i = 1; i < arguments.length; ++i) {
    console.log(arguments[i]);
  }
  return true;
}

// This function sets the global logging level. Levels that are below this
// level will not be printed to the console log, but levels that match or are
// greater than this will be printed.
// 
// l: The log level to set to.
anzovinmod.Logging.setLogLevel = function(l) {
  anzovinmod.Logging._LogLevel = l;
};

// Just returns the current logging level.
// 
// Returns:
// The current log level.
anzovinmod.Logging.getLogLevel = function() {
  return anzovinmod.Logging._LogLevel;
};

}());
/*! AsyncRequestObject.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// Constructor. This class is responsible for sending a request to the
// CCH API, and handling the response. Really this is just a wrapper for the
// XMLHttpRequest object or, in the case of early IE, the appropriate
// replacement ActiveX object. In the case of there needing to be any
// workarounds for particular combinations of browsers or devices, they will
// be here.
// 
// The primary purpose of this class is to handle much of the callback
// and state differences between IE/everything else and asynch setup and
// handlers all all that jazz. Some of the interaction with the request
// object will be included as part of this object/constructor. Other
// interactions may require direct access to the internal request object
// instance, but those should be kept to a minimum.
// 
// Really, if there's something on the internal request object that isn't
// exposed via this class, then it needs to be added.
// 
// type: GET, POST, HEAD, etc. The type of HTTP request being made.
// url: The URL to send as part of this request.
// onSuccess: The callback to use once the request is successful. Function
// will receive a single parameter, which is a handle to this AsyncRequestObject
// instance.
// onFail: The callback to use once the request has failed. Function will
// receive a single parameter, which is a handle to this AsyncRequestObject
// instance.
// onStateChange: A callback to use every time there is a change in the
// request object's connection state. Function will receive a single parameter,
// which is a handle to this AsyncRequestObject instance. This will be called
// when the state changes to a success/fail state. This callback if specified,
// will be called before the onSuccess or onFail callbacks.
anzovinmod.AsyncRequestObject = function(type, url, onSuccess, onFail, onStateChange) {
  type = anzovinmod.Utilities.defaultParam(type, null);
  url = anzovinmod.Utilities.defaultParam(url, null);
  onSuccess = anzovinmod.Utilities.defaultParam(onSuccess, null);
  onFail = anzovinmod.Utilities.defaultParam(onFail, null);
  onStateChange = anzovinmod.Utilities.defaultParam(onStateChange, null);
  if (type == null) {
    var errorMessage = "anzovinmod.AsyncRequestObject() cannot create an empty request object with no type";
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_ERROR, "anzovinmod.AsyncRequestObject", errorMessage);
    throw errorMessage;
  }
  if (url == null) {
    var errorMessage = "anzovinmod.AsyncRequestObject() cannot create an empty request object with no URL";
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_ERROR, "anzovinmod.AsyncRequestObject", errorMessage);
    throw errorMessage;
  }
  
  // INSTANCE VARIABLES
  
  // The type of this request object.
  this._type = type;
  // The URL of this request object.
  this._url = url;
  // A list of all the request headers set. This is so we can set them again
  // when/if we reset the requets object.
  this._setRequestHeaders = [];
  
  // REQUEST OBJECT STUFF
  
  // The request object.
  this._requestObject = null;
  // The type of the request object, eg either XMLHttpRequest or ActiveX.
  // There are anzovinmod.AsyncRequestObject.* constants that you can compare
  // this value against.
  this._requestObjectType = null;
  // The on-success callback. The first parameter of the callback is this
  // AsyncRequestObject object instance.
  this._onSuccessCallback = onSuccess;
  // The on-fail callback. The first parameter of the callback is this
  // AsyncRequestObject object instance.
  this._onFailCallback = onFail;
  // The on-state-change callback. The first parameter of the callback is this
  // AsyncRequestObject object instance.
  this._onStateChangeCallback = onStateChange;
  // An indicator of whether the request object has been activated and the
  // request sent.
  this._requestSent = false;
  
  this._init();
};

// This function is simply the initializer for the constructor. It is called to
// perform any work that is not simply setting default values for the class
// properties.
anzovinmod.AsyncRequestObject.prototype._init = function() {
  // Create the request object instance.
  this._makeRequestObject();
};

// Constant identifiers for the different request object types.
anzovinmod.AsyncRequestObject.REQUEST_OBJECT_XMLHTTPREQUEST = "XMLHttpRequest";
anzovinmod.AsyncRequestObject.REQUEST_OBJECT_ACTIVEXOBJECT_MSXML2_XMLHTTP = "ActiveXObject('Msxml2.XMLHTTP')";
anzovinmod.AsyncRequestObject.REQUEST_OBJECT_ACTIVEXOBJECT_MICROSOFT_XMLHTTP = "ActiveXObject('Microsoft.XMLHTTP')";

// Creates an XMLHttpRequest or ActiveX object for the request, depending on
// which is supported. The internal variable for it is set after this function
// is called, along with an indicator of which was created. Calling this
// function will overwrite an existing request object if one is already created.
anzovinmod.AsyncRequestObject.prototype._makeRequestObject = function() {
  if (this._requestObject != null) {
    this._requestObject.onreadystatechange = function(){};
    this._requestObject.abort();
  }
  // identify the request object types available
  var types = [];
  if (anzovinmod.Utilities.IsIE8() || anzovinmod.Utilities.IsIE9()) {
    types = [
      anzovinmod.AsyncRequestObject.REQUEST_OBJECT_ACTIVEXOBJECT_MSXML2_XMLHTTP,
      anzovinmod.AsyncRequestObject.REQUEST_OBJECT_ACTIVEXOBJECT_MICROSOFT_XMLHTTP,
      anzovinmod.AsyncRequestObject.REQUEST_OBJECT_XMLHTTPREQUEST
    ];
  } else {
    types = [
      anzovinmod.AsyncRequestObject.REQUEST_OBJECT_XMLHTTPREQUEST
    ];
  }
  // create the request object
  for (var i = 0; i < types.length; ++i) {
    var type = types[i];
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.AsyncRequestObject._makeRequestObject", "making a ", type, " request object");
    switch (type) {
      case anzovinmod.AsyncRequestObject.REQUEST_OBJECT_XMLHTTPREQUEST:
        this._requestObject = new XMLHttpRequest();
        this._requestObjectType = type;
        break;
      case anzovinmod.AsyncRequestObject.REQUEST_OBJECT_ACTIVEXOBJECT_MSXML2_XMLHTTP:
        try {
          this._requestObject = new ActiveXObject("Msxml2.XMLHTTP");
          this._requestObjectType = type;
        } catch (e) {
          this._requestObject = null;
          anzovinmod.Logging.logm(anzovinmod.Logging.LOG_WARN, "anzovinmod.AsyncRequestObject._makeRequestObject", "cannot make request object: ", e);
        }
        break;
      case anzovinmod.AsyncRequestObject.REQUEST_OBJECT_ACTIVEXOBJECT_MICROSOFT_XMLHTTP:
        try {
          this._requestObject = new ActiveXObject("Microsoft.XMLHTTP");
          this._requestObjectType = type;
        } catch (e) {
          this._requestObject = null;
          anzovinmod.Logging.logm(anzovinmod.Logging.LOG_WARN, "anzovinmod.AsyncRequestObject._makeRequestObject", "cannot make request object: ", e);
        }
        break;
      default:
        anzovinmod.Logging.logm(anzovinmod.Logging.LOG_WARN, "anzovinmod.AsyncRequestObject._makeRequestObject", "unknown request object type, skipping");
        break;
    }
    if (this._requestObject != null) {
      break;
    }
  }
  // validate request object created
  if (this._requestObject == null) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_ERROR, "anzovinmod.AsyncRequestObject._makeRequestObject", "could not create request object");
    return;
  }
  // set properties on the request object
  this._requestObject.open(this._type, this._url, true);
  this._requestObject.onreadystatechange = anzovinmod.Utilities.bind(this, this._requestObjectStateChangeCallback);
  for (var i = 0; i < this._setRequestHeaders.length; ++i) {
    var reqhead = this._setRequestHeaders[i];
    this._requestObject.setRequestHeader(reqhead[0], reqhead[1]);
  }
};

// This callback function is called on every state change of the request object,
// after the request object has been activated and the URL request sent.
// 
// see: http://www.w3.org/TR/XMLHttpRequest/#dom-xmlhttprequest-readystate
// 
// The "readyState" property of the request object will be an indicator of the
// state of the connection to the remote service.
// 
// 0, UNSENT: The request object has itself been constructed, but has not yet
// even been .open()'ed.
// 1, OPENED: Request object has been .open()'ed, but not yet sent. Request
// headers can be specified at this time.
// 2, HEADERS_RECEIVED: Request has been .send()'ed, and the final response
// headers after any redirects have been read.
// 3, LOADING: The response body is being loaded. In this state, the request
// object may contain a partial representation of the response body and if
// supported, implementation can begin parsing the response while waiting for
// the rest of the resposne to finish load.
// 4, DONE: The entirety of the response body has been loaded and is available.
// Alternatively, an error occurred and there is no response body or a
// partial(?) response body.
// 
// The "status" property of the request object will be an indicator of the
// final HTTP response code from the service, such as "200" or "404".
// 
// Normally, when readyState is "4", then either a success or failure callback
// can be called as the request has finished. This even includes situations
// when CORS is not supported on the remote service and the OPTIONS call
// failed or was not even attempted.
// 
// In FF, no CORS support triggers readyState of [2,4] both with status 0.
// In IE10, no CORS support triggers readyState of just 4 with status 0.
anzovinmod.AsyncRequestObject.prototype._requestObjectStateChangeCallback = function() {
  if (this._onStateChangeCallback != null) {
    this._onStateChangeCallback(this);
  }
  if (this.isDone()) {
    if (this.isSuccess()) {
      if (this._onSuccessCallback != null) {
        this._onSuccessCallback(this);
      }
    } else {
      if (this._onFailCallback != null) {
        this._onFailCallback(this);
      }
    }
  }
};

// This function analyzes the state of the request object, and determines
// whether the request has succeeded or failed. This function will only
// return an accurate value once the request is completed, eg .isDone() returns
// true.
// 
// Returns: True if the request succeeded, false if it failed or still in
// progress or is otherwise not active such as not yet being sent.
anzovinmod.AsyncRequestObject.prototype.isSuccess = function() {
  if (!this._requestSent) {
    return false;
  }
  var state = this._requestObject.readyState;
  var status = this._requestObject.status;
  if (state == 4) {
    if (status == 200) {
      return true;
    }
  }
  return false;
};

// Similar to .isSuccess(), this function will look at the state of the
// request without requiring the request to be completed yet. Eg, this
// can determine if the request is "looking good" while still in progress, eg
// the response headers are 200-OK and the response body is being loaded.
// 
// Returns: True if the request "looks good", false if it does not, or is not
// in progress, or is otherwise not active such as not having been sent yet.
anzovinmod.AsyncRequestObject.prototype.isGood = function() {
  if (!this._requestSent) {
    return false;
  }
  var state = this._requestObject.readyState;
  var status = this._requestObject.status;
  if (state == 2 || state == 3 || state == 4) {
    // todo: add in 2XX codes for cached content for possible type=GET requests?
    if (status == 200) {
      return true;
    }
  }
  return false;
};

// This function reports on whether the requets object is finished sending
// and receiving data. This will only be true when the request has completely
// finished, eg if it is still in progress downloading the response body this
// would still return false.
// 
// Returns: True if the request is finished (either success or failure) or
// false otherwise. If the request has not yet been sent, then this will
// also return false.
anzovinmod.AsyncRequestObject.prototype.isDone = function() {
  if (!this._requestSent) {
    return false;
  }
  var state = this._requestObject.readyState;
  // readyState can = 0 if there is a CORS connection problem, eg the remote
  // server does not support OPTIONS than this will be 0
  return (state == 0 || state == 4);
};

// Returns the response data from a finished (either succeeded or failed) call.
// 
// Returns: A string, or NULL if there is no response data or the response
// is not yet finished.
anzovinmod.AsyncRequestObject.prototype.getResponseData = function() {
  if (this.isDone()) {
    return this._requestObject.responseText;
  }
  return null;
};

// Sends the request with the given content, if applicable for the request
// type. Requests can be reused by simply calling this function whenever one
// wishes for the request to happen.
anzovinmod.AsyncRequestObject.prototype.send = function(content) {
  if (this._requestSent) {
    this._requestObject.abort();
  }
  this._requestSent = true;
  content = anzovinmod.Utilities.defaultParam(content, null);
  this._requestObject.send(content);
};

// Simply sets the request header, exactly like one would do if manually
// interacting with the request object.
// 
// k: The key value for the header.
// v: The value of the header.
anzovinmod.AsyncRequestObject.prototype.setRequestHeader = function(k, v) {
  this._requestObject.setRequestHeader(k, v);
  this._setRequestHeaders.push([k,v]);
};

// Simply returns the ready state of the request object.
// 
// Returns: The 0-5 value of readyState that the internal request object is
// currently within.
anzovinmod.AsyncRequestObject.prototype.getReadyState = function() {
  return this._requestObject.readyState;
};

// Simply returns the status code of the request object, such as 200 or 404. The
// code returned will be 0 if a status code is not yet determined or the
// connection is not yet established, etc.
// 
// Returns: The status code of the request object.
anzovinmod.AsyncRequestObject.prototype.getStatusCode = function() {
  return this._requestObject.status;
};

// Simply returns the user-friendly textual description of the status code
// of the request object. If the status code would be 0, then this is
// an empty string.
// 
// Returns: A string that is the readible version of the status code.
anzovinmod.AsyncRequestObject.prototype.getStatusText = function() {
  return this._requestObject.statusText;
};


anzovinmod.AsyncRequestObject.prototype._timeoutSend = function() {
  this._send();
};

// Just a blank function to override event handlers on the request object, as
// an attempted workaround around IE8 XMLHttpRequest issues.
anzovinmod.AsyncRequestObject._blankFunction = function() {};

}());
/*! Player.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// Constructor. This class is responsible for building and maintaining a player
// for the animation. Functionality for it should be included herein.
// 
// The animations themselves should be constructed well enough to function
// outside of this player. This includes functionality such as self resizing,
// starting playing, play/stop/goto commands (but not buttons for them), etc.
// The player however, can be responsible for those functions as well as other
// ones.
// 
// Structure:
// DOM ELEMENTS              SIZE   POS    TRANSFORM
// <.. parent ..>
//  <div player>             dxd    rel-0
//   <div controls>          0x0    abs-0
//    <div positions>        0x0    abs-d
//   <div animation>         dxd    rel-0
//    <form animation>       0x0    abs-0
//     <div form elements>   0x0    abs-0  yes
//     <canvas>              dxd    rel-0
// 
// A couple of notes about the player controls.
// 
// The pause button can be used to pause (or continue) the animation being
// played. The pause mechanism is different from stopping or playing the
// animation via .stop() or .play(). The pause mechanism hard-pauses the
// animation and sound effects, regardless of the stop/play state.
// 
// parent: The HTML DOM element that will be the parent of the player. Elements
// such as wrappers, player controls, and the animation itself will be created
// as children to this parent element. If null or unspecified, then the document
// body will be used instead.
// replacementContainer: The replacement container to use for this
// player instance.
anzovinmod.Player = function(parent, replacementContainer) {
  parent = anzovinmod.Utilities.defaultParam(parent, null);
  replacementContainer = anzovinmod.Utilities.defaultParam(replacementContainer, null);
  
  // UNIQUE ID
  
  // The unique ID for this player instance.
  this._id = null;
  
  // DISPLAY AND DOM ELEMENTS
  
  // The parent element.
  this._parent = parent;
  // The DIV wrapper element, created by this class and added to the parent
  // node, that contains the player and animation, along with anything else
  // as seen fit.
  this._divWrapper = null;
  // A DIV wrapper for all the player control elements.
  this._divPlayerControls = null;
  
  // OTHER CONTROLS
  
  // Whether this is a mobile device or not. This will be set when the player
  // is loaded through an IsMobile parameter through the replacement variable
  // container, though this can also be changed through an appropriate
  // function call.
  this._isMobile = false;
  // Whether this is a mobile app or not. This will be set/reset when the
  // appropriate function is called.
  this._isMobileApp = false;
  // A list of controls and any set callbacks on those controls. This is
  // used to allow external entities to set callback on control elements.
  this._controlCallbacks = {};
  // A window resize callback function instance, saved so we can attach it and
  // unbind it at will.
  this._windowResizeCallbackFunction = null;
  // A timeout callback function that is called after a short period of time
  // when the window has its focus lost. If the window is refocused before this
  // timeout triggers, the timeout is canceled. This is to get around the lack
  // of a proper focus/blur callback support in IE8/9, which makes ANY
  // action on a page count as both a blur/focus at the same time.
  this._windowBlurTimeout = null;
  // The window blur timeout function that is actually called when the timeout
  // occurs. This is a variable here to prevent too many function()s from
  // being created during normal runtime.
  this._windowBlurTimeoutFunction = anzovinmod.Utilities.bind(this, this._windowBlurTimeoutCallback);
  
  // CONFIGURATIONS
  
  // The configuration for the placement of the player controls within the
  // div wrappers. Each player control should only be included once in this
  // list.
  this._playerControlConfiguration = {
    "frame":        ["loading-frame", "main-pause-frame", "loading-login"],
    "center":       ["main-begin-play", "loading", "loading-text"],
    "top":          ["top-bar", "back-empty"],
    "left":         [],
    "bottom":       ["bottom-bar"],
    "right":        [],
    "top-left":     ["back"],
    "top-right":    ["login"],
    "bottom-left":  ["pause", "replay", "nodenav-back", "nodenav-forward"],
    "bottom-right": ["volume", "volume-slider"]
  };
  // The actual player controls.
  this._playerControls = {};
  // The replacement container for this instance.
  this._replacementContainer = replacementContainer;
  
  // RESIZING
  
  // The width that was explicitely used as the parameter to resize() last. Or,
  // the default value for the class instance if not yet set.
  this._calledWidth = "window";
  // the height that was explicitely used as the parameter to resize() last. Or,
  // the default value for the class instance if not yet set.
  this._calledHeight = "window";
  // The current display width. This can be equal to the canvas size or window
  // size if "window" or "canvas" was used as part of the resize process, or
  // an explicitely defined size passed in.
  this._currentWidth = null;
  // The current display height. This can be equal to the canvas size or window
  // size if "window" or "canvas" was used as part of the resize process, or
  // an explicitely defined size passed in.
  this._currentHeight = null;
  // The current css size declaration. This may be changed whenever this player
  // is resized.
  this._currentCssSize = null;
  
  // ANIMATION
  
  // A reference to the animation instance that this player is responsible for.
  this._mainTimeline = null;
  // A reference to the div wrapper for the main timeline instance.
  this._mainTimelineDivWrapper = null;
  // Whether this is a flash or canvas instance.
  this._isFlash = false;
  // If flash, whether the flash animation has finished loading.
  this._flashLoaded = false;
  // Scenes to attach to the flash animation when it loads.
  this._flashAttachScenes = [];
  
  // CONTROLS
  
  // An indicator of whether the UI is currently shown or not.
  this._uiControlsShown = true;
  // Whether the UI elements can be hidden.
  this._allowHidingUIElements = {
    "playing": false,
    "ended": true,
    "extraheight": true
  };
  // A timer that is triggered if the mouse hasn't been moved in a certain
  // interval. Its job is to hide the UI.
  this._lastMouseMovementTimer = null;
  // The length of time between the last mouse movement and the timer for it.
  this._lastMouseMovementTimerLength = 3000;
  // This is a function that should be used in the timer. It is included here
  // so that it can be reused instead of rebound each time the timer needs to
  // be reset.
  this._lastMouseMovementTimerFunction = null;
  
  // The last timestamp of the last percent update function. This is used to
  // keep the number of CSS updates occurring during the animation, when the
  // UI is visible, to a minimum. This is especially important (apparently)
  // during IE8, which seems to be very slow at it.
  this._lastPercentUpdateTime = null;
  // The last value updated at the last timestamp updated. This is used to
  // manage when the percent value wraps to a new progress node, since the
  // value of the new percent will be less than the value of the old percent.
  this._lastPercentUpdateValue = 0;
  // The last percent value passed in to this player. Separate from the last
  // update value just to keep track of the actual playback percent for
  // various purposes.
  this._lastPercentUpdateRealValue = 0;
  // The interval to determine how frequently to update the jump node percent
  // values. A value of zero means constant updating without the need to
  // calculate times at each interval.
  this._lastPercentUpdateInterval = 1000;
  
  // LOADING
  
  // If there is a poster image, then this is a reference to it. It will be
  // displayed before the animation is displayed.
  this._posterImage = null;
  // Flash needs a callback to call when it is finished loading. This provides
  // that callback, already bound to the scope of this player instance.
  this._flashLoadBoundCallback = null;
  // A time indicating the "start" of the animation loading process.
  this._animationStartLoadTime = new Date();
  
  this._init();
};

// This function is simply the initializer for the constructor. It is called to
// perform any work that is not simply setting default values for the class
// properties.
anzovinmod.Player.prototype._init = function() {
  // Reference to this instance for closuring with.
  var player = this;
  
  // Set unique ID.
  this._id = anzovinmod.Player.uniqueIds.length + 1;
  anzovinmod.Player.uniqueIds.push({"id":this._id, "player":this});
  
  // Create and set the main player div wrapper.
  this._divWrapper = document.createElement("div");
  this._divWrapper.setAttribute("class", "anzovinmod-player");
  if (this._parent == null) {
    this._parent = document.body;
  }
  this._parent.appendChild(this._divWrapper);
  
  // Create and set the player controls wrapper.
  this._divPlayerControls = document.createElement("div");
  this._divPlayerControls.setAttribute("class", "player-controls");
  this._divWrapper.appendChild(this._divPlayerControls);
  
  // Create the per-corner player control div elements.
  for (var position in this._playerControlConfiguration) {
    if (!this._playerControlConfiguration.hasOwnProperty(position)) {
      continue;
    }
    var positionElements = this._playerControlConfiguration[position];
    var positionDiv = document.createElement("div");
    if (position == "frame") {
      positionDiv.setAttribute("class", "frame");
    } else {
      positionDiv.setAttribute("class", "controls controls-" + position);
    }
    this._divPlayerControls.appendChild(positionDiv);
    for (var i = 0; i < positionElements.length; ++i) {
      var controlName = positionElements[i];
      var controlDiv = document.createElement("div");
      { // split the control name by spaces, and assign each element to a
        // css clas name
        var names = controlName.split(" ");
        var className = "control";
        for (var j = 0; j < names.length; ++j) {
          className += " control-" + names[j];
        }
        controlDiv.setAttribute("class", className);
      }
      positionDiv.appendChild(controlDiv);
      this._playerControls[controlName] = controlDiv;
      var divanzovinmod = {
        "state":false,
        "control-name":controlName
      };
      anzovinmod.Utilities.setDomElementProperty(controlDiv, "_anzovinmod", divanzovinmod);
      controlDiv.onclick = anzovinmod.Utilities.bind(this, this._controlClickCallback);
      // specific initialization for controls 
      switch (controlName) {
        case "login":
          var div = document.createElement("div");
          div.setAttribute("class", "text");
          div.innerHTML = "Login";
          anzovinmod.Utilities.setDomElementProperty(div, "_anzovinmod", divanzovinmod);
          controlDiv.appendChild(div);
          break;
        case "back":
        case "pause":
        case "replay":
        case "nodenav-back":
        case "nodenav-forward":
        case "volume":
        case "main-pause":
        case "main-begin-play":
          var div = document.createElement("div");
          div.setAttribute("class", "icon");
          anzovinmod.Utilities.setDomElementProperty(div, "_anzovinmod", divanzovinmod);
          controlDiv.appendChild(div);
          break;
        case "loading":
          var img = document.createElement("img");
          img.setAttribute("class", "loading-image");
          img.src = "../images/loading-size-l.gif";
          controlDiv.appendChild(img);
          break;
        case "loading-text":
          var div = document.createElement("div");
          div.innerHTML = "Loading";
          controlDiv.appendChild(div);
          break;
        case "volume-slider":
          var input = document.createElement("input");
          input.setAttribute("type", "range");
          input.setAttribute("min", "0.0");
          input.setAttribute("max", "1.0");
          input.setAttribute("step", "0.01");
          input.setAttribute("value", "0.5");
          controlDiv.appendChild(input);
          divanzovinmod["rangeslider-input"] = input;
          divanzovinmod["rangeslider-instance"] = new rangeslider(input, {
            "polyfill": false,
            "onSlide": function(p,v) {
              if (player && player._mainTimeline && typeof player._mainTimeline.setVolume !== "undefined") {
                player._mainTimeline.setVolume(v);
              }
            },
            "onSlideEnd": function(p,v) {
              if (player && player._mainTimeline && typeof player._mainTimeline.setVolume !== "undefined") {
                player._mainTimeline.setVolume(v);
              }
            },
            "rangeClass": "rangeslider",
            "fillClass": "fill",
            "handleClass": "handle",
            "extraSelectClass": "extraselect"
          });
          break;
      }
    }
  }
  
  // Create replacement container if not passed in.
  if (this._replacementContainer == null) {
    this._replacementContainer = new anzovinmod.ReplacementVariableContainer();
  }
  
  // Create an animation instance.
  if (anzovinmod.MainTimeline) {
    this._initCanvasMainTimeline();
  } else {
    // flash (doesn't get truly initialized until the stage is started, after
    // attaching scenes)
    this._isFlash = true;
  }
  
  // Attach the on-mouse-move and timeout handlers. But don't set the timeout,
  // as we're starting the animation in a "loading" state, and won't hide the
  // UI until loading finishes and the animation is playing.
  this._divWrapper.onmousemove = anzovinmod.Utilities.bind(this, this._playerMouseMoveHandler);
  this._lastMouseMovementTimerFunction = anzovinmod.Utilities.bind(this, this._playerMouseMoveTimeout);
  
  // Create a poster image if there is one.
  if (this._replacementContainer.has("PosterImageURL")) {
    var posterImageURL = this._replacementContainer.get("PosterImageURL");
    var img = document.createElement("img");
    img.setAttribute("class", "poster");
    // using an onload should help prevent issues with it not loading and
    // being presented to the screen in a bad manner. didn't notice this at all,
    // but thinking ahead
    var wrapper = this._divWrapper;
    img.onload = function() {
      wrapper.appendChild(img);
    };
    img.src = posterImageURL;
    this._posterImage = img;
  }
  
  // Set "is mobile"ness of the player instance.
  if (this._replacementContainer.has("IsMobile")) {
    this.isMobile(this._replacementContainer.get("IsMobile") == "1");
  }
  
  // Add resize listeners.
  this._windowResizeCallbackFunction = anzovinmod.Utilities.bind(this, this._windowResizeCallback);
  if (window.addEventListener) {
    window.addEventListener("resize", this._windowResizeCallbackFunction);
  } else if (window.attachEvent) {
    window.attachEvent("onresize", this._windowResizeCallbackFunction);
  }
  
  // Setup window focus/blur callbacks.
  this._setupWindowFocusBlurCallbacks();
  
  // Setup unhandled stage clicks.
  if (!this._isFlash) {
    this._mainTimeline.addUnhandledMouseClickCallback(anzovinmod.Utilities.bind(this, this._unhandledStageMouseClick));
  }
};

// Just a pass through command, to start the manifest loading process.
anzovinmod.Player.prototype.startManifestLoading = function() {
  this._mainTimeline.startManifestLoading();
};

// This function is called whenever there is an unhandled stage mouse click
// event.
// 
// evt: The mouse click event that triggered this function call.
anzovinmod.Player.prototype._unhandledStageMouseClick = function(evt) {
  if (this._isMobile) {
    if (this._allowHidingUIElements && this._uiControlsShown) {
      this._hideControlsAndCleanupTimeoutTimer();
    } else {
      this._showControlsAndResetMovementTimeoutTimer();
    }
  }
};

// This function simply pauses the current given animation, if possible.
anzovinmod.Player.prototype.pauseAnimation = function() {
  if (this._playerControls.hasOwnProperty("pause")) {
    var pauseControl = this._playerControls["pause"];
    var divanzovinmod = anzovinmod.Utilities.getDomElementProperty(pauseControl, "_anzovinmod");
    if (!divanzovinmod["state"]) {
      this._controlClickHandle(this._playerControls["pause"], "pause");
    }
  }
};

// This function just sets up the window focus/blur events where appropriate,
// so that the animation pauses if sufficient focus is lost.
// 
// Notes:
// IE9: window.focus and window.blur are called when the window loses focus.
// this occurs when you click away from the window, change tab, or minimize.
// Also uses other calls: document.focusout>window.focusout>window.blur when
// selecting away from the window; document.focusin>window.focusin>window.focus
// when selecting into the window. Also, when selecting from the document body
// to the flash element: document.focusout>window.focusout>document.focusin>
// window.focusin. This doesn't appear to give us correct behavior, unless we
// can track the targets of the focusing or the active elements of the page in
// order to know if focus is truly lost.
// IE8: Only document.onfocusout when leaving window and document.onfocusin when
// returning to window. Problem is, these are also called in quick
// succession when moving to a different element within the same document!
// So document.onfocusout>document.onfocusin, even when just clicking on the
// same element on a page. We'd have to set up a short interval callback that is
// cleared immediately after the focus event or something, because otherwise
// every click would be treated as a blur/focus.
anzovinmod.Player.prototype._setupWindowFocusBlurCallbacks = function() {
  if ("hidden" in document) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._setupWindowFocusBlurCallbacks", "setting up callback with 'hidden' support");
    this._attachFocusBlurListener(document, "visibilitychange", this._windowVisibilityChangeCallback);
  } else if ("mozHidden" in document) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._setupWindowFocusBlurCallbacks", "setting up callback with 'mozHidden' support");
    this._attachFocusBlurListener(document, "mozvisibilitychange", this._windowVisibilityChangeCallback);
  } else if ("webkitHidden" in document) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._setupWindowFocusBlurCallbacks", "setting up callback with 'webkitHidden' support");
    this._attachFocusBlurListener(document, "webkitvisibilitychange", this._windowVisibilityChangeCallback);
  } else if ("msHidden" in document) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._setupWindowFocusBlurCallbacks", "setting up callback with 'msHidden' support");
    this._attachFocusBlurListener(document, "msvisibilitychange", this._windowVisibilityChangeCallback);
  } else if (anzovinmod.Utilities.IsIE9()) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._setupWindowFocusBlurCallbacks", "setting up callback without 'hidden' support, for IE9 specifically");
    this._attachFocusBlurListener(window, "focus", this._windowFocusCallback);
    this._attachFocusBlurListener(window, "blur", this._windowBlurCallback);
  } else if (anzovinmod.Utilities.IsIE8()) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._setupWindowFocusBlurCallbacks", "setting up callback without 'hidden' support, for IE8 specifically");
    this._attachFocusBlurListener(document, "onfocusin", this._windowFocusCallback);
    this._attachFocusBlurListener(document, "onfocusout", this._windowBlurCallback);
  } else {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._setupWindowFocusBlurCallbacks", "setting up callback without 'hidden' support, other");
    this._attachFocusBlurListener(document, "focus", this._windowFocusCallback);
    this._attachFocusBlurListener(document, "blur", this._windowBlurCallback);
    this._attachFocusBlurListener(document, "onfocusin", this._windowFocusCallback);
    this._attachFocusBlurListener(document, "onfocusout", this._windowBlurCallback);
    this._attachFocusBlurListener(document, "focusin", this._windowFocusCallback);
    this._attachFocusBlurListener(document, "focusout", this._windowBlurCallback);
    this._attachFocusBlurListener(document, "pageshow", this._windowFocusCallback);
    this._attachFocusBlurListener(document, "pagehide", this._windowBlurCallback);
  }
};

// This function attaches a document or window focus/blur/visibility change
// event listeners.
// 
// elem: The element to attach to.
// event: The event name to trigger on.
// cb: The callback function to use (attached to this).
anzovinmod.Player.prototype._attachFocusBlurListener = function(elem, event, cb) {
  elem = anzovinmod.Utilities.defaultParam(elem, null);
  event = anzovinmod.Utilities.defaultParam(event, null);
  cb = anzovinmod.Utilities.defaultParam(cb, null);
  if (elem == null || event == null || cb == null) {
    return;
  }
  if (elem.addEventListener) {
    elem.addEventListener(event, anzovinmod.Utilities.bind(this, cb));
  } else if (elem.attachEvent) {
    elem.attachEvent(event, anzovinmod.Utilities.bind(this, cb));
  }
};

// This function is called whenever a window/document gains or loses
// visibility. This occurs when the tab is changed or the window is
// minimized, but not when it is obscured by another window or screen.
// The function must determine whether the window/document is visible or
// hidden, and act accordingly.
anzovinmod.Player.prototype._windowVisibilityChangeCallback = function() {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._windowVisibilityChangeCallback", "visibility changed callback");
  var isHidden = false;
  if ("hidden" in document) {
    isHidden = document["hidden"];
  } else if ("mozHidden" in document) {
    isHidden = document["mozHidden"];
  } else if ("webkitHidden" in document) {
    isHidden = document["webkitHidden"];
  } else if ("msHidden" in document) {
    isHidden = document["msHidden"];
  }
  if (isHidden) {
    this.pauseAnimation();
  }
};

// This function is called whenever a window gains focus. This is separate from
// the visibility change callback, which is called whenever a visibility change
// occurs, and it must determine whether the window/document is visible or
// hidden.
anzovinmod.Player.prototype._windowFocusCallback = function() {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._windowFocusCallback", "focused callback");
  if (this._windowBlurTimeout != null) {
    clearTimeout(this._windowBlurTimeout);
    this._windowBlurTimeout = null;
  }
};

// This function is called whenever a window loses focus. This is separate from
// the visibility change callback, which is called whenever a visibility change
// occurs, and it must determine whether the window/document is visible or
// hidden.
anzovinmod.Player.prototype._windowBlurCallback = function() {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._windowBlurCallback", "blurred callback");
  if (this._windowBlurTimeout == null) {
    this._windowBlurTimeout = setTimeout(this._windowBlurTimeoutFunction, 1);
  }
};

// This function is called when the window/document has blurred and has not yet
// come back into focus before the timeout has triggered. This function is
// therefore responsible for assuming that the window/document has indeed been
// blurred and to do any appropriate handling.
anzovinmod.Player.prototype._windowBlurTimeoutCallback = function() {
  this.pauseAnimation();
};

// This function is a callback for when the window is resized.
anzovinmod.Player.prototype._windowResizeCallback = function() {
  this.resize();
};

// This function can be called by anything using the player instance, to tell it
// that the player should be configured for a mobile device layout and
// anything relevant.
// 
// isMobile: True if this should be set to be a mobile layout, false if it
// should not be set.
anzovinmod.Player.prototype.isMobile = function(isMobile) {
  isMobile = anzovinmod.Utilities.defaultParam(isMobile, false, true);
  if (isMobile != this._isMobile) {
    this._isMobile = isMobile;
    var c = this._divWrapper.getAttribute("class");
    var newc = c;
    if (isMobile) {
      newc = anzovinmod.Utilities.addStringComponent(c, "is-mobile");
    } else {
      newc = anzovinmod.Utilities.removeStringComponent(c, "is-mobile");
    }
    if (c != newc) {
      this._divWrapper.setAttribute("class", newc);
    }
  }
};

// This function can be called by anything using the player instance, to tell it
// that the player should be configured for a mobile app layout and anything
// relevant.
// 
// isMobileApp: True if this should be set to be a mobile app layout, false if
// it should not be set.
anzovinmod.Player.prototype.isMobileApp = function(isMobileApp) {
  isMobileApp = anzovinmod.Utilities.defaultParam(isMobileApp, false, true);
  if (isMobileApp != this._isMobileApp) {
    this._isMobileApp = isMobileApp;
    var c = this._divWrapper.getAttribute("class");
    var newc = c;
    if (isMobileApp) {
      newc = anzovinmod.Utilities.addStringComponent(c, "is-mobile-app");
    } else {
      newc = anzovinmod.Utilities.removeStringComponent(c, "is-mobile-app");
    }
    if (c != newc) {
      this._divWrapper.setAttribute("class", newc);
    }
  }
};

// This function can be used to add a callback that is called whenever a
// control is clicked and activated.
// 
// callback: The callback to set. The functional arguments to this callback
// are (player, control), where player=a reference to this player instance, and
// control=the control name, same as the parameter being used here.
// control: The name of the control.
anzovinmod.Player.prototype.addControlCallback = function(callback, control) {
  control = anzovinmod.Utilities.defaultParam(control, null);
  callback = anzovinmod.Utilities.defaultParam(callback, null);
  if (control == null || callback == null) {
    return;
  }
  if (!this._controlCallbacks.hasOwnProperty(control)) {
    this._controlCallbacks[control] = [];
  }
  this._controlCallbacks[control].push(callback);
};

// Removes the indicated control callback. The callback passed in must be the
// same as the one previously added.
// 
// callback: The callback that was previously set, to remove now.
// control: The name of the control. If null, then remove the callback from all
// controls.
anzovinmod.Player.prototype.removeControlCallback = function(callback, control) {
  control = anzovinmod.Utilities.defaultParam(control, null);
  callback = anzovinmod.Utilities.defaultParam(callback, null);
  if (callback == null) {
    return;
  }
  if (control == null) {
    for (var k in this._controlCallbacks) {
      if (!this._controlCallbacks.hasOwnProperty(k)) {
        continue;
      }
      for (var i = 0; i < this._controlCallbacks[k].length; ++i) {
        if (this._controlCallbacks[k][i] == callback) {
          this._controlCallbacks[k].splice(i, 1);
          --i;
        }
      }
    }
  } else {
    if (this._controlCallbacks.hasOwnProperty(control)) {
      for (var i = 0; i < this._controlCallbacks[control].length; ++i) {
        if (this._controlCallbacks[control][i] == callback) {
          this._controlCallbacks[control].splice(i, 1);
          --i;
        }
      }
    }
  }
};

// This function creates a canvas main timeline instance, and applies any
// functions, callbacks, or calls into the timeline instance.
anzovinmod.Player.prototype._initCanvasMainTimeline = function() {
  this._mainTimeline = new anzovinmod.MainTimeline(this._divWrapper, this._replacementContainer);
  this._mainTimelineDivWrapper = this._mainTimeline.getDivWrapper();
  this._mainTimeline.addAnimationEventCallback(
    anzovinmod.Utilities.bind(this, this._handleTriggeredAnimationEvent)
  );
  this._mainTimeline.addStateChangeCallback(
    anzovinmod.Utilities.bind(this, this._mainTimelineStateChangeCallback)
  );
  this._mainTimeline.addPlaybackPercentCallback(
    anzovinmod.Utilities.bind(this, this.setPlaybackPercent)
  );
};

// This function creates a flash main timeline instance, as opposed to the
// canvas one that is created normally.
anzovinmod.Player.prototype._initFlashMainTimeline = function() {
  if (this._flashAttachScenes.length == 0) return;
  var name = this._flashAttachScenes[0]["name"];
  // create the div wrapper
  var divwrapper = document.createElement("div");
  divwrapper.setAttribute("class", "anzovinmod-maintimeline-flash");
  // create the flash 'object' instance
  var objstring = '<object';
  var attributes = {
    "id":"flashInstance",
    "name":"flashInstance",
    "data":"../flash/" + name + ".swf",
    "type":"application/x-shockwave-flash"
  };
  for (var k in attributes) {
    if (!attributes.hasOwnProperty(k)) continue;
    objstring += ' ' + k + '="' + attributes[k] + '"';
  }
  objstring += '>';
  objstring += '<div class="noflash">' +
    '<p>This video requires Adobe Flash Player.</p>' +
    '<p>To watch this video, please install Adobe Flash Player or open this link in a different browser.</p>' +
    '<p><a target="_blank" href="http://get.adobe.com/flashplayer/">Click here to download from Adobe</a></p>' +
    '<a target="_blank" href="http://get.adobe.com/flashplayer/"><img src="../images/getflash.png" width="158px" height="39px"></a>' +
  '</div>';
  // add <param ...> values to the flash object
  var params = {
    "movie":"../flash/" + name + ".swf",
    "quality":"best",
    "bgcolor":"#ffffff",
    "play":"true",
    "loop":"true",
    "wmode":"opaque",
    "scale":"showall",
    "menu":"true",
    "devicefont":"false",
    "salign":"",
    "allowScriptAccess":"always",
    "transparent":"wmode"
  };
  for (var k in params) {
    if (!params.hasOwnProperty(k)) continue;
    objstring += '<param name="' + k + '" value="' + params[k] + '" />';
  }
  objstring += '</object>';
  // append flash object into div wrapper
  divwrapper.innerHTML = objstring;
  var flashobj = divwrapper.children[0];
  // set player instance variables
  this._mainTimeline = flashobj;
  this._mainTimelineDivWrapper = divwrapper;
  this._divWrapper.appendChild(divwrapper);
  // set callbacks and such
  this._flashLoadTimerFunction();
};

// This function callback simply waits for the flash object to be created, then
// performs any initialization upon it. This is needed because the timeline
// instance and our custom variables and external interface methods and such are
// not available until the entire SWF is downloaded and ran.
anzovinmod.Player.prototype._flashLoadTimerFunction = function() {
  if (typeof this._mainTimeline.isState === "undefined") {
    if (this._flashLoadBoundCallback == null) {
      this._flashLoadBoundCallback = anzovinmod.Utilities.bind(this, this._flashLoadTimerFunction);
    }
    setTimeout(this._flashLoadBoundCallback, 100);
    return;
  }
  this._flashLoaded = true;
  for (var i = 0; i < this._flashAttachScenes.length; ++i) {
    this._mainTimeline.setSceneInfo(this._flashAttachScenes[i]);
  }
  this._flashAttachScenes = [];
  this._mainTimeline.setLogLevel(anzovinmod.Logging.getLogLevel());
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._flashLoadTimerFunction", "flash has loaded and has function callbacks");
  this._flashLoadBoundCallback = null;
  this._mainTimeline.addStateChangeCallback(
    "anzovinmod.Player.findPlayerInstance(" + this._id + ") ? " +
    "anzovinmod.Player.findPlayerInstance(" + this._id + ")._mainTimelineStateChangeCallback(#) : " +
    "anzovinmod.Logging.logm(anzovinmod.Logging.LOG_ERROR," +
      "'_','flash callback cannot identify player with id " + this._id + "')"
  );
  this._mainTimeline.addAnimationEventCallback(
    "anzovinmod.Player.findPlayerInstance(" + this._id + ") ? " +
    "anzovinmod.Player.findPlayerInstance(" + this._id + ")._handleTriggeredAnimationEvent(#) : " +
    "anzovinmod.Logging.logm(anzovinmod.Logging.LOG_ERROR," +
      "'_','flash callback cannot identify player with id " + this._id + "')"
  );
  this._mainTimeline.addPlaybackPercentCallback(
    "anzovinmod.Player.findPlayerInstance(" + this._id + ") ? " +
    "anzovinmod.Player.findPlayerInstance(" + this._id + ").setPlaybackPercent(#) : " +
    "anzovinmod.Logging.logm(anzovinmod.Logging.LOG_ERROR," +
      "'_','flash callback cannot identify player with id " + this._id + "')"
  );
  if (this._playerControls.hasOwnProperty("volume-slider")) {
    if (this._playerControls["volume-slider"] != null) {
      this._mainTimeline.setVolume(this._playerControls["volume-slider"].children[0].value);
    }
  }
  this._mainTimeline.setReplacementContainer(this._replacementContainer);
  // we've got to set the states here becaues the flash can't do it on its own:
  // it loads immediately in the loaded state, with no external asset loader,
  // so we have to track it via this load timer callback. we set the same state
  // path that the canvas version would take, so that any callbacks can occur
  // in the correct order
  this._mainTimeline.setStates({"bare":false, "unloaded":true});
  this._mainTimeline.setStates({"unloaded":false, "loading":true});
  this._mainTimeline.setStates({"loading":false, "loaded":true});
  this.resize();
};

// This function callback is called whenever the mtl instance has its state
// changed. This can occur when it is loading, has loaded, starts playing,
// reaches the end of its play cycle, etc.
// 
// states: An array of strings, that are the states that have changed.
anzovinmod.Player.prototype._mainTimelineStateChangeCallback = function(states) {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._mainTimelineStateChangeCallback", "state change callback called by mtl: ", states);
  states = anzovinmod.Utilities.defaultParam(states, [], true);
  var stateValues = {};
  for (var i = 0; i < states.length; ++i) {
    stateValues[states[i]] = this._mainTimeline.isState(states[i]);
  }
  var c;
  var newc;
  for (var i = 0; i < states.length; ++i) {
    switch (states[i]) {
      case "manifestloading":
      case "loading":
        this._resizeLoginButtonOverlay();
        break;
      case "loaded":
        c = this._divWrapper.getAttribute("class");
        newc = anzovinmod.Utilities.addStringComponent(c, "mtl-loaded");
        if (c != newc)
          this._divWrapper.setAttribute("class", newc);
        this._sendAnimationLoadTimeEvent(new Date());
        break;
      case "started":
        c = this._divWrapper.getAttribute("class");
        newc = anzovinmod.Utilities.addStringComponent(c, "mtl-started");
        if (c != newc)
          this._divWrapper.setAttribute("class", newc);
        this.buildJumpBar();
        break;
      case "playing":
        if (stateValues[states[i]]) {
          c = this._divWrapper.getAttribute("class");
          newc = anzovinmod.Utilities.addStringComponent(c, "mtl-playing");
          this._allowHidingUIElements["playing"] = true;
        } else {
          c = this._divWrapper.getAttribute("class");
          newc = anzovinmod.Utilities.removeStringComponent(c, "mtl-playing");
          this._allowHidingUIElements["playing"] = false;
        }
        if (c != newc)
          this._divWrapper.setAttribute("class", newc);
        this._showControlsAndResetMovementTimeoutTimer();
        break;
      case "ended":
        if (stateValues[states[i]]) {
          c = this._divWrapper.getAttribute("class");
          newc = anzovinmod.Utilities.addStringComponent(c, "mtl-ended");
          this._allowHidingUIElements["ended"] = false;
        } else {
          c = this._divWrapper.getAttribute("class");
          newc = anzovinmod.Utilities.removeStringComponent(c, "mtl-ended");
          this._allowHidingUIElements["ended"] = true;
        }
        if (c != newc)
          this._divWrapper.setAttribute("class", newc);
        this._showControlsAndResetMovementTimeoutTimer();
        // set right-hand playback percent if nearly there already
        if (this._lastPercentUpdateRealValue >= 95) {
          this.setPlaybackPercent(100);
        }
        break;
      case "willresize":
        if (this._mainTimeline.isState("willresize")) {
          this.resize();
          this._mainTimeline.setStates({"willresize":false, "resize":false}, false);
        }
        break;
      case "hardpaused":
        // we need to edit the states of some of the buttons in order to let
        // them function correctly with the jump node unpause feature
        var isHardPaused = this._mainTimeline.isState("hardpaused");
        var divStates = ["pause", "main-pause", "main-pause-frame"];
        for (var j = 0; j < divStates.length; ++j) {
          // simple check to ensure we have a div handle
          var divState = null;
          if (this._playerControls.hasOwnProperty(divStates[j])) {
            divState = this._playerControls[divStates[j]];
          }
          if (divState == null) continue;
          // contains 'state' property which tells us the current 'active' state
          var divProps = anzovinmod.Utilities.getDomElementProperty(divState, "_anzovinmod");
          c = divState.getAttribute("class");
          newc = c;
          // edit state if necessary, continue if not
          if (isHardPaused == divProps["state"]) {
            continue;
          } else if (isHardPaused) {
            newc = anzovinmod.Utilities.addStringComponent(c, "active");
          } else {
            newc = anzovinmod.Utilities.removeStringComponent(c, "active");
          }
          divProps["state"] = isHardPaused;
          divState.setAttribute("class", newc);
        }
        break;
    }
  }
};

// This function is called whenever the mouse if moved over the player. Its job
// is to show the UI if it is hidden.
// 
// evt: The mouse move event.
anzovinmod.Player.prototype._playerMouseMoveHandler = function(evt) {
  if (this._uiControlsShown || !this._isMobile) {
    this._showControlsAndResetMovementTimeoutTimer();
  }
};

// This function is called whenever the mouse hasn't been moving over the player
// for the designated amount of time. Its job is to hide the UI.
anzovinmod.Player.prototype._playerMouseMoveTimeout = function() {
  this._hideControlsAndCleanupTimeoutTimer();
};

// This function shows the controls if they are hidden, and sets (or resets)
// the movement timeout timer.
anzovinmod.Player.prototype._showControlsAndResetMovementTimeoutTimer = function() {
  if (this._lastMouseMovementTimer != null) {
    clearTimeout(this._lastMouseMovementTimer);
    this._lastMouseMovementTimer = null;
  }
  if (!this._uiControlsShown) {
    this._divPlayerControls.setAttribute("class",
      anzovinmod.Utilities.removeStringComponent(
        this._divPlayerControls.getAttribute("class"),
        "no-controls"
      )
    );
    this._uiControlsShown = true;
  }
  if (this._mainTimeline != null) {
    var canHide = true;
    canHide = canHide && this._mainTimeline != null;
    canHide = canHide && (typeof this._mainTimeline.isState !== "undefined");
    canHide = canHide && !this._mainTimeline.isState("hardpaused");
    for (var k in this._allowHidingUIElements) {
      if (!this._allowHidingUIElements.hasOwnProperty(k)) continue;
      canHide = canHide && this._allowHidingUIElements[k];
    }
    if (canHide) {
      this._lastMouseMovementTimer = setTimeout(
        this._lastMouseMovementTimerFunction,
        this._lastMouseMovementTimerLength
      );
    }
  }
};

// This function hides the controls if they are shown, and cleans up the
// timer tracking code so that the timer ID is not used anymore.
anzovinmod.Player.prototype._hideControlsAndCleanupTimeoutTimer = function() {
  if (this._uiControlsShown) {
    var canHide = true;
    canHide = canHide && (typeof this._mainTimeline.isState) !== "undefined";
    canHide = canHide && !this._mainTimeline.isState("hardpaused");
    for (var k in this._allowHidingUIElements) {
      if (!this._allowHidingUIElements.hasOwnProperty(k)) continue;
      canHide = canHide && this._allowHidingUIElements[k];
    }
    if (canHide) {
      this._divPlayerControls.setAttribute("class",
        anzovinmod.Utilities.addStringComponent(
          this._divPlayerControls.getAttribute("class"),
          "no-controls"
        )
      );
      this._uiControlsShown = false;
    }
  }
  if (this._lastMouseMovementTimer != null) {
    clearTimeout(this._lastMouseMovementTimer);
    this._lastMouseMovementTimer = null;
  }
};

// This function sends an animation load timer event to the event log, if it
// is setup to do so.
// 
// date: A new Date() instance that is the time of the load to send.
anzovinmod.Player.prototype._sendAnimationLoadTimeEvent = function(date) {
  date = anzovinmod.Utilities.defaultParam(date, null);
  var dateDifference = null;
  if (date != null) {
    dateDifference = date.getTime() - this._animationStartLoadTime.getTime();
  }
  this._handleTriggeredAnimationEvent("LoadTimer", dateDifference);
};

// This function is called whenever the animation instance sends a triggered
// animation event. Primarily these are used for logging currently.
// 
// event: The name of the event.
// msg: The message to send along with the animation event, or null if nothing.
anzovinmod.Player.prototype._handleTriggeredAnimationEvent = function(event, msg) {
  try {
    this._sendEventLog(event, msg);
  } catch (e) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_ERROR, "anzovinmod.Player._handleTriggeredAnimationEvent", "exception occurred while trying to send log event: ", e);
  }
};

// This function just sends an event log request.
// 
// event: The name of the event.
// msg: The log message content to prime (if any).
anzovinmod.Player.prototype._sendEventLog = function(event, msg) {
  event = anzovinmod.Utilities.defaultParam(event, null);
  msg = anzovinmod.Utilities.defaultParam(msg, null);
  var ro = new anzovinmod.AsyncRequestObject(
    "POST",
    this._replacementContainer.get("EventLogURL")
  );
  ro.setRequestHeader("APIKey", this._replacementContainer.get("ApiKey"));
  ro.setRequestHeader("Content-Type", "application/json");
  ro.setRequestHeader("AuthHash", this._replacementContainer.get("AuthHash"));
  // build data
  var data = {
    "EmployerId": this._replacementContainer.get("EmployerId"),
    "EventId": 0,
    "EventName": event,
    "ExperienceUserId": this._replacementContainer.get("ExperienceUserId"),
    "LogComment": msg,
    "ContentId": null
  };
  switch (event) {
    case "StartAnimation":        data["EventId"] = 4; break;
    case "EndAnimation":          data["EventId"] = 5; break;
    case "StartQuiz":             data["EventId"] = 6; break;
    case "EndQuiz":               data["EventId"] = 7; break;
    case "HelpfulYes":            data["EventId"] = 13; break;
    case "HelpfulNo":             data["EventId"] = 14; break;
    case "AuthenticationSuccess": data["EventId"] = 15; break;
    case "ReplayAnimation":       data["EventId"] = 17; break;
    case "GoToCch":               data["EventId"] = 18; break;
    case "GoToPlan":              data["EventId"] = 19; break;
    case "Error":                 data["EventId"] = 22; break;
    case "LoadTimer":             data["EventId"] = 24; break;
    default:
      data["EventId"] = 22;
      data["EventName"] = "Error";
      if (data["LogComment"] == null) data["LogComment"] = '';
      else data["LogComment"] += ";";
      data["LogComment"] += "no event named " + event;
      break;
  }
  // send data
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._sendEventLog", "logging triggered animation event: ", event, " ", msg);
  anzovinmod.Logging.logo(anzovinmod.Logging.LOG_DEBUG, ro);
  anzovinmod.Logging.logo(anzovinmod.Logging.LOG_DEBUG, data);
  ro.send(JSON.stringify(data));
};

// This function is called whenever one of the control div elements is clicked.
// 
// evt: The click event. evt.target is the control div in question.
anzovinmod.Player.prototype._controlClickCallback = function(evt) {
  // note: sometimes the rangeslider will send a click callback from the
  // slider control to this function (don't know why). just checking for the
  // existence of _anzovinmod first seems to be enough to catch this condition.
  // note that other buttons work as expected, and the rangeslider's volume
  // control works as expected, there's just an erroneous event being sent to
  // this function
  // note also, that IE8 doesn't sent an evt on mouseclicks, however there is
  // a global event that can be used instead. note also that we use the
  // anzovinmod.Utilities.IsIE8 property. we could also do this by adjusting
  // the check to 'if (typeof evt === "undefined")', but this is more specific
  if (anzovinmod.Utilities.IsIE8()) {
    evt = window.event;
    var target = evt.srcElement;
    if (target == null) return;
    if (target.getAttribute("class") == "rangeslider") {
      target = target.parentNode;
      if (target == null) return;
    }
    var attr = anzovinmod.Utilities.getDomElementProperty(target, "_anzovinmod");
    if (attr == null) return;
    this._controlClickHandle(target, attr["control-name"]);
  } else {
    if (evt.target == null) return;
    if (!evt.target.hasOwnProperty("_anzovinmod")) return;
    this._controlClickHandle(evt.target, evt.target._anzovinmod["control-name"]);
  }
};

// This function is responsible for performing click events tied to the
// player's control elements.
// 
// controlElement: The actual DOM element of the control.
// control: The name of the control that was clicked.
anzovinmod.Player.prototype._controlClickHandle = function(controlElement, control) {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._controlClickHandle", "click registered on control: ", control);
  controlElement = anzovinmod.Utilities.defaultParam(controlElement, null);
  control = anzovinmod.Utilities.defaultParam(control, null);
  if (controlElement == null) return;
  if (control == null) return;
  if (!this._playerControls.hasOwnProperty(control)) {
    if (control != "jumpnode") {
      anzovinmod.Logging.logm(anzovinmod.Logging.LOG_WARN, "anzovinmod.Player._controlClickHandle", "no owned control of name: ", control);
      return;
    }
  }
  // a reference to the main control
  var controlDiv = this._playerControls[control];
  var targetState = false;
  if (controlDiv != null)
    var targetState = !(anzovinmod.Utilities.getDomElementProperty(controlDiv, "_anzovinmod")["state"]);
  // a list of the controls to change their states of (for linked controls)
  var doStates = [];
  // an indicator of the control callback name to send, this is not always
  // exactly the clicked control name, especially for instances where there are
  // "support" control elements, like the main pause frame
  var controlCallbackName = null;
  // which states to modify
  switch (control) {
    case "volume":
      doStates = ["volume"];
      break;
    case "pause":
    case "main-pause":
    case "main-pause-frame":
      if (this._mainTimeline.isState("playing")) {
        doStates = ["pause", "main-pause", "main-pause-frame"];
      }
      break;
    case "begin-play":
    case "main-begin-play":
      if (this._mainTimeline.isState("loaded") && !this._mainTimeline.isState("playing")) {
        doStates = ["begin-play", "main-begin-play"];
      }
      break;
  }
  // modify states
  for (var i = 0; i < doStates.length; ++i) {
    var doState = doStates[i];
    switch (doState) {
      case "volume":
      case "pause":
      case "main-pause":
      case "main-pause-frame":
        if (!this._playerControls.hasOwnProperty(doState)) continue;
        var stateDiv = this._playerControls[doState];
        // reverse state
        var divProps = anzovinmod.Utilities.getDomElementProperty(stateDiv, "_anzovinmod");
        divProps["state"] = targetState;
        // set class
        if (divProps["state"]) {
          stateDiv.setAttribute("class",
            anzovinmod.Utilities.addStringComponent(
              stateDiv.getAttribute("class"), "active"
            )
          );
        } else {
          stateDiv.setAttribute("class",
            anzovinmod.Utilities.removeStringComponent(
              stateDiv.getAttribute("class"), "active"
            )
          );
        }
        break;
    }
  }
  // perform main external action
  switch (control) {
    case "login":
    case "loading-login":
      var url = null;
      var config = this._mainTimeline.getLoginButtonConfig();
      if (config != null && config.hasOwnProperty("url") && config["url"] != null) {
        url = config["url"];
      }
      if (url == null) url = "http://www.clearcosthealth.com/";
      this._handleTriggeredAnimationEvent("GoToCch", null);
      window.open(url, "_blank");
      break;
    case "volume":
      controlCallbackName = "volume";
      anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._controlClick", "volume [un-]muting");
      if (targetState) {
        this._mainTimeline.setMute(true);
      } else {
        this._mainTimeline.setMute(false);
      }
      break;
    case "pause":
    case "main-pause":
    case "main-pause-frame":
      controlCallbackName = "pause";
      anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._controlClick", "playback [un-]pausing");
      if (this._mainTimeline.isState("playing")) {
        this._mainTimeline.setState("hardpaused", targetState);
      }
      break;
    case "begin-play":
    case "main-begin-play":
      controlCallbackName = "begin-play";
      anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._controlClick", "playback starting");
      if (this._mainTimeline.isState("loaded") && !this._mainTimeline.isState("playing")) {
        if (this._playerControls.hasOwnProperty("volume-slider")) {
          var anz = anzovinmod.Utilities.getDomElementProperty(
            this._playerControls["volume-slider"],
            "_anzovinmod"
          );
          if (anz["rangeslider-instance"]) {
            anz["rangeslider-instance"].setValue(0.5, true);
            anz["rangeslider-instance"].update();
          }
        }
        this._mainTimeline.startAnimation();
      }
      break;
    case "replay":
      controlCallbackName = "replay";
      anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._controlClick", "playback replaying");
      if (this._mainTimeline.isState("ended")) {
        this._mainTimeline.replayAnimation();
      }
      break;
    case "nodenav-back":
      controlCallbackName = "nodenav-back";
      anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._controlClick", "playback jumping back a node");
      if (this._mainTimeline.isState("started")) {
        this._mainTimeline.goNodeBack();
      }
      break;
    case "nodenav-forward":
      controlCallbackName = "nodenav-forward";
      anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._controlClick", "playback jumping forward a node");
      if (this._mainTimeline.isState("started")) {
        this._mainTimeline.goNodeForward();
      }
      break;
    case "jumpnode":
      if (!this._isMobile) {
        controlCallbackName = "jumpnode";
        anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player._controlClick", "playback jumping to a node");
        if (this._mainTimeline.isState("started")) {
          var anz = anzovinmod.Utilities.getDomElementProperty(controlElement, "_anzovinmod");
          this._mainTimeline.goNodeTo(anz["control-value"]);
        }
      }
      break;
    case "back":
      controlCallbackName = "back";
      break;
  }
  // show UI and reset timer
  this._showControlsAndResetMovementTimeoutTimer();
  // send control callback
  if (controlCallbackName != null) {
    if (this._controlCallbacks.hasOwnProperty(controlCallbackName)) {
      for (var i = 0; i < this._controlCallbacks[controlCallbackName].length; ++i) {
        this._controlCallbacks[controlCallbackName][i](this, controlCallbackName);
      }
    }
  }
};

// Simply returns a handle to the main timeline instance.
// 
// Returns: The anzovinmod.MainTimeline instance attached to this player.
anzovinmod.Player.prototype.getMainTimeline = function() {
  return this._mainTimeline;
};

// Resize the player to the given size. The values passed in to this function
// can take on several values and meanings, including the ability to auto-resize
// in certain situations.
// 
// The input parameters are basically the same as those of the main timeline
// class.
// 
// Note that if either 'width' or 'height' parameters are not specified, then
// the last entered values (or their default class values) will be used to
// trigger a resizing event. Eg, if both are set to 'window', then just calling
// resize() would be the same as calling resize('window', 'window').
// 
// width: One of the above values to use for the width of the player.
// height: One of the above values to use for the height of the player.
anzovinmod.Player.prototype.resize = function(width, height) {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.Player.resize", "resizing player to (", width, ",", height, ")");
  width = anzovinmod.Utilities.defaultParam(width, this._calledWidth, true);
  height = anzovinmod.Utilities.defaultParam(height, this._calledHeight, true);
  // save currently-called parameters for later use
  var calledWidth = width;
  var calledHeight = height;
  // use "canvas" values if needed. uses the px size of the canvas if it is
  // currently defined, else use "100%" as fallback, which is parent context
  width = (width != "canvas" ? width :
    (this._mainTimeline._canvasWidth != null ? this._mainTimeline._canvasWidth : "100%")
  );
  height = (height != "canvas" ? height :
    (this._mainTimeline._canvasHeight != null ? this._mainTimeline._canvasHeight : "100%")
  );
  // get calculated resize values, before any scaling. this is really just to
  // simplify the cases of "window" and "xxx%" and turn them into hard pixels.
  // this is not the FINAL size, just the calculated max size based on the
  // simplifications
  var simplifiedSize = anzovinmod.Utilities.simplifyPotentialResizeValues(
    this._parent, width, height
  );
  // calculate canvas scale. this uses the simplified size values as the
  // bounding size for the scaling
  var canvasScale = 1.0;
  var canvasSize = null;
  if (this._mainTimeline != null) {
    if (typeof this._mainTimeline.getCanvasSize !== "undefined") {
      canvasSize = this._mainTimeline.getCanvasSize();
      if (canvasSize != null) {
        if (canvasSize["width"] != null && canvasSize["height"] != null) {
          canvasScale = anzovinmod.Utilities.calculateBoundScale(
            simplifiedSize["width"],
            simplifiedSize["height"],
            canvasSize["width"],
            canvasSize["height"],
            this._mainTimeline.getResizeBehaviors()
          );
        }
      }
    }
  }
  // set current display pixel size. if there's a canvas, use the scaled values
  // from that, otherwise use the simplified values
  var displayWidth = simplifiedSize["width"];
  var displayHeight = simplifiedSize["height"];
  if (canvasSize != null) {
    if (canvasSize["width"] != null && canvasSize["height"] != null) {
      displayWidth = canvasSize["width"] * canvasScale;
      displayHeight = canvasSize["height"] * canvasScale;
    }
  }
  // set values
  this._setResize(calledWidth, calledHeight, displayWidth, displayHeight, canvasScale);
};

// This function is the final stage in resizing. It takes all the data of the
// resize calculations, and sets all the necessary internal data structures
// in order to properly resize everything.
// 
// This function is separate so as to be able to manually set the resize sizes.
// This allows for testing, as well as for interactions with an external
// (or internal) wrapping/wrapped element that can just directly set the
// resize values after handling resizing on its own.
// 
// calledWidth: The called width that was passed in to the resize function.
// calledHeight: The called height that was passed in to the resize function.
// displayWidth: The final resulting horizontal size to display this animation.
// displayHeight: Same as previous.
// canvasScale: The scale of the canvas to use. Normally, this is fixed to fit
// within the displayWidth/Height.
anzovinmod.Player.prototype._setResize = function(calledWidth, calledHeight, displayWidth, displayHeight, canvasScale) {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.Player._setResize", "called(", calledWidth, ",", calledHeight, ") display(", displayWidth, ",", displayHeight, ") scale(", canvasScale, ")");
  calledWidth = anzovinmod.Utilities.defaultParam(calledWidth, null);
  calledHeight = anzovinmod.Utilities.defaultParam(calledHeight, null);
  displayWidth = anzovinmod.Utilities.defaultParam(displayWidth, null);
  displayHeight = anzovinmod.Utilities.defaultParam(displayHeight, null);
  canvasScale = anzovinmod.Utilities.defaultParam(canvasScale, null);
  if (calledWidth == null || calledHeight == null) {
    return;
  }
  this._calledWidth = calledWidth;
  this._calledHeight = calledHeight;
  if (displayWidth == null || displayHeight == null || canvasScale == null) {
    return;
  }
  this._currentWidth = displayWidth;
  this._currentHeight = displayHeight;
  this._divWrapper.style.width = displayWidth + "px";
  this._divWrapper.style.height = displayHeight + "px";
  if (this._mainTimeline != null) {
    if (!this._isFlash) {
      this._mainTimeline.setSize(calledWidth, calledHeight, displayWidth, displayHeight, canvasScale);
    } else {
      this._mainTimeline.setAttribute("width", displayWidth + "px");
      this._mainTimeline.setAttribute("height", displayHeight + "px");
    }
  }
  // set the css size declaration, if it has changed at all
  var cssSize = "size-";
  if (displayWidth > 1400) {
    cssSize += "m";
  } else {
    cssSize += "s";
  }
  if (this._currentCssSize == null || this._currentCssSize != cssSize) {
    var c = this._divWrapper.getAttribute("class");
    var newc = c;
    if (this._currentCssSize != null) {
      newc = anzovinmod.Utilities.removeStringComponent(newc, this._currentCssSize);
    }
    newc = anzovinmod.Utilities.addStringComponent(newc, cssSize);
    if (c != newc) {
      this._divWrapper.setAttribute("class", newc);
    }
    this._currentCssSize = cssSize;
  }
  // resize the login button overlay
  this._resizeLoginButtonOverlay();
  // update the rangeslider input
  if (this._playerControls.hasOwnProperty("volume-slider")) {
    var controlDiv = this._playerControls["volume-slider"];
    var anz = anzovinmod.Utilities.getDomElementProperty(controlDiv, "_anzovinmod");
    if (anz != null && anz.hasOwnProperty("rangeslider-input")) {
      if (anz["rangeslider-instance"]) {
        anz["rangeslider-instance"].update();
      }
    }
  }
  // update the loading image size if it needs to be adjusted
  var loadingDiv = null;
  if (this._playerControls.hasOwnProperty("loading")) {
    loadingDiv = this._playerControls["loading"];
  }
  if (loadingDiv != null) {
    var loadingImage = loadingDiv.children[0];
    var currentSrc = loadingImage.src;
    var newSrc = "../images/loading-" + cssSize + ".gif";
    if (currentSrc != newSrc) {
      loadingImage.src = newSrc;
    }
  }
  // update the poster image
  if (this._posterImage != null) {
    this._posterImage.style.width = displayWidth + "px";
    this._posterImage.style.height = displayHeight + "px";
  }
  // update the extra positions of the ui elements
  var windowSize = anzovinmod.Utilities.getWindowSize();
  var extraHeight = {"top":0, "bottom":0};
  var maxExtraHeight = {"top":0, "bottom":0};
  if (windowSize["height"] > displayHeight) {
    switch (cssSize) {
      case "size-l":
        maxExtraHeight["top"] = 80;
        maxExtraHeight["bottom"] = 100;
        break;
      case "size-m":
        maxExtraHeight["top"] = 80;
        maxExtraHeight["bottom"] = 70;
        break;
      case "size-s":
        maxExtraHeight["top"] = 60;
        if (this._isMobile) {
          maxExtraHeight["bottom"] = 60;
        } else {
          maxExtraHeight["bottom"] = 48;
        }
        break;
    }
    var maxExtraHeightBoth = maxExtraHeight["top"] + maxExtraHeight["bottom"];
    var extraWindowHeight = windowSize["height"] - displayHeight;
    if (extraWindowHeight >= maxExtraHeightBoth) {
      extraHeight["top"] = maxExtraHeight["top"];
      extraHeight["bottom"] = maxExtraHeight["bottom"];
      // show UI continuously
      this._allowHidingUIElements["extraheight"] = false;
      this._showControlsAndResetMovementTimeoutTimer();
    } else {
      this._allowHidingUIElements["extraheight"] = true;
      this._showControlsAndResetMovementTimeoutTimer();
    }
  } else {
    this._allowHidingUIElements["extraheight"] = true;
    this._showControlsAndResetMovementTimeoutTimer();
  }
  if (!this._isFlash) {
    this._mainTimeline.getDivWrapper().style.top = extraHeight["top"] + "px";
  } else {
    if (this._mainTimeline != null) {
      this._mainTimeline.parentNode.style.top = extraHeight["top"] + "px";
    }
  }
  this._divWrapper.style.height = (displayHeight + extraHeight["top"] + extraHeight["bottom"]) + "px";
};

// Builds the jump bar, if possible, and places it in the UI. CSS can
// control if/how it is displayed. The jump buttons themselves will be
// constructed as normal, but hidden if there is no jump bar in use in the
// animation.
// 
// Returns:
// True if the bar is built, false if it is not built.
anzovinmod.Player.prototype.buildJumpBar = function() {
  var progressNodes = this._mainTimeline.getProgressNodes();
  if (progressNodes == null || progressNodes.length == 0) return false;
  // sort nodes based on position (fixes index display issue in ie/flash)
  var sortedProgressNodes = [];
  for (var k in progressNodes) {
    if (!progressNodes.hasOwnProperty(k)) continue
    var progressNode = progressNodes[k];
    if (!progressNode.hasOwnProperty("position")) continue;
    if (progressNode["position"] == null) continue;
    var added = false;
    for (var i = 0; i < sortedProgressNodes.length; ++i) {
      var sortedProgressNode = sortedProgressNodes[i];
      if (sortedProgressNode["position"] >= progressNode["position"]) continue;
      sortedProgressNodes.push(progressNode);
      var lastIndex = sortedProgressNodes.length - 1;
      for (var j = i; j < lastIndex; ++j) {
        var node = sortedProgressNodes[j];
        sortedProgressNodes[j] = sortedProgressNodes[lastIndex];
        sortedProgressNodes[lastIndex] = node;
      }
      added = true;
      break;
    }
    if (!added) sortedProgressNodes.push(progressNode);
  }
  progressNodes = sortedProgressNodes;
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.Player.buildJumpBar", "ordered nodes: ", progressNodes);
  // create wrapper div elements
  var mainWrapper = document.createElement("div");
  mainWrapper.setAttribute("class", "control jumpcontrols");
  this._playerControls["jumpbar"] = mainWrapper;
  var secondWrapper = document.createElement("div");
  secondWrapper.setAttribute("class", "jumpcontrols-wrapped");
  mainWrapper.appendChild(secondWrapper);
  { // build the path pieces, three are needed in order to properly get the
    // left and right curved border working. can be max-width'ed in order to
    // fit properly
    var pathLeft = document.createElement("div");
    pathLeft.setAttribute("class", "path-left");
    secondWrapper.appendChild(pathLeft);
    var pathCenter = document.createElement("div");
    pathCenter.setAttribute("class", "path-center");
    secondWrapper.appendChild(pathCenter);
    var pathRight = document.createElement("div");
    pathRight.setAttribute("class", "path-right");
    secondWrapper.appendChild(pathRight);
  }
  { // build the overlay path
    var pathLeft = document.createElement("div");
    pathLeft.setAttribute("class", "overpath-left");
    secondWrapper.appendChild(pathLeft);
    var pathCenter = document.createElement("div");
    pathCenter.setAttribute("class", "overpath-center");
    secondWrapper.appendChild(pathCenter);
    var pathRight = document.createElement("div");
    pathRight.setAttribute("class", "overpath-right");
    secondWrapper.appendChild(pathRight);
    this._playerControls["jumpbar-overlaypath-center"] = pathCenter;
    this._playerControls["jumpbar-overlaypath-right"] = pathRight;
  }
  var numButtons = 0;
  { // build the buttons. one for each progress node point that has a position
    // attribute that's non-null
    var buttonWrapper = document.createElement("div");
    buttonWrapper.setAttribute("class", "button-wrapper");
    secondWrapper.appendChild(buttonWrapper);
    var buttons = document.createElement("div");
    buttons.setAttribute("class", "buttons");
    buttonWrapper.appendChild(buttons);
    for (var i = 0; i < progressNodes.length; ++i) {
      var progressNode = progressNodes[i];
      if (!progressNode.hasOwnProperty("position")) continue;
      if (progressNode["position"] == null) continue;
      var button = document.createElement("div");
      var divanzovinmod = {
        "state":false,
        "control-name":"jumpnode",
        "control-value":progressNode["name"]
      };
      { // set attribute on the button
        button.setAttribute("class", "button");
        button.style.left = progressNode["position"] + "%";
        anzovinmod.Utilities.setDomElementProperty(button, "_anzovinmod", divanzovinmod);
        button.setAttribute("position", progressNode["position"]);
        button.setAttribute("isactive", 0);
      }
      { // main button display circle
        var buttonCircle = document.createElement("div");
        buttonCircle.setAttribute("class", "button-circle");
        button.appendChild(buttonCircle);
      }
      { // button click area, overlayed on top of progress bar overlay
        var buttonClickArea = document.createElement("div");
        buttonClickArea.setAttribute("class", "button-click-area");
        var f = anzovinmod.Utilities.bind(this, this._controlClickCallback);
        buttonClickArea.onclick = f;
        anzovinmod.Utilities.setDomElementProperty(buttonClickArea, "_anzovinmod", divanzovinmod);
        button.appendChild(buttonClickArea);
      }
      buttons.appendChild(button);
      ++numButtons;
    }
    this._playerControls["jumpbar-buttons"] = buttons;
  }
  if (numButtons == 0) return false;
  // put this jump bar in one specific control area
  for (var i = 0; i < this._divPlayerControls.children.length; ++i) {
    var controls = this._divPlayerControls.children[i];
    if (anzovinmod.Utilities.hasStringComponent(controls.getAttribute("class"), "controls-bottom")) {
      controls.appendChild(mainWrapper);
      return true;
    }
  }
  return false;
};

// Attaches the indicated scene to the main timeline instance. In the case
// of a canvas animation, it just passes in the name to the mtl. In the case
// of a flash animation, it just passes in the config object to the mtl.
// 
// sceneName: The name of the scene to attach.
anzovinmod.Player.prototype.attachScene = function(sceneName) {
  if (!this._isFlash) {
    this._mainTimeline.attachScene(sceneName);
  } else {
    var sceneInfo = anzovinmod.instance.Scenes[sceneName]["properties"];
    if (sceneInfo != null) {
      if (this._flashLoaded) {
        this._mainTimeline.setSceneInfo(sceneInfo);
      } else {
        this._flashAttachScenes.push(sceneInfo);
      }
    }
  }
};

// Just triggers the stage start, if it is not started already.
anzovinmod.Player.prototype.startStage = function() {
  if (!this._isFlash) {
    this._mainTimeline.startStage();
  } else {
    this._initFlashMainTimeline();
  }
};

// This function sets the playback percent amount. This is used to affect
// the progress node jump bar as of now, but might affect other things later.
// 
// The number passed in is a float number. It represents the current position of
// the progress bar that should be filled in. This already takes into account
// the jump node positional adjustments. If the position is equal to one of the
// jump node positions, then that jump node can be considered to be "reached".
// 
// Alternatively, you could grab the progress node stack of the mtl instance and
// just make any nodes on that stack "active".
// 
// percent: A number between 0 and 100 to indicate the playback percent amount.
anzovinmod.Player.prototype.setPlaybackPercent = function(percent) {
  percent = anzovinmod.Utilities.defaultParam(percent, null);
  if (percent == null) return;
  var currentTime = new Date().getTime();
  var doUpdate = !(this._lastPercentUpdateInterval > 0);
  // snap value
  if (percent <= 0) {
    percent = 0;
    doUpdate = true;
  } else if (percent >= 100) {
    percent = 100;
    doUpdate = true;
  }
  // only update in certain circumstances to reduce load
  if (!doUpdate) {
    // force an update if the x-ms interval has passed
    if (this._lastPercentUpdateInterval == 0 || this._lastPercentUpdateTime == null || currentTime - this._lastPercentUpdateTime >= this._lastPercentUpdateInterval) {
      doUpdate = true;
    }
    if (!doUpdate) {
      // determine if there is a new active node between the last percent time
      // and the current percent time
      if (this._playerControls.hasOwnProperty("jumpbar-buttons")) {
        var nodes = this._playerControls["jumpbar-buttons"];
        for (var i = 0; i < nodes.children.length; ++i) {
          var node = nodes.children[i];
          if (this._lastPercentUpdateValue < node.getAttribute("position") && node.getAttribute("position") <= percent) {
            doUpdate = true;
            break;
          }
        }
      }
    }
  }
  // skip update if necessary
  this._lastPercentUpdateRealValue = percent;
  if (!doUpdate) {
    return;
  }
  // set update values for next iteration
  if (this._lastPercentUpdateInterval > 0) {
    this._lastPercentUpdateTime = currentTime;
    this._lastPercentUpdateValue = percent;
  }
  // set the manual length of the overlay path
  if (this._playerControls.hasOwnProperty("jumpbar-overlaypath-center")) {
    var path = this._playerControls["jumpbar-overlaypath-center"];
    if (path.style.width != percent + "%") {
      path.style.width = percent + "%";
    }
  }
  if (this._playerControls.hasOwnProperty("jumpbar-overlaypath-right")) {
    var path = this._playerControls["jumpbar-overlaypath-right"];
    if (path.style.left != percent + "%") {
      path.style.left = percent + "%";
    }
  }
  // manage nodeovers
  if (this._playerControls.hasOwnProperty("jumpbar-buttons")) {
    var nodes = this._playerControls["jumpbar-buttons"];
    for (var i = 0; i < nodes.children.length; ++i) {
      var node = nodes.children[i];
      if (node.getAttribute("position") > percent) {
        if (node.getAttribute("isactive") == 1) {
          var c = node.getAttribute("class");
          var newc = anzovinmod.Utilities.removeStringComponent(c, "active");
          if (c != newc)
            node.setAttribute("class", newc);
          node.setAttribute("isactive", 0);
        }
      } else {
        if (node.getAttribute("isactive") == 0) {
          var c = node.getAttribute("class");
          var newc = anzovinmod.Utilities.addStringComponent(c, "active");
          if (c != newc)
            node.setAttribute("class", newc);
          node.setAttribute("isactive", 1);
        }
      }
    }
  }
};

// Resizes a login button overlay.
anzovinmod.Player.prototype._resizeLoginButtonOverlay = function() {
  if (this._currentWidth == null || this._currentHeight == null)
    return;
  if (!this._playerControls.hasOwnProperty("loading-login"))
    return;
  var controlDiv = this._playerControls["loading-login"];
  var config = null;
  if (this._mainTimeline != null) {
    config = this._mainTimeline.getLoginButtonConfig();
  }
  // set up default values
  var scaleX = 1.0;
  var scaleY = 1.0;
  var width = 0;
  var height = 0;
  var x = 0;
  var y = 0;
  // use config values if present
  if (config != null) {
    scaleX = this._currentWidth / config["imageSizeWidth"];
    scaleY = this._currentHeight / config["imageSizeHeight"];
    width = config["width"];
    height = config["height"];
    x = config["x"];
    y = config["y"];
  }
  controlDiv.style.width = (scaleX * width) + "px";
  controlDiv.style.height = (scaleY * height) + "px";
  controlDiv.style.left = (scaleX * x) + "px";
  controlDiv.style.top = (scaleY * y) + "px";
};

// Static function. Is called from the flash main timeline in order to use and
// identify an individual player instance. There really should only be one
// player instance on a page, but just in case this ever changes, might as well
// write it in such a way now. The whole project thus far has been written with
// this in mind anyways.
// 
// id: The unique ID to the player instance (generated on player construction).
// 
// Returns:
// A reference to the player object if there is an ID match, otherwise NULL.
anzovinmod.Player.findPlayerInstance = function(id) {
  for (var i = 0; i < anzovinmod.Player.uniqueIds.length; ++i) {
    var uniqueId = anzovinmod.Player.uniqueIds[i];
    if (uniqueId["id"] == id) {
      return uniqueId["player"];
    }
  };
  return null;
};

// Static variable, of the player instance identifiers.
// Elements are: {id:#,player:obj}
anzovinmod.Player.uniqueIds = [];

}());
/*! ReplacementVariableContainer.js */
var anzovinmod = anzovinmod || {};
(function() {
"use strict";

// A general constructor.
anzovinmod.ReplacementVariableContainer = function() {
  // An associative-array (object) of identifiers and values. These get set
  // when added to this container.
  this._adds = {};
  // A list of the default values, to use if an identifier is not defined
  // for 'adds' arguments.
  this._defs = {};
};

// Adds a value to this replacement container. If the replacement string
// has not yet been replaced, then it will not set the value in this container.
// 
// Note that the 'id' must not be null. However, the value can be anything
// (except undefined).
// 
// Note also, that while 'check' is optional, if it is null and 'value' is also
// null, then no value will be set.
// 
// id: The identifier for the element being added to this.
// value: The value that should be set.
// check: This is an optional string that, if it exactly matches the 'value',
// then this adding of the value should be ignored. This means that the
// replacement was not made.
anzovinmod.ReplacementVariableContainer.prototype.add = function(id, value, check) {
  id = anzovinmod.Utilities.defaultParam(id, null);
  value = anzovinmod.Utilities.defaultParam(value, null);
  check = anzovinmod.Utilities.defaultParam(check, null);
  if (id == null) return;
  if (check != value) this._adds[id] = value;
};

// Sets a default value for any non-added (eg, check/valued) identifiers.
// .has() and .get() will first read from .add()'ed identifiers, then check
// .def()'ed identifiers if none are found.
// 
// id: The identifier to set a default value for.
// value: The value to set.
anzovinmod.ReplacementVariableContainer.prototype.def = function(id, value) {
  id = anzovinmod.Utilities.defaultParam(id, null);
  value = anzovinmod.Utilities.defaultParam(value, null);
  if (id == null) return;
  this._defs[id] = value;
};

// Determines if the given identifier is .add()'ed or .def()'ed to
// this container.
// 
// id: The identifier to determine if it exists within this container.
// 
// Returns: True if it exists, false otherwise.
anzovinmod.ReplacementVariableContainer.prototype.has = function(id) {
  id = anzovinmod.Utilities.defaultParam(id, null);
  if (id == null) return false;
  return this.hasAdd(id) || this.hasDef(id);
};

// Same as .has(), except this only checks the .add()'ed variables.
//
// id: The identifier to determine if it exists within this container.
// 
// Returns: True if it exists, false otherwise.
anzovinmod.ReplacementVariableContainer.prototype.hasAdd = function(id) {
  id = anzovinmod.Utilities.defaultParam(id, null);
  if (id == null) return false;
  return this._adds.hasOwnProperty(id);
};

// Same as .has(), except this only checks the .add()'ed variables.
//
// id: The identifier to determine if it exists within this container.
// 
// Returns: True if it exists, false otherwise.
anzovinmod.ReplacementVariableContainer.prototype.hasDef = function(id) {
  id = anzovinmod.Utilities.defaultParam(id, null);
  if (id == null) return false;
  return this._defs.hasOwnProperty(id);
};

// Retrieves the given value from this container. Retrieves values that are
// .add()'ed first, and if not defined there, retrieves values that have
// been .def()'ed.
// 
// id: The identifier to retrieve its value of.
// 
// Returns: The value of the identifier's storage, or null if it is not
// present.
anzovinmod.ReplacementVariableContainer.prototype.get = function(id) {
  id = anzovinmod.Utilities.defaultParam(id, null);
  if (id == null) return;
  if (this.hasAdd(id)) return this._adds[id];
  if (this.hasDef(id)) return this._defs[id];
  return null;
};

// Same as .get(), except this only checks the .add()'ed variables.
//
// id: The identifier to get from this container.
// 
// Returns: Whatever value was previously stored, or null.
anzovinmod.ReplacementVariableContainer.prototype.getAdd = function(id) {
  id = anzovinmod.Utilities.defaultParam(id, null);
  if (id == null) return;
  if (this.hasAdd(id)) return this._adds[id];
  return null;
};

// Same as .get(), except this only checks the .def()'ed variables.
//
// id: The identifier to get from this container.
// 
// Returns: Whatever value was previously stored, or null.
anzovinmod.ReplacementVariableContainer.prototype.getDef = function(id) {
  id = anzovinmod.Utilities.defaultParam(id, null);
  if (id == null) return;
  if (this.hasDef(id)) return this._defs[id];
  return null;
};

// Adds variables obtained via a window hash to the list of .add()'ed
// parameters.
// 
// hash: A window hash.
anzovinmod.ReplacementVariableContainer.prototype.addWindowHash = function(hash) {
  var values = this._parseWindowHash(hash);
  for (var i = 0; i < values.length; ++i) {
    this._adds[values[i][0]] = values[i][1];
  }
};

// Adds variables obtained via a window hash to the list of .def()'ed
// parameters.
// 
// hash: A window hash.
anzovinmod.ReplacementVariableContainer.prototype.defWindowHash = function(hash) {
  var values = this._parseWindowHash(hash);
  for (var i = 0; i < values.length; ++i) {
    this._defs[values[i][0]] = values[i][1];
  }
};

// This function parses a window hash "#foo=bar&q=asdf" and returns an array
// of [0]/[1] k/v values representing the variable assignments stored within.
// 
// hash: A window hash.
// 
// Returns: An array. [0] of each element is the key, [1] is the value.
anzovinmod.ReplacementVariableContainer.prototype._parseWindowHash = function(hash) {
  hash = anzovinmod.Utilities.defaultParam(hash, null);
  var ret = [];
  if (hash == null || hash == "" || hash == "#") return ret;
  var x = hash.substr(1);
  var xarr = x.split('&');
  for (var i = 0; i < xarr.length; ++i) {
    var yarr = xarr[i].split('=');
    if (yarr.length >= 2) {
      var first = yarr.shift();
      var second = yarr.join('=');
      ret.push([
        decodeURIComponent(first),
        decodeURIComponent(second)
      ]);
    }
  }
  for (var i = 0; i < ret.length; ++i) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.ReplacementVariableContainer._parseWindowHash", ret[i][0], ret[i][1]);
  }
  return ret;
};

}());
