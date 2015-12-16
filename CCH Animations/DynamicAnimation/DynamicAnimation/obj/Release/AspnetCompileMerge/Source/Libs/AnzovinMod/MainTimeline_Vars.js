/*! MainTimeline_Vars.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// This class is just a manager for the internal variables used by the mtl.
anzovinmod.MainTimeline_Vars = function(mtl) {
  // A reference back to the owning mtl instance.
  this._mtl = mtl;
  // A map of all the variables that this timeline needs to keep track of.
  this._vars = {};
  // This map houses functions that are called when a variable is changed.
  // Anytime a variable is changed through the associated timeline function,
  // these callbacks are searched for matches against the conditions and run.
  // 
  // Note: that if [n] is actually null and not an array, then the format
  // and meaning of the callback function changes. Instead of matching any
  // and all names that match, the function is just called directly whenever
  // the variable changes. The function signature only has a single, mtl
  // parameter.
  // 
  // Format:
  // {k} = [{"names":[n], "callback":f}]
  // k = The key name, the variable that is being changed.
  // [n] = An array of names of objects in the scenes to apply the callback to.
  // f = The callback function. It should have the format:
  // function(mtl, obj)
  // mtl = This timeline class instance. The variable that changed can be
  // retrieved via "mtl.getVar()".
  // obj = A reference to the object that matches one of [n].
  this._varChangeCallbacks = {};
};

// Simply returns true or false, whether the given variable name has ever been
// set. This is similar to the logic used by getVar(), of whether or not to
// return the value stored in the variable or the optional parameter 'o'.
// 
// k: The key name to determine whether it has been set.
// 
// Returns: True if the key has ever been set, false otherwise.
anzovinmod.MainTimeline_Vars.prototype.hasVar = function(k) {
  return this._vars.hasOwnProperty(k);
};

// Simply returns the value of the given variable as stored. If it is not
// stored, then just return the second object passed in to this function
// (defaults to return NULL if the variable is not stored).
// 
// k: The variable key name to get.
// o: If the variable key name has not been set yet, then return this value
// instead of any undefined value. Defaults to null.
// 
// Returns: The value corresponding to the var key name, or the 'o' parameter,
// or null.
anzovinmod.MainTimeline_Vars.prototype.getVar = function(k, o) {
  o = anzovinmod.Utilities.defaultParam(o, null);
  if (this._vars.hasOwnProperty(k)) {
    return this._vars[k];
  }
  return o;
};

// Is called to change a variable. This triggers the execution of any
// onVarChanged callbacks as long as the doCallbacks argument is true.
// This should remain set to true unless the callee knows what it is doing.
// 
// k: The variable key name to set.
// v: The value to set the key name to.
// doCallbacks: A boolean true/false value of whether or not to perform any
// of the on-change callbacks or not. False suppresses them. Default true.
anzovinmod.MainTimeline_Vars.prototype.setVar = function(k, v, doCallbacks) {
  doCallbacks = anzovinmod.Utilities.defaultParam(doCallbacks, true);
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.MainTimeline_Vars.setVar", "setting variable '", k, "' to value '", v, "' with callbacks '", doCallbacks, "'");
  this._vars[k] = v;
  if (doCallbacks) {
    if (this._varChangeCallbacks.hasOwnProperty(k)) {
      for (var i = 0; i < this._varChangeCallbacks[k].length; ++i) {
        var vccb = this._varChangeCallbacks[k][i];
        if (vccb["names"] != null) {
          var matches = this._mtl.findChildren(vccb["names"], true);
          for (var j = 0; j < matches.length; ++j) {
            anzovinmod.Logging.logm(anzovinmod.Logging.LOG_TRACE, "anzovinmod.MainTimeline_Vars.setVar", "found match for object '", matches[j].name, "', applying callback");
            vccb["callback"](this._mtl, matches[j]);
          }
        } else {
          anzovinmod.Logging.logm(anzovinmod.Logging.LOG_TRACE, "anzovinmod.MainTimeline_Vars.setVar", "applying callback for var change solo");
          vccb["callback"](this._mtl);
        }
      }
    }
  }
};

// Registers a function to be called when a specific variable is changed.
// Unlike the .AS version, the Canvas created by Flash Pro pre-creates all
// objects that will appear on the scene. As such, we do not need to worry
// about new objects created on the scene at a later point. The callbacks
// are still useful however, as the variables might not be set until after
// the scenes are added to the stage, or they may change during runtime.
// 
// k: The property name to trigger variable change callbacks, if applicable.
// n: An array of names to search for when a var change callback is issued, or
// to apply now.
// f: The function itself.
// forVarChanges: Whether to register this function for variable changes.
// applyNow: Whether to also apply the function right now, independent of
// storing it for var changes later.
anzovinmod.MainTimeline_Vars.prototype.registerCallback = function(k, n, f, forVarChanges, applyNow) {
  forVarChanges = anzovinmod.Utilities.defaultParam(forVarChanges, true);
  applyNow = anzovinmod.Utilities.defaultParam(applyNow, true);
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.MainTimeline_Vars.registerCallback", "adding callback of var '", k, "' for objects '", n, "' for variable changes '", forVarChanges, "' applying now '", applyNow, "'");
  if (forVarChanges) {
    if (!this._varChangeCallbacks.hasOwnProperty(k)) {
      this._varChangeCallbacks[k] = [];
    }
    this._varChangeCallbacks[k].push({"names":n, "callback":f});
  }
  if (applyNow) {
    if (n != null) {
      var matches = this._mtl.findChildren(n, true);
      for (var i = 0; i < matches.length; ++i) {
        anzovinmod.Logging.logm(anzovinmod.Logging.LOG_TRACE, "anzovinmod.MainTimeline_Vars.registerCallback", "found match for object '", matches[i].name, "', applying callback");
        f(this._mtl, matches[i]);
      }
    } else {
      anzovinmod.Logging.logm(anzovinmod.Logging.LOG_TRACE, "anzovinmod.MainTimeline_Vars.registerCallback", "applying callback for var change solo");
      f(this._mtl);
    }
  }
};

// Just like the registerCallback() function, except this one is just a
// shorthand for that version that just applies the callback function to any
// names matches and that's it.
// 
// n: The names array to search for.
// f: The function to apply.
anzovinmod.MainTimeline_Vars.prototype.applyCallback = function(n, f) {
  this.registerCallback(null, n, f, false, true);
};

}());
