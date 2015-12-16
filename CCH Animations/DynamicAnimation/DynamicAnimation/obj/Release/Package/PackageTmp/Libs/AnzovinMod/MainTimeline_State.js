/*! MainTimeline_State.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// This class is just a manager for the internal state used by the mtl.
anzovinmod.MainTimeline_State = function(mtl) {
  // A reference back to the owning mtl instance.
  this._mtl = mtl;
  // Stores the current state of the main timeline instance. Currently available
  // states are as follows. States are used externally to the actual animation,
  // and are instead used to describe the state of the timeline instance,
  // and are to be used to determine the state of the timeline instance. For
  // example, animation dependent actions should use vars, while player actions
  // should rely on states, such as to determine if the animation is loaded,
  // is playing, has reached the end, etc.
  // 
  // bare: True when the main timeline instance is just created and no scenes
  // are loaded into it. False when at least one scene is added to the mtl.
  // unloaded: True when at least one scene is added to the timeline instance,
  // but it is not yet loading assets for it. False when the loading has
  // been completed.
  // loading: True when at least one scene is added to the timeline instance and
  // the instance is called to start loading the assets and construct the stage.
  // False when the loading is completed.
  // loaded: True when the timeline instance is finished loading.
  // started: True when the animation is started to play for the first time.
  // If false, then the animation has not yet been played at all.
  // playing: True when the animation calls to the main timeline instance that
  // it is actually playing (and not just displaying a play graphic), then this
  // state is set.
  // ended: True when the animation calls to the main timeline instance that it
  // has reached the end of its animation, then this state is set.
  // 
  // Available default state transitions are as follows.
  // bare -> unloaded -> unloaded, loading -> loaded ->
  // loaded, playing -> loaded, ended
  this._states = {
    "bare":true,
    "unloaded":false,
    "manifestloading":false,
    "manifestloaded":false,
    "loading":false,
    "loaded":false,
    "started":false,
    "playing":false,
    "ended":false,
    "hardpaused":false
  };
  // A state-change callback. If a function is set here, it will be called
  // whenever this timeline instance changes state values.
  // 
  // The format for these callbacks is: f(states)
  // "states" is a single array that contains strings. Each string is a
  // state value that had changed. The state value may be true or false in the
  // internal state controller, but it has had its value changed through a
  // setState() command. The callbacks should iterate over the list of changed
  // states, call isState() to get the current value of the states, then
  // perform any actions as appropriate
  this._stateChangeCallbacks = [];
};

// Returns whether the current indicated state of this timeline instance
// has the given state value.
// 
// state: The state value to check for.
// 
// Returns:
// True/False if the state value indicated is set.
anzovinmod.MainTimeline_State.prototype.isState = function(state) {
  state = anzovinmod.Utilities.defaultParam(state, null);
  if (state == null) return false;
  return this._states.hasOwnProperty(state) && this._states[state];
};

// Sets the current state of the timeline instance. If the current state is
// already set (optionally to the value indicated), then the operation is
// skipped and the callbacks are skipped.
// 
// state: String of the state to set.
// value: Default True. The value of the state to set (true/false).
// doCallbacks: Default True. If false, do not perform the state change
// callbacks, otherwise they will be performed.
anzovinmod.MainTimeline_State.prototype.setState = function(state, value, doCallbacks) {
  state = anzovinmod.Utilities.defaultParam(state, null);
  value = anzovinmod.Utilities.defaultParam(value, true, true);
  doCallbacks = anzovinmod.Utilities.defaultParam(doCallbacks, true, true);
  if (state == null) return;
  if (!this._setState(state, value)) return;
  if (doCallbacks) {
    this._sendStateChangeCallbacks([state]);
  }
};

// Sets the current states of the timeline instance to the values indicated by
// the properties in the given object. This is really just a multi-version of
// setState(). The callbacks are called if any single state changes.
// 
// states: key/value pairs of state information to set.
// doCallbacks: Default True. If false, do not perform the state change
// callbacks, otherwise they will be performed.
anzovinmod.MainTimeline_State.prototype.setStates = function(states, doCallbacks) {
  states = anzovinmod.Utilities.defaultParam(states, null);
  doCallbacks = anzovinmod.Utilities.defaultParam(doCallbacks, true, true);
  if (states == null) return;
  var changedStates = [];
  for (var k in states) {
    if (!states.hasOwnProperty(k)) continue;
    if (!this._setState(k, states[k])) continue;
    changedStates.push(k);
  }
  if (changedStates.length > 0 && doCallbacks) {
    this._sendStateChangeCallbacks(changedStates);
  }
};

// Internal function to actually perform the work of setting the state value.
// If there are special-case state actions, perform them here as well.
// 
// state: The state value to set.
// value: The value to set the state to.
// 
// Returns:
// True if the value was changed, false if it was not changed.
anzovinmod.MainTimeline_State.prototype._setState = function(state, value) {
  if (this._states.hasOwnProperty(state)) {
    if (this._states[state] == value) return false;
  } else {
    if (!value) return false;
  }
  this._states[state] = value;
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_State._setState", "state '", state, "' set to ", value);
  return true;
};

// Actually performs the work of triggering the state change callbacks.
// 
// states: An array of states that have changed, to trigger.
anzovinmod.MainTimeline_State.prototype._sendStateChangeCallbacks = function(states) {
  for (var i = 0; i < this._stateChangeCallbacks.length; ++i) {
    this._stateChangeCallbacks[i](states);
  }
};

// Set a state change callback. When this main timeline instance transitions
// to a new state, this function will be called.
// 
// The function definition only has a single parameter, which is a reference to
// this timeline instance. From this reference, the function can obtain the
// current state.
// 
// f: The callback function.
anzovinmod.MainTimeline_State.prototype.addStateChangeCallback = function(f) {
  f = anzovinmod.Utilities.defaultParam(f, null);
  if (f == null) return;
  this._stateChangeCallbacks.push(f);
};

}());
