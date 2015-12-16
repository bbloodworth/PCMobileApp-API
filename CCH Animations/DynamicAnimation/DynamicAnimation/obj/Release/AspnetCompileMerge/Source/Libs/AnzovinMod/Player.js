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
