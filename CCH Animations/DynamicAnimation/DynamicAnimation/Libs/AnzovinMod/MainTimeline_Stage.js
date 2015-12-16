/*! MainTimeline_Stage.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// This class holds the stage instance and performs any actions upon the stage
// for the main mtl instance.
anzovinmod.MainTimeline_Stage = function(mtl) {
  // This is just a reference to the owning mtl instance.
  this._mtl = mtl;
  // The stage that this timeline is attached to. It is created by our timeline
  // object. There is only one stage per main timeline.
  this._stage = null;
  // Whether to actually fully load the stage, based on if the prerequsite
  // asynchronous loaders are complete. This only gets set to true when the
  // callee tells the main timeline to load the stage (when it can). This is
  // then set to false when the stage is loaded, to prevent it from loading
  // more than once.
  this._doStageLoad = false;
  // A control object for the stage update event listeners, allowing us to
  // control whether to disable ticking or drawing on specific calls to
  // stage.update().
  this._stageUpdate = {
    "disableTick": false,
    "disableDraw": false,
    "currentlyUpdating": false
  };
  
  // For click detection, the last object that was pressed during the mouse
  // down event, to compare to the mouse up event.
  this._stagePressedObject = null;
  // Callbacks to be issued when there is an unhandled click event.
  this._unhandledMouseClickCallbacks = [];
  
  // INIT
  
  // Create the stage and attach it to the canvas element.
  this._stage = new createjs.Stage(this._mtl.getCanvas());
  // Set stage event listeners for control.
  this._stage.addEventListener("tickstart",
    anzovinmod.Utilities.bind(this, this._stageUpdateEventListener_tickstart)
  );
  this._stage.addEventListener("drawstart",
    anzovinmod.Utilities.bind(this, this._stageUpdateEventListener_drawstart)
  );
  // Setup click detection.
  this._setupClickDetection();
};

// This function just adds the indicated unhandled mouse click callback to be
// used when there is an unhandled mouse click on the stage.
// 
// f: The function to call. It is passed the event of a mouse click if there
// is one.
anzovinmod.MainTimeline_Stage.prototype.addUnhandledMouseClickCallback = function(f) {
  f = anzovinmod.Utilities.defaultParam(f, null);
  if (f == null) return;
  this._unhandledMouseClickCallbacks.push(f);
};

// This function sets up click detection. The detection will know when a click
// occurs anywhere on the stage, and what object on the stage is being clicked.
// This kind of acts as a replacement "click" event from CreateJS, except when
// a click happens and there is no object representing the click event, it will
// just have a "null" target instead of no click at all.
// 
// The resulting replacement click events will only be thrown if the target
// during the "down" and "up" portions of the mouse click events are the same
// object instance. This means that if someone clicks, drags, then releases the
// click, a click may or may not be registered.
// 
// It should also be noted, that the click event only fires for objects that
// have click events registered on them. This is so that we can capture any
// clicks that are not handled by the createjs library itself, and potentially
// handle them ourselves.
anzovinmod.MainTimeline_Stage.prototype._setupClickDetection = function() {
  this._stage.addEventListener("stagemousedown", anzovinmod.Utilities.bind(this, this._stageMouseDownEventCallback));
  this._stage.addEventListener("stagemouseup", anzovinmod.Utilities.bind(this, this._stageMouseUpEventCallback));
};

// This function is called whenever a mouse click has been registered through
// our own little work-arounds for the transparent stage. The passed obj is
// either a display object or null, indicating that nothing was clicked (eg,
// the stage).
// 
// evt: The event that caused this mouse click event result.
// obj: The display object under the mouse click (that can receive click
// events) or null if it is just the bare stage.
anzovinmod.MainTimeline_Stage.prototype._stageMouseEventResult = function(evt, obj) {
  if (obj == null) {
    for (var i = 0; i < this._unhandledMouseClickCallbacks.length; ++i) {
      this._unhandledMouseClickCallbacks[i](evt);
    }
  }
};

// This is a createjs callback for the "stagemousedown" event on the stage object.
// 
// evt: The event instance. The important properties here are "stageX" and
// "stageY", which represent the currently-scaled X/Y coordinates of the click
// on the stage.
anzovinmod.MainTimeline_Stage.prototype._stageMouseDownEventCallback = function(evt) {
  var scaleX = this._stage.scaleX;
  var scaleY = this._stage.scaleY;
  if (scaleX == 0 || scaleY == 0) {
    return;
  }
  this._stagePressedObject = this._stage.getObjectUnderPoint(evt.stageX / scaleX, evt.stageY / scaleY, 2);
};

// This is a createjs callback for the "stagemouseup" event on the stage object.
// 
// evt: The event instance. The important properties here are "stageX" and
// "stageY", which represent the currently-scaled X/Y coordinates of the click
// on the stage.
anzovinmod.MainTimeline_Stage.prototype._stageMouseUpEventCallback = function(evt) {
  var scaleX = this._stage.scaleX;
  var scaleY = this._stage.scaleY;
  if (scaleX == 0 || scaleY == 0) {
    this._stageMouseEventResult("null");
    this._stagePressedObject = null;
    return;
  }
  var pressedObject = this._stage.getObjectUnderPoint(evt.stageX / scaleX, evt.stageY / scaleY, 2);
  if (pressedObject == this._stagePressedObject) {
    this._stageMouseEventResult(evt, pressedObject);
    this._stagePressedObject = null;
    return;
  }
};

// Start the animation sequence. This will start the loading process of external
// scene assets and create the stage elements, and wait for them both to finish
// before combining them together, performing the last initialization tasks,
// and attaching everything to the stage.
anzovinmod.MainTimeline_Stage.prototype.startStage = function() {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Stage.startStage", "starting the stage loader");
  this._doStageLoad = true;
  if (this._mtl.isState("unloaded") && !this._mtl.isState("loading") && !this._mtl.isState("loaded")) {
    this._mtl.setState("loading");
  }
  if (this._mtl.isState("unloaded") && !this._mtl.isState("manifestloading") && !this._mtl.isState("manifestloaded")) {
    this._mtl.setState("manifestloading");
  }
  this._mtl._mtl_scenes.initializeScenes();
  this.checkLoadStage();
};

// Callback for the loader, and also for the stage loader, this waits until
// all the prerequisite asynchronous loaders finish before actually loading
// everything onto the stage.
anzovinmod.MainTimeline_Stage.prototype.checkLoadStage = function() {
  if (!this._doStageLoad) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_NOTICE, "anzovinmod.MainTimeline_Stage.checkLoadStage", "stage not marked as loadable, either not yet ready to load or already loaded");
    return;
  }
  if (!this._mtl.isState("manifestloaded")) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Stage.checkLoadStage", "stage still loading assets");
    return;
  }
  if (!this._mtl.isLateLoadFinished()) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Stage.checkLoadStage", "stage/scene objects still being created");
    return;
  }
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Stage.checkLoadStage", "stage ready to complete loading");
  this._doStageLoad = false;
  this._loadStageFinalize();
};

// Actually starts the stage loading. This assumes that all the necessary
// prerequisites are complete, such as attaching scenes to this main timeline,
// preloading assets, etc. After this call, the scenes will be created and
// added to the stage, any necessary one-off script fixes will be applied
// (such as fixing text element positions), and the stage started.
// 
// MSC = yes. Parses all loaded scenes, checking the properties of all scenes.
// Draws the first frame of the first (current) scene.
anzovinmod.MainTimeline_Stage.prototype._loadStageFinalize = function() {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Stage._loadStageFinalize", "finalizing stage loading");
  var scenes = this._mtl.getScenes();
  for (var k in scenes) {
    if (!scenes.hasOwnProperty(k)) continue;
    var scene = scenes[k];
    this._stage.addChild(scene);
    // initialization tasks
    anzovinmod.TimelineFindChildren.namifyAllChildren(scene);
    this._mtl._mtl_scenes._initializeScenes_listButtonHelpers(scene);
    this._mtl._textController.initTextNodesFromMtl(this._mtl, scene);
  }
  // get scene properties
  var props = this._mtl._mtl_scenes.getAndValidateBasicSceneProperties();
  if (props == null) return;
  // set properties
  if (props["color"] != null) {
    this._mtl._canvas.style.backgroundColor = props["color"];
  }
  // set size appropriately
  this._mtl._mtl_size.setCanvasSize(props["width"], props["height"]);
  this._mtl._textController.setSize(props["width"], props["height"]);
  this._mtl._mtl_size.triggerResize();
  // start ticking
  this._mtl._ticker.setFPS(props["fps"]);
  this._mtl._ticker.start();
  // specifically goto the first frame of the scene before rendering it out.
  // this is technically the first run frame of the animation.
  // this line also fixes the "snap" on the first frame of the first scene
  // when the animation loads. subsequent scenes are okay because of the
  // way nextScene() works (in that it calls gotoAndStop(0) itself). this
  // essentially just takes the place of the first frame's tick calculations,
  // because the scene hasn't yet had the opportunity to calculate positions of
  // anything
  this._mtl.gotoAndStop(0);
  this.updateStage({
    "doTick":false,
    "resetTickerTimeout":true,
  });
  if (this._mtl.isState("loading")) {
    this._mtl.setStates({"unloaded":false, "loading":false, "loaded":true});
  }
};

// This is an event callback for the stage, for when it throws a "tickstart"
// event just before it begins to actually apply the tick to the entire
// stage. This function allows us to cancel the tick, such as for when we wish
// to redraw the canvas but without iterating it forward in time.
// 
// The ticking can be disabled by calling evt.preventDefault().
// 
// evt: The event object.
anzovinmod.MainTimeline_Stage.prototype._stageUpdateEventListener_tickstart = function(evt) {
  if (this._stageUpdate["disableTick"]) {
    evt.preventDefault();
  }
};

// This is an event callback for the stage, for when it throws a "drawstart"
// event just before it begins to draw to the canvas context. This function
// allows us to cancel the drawing event, such as for when we wish to perform
// additional processing after the stage update tick, but before the final
// draw context is done.
// 
// The drawing can be disabled by calling evt.preventDefault().
// 
// evt: The event object.
anzovinmod.MainTimeline_Stage.prototype._stageUpdateEventListener_drawstart = function(evt) {
  if (this._stageUpdate["disableDraw"]) {
    evt.preventDefault();
  }
};

// Update the stage. Optionally perform several other tasks, including
// whether to do a tick or not, whether to update the HTML form input
// elements on the frame, etc.
// 
// Stage updating has the following event flow:
// event: tickstart
//  do ticks
// event: tickend
// event: drawstart
//  do draw
// event: drawend
// 
// We desire to interrupt the normal flow of these events. In particular,
// we desire the ability to perform custom modifications on objects after
// they are modified by the tick, but before the draw event fires (such as
// fixing text positions).
// 
// We can do this by ticking normally, but with a caught "drawstart" event.
// We can disable the drawing, then perform our necessary corrective events,
// then disable tickOnUpdate on the stage, then perform another update but
// without the caught "drawstart" event. The context drawing will occur,
// then we can reset the tickOnUpdate variable and all is done.
// 
// Alternatively, we can just add an event listener to "tickend", which
// will be called when the tickOnUpdate finishes processing. This callback
// would be responsible for all of the corrective events that would need to
// occur before the draw event actually occurs.
// 
// Technically, either method would provide the same functionality.
// 
// I am of the mind to prefer the former. This way all the logic is tied to
// this function and not spread out over several event listeners that need to
// somehow retain state with one another. Additionally, should we ever WANT to
// completely disable one or the other (tick or draw) for a frame, we can do
// that easily.
// 
// The following are the optional parameters to this function. They are to be
// passed in as a configuration object as the first parameter to this function.
// 
// doTick: Default True.
// If False, ensure that the stage will not tick on an update. If the
// stage or scenes are paused elsewhere and this is True, then it still may not
// observe to tick, but the tick event will be applied to the stage.
// 
// doDraw: Default True.
// If False, then do not draw the canvas elements to the stage: The previous
// frame will persist in the framebuffer. If True, then the stage is redrawn
// and the frame in the framebuffer updated.
// 
// resetTickerTimeout: Default False.
// If True, then the ticker timeout will be reset at this instant so that it
// will wait a full frame time until updating to the next frame. This should
// be called whenever the animation is reset, paused, or resumed, in order to
// more correctly keep track of the playback. The reset is done BEFORE any
// ticking or other work performed, as in all other instances of updating the
// stage, the tick is triggered BEFORE any updating or drawing is done.
// 
// checkTextAdjustments: Default True.
// If True, then check text fields for fixes and adjustments. For instance,
// canvas text fields are always a few pixels off, so this nudges them into
// their correct positions. Set to false to disable the check. This check is
// performed after the tick event, regardless of whether the tick is done or
// canceled with doTick=False. If you disable the tick, you may want to consider
// disabling this check.
// 
// doHtmlFrameUpdate: Default True.
// If True, check and update the HTML input elements positions. If false, do
// not perform these tasks. This can be done after the draw, so as to have as
// little time as possible between the frame being updated and the positions
// being adjusted.
// 
// checkButtonMouseovers: Default True.
// If True, check ButtonHelper instances in the animation for their hover
// states. Rollover and rollout events will be fake-passed in to the button
// helper handlers, and the canvas's cursor will be changed. If False, then do
// not check their states at all.
// 
// doNodeFrameTracking: Default True.
// If True, update the frame node count after ticking. If false, the node
// frame is not updated.
// 
// doNodeTracking: Default True.
// If True, does any on-node events and tracking. For the most part, this
// really just means to play sounds on the current nodeframe.
// 
// ACTUAL parameters are listed below.
// 
// MSC = yes. Does require that callee properly use the correct config options,
// but does work with msc. Specifically, only the current scene is operated
// upon in any fashion. Again however, callee must ensure that the correct
// scene is active (current) and options are correct. The helper functions to
// this function only operate on the scene passed in to them, so those are
// assumed to be MSC = yes themselves.
// 
// config: A configuration object with the above mentioned parameters for
// control. Defaults values are used if this object is null or unspecified.
anzovinmod.MainTimeline_Stage.prototype.updateStage = function(config) {
  config = anzovinmod.Utilities.defaultParam(config, {}, true);
  if (!config.hasOwnProperty("doTick"))
    config["doTick"] = true;
  if (!config.hasOwnProperty("doDraw"))
    config["doDraw"] = true;
  if (!config.hasOwnProperty("resetTickerTimeout"))
    config["resetTickerTimeout"] = false;
  if (!config.hasOwnProperty("checkTextAdjustments"))
    config["checkTextAdjustments"] = true;
  if (!config.hasOwnProperty("doHtmlFrameUpdate"))
    config["doHtmlFrameUpdate"] = true;
  if (!config.hasOwnProperty("checkButtonMouseovers"))
    config["checkButtonMouseovers"] = true;
  if (!config.hasOwnProperty("playSounds"))
    config["playSounds"] = true;
  if (!config.hasOwnProperty("doNodeTracking"))
    config["doNodeTracking"] = true;
  if (!config.hasOwnProperty("doNodeFrameTracking"))
    config["doNodeFrameTracking"] = true;
  // vars
  var scene = this._mtl.getCurrentScene();
  if (scene == null) return;
  // check for an already-updating update
  if (this._stageUpdate["currentlyUpdating"]) return;
  this._stageUpdate["currentlyUpdating"] = true;
  // scene vars
  scene._anzovinmod["tickedLastUpdate"] = false;
  scene._anzovinmod["drawnLastUpdate"] = false;
  // set the ticker time to be the time from this instant until it draws the
  // next frame
  if (config["resetTickerTimeout"]) {
    this._mtl.resetTickerTimeout();
  }
  // if we are to tock, then do it now but skip the draw phase. this lets the
  // positions of all elements be updated, but not drawn, so we can adjust if
  // necessary
  if (config["doTick"]) {
    this._updateStage_doTick(scene);
  }
  // increment the node frame number, if we are to keep track of it
  if (config["doNodeFrameTracking"]) {
    this._updateStage_doNodeFrameTracking(scene);
  }
  // keep track of the current traversed node path if a node is reached
  if (config["doNodeTracking"]) {
    this._updateStage_doNodeTracking(scene);
  }
  // play sounds that should be played
  if (config["playSounds"]) {
    this._updateStage_playSounds(scene);
  }
  // fix createjs.Text() elements having the wrong y position
  if (config["checkTextAdjustments"]) {
    this._updateStage_doTextFixes(scene);
  }
  // check for button mouseovers
  // note that this is really just for calculating the hover effect for
  // ButtonHelper instances created and used by Flash's output format. if other
  // hover effects are desired, then this might need to be reworked a bit
  if (config["checkButtonMouseovers"]) {
    this._updateStage_doButtonHelperHovers(scene);
  }
  // draw the canvas
  if (config["doDraw"]) {
    this._updateStage_doDraw(scene);
  }
  // position the HTML text input elements over the CJS text elements
  if (config["doHtmlFrameUpdate"]) {
    this._mtl._textController.inputTextFrameUpdate(scene.name, scene);
  }
  this._stageUpdate["currentlyUpdating"] = false;
};

// Performs a tick (and only a tick, not a draw).
// 
// scene: The scene being ticked.
anzovinmod.MainTimeline_Stage.prototype._updateStage_doTick = function(scene) {
  this._stageUpdate["disableTick"] = false;
  this._stageUpdate["disableDraw"] = true;
  this._stage.update();
  scene._anzovinmod["tickedLastUpdate"] = true;
};

// Performs the draw on the stage.
// 
// scene: The current scene being drawn.
anzovinmod.MainTimeline_Stage.prototype._updateStage_doDraw = function(scene) {
  this._stageUpdate["disableTick"] = true;
  this._stageUpdate["disableDraw"] = false;
  this._stage.update();
  scene._anzovinmod["drawnLastUpdate"] = true;
};

// Checks for button helpers rollover/rollout states.
// 
// scene: The scene to check button helper states of.
anzovinmod.MainTimeline_Stage.prototype._updateStage_doButtonHelperHovers = function(scene) {
  if (scene._anzovinmod["buttonHelpers"] == null) return;
  var arr = [];
  var pos = {x:this._stage.mouseX, y:this._stage.mouseY};
  var cursor = '';
  for (var i = 0; i < scene._anzovinmod["buttonHelpers"].length; ++i) {
    var button = scene._anzovinmod["buttonHelpers"][i];
    var buttonCursor = null;
    // skip button if it is not visibly visible
    if (!button.isVisible()) continue;
    // skip button if parent-* not on scene
    buttonCursor = button.cursor;
    var parent = button.parent;
    while (parent != null) {
      if (parent.cursor != null) buttonCursor = parent;
      if (scene == parent) break;
    }
    if (parent == null) continue;
    // calculate local position and do hittest
    var cpos = button.globalToLocal(pos.x, pos.y);
    var isOver = button.hitArea.hitTest(cpos.x, cpos.y);
    if (isOver) {
      cursor = buttonCursor;
      if (!button._anzovinmod["buttonHelperIsOver"]) {
        button._anzovinmod["buttonHelperIsOver"] = true;
        button._anzovinmod["buttonHelper"].handleEvent({type:"rollover"});
      }
    } else {
      if (button._anzovinmod["buttonHelperIsOver"]) {
        button._anzovinmod["buttonHelperIsOver"] = false;
        button._anzovinmod["buttonHelper"].handleEvent({type:"rollout"});
      }
    }
  }
  if (this._mtl._canvas.style.cursor != cursor) {
    this._mtl._canvas.style.cursor = cursor;
  }
};

// Performs text fixes and adjustments, if such adjustments are not finalized
// and can be performed.
// 
// scene: The scene to update text fixes of.
anzovinmod.MainTimeline_Stage.prototype._updateStage_doTextFixes = function(scene) {
  // can skip if scene is specifically marked and already done once
  if (!scene["_anzovinmod"]["textAdjustmentsFinal"]) {
    this._mtl._textController.allTextFrameUpdate(
      scene.name,
      !scene.paused && scene._anzovinmod["tickedLastUpdate"]
    );
    if (scene["_anzovinmod"]["properties"]["noAnimatedText"]) {
      scene["_anzovinmod"]["textAdjustmentsFinal"] = true;
    }
  }
};

// This function is called on each update stage event, when we wish to look for
// sounds that should be played (based on frame and/or node criteria) and play
// them.
// 
// Sounds played via this function should be sounds that are to be played ON
// a given frame within a given node. This is not a function that back-plays
// sounds.
// 
// scene: The scene that is currently playing, and to play sounds from.
anzovinmod.MainTimeline_Stage.prototype._updateStage_playSounds = function(scene) {
  this._mtl._mtl_nodes.updateStagePlayNodeSounds(scene);
};

// This function is called on each update stage event, when we wish to keep
// track of each frame of the node animation section.
// 
// scene: The scene that is currently playing.
anzovinmod.MainTimeline_Stage.prototype._updateStage_doNodeFrameTracking = function(scene) {
  this._mtl._mtl_nodes.updateStageDoNodeFrameTracking(scene);
};

// This function is called on each update stage event, when we wish to check
// for a changed tracking node position and update the mtl's internal stack of
// traversed nodes.
// 
// scene: The scene that is currently playing, and to check for nodes against.
anzovinmod.MainTimeline_Stage.prototype._updateStage_doNodeTracking = function(scene) {
  this._mtl._mtl_nodes.updateStageDoNodeTracking(scene);
};

// Capture every tick and handle scene transitions on the stage.
// 
// MSC = no. Actually looks like it does, but just needs review.
// 
// recommendedSkip: If set, indicates the number of ticks that should be
// skipped in order to catch-up to the actual animation time.
anzovinmod.MainTimeline_Stage.prototype.onTick = function(recommendedSkip) {
  if (this._currentScene < 0) return;
  if (this._mtl.isState("hardpaused")) return;
  if (!recommendedSkip) recommendedSkip = 0;
  if (recommendedSkip > 0) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.MainTimeline_Stage.onTick", "recommended skip frames: ", recommendedSkip);
  }
  do {
    var scene = this._mtl._mtl_scenes.getCurrentScene();
    if (scene == null) return;
    var gotoNextScene = false;
    if (scene instanceof createjs.MovieClip) {
      gotoNextScene = (scene.timeline.position == scene.timeline.duration - 1) && !scene.paused;
    } else {
      gotoNextScene = !scene.paused;
    }
    if (recommendedSkip > 0) {
      this._mtl._ticker.skippedFrame();
      if (gotoNextScene) {
        this._mtl._mtl_scenes.nextScene();
        this.updateStage({
          "doTick":false,
          "doDraw":false,
          "checkTextAdjustments":false,
          "doHtmlFrameUpdate":false,
          "checkButtonMouseovers":false
        });
      } else {
        this.updateStage({
          "doDraw":false,
          "checkTextAdjustments":false,
          "doHtmlFrameUpdate":false,
          "checkButtonMouseovers":false
        });
      }
    } else {
      if (gotoNextScene) {
        this._mtl._mtl_scenes.nextScene();
        this.updateStage({
          "doTick":false
        });
      } else {
        this.updateStage({});
      }
    }
  } while (recommendedSkip-- > 0);
};

// Sets the stage's display scale.
// 
// x: Scale in x direction. Defaults 1.0.
// y: Scale in y direction. Defaults 1.0.
anzovinmod.MainTimeline_Stage.prototype.setScale = function(x, y) {
  x = anzovinmod.Utilities.defaultParam(x, 1.0, true);
  y = anzovinmod.Utilities.defaultParam(y, 1.0, true);
  this._stage.scaleX = x;
  this._stage.scaleY = y;
};

}());
