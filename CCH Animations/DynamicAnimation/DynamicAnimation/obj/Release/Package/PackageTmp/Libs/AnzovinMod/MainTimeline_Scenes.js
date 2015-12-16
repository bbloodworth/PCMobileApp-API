/*! MainTimeline_Scenes.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// This class is responsible for storing and managing the animation
// scenes. This includes attaching, manifesting, preloading, initializing,
// etc.
anzovinmod.MainTimeline_Scenes = function(mtl) {
  // A reference to the owning mtl instance.
  this._mtl = mtl;
  
  // SCENES
  
  // A list of the scenes that are attached to this timeline. The stage itself
  // has the scenes within them, this is just a reference to them for our
  // benefits.
  this._scenes = {};
  // The order that the scenes were added in. This is important to allow
  // animation to flow through scene-to-scene in a default manner.
  this._sceneOrder = [];
  // The currently playing scene ID. This is an index to the sceneOrder
  // property.
  this._currentScene = -1;
  
  // MANIFEST AND LOADING
  
  // A global loader for all assets in this stage timeline.
  this._mainLoader = new createjs.LoadQueue(false);
  // A global loader cache for the asset loader. This is used by us to help
  // streamline the loading of shared assets between multiple scenes on the
  // timeline.
  this._mainLoaderCache = {};
  
  // NODES
  
  // An object of all progress nodes on all scenes attached to this instance.
  this._progressNodes = {};
  // An array of all sound nodes that can be played on these scene instances.
  this._soundsAtNodes = [];
  
  // OTHER
  
  // An indicator of whether this instance has set the mtl canvas size
  // variables yet. When they are set for the first time, a resize is
  // in order as well.
  this._haveSetCanvasSize = false;
  
  // INIT
  
  // Tell the loader that it might be handling sounds.
  this._mainLoader.installPlugin(createjs.Sound);
  // Event listener for adding a new item to the loader.
  this._mainLoader.addEventListener("fileload", anzovinmod.Utilities.bind(this, this._manifestFileLoadedCallback));
  // Event listener for when the loader is complete.
  this._mainLoader.addEventListener("complete", anzovinmod.Utilities.bind(this, this._loaderCompleteCallback));
  // Increase maximum number of simultaneous downloads
  this._mainLoader.setMaxConnections(5);
};

// Is used to determine if the late loading process has finished for all
// attached scenes.
// 
// Returns: True if all late loading is finished, false otherwise.
anzovinmod.MainTimeline_Scenes.prototype.isLateLoadFinished = function() {
  for (var k in this._scenes) {
    if (!this._scenes.hasOwnProperty(k)) continue;
    if (!this._scenes[k]["_anzovinmod"]["lateLoaded"]) return false;
  }
  return true;
};

// Indicates that the given scene has finished late loading.
// 
// scene: The scene instance, the same that was created through .make().
anzovinmod.MainTimeline_Scenes.prototype.lateLoadComplete = function(scene) {
  if (scene != null && scene.hasOwnProperty("_anzovinmod")) {
    scene["_anzovinmod"]["lateLoaded"] = true;
  }
  this._mtl._mtl_stage.checkLoadStage();
};

// Callback for when the loader has loaded a file. This sets the file data
// to the specific object that the scenes will be looking at.
// 
// MSC = yes. This sets the given loaded file to any scene that had it
// registered.
// 
// evt: The event sent with the callback. evt.item is a reference to the object
// that was passed in to the load queue initially. evt.result is a reference to
// the actual loaded content.
anzovinmod.MainTimeline_Scenes.prototype._manifestFileLoadedCallback = function(evt) {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Scenes._manifestFileLoadedCallback", "event 'fileload' caught for ", evt.item.id);
  delete this._mainLoaderCache[evt.item.id];
  for (var i = 0; i < this._sceneOrder.length; ++i) {
    var sceneName = this._sceneOrder[i];
    if (!anzovinmod.instance.Scenes.hasOwnProperty(sceneName))
      break;
    var anzScene = anzovinmod.instance.Scenes[sceneName];
    if (!anzScene["manifest"].hasOwnProperty(evt.item.id)) {
      for (var j = 0; j < anzScene["properties"]["manifest"].length; ++j) {
        if (anzScene["properties"]["manifest"][j]["id"] == evt.item.id) {
            anzScene["manifest"][evt.item.id] = evt.result;
            anzScene["properties"]["manifest"][j] = evt.item;
            break;
        }
      }
    }
  }
};

// This function searches all the loaded scenes, and attempts to find and
// return the indicated sound load event result. This is the "evt.result" from
// the main loader/preloader, and contains useful pieces of information
// regarding the sound, including duration and src.
// 
// Note that for sounds, they are shared between attached scenes wherever
// possible, so the first ID match is returned.
// 
// id: The sound ID to get the audio manifest of. Searched on all attached
// scenes, in order of attachment.
// 
// Returns:
// The <audio> manifest of the previously preloaded sound.
anzovinmod.MainTimeline_Scenes.prototype.getSoundAudioManifest = function(id) {
  for (var i = 0; i < this._sceneOrder.length; ++i) {
    var sceneName = this._sceneOrder[i];
    if (!anzovinmod.instance.Scenes.hasOwnProperty(sceneName))
      break;
    var anzScene = anzovinmod.instance.Scenes[sceneName];
    if (!anzScene["manifest"].hasOwnProperty(id))
      break;
    return anzScene["manifest"][id];
  }
  return null;
};

// This function is just called when the loader has completed. This is a
// separate function really just so we can log the event and trace when
// it was called more precisely.
anzovinmod.MainTimeline_Scenes.prototype._loaderCompleteCallback = function() {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Scenes._loaderCompleteCallback", "loader has called 'complete'");
  this._mtl.setStates({"manifestloading":false, "manifestloaded":true});
};

// Begins loading the main loader instance.
anzovinmod.MainTimeline_Scenes.prototype.loadMainLoader = function() {
  this._mainLoader.load();
};

// Create then attach the given scene name to this timeline. Scenes are expected
// to be added in the order they are desired to be played by default.
// 
// MSC = yes. Only one scene of the same name can be attached, multiple attempts
// will be rejected. Shared manifest data between scenes is saved, so it does
// not need to be downloaded more than once for all scenes.
anzovinmod.MainTimeline_Scenes.prototype.attachScene = function(name) {
  name = anzovinmod.Utilities.defaultParam(name, null);
  if (name == null) return;
  anzovinmod.Logging.logit(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Scenes.attachScene() attaching scene '", name, "'");
  // check that the scene is not already added to the stage
  for (var i = 0; i < this._sceneOrder.length; ++i) {
    if (this._sceneOrder[i] == name) {
      anzovinmod.Logging.logit(anzovinmod.Logging.LOG_WARN, "anzovinmod.MainTimeline_Scenes.attachScene() cannot add a scene with the same name as an already added scene");
      return;
    }
  }
  this._sceneOrder.push(name);
  // set any custom properties on the scene instance
  var scene = anzovinmod.instance.Scenes[name];
  this._attachScene_handleNodes(scene);
  var sceneProps = scene["properties"];
  if (!sceneProps.hasOwnProperty("noAnimatedText"))
    sceneProps["noAnimatedText"] = false;
  // iterate over the scene's manifest, and add it to the queue
  this._attachScene_loadManifest(scene);
  // set canvas width/height if not yet set (this way we can "resize" the
  // mtl before everything is loaded)
  if (!this._haveSetCanvasSize) {
    this._haveSetCanvasSize = true;
    this._mtl.setCanvasSize(sceneProps.width, sceneProps.height);
    this._mtl.triggerResize();
  }
  // set the state if necessary
  if (this._mtl.isState("bare")) {
    this._mtl.setStates({"bare":false, "unloaded":true});
  }
};

// This function is a helper for attaching scenes to the animation instance.
// This function handles cleaning up any node information attached to the
// indicated scene instance, such as setting default values, and adding
// node information to the internal data sets.
// 
// scene: An anzovinmod.instance.Scenes[*] entity, to which the nodes should
// be checked and handled.
anzovinmod.MainTimeline_Scenes.prototype._attachScene_handleNodes = function(scene) {
  var props = scene["properties"];
  // set default values where properties are missing
  if (!props.hasOwnProperty("soundsAtNodes"))
    props["soundsAtNodes"] = [];
  if (!props.hasOwnProperty("progressNodes"))
    props["progressNodes"] = {};
  for (var i = 0; i < props["soundsAtNodes"].length; ++i) {
    var e = props["soundsAtNodes"][i];
    if (!e.hasOwnProperty("loop")) e["loop"] = 0;
  }
  for (var k in props["progressNodes"]) {
    if (!props["progressNodes"].hasOwnProperty(k)) continue;
    var e = props["progressNodes"][k];
    if (!e.hasOwnProperty("nextNode")) e["nextNode"] = null;
  }
  // add these nodes to the internal sets
  this._addProgressNodes(props["progressNodes"]);
  this._addSoundsAtNodes(props["soundsAtNodes"]);
};

// This function is a helper for the attachScene function. This function takes
// in a scene instance, and adds any manifest elements to this animation
// instance's main loader.
// 
// scene: An anzovinmod.instance.Scenes[*] entity, to which the manifest
// should be checked, handled, and loaded to the main loader.
anzovinmod.MainTimeline_Scenes.prototype._attachScene_loadManifest = function(scene) {
  var props = scene["properties"];
  var sceneName = props["name"];
  // iterate over the scene's manifest, and add it to the queue. 'id' values
  // are shared across all scenes on the stage to simplify loading of shared
  // assets
  var sceneManifestList = props["manifest"];
  var sceneManifestData = scene["manifest"];
  var ourManifest = [];
  manifestLoop:
  for (var i = 0; i < sceneManifestList.length; ++i) {
    var manifestItem = sceneManifestList[i];
    // skip if already loaded in this scene
    if (sceneManifestData.hasOwnProperty(manifestItem["id"])) {
      continue manifestLoop;
    }
    // skip if already queued for loading in this iteration
    for (var j = 0; j < ourManifest.length; ++j) {
      if (ourManifest[j]["id"] == manifestItem["id"]) {
        continue manifestLoop;
      }
    }
    // skip if already queued for loading in this loader cache
    if (this._mainLoaderCache.hasOwnProperty(manifestItem["id"])) {
      continue manifestLoop;
    }
    // skip (and copy over) if found in another scene
    for (var p in anzovinmod.instance.Scenes) {
      if (!anzovinmod.instance.Scenes.hasOwnProperty(p)) continue;
      // skip this scene, we're processing it right now
      if (p == sceneName) continue;
      var pScene = anzovinmod.instance.Scenes[p];
      var pSceneManifestData = pScene["manifest"];
      // look for an already-loaded manifest item from another scene. match both
      // the manifest data and manifest list before copying over values
      if (pSceneManifestData.hasOwnProperty(manifestItem["id"])) {
        var pSceneManifestList = pScene["properties"]["manifest"];
        for (var j = 0; j < pSceneManifestList.length; ++j) {
          if (pSceneManifestList[j]["id"] == manifestItem["id"]) {
            sceneManifestData[manifestItem["id"]] = pSceneManifestData[manifestItem["id"]];
            sceneManifestList[i] = pSceneManifestList[j];
            continue manifestLoop;
          }
        }
      }
    }
    // not found, add to queue
    ourManifest.push(manifestItem);
  }
  // add manifest to the loader
  if (ourManifest.length > 0) {
    for (var i = 0; i < ourManifest.length; ++i) {
      anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Scenes._attachScene_loadManifest", "adding manifest entry: ", ourManifest[i].id);
      this._mainLoaderCache[ourManifest[i]["id"]] = ourManifest[i];
    }
    this._mainLoader.loadManifest(ourManifest, false);
  }
};

// Initializes the attached scenes.
// 
// MSC = yes. All scenes are independently initialized, the first is made the
// current, starting scene.
anzovinmod.MainTimeline_Scenes.prototype.initializeScenes = function() {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Scenes.initializeScenes", "loading all stage scenes");
  for (var i = 0; i < this._sceneOrder.length; ++i) {
    var name = this._sceneOrder[i];
    var anzScene = anzovinmod.instance.Scenes[name];
    if (anzScene["instance"].make) {
      // initialize now
      var scene = this._initializeScenes_createScene(name);
      this._scenes[name] = scene;
      // the just-loaded scene is the main one on the stage
      if (i == 0) {
        this._currentScene = 0;
      } else {
        // make sure all other scenes are stopped and hidden
        scene.stop();
        scene.visible = false;
      }
    } else {
      // initialize later
      anzScene["loadedCallbacks"].push(anzovinmod.Utilities.bind(this, this._initializeLoadedScene));
    }
  }
};

// Initializes the just loaded scene, because scene initialization has already
// started.
// 
// name: The scene name that was just loaded.
anzovinmod.MainTimeline_Scenes.prototype._initializeLoadedScene = function(name) {
  breaker: do {
    for (var i = 0; i < this._sceneOrder.length; ++i) {
      if (this._sceneOrder[i] == name) {
        var scene = this._initializeScenes_createScene(name);
        this._scenes[name] = scene;
        // the just-loaded scene is the main one on the stage
        if (i == 0) {
          this._currentScene = 0;
        } else {
          // make sure all other scenes are stopped and hidden
          scene.stop();
          scene.visible = false;
        }
        break breaker;
      }
    }
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_WARN, "anzovinmod.MainTImeline_Scenes._initializeLoadedScene", "unhandled scene loaded: ", name);
  } while (false);
  this._mtl.checkLoadStage();
};

// This function is a helper during scene initialization. It performs the
// work of creating a new scene MovieClip/Container instance, setting any
// necessary state variables within it.
// 
// name: The string name of the scene to create.
// 
// Returns:
// A new createjs container/movieclip instance of the scene in question, with
// whatever standard initialization or internal variable setting (_anz) done.
anzovinmod.MainTimeline_Scenes.prototype._initializeScenes_createScene = function(name) {
  // create the scene createjs MovieClip instance
  var scene = new anzovinmod.instance.Scenes[name]["instance"].make(this._mtl);
  // attach some useful data to it, including a specific container for our
  // own properties should we need them
  scene.name = name;
  scene._anzovinmod = {
    // just a handle to the main scene instance constructor
    "instance": anzovinmod.instance.Scenes[name]["instance"],
    // just a handle to the main scene properties list
    "properties": anzovinmod.instance.Scenes[name]["properties"],
    // the name of the scene, constant through all usage
    "name": name,
    // whether all text fixes and adjustments are forever done
    "textAdjustmentsFinal": false,
    // whether the scene is a movieclip (or container if false)
    "isMovieclip": true,
    // whether the scene has been ticked on its last update
    "tickedLastUpdate": false,
    // whether the scene has been drawn on its last update
    "drawnLastUpdate": false,
    // a list of all the button helpers attached to this scene
    "buttonHelpers": null,
    // whether the scene has been late loaded
    "lateLoaded": false,
  };
  // if the scene is technically a createjs Container, add expected
  // first-level properties and methods to it
  if (!(scene instanceof createjs.MovieClip)) {
    scene.gotoAndStop = function() { this.stop(); };
    scene.gotoAndPlay = function() { this.play(); };
    scene.stop = function() { this.paused = true; };
    scene.play = function() { this.paused = false; };
    scene.paused = false;
    scene._anzovinmod["isMovieclip"] = false;
  }
  return scene;
};

// Function that parses all the children of a scene, identifies ButtonHelpers,
// and sets them to an internal scene list. This only needs to be performed
// once on a scene. If called multiple times, it will not recalculate the
// button helpers, as they shouldn't be able to change after the first run.
// 
// MSC = yes. Yes because this is just called once on a scene, so as long as
// the callee parses all scenes, this will be fine. Also, the button helper
// information is only stored on the scene itself, there is no global state
// to worry about.
// 
// scene: The scene instance to parse children for button helpers. This is
// a MovieClip/container instance.
anzovinmod.MainTimeline_Scenes.prototype._initializeScenes_listButtonHelpers = function(scene) {
  scene = anzovinmod.Utilities.defaultParam(scene, null);
  if (scene == null) return;
  // alread parsed, no need to do it again
  if (scene._anzovinmod["buttonHelpers"] != null) return;
  var elements = anzovinmod.TimelineFindChildren.findChildren(
    scene.name + ".**.*", scene, this._mtl, true
  );
  var buttons = [];
  for (var i = 0; i < elements.length; ++i) {
    var element = elements[i];
    // find only button helper elements
    if (!element.hasOwnProperty("_anzovinmod")) continue;
    if (!element._anzovinmod.hasOwnProperty("isButtonHelper")) continue;
    if (!element._anzovinmod["isButtonHelper"]) continue;
    // ensure properties are set
    element._anzovinmod["buttonHelperIsOver"] = false;
    buttons.push(element);
  }
  scene._anzovinmod["buttonHelpers"] = buttons;
};

// This function parses all the scenes on the stage, and validates their
// basic properties (width, height, fps, color, etc) for consistency (as we
// don't currently support differing values really), as well as simply returning
// the values to the callee.
// 
// Returns:
// An object with the following properties: width; height; color; fps. If any
// of these values are "invalid", then null is returned. For example, if fps
// is < 0.
anzovinmod.MainTimeline_Scenes.prototype.getAndValidateBasicSceneProperties = function() {
  var ret = {
    "width":null,
    "height":null,
    "fps":null,
    "color":null
  };
  for (var i = 0; i < this._sceneOrder.length; ++i) {
    var props = anzovinmod.instance.Scenes[this._sceneOrder[i]]["properties"];
    for (var prop in ret) {
      if (!ret.hasOwnProperty(prop)) continue;
      if (!props.hasOwnProperty(prop)) {
        anzovinmod.Logging.logm(anzovinmod.Logging.LOG_WARN, "anzovinmod.MainTimeline_Scenes.getAndValidateBasicSceneProperties", "scene does not have '", prop, "' property: ", this._sceneOrder[i]);
      } else {
        if (ret[prop] == null || ret[prop] == props[prop]) {
          ret[prop] = props[prop];
        } else {
          anzovinmod.Logging.logm(anzovinmod.Logging.LOG_WARN, "anzovinmod.MainTimeline_Scenes.getAndValidateBasicSceneProperties", "attached scenes have differing '", prop, "' values; only the first scene's value will be used");
        }
      }
    }
  }
  // ensure there are valid values
  var checkNullZero = ["fps", "width", "height"];
  for (var i = 0; i < checkNullZero.length; ++i) {
    var prop = checkNullZero[i];
    if (ret[prop] == null || ret[prop] < 0) {
      anzovinmod.Logging.logit(anzovinmod.Logging.LOG_ERROR, "anzovinmod.MainTimeline_Scenes.getAndValidateBasicSceneProperties", "cannot validate parameters on attached scenes, property '", prop, "' is null or <0");
      return null;
    }
  }
  return ret;
};

// This function simply returns the current scene, or null if there is none.
// 
// Returns:
// The current scene instance (MovieClip/Container, on the stage), or null.
anzovinmod.MainTimeline_Scenes.prototype.getCurrentScene = function() {
  if (this._currentScene < 0) return null;
  if (this._currentScene >= this._sceneOrder.length) return null;
  return this._scenes[this._sceneOrder[this._currentScene]];
};

// Returns an associative array (object) of all the scenes that are currently
// loaded and initialized.
// 
// Returns:
// An object with key/value pairs of the name of scenes to the scene objects
// themselves. The scene objects are MovieClip/Container instances, though
// they do have _anzovinmod properties.
anzovinmod.MainTimeline_Scenes.prototype.getScenes = function() {
  return this._scenes;
};

// This function returns an associative array (object) of the first scene's
// login button configuration object.
// 
// Note that in cases of multiple scenes, only the first one is used.
// 
// Returns:
// An object of key/value pairs of login button configuration options.
anzovinmod.MainTimeline_Scenes.prototype.getLoginButtonConfig = function() {
  if (this._sceneOrder.length == 0)
    return null;
  var props = anzovinmod.instance.Scenes[this._sceneOrder[0]]["properties"];
  if (!props.hasOwnProperty("loginButtonConfig"))
    return null;
  var url = null;
  var rc = this._mtl.getReplacementContainer();
  if (rc != null) {
    var mcdname = props["name"] + "-MemberContentData";
    if (rc.has(mcdname)) {
      var mcd = {};
      try {
        mcd = JSON.parse(rc.get(mcdname));
      } catch (e) {
        anzovinmod.Logging.logm(anzovinmod.Logging.LOG_ERROR, "anzovinmod.Player._mainTimelineStateChangeCallback", "cannot json parse ", mcdname, ": ", rc.get(mcdname));
      }
      if (mcd.hasOwnProperty("EmployerURL")) {
        url = mcd["EmployerURL"];
      }
    }
  }
  var ret = anzovinmod.Utilities.cloneObject(props["loginButtonConfig"]);
  if (url != null)
    ret["url"] = url;
  return ret;
};

// This function returns an associative array (object) of all progress nodes
// that exist in all scenes.
// 
// Note that in cases of multiple scenes having the same named progress nodes,
// overwriting will occur! It is best if progress nodes have names that
// incorporate the scene name, like "sceneName.startNode".
// 
// MSC = yes. Returns all ProgressNodes of all scenes attached to this
// instance.
// 
// Returns:
// An object of key/value pairs of the progress nodes in the animation scenes.
anzovinmod.MainTimeline_Scenes.prototype.getProgressNodes = function() {
  return this._progressNodes;
};

// This function returns an array of all the sound nodes that can be played
// at any progress node positions.
// 
// MSC = yes. Returns all SoundsAtNodes from all scenes attached to this
// instance.
// 
// Returns:
// An array of SoundsAtNodes for the animation scenes.
anzovinmod.MainTimeline_Scenes.prototype.getSoundsAtNodes = function() {
  return this._soundsAtNodes;
};

// This function takes the given scene, and adds its progress nodes to the
// internal list of progress nodes. This is done as scenes are added instead
// of on demand just in order to essentially cache this result.
// 
// progressNodes: The object of progress nodes from the scene instance
// lib properties.
anzovinmod.MainTimeline_Scenes.prototype._addProgressNodes = function(progressNodes) {
  progressNodes = anzovinmod.Utilities.defaultParam(progressNodes, {}, true);
  for (var k in progressNodes) {
    if (!progressNodes.hasOwnProperty(k)) continue;
    this._progressNodes[k] = progressNodes[k];
  }
};

// This function takes the given scene, and adds its sounds-at-nodes to the
// interna list of nodes. This is done as scenes are added instead of on
// demand just in order to essentially cache this result.
// 
// soundsAtNodes: The array of sound nodes from the scene instance lib
// properties.
anzovinmod.MainTimeline_Scenes.prototype._addSoundsAtNodes = function(soundsAtNodes) {
  soundsAtNodes = anzovinmod.Utilities.defaultParam(soundsAtNodes, [], true);
  for (var i = 0; i < soundsAtNodes.length; ++i) {
    this._soundsAtNodes.push(soundsAtNodes[i]);
  }
};

// Start playing the current movieclip at the current frame/scene.
// 
// MSC = yes. Plays the current scene.
anzovinmod.MainTimeline_Scenes.prototype.play = function() {
  if (this._mtl.isState("hardpaused")) return;
  var scene = this.getCurrentScene();
  if (scene == null) return;
  scene.play();
  this._mtl.resetTickerTimeout();
};

// Stop playing any movieclips.
// 
// MSC = yes. Stops the current scene.
anzovinmod.MainTimeline_Scenes.prototype.stop = function() {
  if (this._mtl.isState("hardpaused")) return;
  var scene = this.getCurrentScene();
  if (scene == null) return;
  scene.stop();
};

// Goes to the given frame/stage combination and starts playing the movieclip.
// 
// MSC = yes. This just directly calls _gotoAndPlayOrStop() for handling all the
// behavior and logic, so all the MSC compatability is just passed on to that
// function.
// 
// f: The frame to go to.
// s: The scene to go to, or the current scene if null or undefined.
// config: A config object to pass in to the update stage function. Manually
// necessary configuration parameters will be set, but otherwise it will be
// used as-is.
anzovinmod.MainTimeline_Scenes.prototype.gotoAndPlay = function(f, s, config) {
  this._gotoAndPlayOrStop(f, s, true, true, false, config);
};

// Goes to the given frame/stage combination and stops playing the movieclip.
// 
// MSC = yes. This just directly calls _gotoAndPlayOrStop() for handling all the
// behavior and logic, so all the MSC compatability is just passed on to that
// function.
// 
// f: The frame to go to.
// s: The scene to go to, or the current scene if null or undefined.
// config: A config object to pass in to the update stage function. Manually
// necessary configuration parameters will be set, but otherwise it will be
// used as-is.
anzovinmod.MainTimeline_Scenes.prototype.gotoAndStop = function(f, s, config) {
  this._gotoAndPlayOrStop(f, s, false, true, false, config);
};

// Wrapper for gotoAndPlay() and gotoAndStop().
// 
// MSC = no. Needs review to ensure it properly supports MSC with required and
// desired behaviors and logic.
// 
// f: The frame number or name to go to.
// s: The scene name to go to. If null, then don't go to any other scene than
// the current one.
// p: Whether to play or stop.
// u: Whether to update the stage after the goto. When the update is called,
// it doesn't perform the tick, because the goto() call to the scene does the
// tick inherently.
// r: Whether to reset the ticker timer after the goto and stage update. This
// isn't particularly needed when stopping.
// config: A config object to pass in to the update stage function. Manually
// necessary configuration parameters will be set, but otherwise it will be
// used as-is.
anzovinmod.MainTimeline_Scenes.prototype._gotoAndPlayOrStop = function(f, s, p, u, r, config) {
  if (this._mtl.isState("hardpaused")) return;
  f = anzovinmod.Utilities.defaultParam(f, null);
  s = anzovinmod.Utilities.defaultParam(s, null);
  config = anzovinmod.Utilities.defaultParam(config, {}, true);
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.MainTimeline_Scenes._gotoAndPlayOrStop", "going to scene '", s, "' frame '", f, "' with playing '", p, "', with updating stage '", u, "' with reset ticker '", r, "'");
  // get the current playing scene
  var currentScene = this.getCurrentScene();
  if (currentScene == null) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_ERROR, "anzovinmod.MainTimeline_Scenes._gotoAndPlayOrStop", "current scene not valid");
    return;
  }
  // check if the named scene is the current scene
  if (s != null && s == currentScene.name) {
    // null indicates NOT to change the scene
    s = null;
  }
  // goto a position in the current scene
  if (s == null) {
    if (currentScene instanceof createjs.MovieClip) {
      f = currentScene.timeline.resolve(f);
    }
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.MainTimeline_Scenes._gotoAndPlayOrStop", "going to current scene frame '", f, "'");
    if (p) {currentScene.gotoAndPlay(f);}
    else {currentScene.gotoAndStop(f);}
    if (u) {
      config["doTick"] = false;
      config["resetTickerTimeout"] = r;
      this._mtl._mtl_stage.updateStage(config);
    }
    else if (r) {this._mtl.resetTickerTimeout();}
    return;
  }
  // find the other scene (we need the scene ID so why not search that first)
  var newSceneId = -1;
  for (var i = 0; i < this._sceneOrder.length; ++i) {
    if (this._sceneOrder[i] == s) {
      newSceneId = i;
      break;
    }
  }
  var newScene = null;
  if (this._scenes.hasOwnProperty(s)) {
    newScene = this._scenes[s];
  }
  if (newSceneId < 0 || newScene == null) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_ERROR, "anzovinmod.MainTimeline_Scenes._gotoAndPlayOrStop", "cannot go to scene/id, not valid: ", newSceneId, " ", newScene);
    return;
  }
  // swap the old/new scenes
  var stopOldScene = (this._currentScene != newSceneId);
  if (newScene instanceof createjs.MovieClip) {
    f = newScene.timeline.resolve(f);
  }
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.MainTimeline_Scenes._gotoAndPlayOrStop", "going to scene '", s, "' frame '", f, "'");
  this._currentScene = newSceneId;
  if (stopOldScene) {oldScene.stop();}
  if (p) {newScene.gotoAndPlay(f);}
  else {newScene.gotoAndStop(f);}
  if (stopOldScene) {oldScene.visible = false;}
  newScene.visible = true;
  if (u) {
    config["doTick"] = false;
    config["resetTickerTimeout"] = r;
    this._mtl._mtl_stage.updateStage(config);
  }
  else if (r) {this._mtl.resetTickerTimeout();}
};

// Increment the scene, hiding, showing, stopping, and playing them as
// necessary. If there is no currently playing scene, then start one now.
// This is primarily used internally on a tick that ends one scene, as the
// default action is to start playing the next scene in line.
anzovinmod.MainTimeline_Scenes.prototype.nextScene = function() {
  // determine any old/new scene IDs and references based on logic
  var currentScene = this.getCurrentScene();
  var currentSceneId = this._currentScene;
  var nextSceneId = currentSceneId + 1;
  if (nextSceneId >= this._sceneOrder.length)
    nextSceneId = 0;
  var nextScene = null;
  if (nextSceneId >= 0 && nextSceneId < this._sceneOrder.length) {
    nextScene = this._scenes[this._sceneOrder[nextSceneId]];
  }
  if (nextSceneId < 0 || nextSceneId >= this._sceneOrder.length || nextScene == null) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_ERROR, "anzovinmod.MainTimeline_Scenes.nextScene", "scene id or object is not valid: ", nextSceneId, " ", nextScene);
    return;
  }
  // swap the old/new scenes
  var stopOldScene = (currentSceneId != newSceneId && currentScene != null);
  this._currentScene = newSceneId;
  if (stopOldScene) oldScene.stop();
  newScene.gotoAndPlay(0);
  if (stopOldScene) this._mtl._textController.hideSceneElements(oldScene.name);
  if (stopOldScene) oldScene.visible = false;
  newScene.visible = true;
};

}());
