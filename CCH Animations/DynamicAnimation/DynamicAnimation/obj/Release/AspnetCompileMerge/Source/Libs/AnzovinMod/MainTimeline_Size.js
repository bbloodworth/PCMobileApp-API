/*! MainTimeline_Size.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// This class handles sizing for the mtl instance.
anzovinmod.MainTimeline_Size = function(mtl) {
  // A reference to the owning mtl instance.
  this._mtl = mtl;
  // The width that was explicitely used as the parameter to resize() last. Or,
  // the default value for the class instance if not yet set.
  this._calledWidth = "window";
  // the height that was explicitely used as the parameter to resize() last. Or,
  // the default value for the class instance if not yet set.
  this._calledHeight = "window";
  // Resize behaviors for this timeline.
  // "canScaleUp": Whether the animation can display larger than its canvas
  // draw size.
  // "canScaleDown": Same, but display smaller than its canvas draw size.
  this._resizeBehaviors = {
    "canScaleUp": true,
    "canScaleDown": true
  };
  // The canvas width. Note that this library only supports a single canvas
  // width that must be the same across all scenes on the stage.
  this._canvasWidth = null;
  // The canvas height. Note that this library only supports a single canvas
  // height that must be the same across all scenes on the stage.
  this._canvasHeight = null;
  // The current display width. This can be equal to the canvas size or window
  // size if "window" or "canvas" was used as part of the resize process, or
  // an explicitely defined size passed in.
  this._currentWidth = null;
  // The current display height. This can be equal to the canvas size or window
  // size if "window" or "canvas" was used as part of the resize process, or
  // an explicitely defined size passed in.
  this._currentHeight = null;
  // The current display canvas scale factor.
  this._currentCanvasScale = null;
  // This is just a placeholder for when triggering a resize call. The resize
  // call is not done immediately, it is only done as the result of a state
  // callback to give something else the opportunity to resize itself.
  this._triggeredCalledWidth = null;
  // This is just a placeholder for when triggering a resize call. The resize
  // call is not done immediately, it is only done as the result of a state
  // callback to give something else the opportunity to resize itself.
  this._triggeredCalledHeight = null;
};

// Resizes the mtl elements to be the given size. The values
// passed in to this function can take on several values and meanings, including
// the ability to auto-resize based on the window width/height. This resizing
// does not happen continuously unless something calls this function: it just
// resizes to the defined size once during this function call.
// 
// If the canvas has been created, then it will be scaled up or down (or not at
// all) until it is fully within the defined size, based on the input params
// and the allowable resizing behaviors. If the canvas instance is not yet
// created, then this is largely ignored.
// 
// If either input value is "window", then it is replaced with the current
// window size as calculated at the moment this function is called.
// 
// If either input value is "canvas", then it is replaced with the actual
// corresponding value of the canvas animation. If the canvas is yet to be
// generated, then it is replaced with "100%".
// 
// If either value is a percent (eg "100%"), then it is replaced with the
// available size of this timeline's main parent element.
// 
// If a value passed in is "null", or undefined, then the default value is used,
// or the value that was used in the last resize call.
// 
// Note about resizing: The logic of the stage scaling and canvas resizing
// I got from this URL, in an attempt to improve performance when rendering to a
// smaller window such as a phone or smaller iframe. The Canvas will be
// rendering to a smaller area, instead of rendering at the full (eg 1080p)
// resolution then downscaling. This looks fine at 100% zoom, but when zooming
// in to the animation, instead of getting a "crisper" view, the view just gets
// blurred. We could always of course, "capture" the zoom event and rescale
// the canvas animation to allow for crisper views, and even to allow for
// some improved performance when zoomed out by further reducing the render
// size of the canvas. That is beyond the focus of this function, however.
// http://community.createjs.com/discussions/createjs/547-resizing-canvas-and-its-content-proportionally-cross-platform
// 
// width: The desired target resize width.
// height: The desired target resize height.
anzovinmod.MainTimeline_Size.prototype.resize = function() {
  var width = this._triggeredCalledWidth;
  var height = this._triggeredCalledHeight;
  this._triggeredCalledWidth = null;
  this._triggeredCalledHeight = null;
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Size.resize", "resizing main timeline to called size ", width, "-", height);
  // use default values for null. this just uses the previously resized()
  // values if any are missing here
  if (width == null) width = this._calledWidth;
  if (height == null) height = this._calledHeight;
  // save currently-called parameters for later use
  var calledWidth = width;
  var calledHeight = height;
  // use "canvas" values if needed. uses the px size of the canvas if it is
  // currently defined, else use "100%" as fallback, which is parent context
  width = (width != "canvas" ? width :
    (this._canvasWidth != null ? this._canvasWidth : "100%")
  );
  height = (height != "canvas" ? height :
    (this._canvasHeight != null ? this._canvasHeight : "100%")
  );
  // get calculated resize values, before any scaling. this is really just to
  // simplify the cases of "window" and "xxx%" and turn them into hard pixels.
  // this is not the FINAL size, just the calculated max size based on the
  // simplifications
  var simplifiedSize = anzovinmod.Utilities.simplifyPotentialResizeValues(
    this._mtl.getParent(), width, height
  );
  if (simplifiedSize == null) return;
  // calculate canvas scale. this uses the simplified size values as the
  // bounding size for the scaling
  var canvasScale = 1.0;
  if (this._canvasWidth != null && this._canvasHeight != null) {
    canvasScale = anzovinmod.Utilities.calculateBoundScale(
      simplifiedSize["width"],
      simplifiedSize["height"],
      this._canvasWidth,
      this._canvasHeight,
      this._resizeBehaviors
    );
  }
  // set current display pixel size. if there's a canvas, use the scaled values
  // from that, otherwise use the simplified values
  var displayWidth = simplifiedSize["width"];
  var displayHeight = simplifiedSize["height"];
  if (this._canvasWidth != null && this._canvasHeight != null) {
    displayWidth = this._canvasWidth * canvasScale;
    displayHeight = this._canvasHeight * canvasScale;
  }
  // set values
  this.setSize(calledWidth, calledHeight, displayWidth, displayHeight, canvasScale);
};

// This function triggers a resize. It does this by setting the called size
// values, then setting the state of the mtl instance. If the state does not
// get unset, such as from an alternate resize callback, the mtl will call
// back into this mtl-size instance and request a resize.
// 
// width: The desired width to resize.
// height: The desired height to resize.
anzovinmod.MainTimeline_Size.prototype.triggerResize = function(width, height) {
  width = anzovinmod.Utilities.defaultParam(width, null);
  height = anzovinmod.Utilities.defaultParam(height, null);
  this._triggeredCalledWidth = width;
  this._triggeredCalledHeight = height;
  this._mtl.setState("willresize", true);
  this._mtl.setState("resize", true);
};

// This function is the final stage in resizing. It takes all the data of the
// resize calculations, and sets all the necessary internal data structures
// in order to properly resize everything.
// 
// This function is separate so as to be able to manually set the resize sizes.
// This allows for testing, as well as for interactions with an external
// wrapping player that can just directly set the resize values of this
// main timeline class after it handles resizing on its own.
// 
// calledWidth: The called width that was passed in to the resize function.
// calledHeight: The called height that was passed in to the resize function.
// displayWidth: The final resulting horizontal size to display this animation.
// displayHeight: Same as previous.
// canvasScale: The scale of the canvas to use. Normally, this is fixed to fit
// within the displayWidth/Height.
anzovinmod.MainTimeline_Size.prototype.setSize = function(calledWidth, calledHeight, displayWidth, displayHeight, canvasScale) {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.MainTimeline_Size.setSize", "called(", calledWidth, ",", calledHeight, ") display(", displayWidth, ",", displayHeight, ") scale(", canvasScale, ")");
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
  this._currentCanvasScale = canvasScale;
  this._mtl.setSizeOnTimelineElements(displayWidth, displayHeight, canvasScale);
};

// This function sets the internal width/height of the canvas elements. This
// should be called as soon as the canvas size is known, so as to give as much
// of a chance to properly scale the canvas. Calling this function does not
// actually perform a resize, it merely sets some of the internal values used
// to calculate resizing later on.
// 
// width: The width of the canvas size, eg the scene width. This is most likely
// something like 1920, for a 1080p animation sequence.
// height: The height of the canvas size, eg the scene height. This is most
// likely something like 1080, for a 1080p animation sequence.
anzovinmod.MainTimeline_Size.prototype.setCanvasSize = function(width, height) {
  this._canvasWidth = width;
  this._canvasHeight = height;
};

// This returns an object containing the "width" and "height" of the canvas.
// 
// Returns:
// "width"/"height" in an object, defining the canvas size.
anzovinmod.MainTimeline_Size.prototype.getCanvasSize = function() {
  return {
    "width":this._canvasWidth,
    "height":this._canvasHeight
  };
};

// Returns the resize behaviors for this instance.
// 
// Returns:
// Resize behaviors object.
anzovinmod.MainTimeline_Size.prototype.getResizeBehaviors = function() {
  return this._resizeBehaviors;
};

}());
