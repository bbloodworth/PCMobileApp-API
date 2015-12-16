/*! TimelineFindChildren.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// Constructor, this should prevent instantiation. Also provides a base to
// create static methods/properties.
anzovinmod.TimelineFindChildren = function() {
  throw "cannot instantiate anzovinmod.TimelineFindChildren";
};

// This function parses all of the children on the given object (optionally
// recursively) and gives then their appropriate names. This makes their names
// equal to their property values on the parent object.
// 
// So, if an object o has a child-property "o.textInstance", then the value
// "o.textInstance.name" will become equal to "textInstance". Note that this
// will also remove any "_#" identifiers when setting the "name" property.
// 
// Normally, at least in Flash Pro's output, elements are given properties
// like "o.text_4", and they are not given any .name value, so it is just
// null. When you manually specify a name for an element it is given the
// property like "o.manualName" and it is given a .name value equal to
// the name given. As such, if an element has a .name value, that will be used
// explicitely and without modification, instead of calculating a name based
// on the property name of the child element.
// 
// o: The object to parse children of and give names to.
// doRecurse: Default true. If true, then also parse and children objects that
// are movieclips or containers.
anzovinmod.TimelineFindChildren.namifyAllChildren = function(o, doRecurse) {
  o = anzovinmod.Utilities.defaultParam(o, null);
  doRecurse = anzovinmod.Utilities.defaultParam(doRecurse, true);
  if (o == null) {
    return;
  }
  anzovinmod.Logging.logit(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.TimelineFindChildren.namifyAllChildren() starting to namify children on object ", o);
  var childProperties = anzovinmod.Utilities.listChildDisplayObjectProperties(o);
  for (var i = 0; i < childProperties.length; ++i) {
    var obj = o[childProperties[i]];
    if (obj == null) {
      continue;
    }
    if (obj.name == null) {
      var objName = childProperties[i];
      objName = anzovinmod.TimelineFindChildren._removeUnderscoreInteger(objName);
      try {
        obj.name = objName;
      } catch (err) {
        anzovinmod.Logging.logit(anzovinmod.Logging.LOG_ERROR, "anzovinmod.TimelineFindChildren.namifyAllChildren() exception when attempting to access property '", childProperties[i], "' on object ", o, ", consider adding to anzovinmod.Utilities.CreateJSObjects.*.nonObjectAttributes");
        throw err;
      }
    }
    if (doRecurse && (obj instanceof createjs.MovieClip || obj instanceof createjs.Container)) {
      anzovinmod.TimelineFindChildren.namifyAllChildren(obj, doRecurse);
    }
  }
};

// Finds any children in the currently loaded stage's scene elements
// that have a name that matches any of the given name.
// 
// See the AS code for additional comments, for this function and others.
// 
// One point to make here, is that the AS version can ONLY search for elements
// that are on the current scene and playing, while this version can search
// for elements regardless of whether the scene is playing or if the display
// objects are shown or not. As this is a different behavior, such functionality
// can be enabled through an additional parameter passed in to this function.
// 
// It should also be mentioned however, that the JS main timeline class needs
// this behavior to do its own things, such as performing the registered
// callbacks. This is because there is no (current) callback method or event
// for the adding of items to the stage, and even if there were, they are not
// dynamically created but are constant.
// 
// Also note, that the addition of the property 'mtl' does not really
// differentiate to use cases of this function. It is only designed to be used
// on a MainTimeline (MovieClip) object in AS, of which the MainTimeline class
// in JS emulates. It is a separate parameter however, because 'o' and 'mtl' are
// the same object in AS, but different objects in JS.
// 
// n: The array of names to search for. Or, a String representing a single
// name to search for globally. Or, an object to use of already-parsed array
// names.
// o: The root stage object to find children from. Is a movieclip or container
// object.
// mtl: The MainTimeline class instance that contains the stage objects.
// full: Default false. If true, then search all scenes and displayed objects,
// not just those that are on the current scene and display.
// 
// Returns: An array of zero or more elements.
anzovinmod.TimelineFindChildren.findChildren = function(n, o, mtl, full) {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.TimelineFindChildren.findChildren", "starting search in ", o, " for ", n, " with mtl ", mtl, " as full ", full);
  n = anzovinmod.Utilities.defaultParam(n, null);
  o = anzovinmod.Utilities.defaultParam(o, null);
  mtl = anzovinmod.Utilities.defaultParam(mtl, null);
  full = anzovinmod.Utilities.defaultParam(full, false, true);
  if (n == null || o == null || mtl == null) {
    return [];
  }
  // convert to standard types
  var useN = anzovinmod.TimelineFindChildren._optimizeN(n);
  if (typeof(n) == "string") {
    useN = anzovinmod.TimelineFindChildren._translateN([useN]);
  } else if (n instanceof Array) {
    useN = anzovinmod.TimelineFindChildren._translateN(useN);
  }
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_TRACE, "anzovinmod.TimelineFindChildren.findChildren", "translated to: ", useN);
  // parse out only this scene when searching
  var currentScene = mtl.getCurrentScene();
  var thisSceneN = {};
  for (var s in useN) {
    if (!useN.hasOwnProperty(s)) continue;
    if (s == "*" || (currentScene != null && currentScene.name == s)) {
      anzovinmod.TimelineFindChildren._mergeN(thisSceneN, useN[s]);
    }
  }
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_TRACE, "anzovinmod.TimelineFindChildren.findChildren", "searching with names object ", thisSceneN);
  var ret = anzovinmod.TimelineFindChildren._recurse(thisSceneN, o, full);
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_TRACE, "anzovinmod.TimelineFindChildren.findChildren", "found: ", ret);
  return ret;
};

// Simply takes an identifier like "name_1" and removes the underscore plus
// integer to return "name". This is used in situations where Canvas will have
// the same named identifier, Flash Pro just creates iterations of the name
// with this pattern.
// 
// x: A full identifier, such as "name_1".
// 
// Returns: The identifier with a "_#" removed from the end. If it does not
// exactly match, then the original value x is returned.
anzovinmod.TimelineFindChildren._removeUnderscoreInteger = function(x) {
  x = anzovinmod.Utilities.defaultParam(x, null);
  if (x == null) {
    return x;
  }
  var xSplit = x.split("_");
  if (xSplit.length <= 1) {
    return x;
  }
  var xSplitEnd = xSplit[xSplit.length - 1];
  var xSplitEndInt = parseInt(xSplitEnd);
  if (!isNaN(xSplitEndInt) && xSplitEndInt.toString() === xSplitEnd) {
    xSplit.pop();
    return xSplit.join("_");
  }
  return x;
};

// A sub-function of findChildren(), this one operates on movieclips or
// containers that are a part of the main timeline.
// 
// n: A search object of names to search for.
// o: The container object to search for matching elements in.
// full: Default false. If true, then search all scenes and displayed objects,
// not just those that are on the current scene and display.
// 
// Returns: An array of zero or more elements, of matching elements from the
// container o. o itself is never included in this return list.
anzovinmod.TimelineFindChildren._recurse = function(n, o, full) {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_TRACE_2, "anzovinmod.TimelineFindChildren._recurse", "recursing search ", n, " for ", o);
  n = anzovinmod.Utilities.defaultParam(n, null);
  o = anzovinmod.Utilities.defaultParam(o, null);
  full = anzovinmod.Utilities.defaultParam(full, false, true);
  var ret = [];
  if (n == null || o == null) return ret;
  // get a list of the children to search
  var childrenToSearch = [];
  if (full) {
    // get a list of all the child properties that are display objects. this
    // utility function strips out unowned properties and createjs/anzovinmod
    // specific properties, and leaves only the display object children
    var childProperties = anzovinmod.Utilities.listChildDisplayObjectProperties(o);
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_TRACE_2, "anzovinmod.TimelineFindChildren._recurse", "found in ", o, " children ", childProperties);
    for (var i = 0; i < childProperties.length; ++i) {
      childrenToSearch.push(o[childProperties[i]]);
    }
  } else {
    for (var i = 0; i < o.children.length; ++i) {
      childrenToSearch.push(o.children[i]);
    }
  }
  for (var i = 0; i < childrenToSearch.length; ++i) {
    var obj = childrenToSearch[i];
    // just a null catch (in AS the timeline of the thrown events can cause
    // this to be null, but it really shouldn't be null here)
    if (obj == null) continue;
    // get the object name. NULL is NOT a valid name, as all displayobjects
    // created by flash pro have names! usually they are of the form
    // text_1, text_2, etc, but the namify function should have standardized
    // all the names for us
    var objName = obj.name;
    if (objName == null) continue;
    var isMatch = anzovinmod.TimelineFindChildren._isMatchN(n, objName);
    if (isMatch) ret.push(obj);
    if (obj instanceof createjs.MovieClip || obj instanceof createjs.Container) {
      var childSearchObject = anzovinmod.TimelineFindChildren._parseN(n, objName);
      ret = ret.concat(anzovinmod.TimelineFindChildren._recurse(childSearchObject, obj, full));
    }
  }
  return ret;
};

// Returns true or false if the given name x matches the current level of
// name search object n. Will match 'name', '*', '**.name', or '**.*'
// elements.
// 
// n: The current level search object.
// x: The string name to match against. Can be null, in which case only
// * will match.
// 
// Returns: True/False if there is a match on this search level.
anzovinmod.TimelineFindChildren._isMatchN = function(n, x) {
  n = anzovinmod.Utilities.defaultParam(n, null);
  x = anzovinmod.Utilities.defaultParam(x, null);
  if (n == null) {
    return false;
  }
  // match name
  if (x != null && n.hasOwnProperty(x) && n[x] != null && n[x].hasOwnProperty(null)) {
    return true;
  }
  // match *
  if (n.hasOwnProperty("*") && n["*"] != null && n["*"].hasOwnProperty(null)) {
    return true;
  }
  // match **
  if (n.hasOwnProperty("**") && n["**"] != null) {
    // match **.name
    if (x != null && n["**"].hasOwnProperty(x) && n["**"][x] != null && n["**"][x].hasOwnProperty(null)) {
      return true;
    }
    // match **.*
    if (n["**"].hasOwnProperty("*") && n["**"]["*"] != null && n["**"]["*"].hasOwnProperty(null)) {
      return true;
    }
  }
  return false;
};

// Translates the list of names n into an object structure. This includes
// s as the first component of the structure.
// 
// n: An array of names to translate.
// 
// Returns: The list of names translated into an object structure.
anzovinmod.TimelineFindChildren._translateN = function(n) {
  n = anzovinmod.Utilities.defaultParam(n, null);
  var ret = {};
  if (n == null) {
    return ret;
  }
  for (var i = 0; i < n.length; ++i) {
    var parts = n[i].split(".");
    var retPart = ret;
    var part = null;
    var partsLength = parts.length;
    while (parts.length > 0) {
      // newPart so we can ignoer any empty string component and not
      // affect other processing in any way
      var newPart = parts.shift();
      if (newPart == "") {
        newPart = null;
      }
      if (newPart == null) {
        continue;
      }
      part = newPart;
      // simply create a new {} or select an existing one
      if (!retPart.hasOwnProperty(part)) {
        retPart[part] = {};
      }
      retPart = retPart[part];
    }
    // ensure that "s" only does not result in a null=null
    if (partsLength > 1) {
      retPart[null] = null;
    }
  }
  return ret;
};

// Takes an existing n names search object, a current object x that may be
// represented by any component of the search object, and returns a new search
// object that contains the search elements that pertain to x. This
// includes any search objects that may be *, **, or exactly x.
// 
// n: An object of names having been parsed into a structure already.
// x: The name of the current child element to parse out of n.
// 
// Returns: A new object structure parsed from n, relating to possible child
// matches using component x. As the objects are slightly modified in order
// to cosntruct the most concise and accurate selector, this is newly created
// from clones of parts of n, so any modifications to not affect the
// original n.
anzovinmod.TimelineFindChildren._parseN = function(n, x) {
  anzovinmod.Logging.logit(anzovinmod.Logging.LOG_TRACE+1, "anzovinmod.TimelineFindChildren._parseN() parsing ", n, " with ", x);
  n = anzovinmod.Utilities.defaultParam(n, null);
  x = anzovinmod.Utilities.defaultParam(x, null);
  var newSearchN = {};
  if (n == null || x == null) {
    return newSearchN;
  }
  for (var nE in n) {
    // nE = element of n (first order array selector)
    // nEE = element of element of n (secodn order array selector)
    if (!n.hasOwnProperty(nE)) {
      continue;
    }
    if (nE == "**") {
      anzovinmod.TimelineFindChildren._mergeNE(newSearchN, n, nE);
      for (var nEE in n[nE]) {
        if (!n[nE].hasOwnProperty(nEE)) {
          continue;
        }
        if (nEE == "*" || nEE == x) {
          anzovinmod.TimelineFindChildren._mergeN(newSearchN, n[nE][nEE]);
        }
      }
    } else if (nE == "*" || nE == x) {
      anzovinmod.TimelineFindChildren._mergeN(newSearchN, n[nE]);
    }
  }
  anzovinmod.Logging.logit(anzovinmod.Logging.LOG_TRACE+1, "anzovinmod.TimelineFindChildren._parseN() parsing result ", newSearchN);
  return newSearchN;
};

// Merges the second search object into the first search object. Usually the
// second search object was selected from a matching element, and is to be
// put into a new root search object.
// 
// nA: The destination search object.
// nB: The search object to merge into the destination object. Any new elements
// are copied as clones, not as original elements from nB.
// 
// Returns: Just a reference to nA. Not entirely needed, as nA is modified by
// this function, but this lets the function be used inline.
anzovinmod.TimelineFindChildren._mergeN = function(nA, nB) {
  anzovinmod.Logging.logit(anzovinmod.Logging.LOG_TRACE+1, "anzovinmod.TimelineFindChildren._mergeN() merging ", nA, " with ", nB);
  nA = anzovinmod.Utilities.defaultParam(nA, null);
  nB = anzovinmod.Utilities.defaultParam(nB, null);
  if (nA == null || nB == null) {
    return nA;
  }
  for (var nBE in nB) {
    if (!nB.hasOwnProperty(nBE)) {
      continue;
    }
    if (!nA.hasOwnProperty(nBE)) {
      nA[nBE] = anzovinmod.Utilities.cloneObject(nB[nBE], true);
    } else {
      anzovinmod.TimelineFindChildren._mergeNE(nA, nB, nBE);
    }
  }
  anzovinmod.Logging.logit(anzovinmod.Logging.LOG_TRACE+1, "anzovinmod.TimelineFindChildren._mergeN() result ", nA);
  return nA;
};

// Merges the second search object into the first search object, using the
// given first-order array selector. This is functionally equivalent to
// _merge(nA[nE], nB[nE]), though with some additional handling for cases
// when nE may or may not be originally found in either search object
// nA or nB.
// 
// nA: The destination search object.
// nB: The search object to merge into the destination object.
// nE: The first-order array inded to use on both nA and nB. Only data from
// nA[nE] and nB[nE] are looked at or merged.
// 
// Returns: Just a reference to nA.
anzovinmod.TimelineFindChildren._mergeNE = function(nA, nB, nE) {
  anzovinmod.Logging.logit(anzovinmod.Logging.LOG_TRACE+1, "anzovinmod.TimelineFindChildren._mergeNE() merging ", nA, " with ", nB, " via ", nE);
  nA = anzovinmod.Utilities.defaultParam(nA, null);
  nB = anzovinmod.Utilities.defaultParam(nB, null);
  nE = anzovinmod.Utilities.defaultParam(nE, null);
  if (nA == null || nB == null || !nB.hasOwnProperty(nE)) {
    return nA;
  }
  if (!nA.hasOwnProperty(nE)) {
    nA[nE] = anzovinmod.Utilities.cloneObject(nB[nE], true);
  } else {
    for (var nBE in nB[nE]) {
      if (!nB[nE].hasOwnProperty(nBE)) {
        continue;
      }
      anzovinmod.TimelineFindChildren._mergeNE(nA[nE], nB[nE], nBE);
    }
  }
  anzovinmod.Logging.logit(anzovinmod.Logging.LOG_TRACE+1, "anzovinmod.TimelineFindChildren._mergeNE() result ", nA);
  return nA;
};

// Determine if the given display object is a match for the given search
// string, array, or object n
// 
// Note that in the AS version, the 'timeline' parameter is a MovieClip instance
// while in this JS version it is a MainTimeline instance. They are relatively
// equivalent in their meaning however, as the root MovieClip instance in AS
// contains the stage.
// 
// n: A string or array or object that is the search elements
// o: An object to determine if it matches. This is an object that may or may
// not be on any timeline. The root object is just determined by a parent
// being NULL eventually.
// timeline: Default NULL. A MainTimeline instance that represents the root
// of the current timeline. This object is used in contexts of the following
// additional optional parameters.
// useTimelineScene: Default true. If True and 'timeline' is specified, then
// the value of that timeline's currently playing scene is used to identify
// the "s" components of n. If False, then a matched search would only be
// able to use the "*" scene identifier.
// hasTimelineAsRoot: Default true. If True and 'timeline' is specified,
// then the timeline must be the rootmost element in the parent history path
// of o in order for this function to return True. If False, then this
// restrictions will not be in place. Note that in this JS version, this
// comparison is actually made on the root stage object, not the MovieClips or
// MainTimeline instance objects, but the results are the same as that of the
// AS version.
// 
// Returns: True if object o fully matches any n, otherwise False.
anzovinmod.TimelineFindChildren.isMatch = function(n, o, timeline, useTimelineScene, hasTimelineAsRoot) {
  n = anzovinmod.Utilities.defaultParam(n, null);
  o = anzovinmod.Utilities.defaultParam(o, null);
  timeline = anzovinmod.Utilities.defaultParam(timeline, null);
  useTimelineScene = anzovinmod.Utilities.defaultParam(useTimelineScene, true);
  hasTimelineAsRoot = anzovinmod.Utilities.defaultParam(hasTimelineAsRoot, true);
  if (n == null || o == null || !o.hasOwnProperty("name")) {
    return false;
  }
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_TRACE, "anzovinmod.TimelineFindChildren.isMatch", "determining match for ", o.name, " against matches ", n);
  // allow for string or array or object parameter for n
  var useN = anzovinmod.TimelineFindChildren._optimizeN(n);
  if (typeof(n) == "string") {
    useN = anzovinmod.TimelineFindChildren._translateN([useN]);
  } else if (n instanceof Array) {
    useN = anzovinmod.TimelineFindChildren._translateN(useN);
  }
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_TRACE, "anzovinmod.TimelineFindChildren.isMatch", "translated to: ", useN);
  // build a path up to the object's root node
  var path = [];
  var parent = o;
  var timelineIsRoot = false;
  while (parent != null && !(parent instanceof createjs.Stage) && (parent instanceof createjs.MovieClip || parent instanceof createjs.Container)) {
    path.push(parent.name);
    parent = parent.parent;
  }
  if (parent instanceof createjs.Stage) {
    if (timeline != null && timeline._stage == parent) {
      timelineIsRoot = true;
    }
  }
  // reverse search path
  path.reverse();
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_TRACE, "anzovinmod.TimelineFindChildren.isMatch", "built path: ", path, " timeline is root: ", timelineIsRoot);
  // only search current scenes
  var thisSceneN = {};
  var timelineScene = null;
  if (timeline != null && useTimelineScene && timeline._currentScene >= 0) {
    timelineScene = timeline._sceneOrder[timeline._currentScene];
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_TRACE, "anzovinmod.TimelineFindChildren.isMatch", "using timeline scene: ", timelineScene);
  }
  for (var s in useN) {
    if (!useN.hasOwnProperty(s)) {
      continue;
    }
    if (s == "*" || (timelineScene != null && timelineScene == s)) {
      anzovinmod.TimelineFindChildren._mergeN(thisSceneN, useN[s]);
    }
  }
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_TRACE, "anzovinmod.TimelineFindChildren.isMatch", "scene searching with n: ", thisSceneN);
  // transverse the search path
  for (var i = 0; i < path.length; ++i) {
    thisSceneN = anzovinmod.TimelineFindChildren._parseN(thisSceneN, path[i]);
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_TRACE, "anzovinmod.TimelineFindChildren.isMatch", "path component '", path[i], "' in scene n: ", thisSceneN);
  }
  // if a match is found to this point, there will be a null property
  var ret = false;
  if (thisSceneN.hasOwnProperty(null)) {
    ret = true && (!hasTimelineAsRoot || timelineIsRoot);
  }
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_TRACE, "anzovinmod.TimelineFindChildren.isMatch", "is a match: ", ret);
  return ret;
};

// Optimizes the names array, perfoming simplifications and expansions where
// appropriate. For example, "*.**.**.*" can be simplified to "*.**.*", while
// "name" will be expanded to "*.**.name", etc.
// 
// n: The names array to expand. This can be a single string element or an
// array of elements.
// 
// Returns: An array of elements after processing, or a single string element
// depending on the input type of the original parameter n. If input was not an
// array or a string, then null is returned.
anzovinmod.TimelineFindChildren._optimizeN = function(n) {
  n = anzovinmod.Utilities.defaultParam(n, null);
  if (n == null) return null;
  // convert to standard type
  var useN = [];
  if (typeof(n) == "string") {
    useN = [n];
  } else if (n instanceof Array) {
    useN = n;
  } else {
    return null;
  }
  // handle each element
  for (var i = 0; i < useN.length; ++i) {
    var thisN = useN[i];
    // "x" -> "*.**.x"
    if (thisN.indexOf(".") < 0) {
      thisN = "*.**." + thisN;
    }
    useN[i] = thisN;
  }
  // return
  if (typeof(n) == "string") {
    return useN[0];
  }
  return useN;
};

}());
