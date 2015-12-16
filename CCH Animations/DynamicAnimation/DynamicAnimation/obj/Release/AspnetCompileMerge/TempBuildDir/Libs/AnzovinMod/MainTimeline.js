/*! MainTimeline.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// This class will be the main timeline instance for our project stage. It is
// created, scenes are attached to the main timeline, then when the stage is
// created, all the scenes are added to the stage. This is because normally,
// the CreateJS library and the Flash Pro output only allows for a single
// scene per stage. This lets multiple scenes be used simultaneously, managed
// by this class instance.
// 
// <div wrapper>
//  <form>
//   <div inputs>
//   <canvas>
// 
// parent: The HTML DOM element that will be the parent of the animation.
// Elements (primarily a DIV wrapper that contains the actual canvas) will be
// created as children to this parent element. Other properties of the parent
// element will not be touched at all. If null or unspecified, then the document
// body will be used instead.
// replacementContainer: The replacment container to use for this timeline
// instance.
anzovinmod.MainTimeline = function(parent, replacementContainer) {
  parent = anzovinmod.Utilities.defaultParam(parent, null);
  replacementContainer = anzovinmod.Utilities.defaultParam(replacementContainer, null);
  
  // CONTROLLERS
  
  this._mtl_sound = null;
  this._mtl_vars = null;
  this._mtl_state = null;
  this._mtl_scenes = null;
  this._mtl_size = null;
  this._mtl_stage = null;
  this._mtl_nodes = null;
  
  // DISPLAY AND DOM ELEMENTS
  
  // The parent element.
  this._parent = parent;
  // The DIV wrapper element, created by this class and added to the parent
  // node, that contains the canvas and HTML form inputs, along with anything
  // else as seen fit.
  this._divWrapper = null;
  // The canvas object on the page that we will be interacting with. Is a child
  // of the div wrapper.
  this._canvas = null;
  // The list of text elements that are editable and need html inputs overlayed
  // upon them. Completely managed by an external class. Also manages general
  // text fixes.
  this._textController = new anzovinmod.TextController();
  
  // STAGE
  
  // The ticker for this timeline instance.
  this._ticker = new anzovinmod.SelfAdjustingTicker();
  // PLAYBACK, SOUNDS
  
  // Whether, upon resuming from a hard pause, the animation should be
  // immediately played. This would be true in the middle of an animation
  // sequence, but false if the animation is stopped, such as by waiting for
  // user-input.
  this._hardPauseResumePlay = false;
  
  // OTHER
  
  // This is just an array of animation event listeners for our instances. These
  // listeners will receive all events sent by the animation, so they must be
  // able to perform an appropriate action on only appropriate events. The
  // format for these functions are:
  // function(event, msg)
  // event: The string of the event.
  // msg: The message to send to the event, or null if nothing.
  this._animationEventCallbacks = [];
  // This is just an array of playback percent callbacks. When the animation
  // reaches a certain playback percent, these are called. The percent value
  // sent is a value that takes into account the jump node positions.
  // The function formats are:
  // function(percent)
  // percent: Number between 0 and 100.
  this._playbackPercentCallbacks = [];
  
  // An contain that has animation data passed in from outside of the
  // timeline instance.
  this._replacementContainer = replacementContainer;
  
  this._init();
};

// This function is simply the initializer for the constructor. It is called to
// perform any work that is not simply setting default values for the class
// properties.
anzovinmod.MainTimeline.prototype._init = function() {
  // Create elements and add them to the page DOM. Set styles appropriately.
  // Width/height of the canvas itself will be decided by the scene properties
  // objects later during stage instantiation. CSS width/height is similar,
  // except that the CSS width/height is adjusted whenever the resize function
  // is called (or automatic if that functionality is added to this class).
  this._divWrapper = document.createElement("div");
  this._divWrapper.setAttribute("class", "anzovinmod-maintimeline");
  if (this._parent == null) {
    this._parent = document.body;
  }
  this._parent.appendChild(this._divWrapper);
  // Add the overlapping HTML form elements for editable text fields to the
  // div wrapper.
  this._divWrapper.appendChild(this._textController.createGetTextForm());
  
  // Create the actual canvas element used by the animation.
  this._canvas = document.createElement("canvas");
  
  // Create controllers.
  this._mtl_sound = new anzovinmod.MainTimeline_Sound(this);
  this._mtl_vars = new anzovinmod.MainTimeline_Vars(this);
  this._mtl_state = new anzovinmod.MainTimeline_State(this);
  this._mtl_scenes = new anzovinmod.MainTimeline_Scenes(this);
  this._mtl_size = new anzovinmod.MainTimeline_Size(this);
  this._mtl_stage = new anzovinmod.MainTimeline_Stage(this);
  this._mtl_nodes = new anzovinmod.MainTimeline_Nodes(this);
  
  // Attach the ticker listener.
  this._ticker.setCallback(anzovinmod.Utilities.bind(this._mtl_stage, this._mtl_stage.onTick));
  
  // The canvas element is a child of the HTML form.
  this._textController.createGetTextForm().appendChild(this._canvas);
  
  // Create replacement container if not passed in.
  if (this._replacementContainer == null) {
    this._replacementContainer = new anzovinmod.ReplacementVariableContainer();
  }
  
  // Add alternate sound extensions.
  if (typeof(createjs) !== "undefined" && typeof(createjs.Sound) !== "undefined") {
    createjs.Sound.alternateExtensions = ["ogg", "mp3"];
  }
  
  // Attach self state change callback.
  this.addStateChangeCallback(anzovinmod.Utilities.bind(this, this._selfStateChangeCallback));
};

// Simply opens the indicated URL.
// 
// url: The URL to open to.
anzovinmod.MainTimeline.prototype.openUrl = function(url) {
  url = anzovinmod.Utilities.defaultParam(url, null);
  if (url == null) return;
  window.open(url, "_blank");
};

// Simply returns the instance to the div wrapper of this main timeline
// instance.
// 
// Returns: A reference to the DIV DOM element.
anzovinmod.MainTimeline.prototype.getDivWrapper = function() {
  return this._divWrapper;
};

// Simply returns this instance's replacement container.
// 
// Returns: Replacement container instance to use for the animation.
anzovinmod.MainTimeline.prototype.getReplacementContainer = function() {
  return this._replacementContainer;
};

// Adds the given function to the list of triggered animation event
// listeners.
// 
// f: The function to add to the list of animation event listeners.
anzovinmod.MainTimeline.prototype.addAnimationEventCallback = function(f) {
  this._animationEventCallbacks.push(f);
};

// Adds the given function to the list of playback percent callbacks.
// 
// f: The function to add to the list of callbacks.
anzovinmod.MainTimeline.prototype.addPlaybackPercentCallback = function(f) {
  this._playbackPercentCallbacks.push(f);
};

// Sets the playback percent. Callbacks are called.
// 
// percent: The playback percent, between 0 and 100.
anzovinmod.MainTimeline.prototype.sendPlaybackPercent = function(percent) {
  for (var i = 0; i < this._playbackPercentCallbacks.length; ++i) {
    this._playbackPercentCallbacks[i](percent);
  }
};

// Triggers the indicated event to be sent. Anything that is attached and
// listening to events from this main timeline instance will receive all
// events being triggered. These events are specifically animation events.
// 
// event: The string event being triggered.
// msg: The message to send to the event, or null if nothing.
anzovinmod.MainTimeline.prototype.triggerAnimationEvent = function(event, msg) {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline.triggerAnimationEvent", "animation event triggered: ", event, " ", msg);
  for (var i = 0; i < this._animationEventCallbacks.length; ++i) {
    this._animationEventCallbacks[i](event, msg);
  }
};

// This function starts the animation.
// 
// MSC = yes. This should only be called the first time the animation is
// started (not replayed) so it should already be on the correct starting scene.
anzovinmod.MainTimeline.prototype.startAnimation = function() {
  this._mtl_nodes.resetNodes();
  this.gotoAndPlay("start");
  this.setState("started");
};

// This function resets and replays the animation. It resets the node tracker,
// cleans up any audio, and starts playing from the first designated frame (if
// there is one).
// 
// MSC = no. This would actually need to goto the 0th scene, then start on
// the frame "start", or zero if there is none. scene.timeline.resolve("start")
anzovinmod.MainTimeline.prototype.replayAnimation = function() {
  this._mtl_nodes.resetNodes();
  this.gotoAndPlay("start");
  // playing/ended state is set via animation frame functions
};

// Plays the given sound. Normally this is just a global playSound() function in
// the generated Flash Pro output. This instead ties it more directly into this
// specific timeline instance, and allows us to affect the sound if we so
// desire it.
// 
// This function should be called from the animation source.js file when a sound
// is desired to be played. As a result of backtracking, foretracking, or
// on-update node checking, the sound should instead be played using
// the mtl.sound instance directly.
// 
// Sounds played using this function should be short-lived sounds, such as
// those that are button effects or simple "swooshes" that are short lived.
// Longer sounds should be tracked and managed in the scene properties list.
// 
// Sounds played using this function are played without regard to node or frame
// position: They are played as-is, though they are indeed played with any
// minor needed offset to adjust for recommended skipped frames or proper
// timing to the frame point.
// 
// Note: There is a relatively minor issue with the animations that can cause
// them to play sounds as soon as the animation loads but before anything
// happens. Using this function to play those sounds will stop them from
// playing automatically, as we can detect that the scenes have not yet been
// initialized and stop sounds from playing. This is good: This is a subtle way
// of fixing an incorrect behavior in the animation, by using this function.
// 
// id: The ID of the sound instance that is to be played.
// config: The configuration object or null for all defaults.
anzovinmod.MainTimeline.prototype.playSound = function(id, config) {
  config = anzovinmod.Utilities.defaultParam(config, {}, true);
  // this scene check actually has a purpose: sounds that are played
  // automatically (but incorrectly) when the scene is immediately loaded
  // can be prevented from playing
  if (this._mtl_scenes.getCurrentScene() == null) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_NOTICE, "anzovinmod.MainTimeline.playSound", "sound is attempting to play before scenes are fully loaded: ", id);
    return;
  }
  config["addTickerTimeOffset"] = true;
  this._mtl_sound.playSoundConfig(id, config);
};

// Hard-pauses the animation.
// 
// MSC = yes. Looks at the current scene and stores its paused state so that it
// can be resumed in the correct state. Also calls stop(), so as long as MSC=yes
// in that function, then this one is good.
anzovinmod.MainTimeline.prototype._hardPause = function() {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline._hardPause", "hard-pausing");
  this._hardPauseResumePlay = false;
  var scene = this._mtl_scenes.getCurrentScene();
  if (scene != null && !scene.paused) this._hardPauseResumePlay = true;
  var hardpaused = this.isState("hardpaused");
  this._mtl_sound.pauseSounds();
  this.setState("hardpaused", false, false);
  this.stop();
  this.setState("hardpaused", hardpaused, false);
};

// Hard-resumes the animation.
// 
// MSC = yes. Just calls play(), so as long as MSC=yes in that function, then
// this one is good too.
anzovinmod.MainTimeline.prototype._hardResume = function() {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline._hardResume", "hard-resuming");
  var hardpaused = this.isState("hardpaused");
  this.setState("hardpaused", false, false);
  if (this._hardPauseResumePlay) this.play();
  this.setState("hardpaused", hardpaused, false);
  this._hardPauseResumePlay = false;
  this._mtl_sound.resumeSounds();
};

// Finds any children in the currently loaded stage's scene elements that
// have a name that matches any of the given names.
// 
// Note that for this JS version, names will also match if they contain any
// number of "x" or "x_1" or "x_2" etc, due to these instances being created
// automatically by the Flash Pro output/createjs libraries.
// 
// see: TimelineFindChildren for more information.
// 
// MSC = yes. Passes each scene in to the search function for processing,
// coalescing all return data into a single return array.
// 
// n: The array of names to search for elements for. String, Array, or
// an already translated Object.
// full: Defaults false. If true, find all children, even if they are not on
// the current stage (eg, are not shown).
// 
// Returns: An array of zero or more elements matching the displayed object
// names indicated.
anzovinmod.MainTimeline.prototype.findChildren = function(n, full) {
  anzovinmod.Utilities.defaultParam(full, false, true);
  var ret = [];
  var scenes = this.getScenes();
  for (var k in scenes) {
    if (!scenes.hasOwnProperty(k)) continue;
    ret = ret.concat(anzovinmod.TimelineFindChildren.findChildren(n, scenes[k], this, full));
  }
  return ret;
};

// This own mtl instance's state change callback, to be called whenever the
// internal states have changed and the callbacks are called.
// 
// states: An array of states that have changed.
anzovinmod.MainTimeline.prototype._selfStateChangeCallback = function(states) {
  for (var i = 0; i < states.length; ++i) {
    var state = states[i];
    switch (state) {
      case "hardpaused":
        if (this.isState(state)) this._hardPause();
        else this._hardResume();
        break;
      case "manifestloaded":
        if (this.isState("manifestloaded"))
          this._mtl_stage.checkLoadStage();
        break;
      case "resize":
        if (this.isState("willresize") && this.isState("resize")) {
          this._mtl_size.resize();
          this.setStates({"willresize":false, "resize":false}, false);
        } else if (this.isState("resize")) {
          this.setStates({"willresize":false, "resize":false}, false);
        }
        break;
      case "loading":
        if (this.isState("loading")) {
          this._mtl_scenes.loadMainLoader();
        }
        break;
    }
  }
};

// Reset the ticker ticking to be based on the current point in time,
// instead of waiting for the current timer to elapse to reset it. This lets you
// change a frame at an arbitraty point in time, and reset the time to the next
// frame instead of possibly getting a short frametime. Note that this will not
// start the timer if it is not yet started, that must be done manually.
anzovinmod.MainTimeline.prototype.resetTickerTimeout = function() {
  if (this._ticker.isStarted()) {
    this._ticker.stop();
    this._ticker.start();
  }
};

// Simply a function that returns the current framerate.
// 
// Returns:
// The current framerate of the animation.
anzovinmod.MainTimeline.prototype.getFPS = function() {
  return this._ticker.getFPS();
};

// This function returns the parent DOM element of this mtl instance. This
// is useful for situations involving operations on the mtl instance, such as
// resizing (to get the size of the parent element available to the mtl
// instance), or just to figure out where in the DOM the mtl is exactly.
// 
// Returns: The DOM parent entity of this mtl instance. The wrapper goes inside
// of this DOM element. This can be the document.body, or an element inside
// of it.
anzovinmod.MainTimeline.prototype.getParent = function() {
  return this._parent;
};

// Simply returns the reference to the canvas element this mtl instance is in
// control of.
// 
// Returns:
// HTML DOM Canvas element instance.
anzovinmod.MainTimeline.prototype.getCanvas = function() {
  return this._canvas;
};

// INTERACTIONS WITH: MAINTIMELINE_SIZE

// The final size setting step. This function actually sets the size values
// to the internal data structures, DOM elements, canvas elements, stage,
// etc.
// 
// width: The width to display, in px.
// height: The height to display, in px.
// scale: The scale that the canvas is displayed at, in order to fix into
// the width/height values indicated.
anzovinmod.MainTimeline.prototype.setSizeOnTimelineElements = function(width, height, scale) {
  this._divWrapper.style.width = width + "px";
  this._divWrapper.style.height = height + "px";
  this._canvas.setAttribute("width", width + "px");
  this._canvas.setAttribute("height", height + "px");
  this._mtl_stage.setScale(scale, scale);
  this._textController.setScale(scale);
};

// MAINTIMELINE_NODES

anzovinmod.MainTimeline.prototype.goNodeBack = function() {
  this._mtl_nodes.goNodeBack();
};
anzovinmod.MainTimeline.prototype.goNodeForward = function() {
  this._mtl_nodes.goNodeForward();
};
anzovinmod.MainTimeline.prototype.goNodeTo = function(node) {
  this._mtl_nodes.goNodeTo(node);
};

// MAINTIMELINE_STAGE

anzovinmod.MainTimeline.prototype.startStage = function() {
  this._mtl_stage.startStage();
};
anzovinmod.MainTimeline.prototype.addUnhandledMouseClickCallback = function(f) {
  this._mtl_stage.addUnhandledMouseClickCallback(f);
};
anzovinmod.MainTimeline.prototype.checkLoadStage = function() {
  this._mtl_stage.checkLoadStage();
};

// MAINTIMELINE_SIZE

anzovinmod.MainTimeline.prototype.triggerResize = function(width, height) {
  this._mtl_size.triggerResize(width, height);
};
anzovinmod.MainTimeline.prototype.setSize = function(calledWidth, calledHeight, displayWidth, displayHeight, canvasScale) {
  this._mtl_size.setSize(calledWidth, calledHeight, displayWidth, displayHeight, canvasScale);
};
anzovinmod.MainTimeline.prototype.setCanvasSize = function(width, height) {
  this._mtl_size.setCanvasSize(width, height);
};
anzovinmod.MainTimeline.prototype.getCanvasSize = function() {
  return this._mtl_size.getCanvasSize();
};
anzovinmod.MainTimeline.prototype.getResizeBehaviors = function() {
  return this._mtl_size.getResizeBehaviors();
};

// MAINTIMELINE_SCENES

anzovinmod.MainTimeline.prototype.startManifestLoading = function() {
  this._mtl_scenes.loadMainLoader();
};

anzovinmod.MainTimeline.prototype.attachScene = function(name) {
  this._mtl_scenes.attachScene(name);
};
anzovinmod.MainTimeline.prototype._gotoAndPlayOrStop = function(f, s, p, u, r, config) {
  this._mtl_scenes._gotoAndPlayOrStop(f, s, p, u, r, config);
};
anzovinmod.MainTimeline.prototype.gotoAndPlay = function(f, s, config) {
  this._mtl_scenes.gotoAndPlay(f, s, config);
};
anzovinmod.MainTimeline.prototype.gotoAndStop = function(f, s, config) {
  this._mtl_scenes.gotoAndStop(f, s, config);
};
anzovinmod.MainTimeline.prototype.play = function() {
  this._mtl_scenes.play();
};
anzovinmod.MainTimeline.prototype.stop = function() {
  this._mtl_scenes.stop();
};
anzovinmod.MainTimeline.prototype.getCurrentScene = function() {
  return this._mtl_scenes.getCurrentScene();
};
anzovinmod.MainTimeline.prototype.getScenes = function() {
  return this._mtl_scenes.getScenes();
};
anzovinmod.MainTimeline.prototype.getProgressNodes = function() {
  return this._mtl_scenes.getProgressNodes();
};
anzovinmod.MainTimeline.prototype.getLoginButtonConfig = function() {
  return this._mtl_scenes.getLoginButtonConfig();
};
anzovinmod.MainTimeline.prototype.isLateLoadFinished = function() {
  return this._mtl_scenes.isLateLoadFinished();
};
anzovinmod.MainTimeline.prototype.lateLoadComplete = function(scene) {
  return this._mtl_scenes.lateLoadComplete(scene);
};

// MAINTIMELINE_SOUND

anzovinmod.MainTimeline.prototype.setVolume = function(v) {
  this._mtl_sound.setVolume(v);
};
anzovinmod.MainTimeline.prototype.setMute = function(m) {
  this._mtl_sound.setMute(m);
};

// MAINTIMELINE_VARS

anzovinmod.MainTimeline.prototype.hasVar = function(k) {
  return this._mtl_vars.hasVar(k);
};
anzovinmod.MainTimeline.prototype.getVar = function(k, o) {
  return this._mtl_vars.getVar(k, o);
};
anzovinmod.MainTimeline.prototype.setVar = function(k, v, doCallbacks) {
  this._mtl_vars.setVar(k, v, doCallbacks);
};
anzovinmod.MainTimeline.prototype.registerCallback = function(k, n, f, forVarChanges, applyNow) {
  this._mtl_vars.registerCallback(k, n, f, forVarChanges, applyNow);
};
anzovinmod.MainTimeline.prototype.applyCallback = function(n, f) {
  this._mtl_vars.applyCallback(n, f);
};

// MAINTIMELINE_STATE

anzovinmod.MainTimeline.prototype.isState = function(state) {
  return this._mtl_state.isState(state);
};
anzovinmod.MainTimeline.prototype.setState = function(state, value, doCallbacks) {
  this._mtl_state.setState(state, value, doCallbacks);
};
anzovinmod.MainTimeline.prototype.setStates = function(states, doCallbacks) {
  this._mtl_state.setStates(states, doCallbacks);
};
anzovinmod.MainTimeline.prototype.addStateChangeCallback = function(f) {
  this._mtl_state.addStateChangeCallback(f);
};

}());
