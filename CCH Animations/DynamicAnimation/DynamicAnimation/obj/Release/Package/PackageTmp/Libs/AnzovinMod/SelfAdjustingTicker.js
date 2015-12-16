/*! SelfAdjustingTicker.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// Constructor. This class is responsible for ticking at a defined interval,
// triggering an animation frame. This class specifically is better equipped to
// attempt to self-adjust timer intervals for speeding up or slowing down the
// timers to attempt to keep the animation at a constant framerate.
// 
// This version of the ticker is also able to more easily keep track of the
// framerate and send recommendations to the callbacks about how many frames to
// skip to keep in line with the actual desired framerate. It is up to the
// callbacks to decide whether to accept these recommendations, however.
anzovinmod.SelfAdjustingTicker = function() {
  // The current timer object. When a tick occurs, this is reset to null, and
  // when a tick is scheduled, this is set to the timer value.
  this._timer = null;
  // The current desired framerate. This must be set to some value before the
  // ticker can be started.
  this._fps = null;
  // The callback to call on every tick. This callback function takes an
  // optional first parameter, which is the number of recommended frames to
  // skip. It is either zero or a positive integer.
  this._callback = null;
  // This object contains the last X number of frame render delta-times. It
  // is used to quickly manage them, and to calculate the average framerate over
  // a given delta-t.
  this._tickStatistics = null;
  // This is the time of the last tick, or in the case of the first tick not yet
  // having ticked, the time when the ticks were started. This is used to
  // calculate how long a tick takes to complete.
  this._lastTick = null;
  // This is the time of the first tick. This is used as the start point, to
  // ensure that the framerate is correct throughout the animation and to
  // provide adjustment recommendations from.
  this._startTime = null;
  // The number of ticks that have happened. When the ticker is started (or
  // restarted) this is set to zero, and every tick increments this by one.
  this._tickCount = 0;
  // This is the current recommended number of frames to skip, in order to
  // catch up to the current framerate (or a nearer position than is currently).
  this._recommendedFrameskip = 0;
};

// Simply returns the previously set framerate.
// 
// Returns:
// The framerate, in frames per second.
anzovinmod.SelfAdjustingTicker.prototype.getFPS = function() {
  return this._fps;
}

// Sets the desired framerate for the ticker.
// 
// fps: The desired framerate. Must be > 0.
anzovinmod.SelfAdjustingTicker.prototype.setFPS = function(fps) {
  if (this._timer != null) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_ERROR, "anzovinmod.SelfAdjustingTicker.setFPS", "cannot set tick FPS while ticker is running, stop ticker first");
    return;
  }
  if (fps <= 0) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_WARNING, "anzovinmod.SelfAdjustingTicker.setFPS", "cannot set tick FPS to a value that is not > 0, tick FPS not set");
    return;
  }
  this._fps = fps;
};

// Sets the ticker's callback to the function desired. This function is called
// every time there is a tick.
// 
// f: The function to call on every tick. If unset or null, then any current
// callback is removed.
anzovinmod.SelfAdjustingTicker.prototype.setCallback = function(f) {
  f = anzovinmod.Utilities.defaultParam(f, null);
  this._callback = f;
};

// Simply returns whether or not the ticker is currently started and ticking.
// 
// Returns:
// True if the ticker is started, False otherwise.
anzovinmod.SelfAdjustingTicker.prototype.isStarted = function() {
  return this._timer != null;
};

// Starts the ticker ticking, at the currently desired framerate, if it is not
// yet ticking. If there is no framerate defined yet, then there will be no
// ticking started and an error message printed. If the ticker is already
// ticking, this function does nothing.
anzovinmod.SelfAdjustingTicker.prototype.start = function() {
  if (this._timer != null) return;
  if (this._fps == null) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_ERROR, "anzovinmod.SelfAdjustingTicker.start", "cannot start ticker without a defined framerate");
    return;
  }
  var msTime = 1000.0/this._fps;
  this._recommendedFrameskip = 0;
  this._tickCount = 0;
  this._lastTick = new Date().getTime();
  this._startTime = this._lastTick;
  this._tickStatistics = new anzovinmod.SimpleMovingArray();
  this._tickStatistics.setSize(this._fps*1);
  this._timer = setTimeout(anzovinmod.Utilities.bind(this, this._onTick), msTime);
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.SelfAdjustingTicker.start", "started ticker at ", this._fps, " fps and tick time of ", msTime, " ms");
};

// Stops the ticking if it is started already.
anzovinmod.SelfAdjustingTicker.prototype.stop = function() {
  if (this._timer == null) return;
  clearTimeout(this._timer);
  this._timer = null;
};

// This internal function is called on every tick of the timer. If there is a
// callback defined, then it is called on every tick.
anzovinmod.SelfAdjustingTicker.prototype._onTick = function() {
  // get state on this latest tick time
  var thisTick = new Date().getTime();
  var timeThisTickTook = thisTick - this._lastTick;
  this._tickStatistics.addData(timeThisTickTook);
  // save tick values
  this._lastTick = thisTick;
  this._tickCount++;
  // the number of ms that should exist between frames
  var msPerFrame = 1000.0/this._fps;
  // the number of ticks that should have been reached, when looking at the
  // overall start time and the ms-per-frame value
  var ticksThatShouldHaveTickedByNow = (thisTick - this._startTime) / msPerFrame;
  // the number of ticks we are behind (positive) or ahead (negative).
  var tickDifference = ticksThatShouldHaveTickedByNow - this._tickCount;
  // the ms-time we should wait until the next tick
  var nextOffset = ((-tickDifference) + 1) * msPerFrame;
  if (nextOffset < 1) nextOffset = 1;
  // set the tick timer
  this._timer = setTimeout(anzovinmod.Utilities.bind(this, this._onTick), nextOffset);
  // set the recommended frameskip
  if (tickDifference >= 1) {
    this._recommendedFrameskip = tickDifference;
  } else {
    this._recommendedFrameskip = 0;
  }
  // if there is a callback, use it, otherwise be done
  if (this._callback != null) {
    this._callback(this._recommendedFrameskip);
  }
};

// This function is called whenever the callee of the ticker skips a frame.
// It makes this ticker "know" of the skipped frames so that it does not
// continuously request that frames be skipped later in the calculations.
anzovinmod.SelfAdjustingTicker.prototype.skippedFrame = function() {
  this._tickCount++;
};

// The same as the above, but can be called once to indicate multiple skipped
// frames.
// 
// skippedFrames: The number of skipped frames. If unset or null, then defaults
// to 1. Must be greater than zero.
anzovinmod.SelfAdjustingTicker.prototype.skippedFrames = function(skippedFrames) {
  anzovinmod.Utilities.defaultParam(skippedFrames, 1, true);
  if (!(skippedFrames > 0)) return;
  this._tickCount += skippedFrames;
};

// This function returns an integer number of milliseconds, that is the time
// difference between the ticker's current tick count and the current time.
// 
// The primary purpose of this function is to allow for things to be properly
// timed. For instance, to begin playing an audio file in an animation when
// the audio file is being played on a skipped frame. You wouldn't want to just
// start the audio file being played at the beginning, because it would not be
// in sync after some frames are skipped, so you must start it at an offset.
// 
// This function returns a positive integer, which is the number of milliseconds
// that the current tick count is behind the current time. This function will
// not return a negative number.
// 
// Returns:
// An integer number of milliseconds that is the difference between the ticker's
// current tick count and the current time.
anzovinmod.SelfAdjustingTicker.prototype.getCurrentFrameOffsetTimeFromNow = function() {
  var now = new Date().getTime();
  var msPerFrame = 1000.0/this._fps;
  var ret = now - this._startTime - (msPerFrame * this._tickCount);
  if (ret < 0) ret = 0;
  return ret;
};

// Returns the average framerate between each tick in the last one second. This
// only tracks ticks from the last time it was started. If the ticker is
// stopped, you can still get the results from before the last stop, but once it
// is started again the statistics are reset.
// 
// Returns:
// A number representing the average tick time of the past one second,
// or null if there were no ticks in the last one second or it otherwise cannot
// be calculated (such as if there was just a single tick).
anzovinmod.SelfAdjustingTicker.prototype.getMeasuredFPS = function() {
  if (this._tickStatistics != null) {
    var averageTickDeltaT = this._tickStatistics.calculateAverage();
    return (averageTickDeltaT == 0 ? 0 : 1000.0/averageTickDeltaT);
  }
  return 0;
};

}());
