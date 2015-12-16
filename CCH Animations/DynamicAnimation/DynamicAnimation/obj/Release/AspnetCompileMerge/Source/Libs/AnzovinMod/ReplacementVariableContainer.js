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
