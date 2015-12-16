/*! AsyncRequestObject.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// Constructor. This class is responsible for sending a request to the
// CCH API, and handling the response. Really this is just a wrapper for the
// XMLHttpRequest object or, in the case of early IE, the appropriate
// replacement ActiveX object. In the case of there needing to be any
// workarounds for particular combinations of browsers or devices, they will
// be here.
// 
// The primary purpose of this class is to handle much of the callback
// and state differences between IE/everything else and asynch setup and
// handlers all all that jazz. Some of the interaction with the request
// object will be included as part of this object/constructor. Other
// interactions may require direct access to the internal request object
// instance, but those should be kept to a minimum.
// 
// Really, if there's something on the internal request object that isn't
// exposed via this class, then it needs to be added.
// 
// type: GET, POST, HEAD, etc. The type of HTTP request being made.
// url: The URL to send as part of this request.
// onSuccess: The callback to use once the request is successful. Function
// will receive a single parameter, which is a handle to this AsyncRequestObject
// instance.
// onFail: The callback to use once the request has failed. Function will
// receive a single parameter, which is a handle to this AsyncRequestObject
// instance.
// onStateChange: A callback to use every time there is a change in the
// request object's connection state. Function will receive a single parameter,
// which is a handle to this AsyncRequestObject instance. This will be called
// when the state changes to a success/fail state. This callback if specified,
// will be called before the onSuccess or onFail callbacks.
anzovinmod.AsyncRequestObject = function(type, url, onSuccess, onFail, onStateChange) {
  type = anzovinmod.Utilities.defaultParam(type, null);
  url = anzovinmod.Utilities.defaultParam(url, null);
  onSuccess = anzovinmod.Utilities.defaultParam(onSuccess, null);
  onFail = anzovinmod.Utilities.defaultParam(onFail, null);
  onStateChange = anzovinmod.Utilities.defaultParam(onStateChange, null);
  if (type == null) {
    var errorMessage = "anzovinmod.AsyncRequestObject() cannot create an empty request object with no type";
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_ERROR, "anzovinmod.AsyncRequestObject", errorMessage);
    throw errorMessage;
  }
  if (url == null) {
    var errorMessage = "anzovinmod.AsyncRequestObject() cannot create an empty request object with no URL";
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_ERROR, "anzovinmod.AsyncRequestObject", errorMessage);
    throw errorMessage;
  }
  
  // INSTANCE VARIABLES
  
  // The type of this request object.
  this._type = type;
  // The URL of this request object.
  this._url = url;
  // A list of all the request headers set. This is so we can set them again
  // when/if we reset the requets object.
  this._setRequestHeaders = [];
  
  // REQUEST OBJECT STUFF
  
  // The request object.
  this._requestObject = null;
  // The type of the request object, eg either XMLHttpRequest or ActiveX.
  // There are anzovinmod.AsyncRequestObject.* constants that you can compare
  // this value against.
  this._requestObjectType = null;
  // The on-success callback. The first parameter of the callback is this
  // AsyncRequestObject object instance.
  this._onSuccessCallback = onSuccess;
  // The on-fail callback. The first parameter of the callback is this
  // AsyncRequestObject object instance.
  this._onFailCallback = onFail;
  // The on-state-change callback. The first parameter of the callback is this
  // AsyncRequestObject object instance.
  this._onStateChangeCallback = onStateChange;
  // An indicator of whether the request object has been activated and the
  // request sent.
  this._requestSent = false;
  
  this._init();
};

// This function is simply the initializer for the constructor. It is called to
// perform any work that is not simply setting default values for the class
// properties.
anzovinmod.AsyncRequestObject.prototype._init = function() {
  // Create the request object instance.
  this._makeRequestObject();
};

// Constant identifiers for the different request object types.
anzovinmod.AsyncRequestObject.REQUEST_OBJECT_XMLHTTPREQUEST = "XMLHttpRequest";
anzovinmod.AsyncRequestObject.REQUEST_OBJECT_ACTIVEXOBJECT_MSXML2_XMLHTTP = "ActiveXObject('Msxml2.XMLHTTP')";
anzovinmod.AsyncRequestObject.REQUEST_OBJECT_ACTIVEXOBJECT_MICROSOFT_XMLHTTP = "ActiveXObject('Microsoft.XMLHTTP')";

// Creates an XMLHttpRequest or ActiveX object for the request, depending on
// which is supported. The internal variable for it is set after this function
// is called, along with an indicator of which was created. Calling this
// function will overwrite an existing request object if one is already created.
anzovinmod.AsyncRequestObject.prototype._makeRequestObject = function() {
  if (this._requestObject != null) {
    this._requestObject.onreadystatechange = function(){};
    this._requestObject.abort();
  }
  // identify the request object types available
  var types = [];
  if (anzovinmod.Utilities.IsIE8() || anzovinmod.Utilities.IsIE9()) {
    types = [
      anzovinmod.AsyncRequestObject.REQUEST_OBJECT_ACTIVEXOBJECT_MSXML2_XMLHTTP,
      anzovinmod.AsyncRequestObject.REQUEST_OBJECT_ACTIVEXOBJECT_MICROSOFT_XMLHTTP,
      anzovinmod.AsyncRequestObject.REQUEST_OBJECT_XMLHTTPREQUEST
    ];
  } else {
    types = [
      anzovinmod.AsyncRequestObject.REQUEST_OBJECT_XMLHTTPREQUEST
    ];
  }
  // create the request object
  for (var i = 0; i < types.length; ++i) {
    var type = types[i];
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.AsyncRequestObject._makeRequestObject", "making a ", type, " request object");
    switch (type) {
      case anzovinmod.AsyncRequestObject.REQUEST_OBJECT_XMLHTTPREQUEST:
        this._requestObject = new XMLHttpRequest();
        this._requestObjectType = type;
        break;
      case anzovinmod.AsyncRequestObject.REQUEST_OBJECT_ACTIVEXOBJECT_MSXML2_XMLHTTP:
        try {
          this._requestObject = new ActiveXObject("Msxml2.XMLHTTP");
          this._requestObjectType = type;
        } catch (e) {
          this._requestObject = null;
          anzovinmod.Logging.logm(anzovinmod.Logging.LOG_WARN, "anzovinmod.AsyncRequestObject._makeRequestObject", "cannot make request object: ", e);
        }
        break;
      case anzovinmod.AsyncRequestObject.REQUEST_OBJECT_ACTIVEXOBJECT_MICROSOFT_XMLHTTP:
        try {
          this._requestObject = new ActiveXObject("Microsoft.XMLHTTP");
          this._requestObjectType = type;
        } catch (e) {
          this._requestObject = null;
          anzovinmod.Logging.logm(anzovinmod.Logging.LOG_WARN, "anzovinmod.AsyncRequestObject._makeRequestObject", "cannot make request object: ", e);
        }
        break;
      default:
        anzovinmod.Logging.logm(anzovinmod.Logging.LOG_WARN, "anzovinmod.AsyncRequestObject._makeRequestObject", "unknown request object type, skipping");
        break;
    }
    if (this._requestObject != null) {
      break;
    }
  }
  // validate request object created
  if (this._requestObject == null) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_ERROR, "anzovinmod.AsyncRequestObject._makeRequestObject", "could not create request object");
    return;
  }
  // set properties on the request object
  this._requestObject.open(this._type, this._url, true);
  this._requestObject.onreadystatechange = anzovinmod.Utilities.bind(this, this._requestObjectStateChangeCallback);
  for (var i = 0; i < this._setRequestHeaders.length; ++i) {
    var reqhead = this._setRequestHeaders[i];
    this._requestObject.setRequestHeader(reqhead[0], reqhead[1]);
  }
};

// This callback function is called on every state change of the request object,
// after the request object has been activated and the URL request sent.
// 
// see: http://www.w3.org/TR/XMLHttpRequest/#dom-xmlhttprequest-readystate
// 
// The "readyState" property of the request object will be an indicator of the
// state of the connection to the remote service.
// 
// 0, UNSENT: The request object has itself been constructed, but has not yet
// even been .open()'ed.
// 1, OPENED: Request object has been .open()'ed, but not yet sent. Request
// headers can be specified at this time.
// 2, HEADERS_RECEIVED: Request has been .send()'ed, and the final response
// headers after any redirects have been read.
// 3, LOADING: The response body is being loaded. In this state, the request
// object may contain a partial representation of the response body and if
// supported, implementation can begin parsing the response while waiting for
// the rest of the resposne to finish load.
// 4, DONE: The entirety of the response body has been loaded and is available.
// Alternatively, an error occurred and there is no response body or a
// partial(?) response body.
// 
// The "status" property of the request object will be an indicator of the
// final HTTP response code from the service, such as "200" or "404".
// 
// Normally, when readyState is "4", then either a success or failure callback
// can be called as the request has finished. This even includes situations
// when CORS is not supported on the remote service and the OPTIONS call
// failed or was not even attempted.
// 
// In FF, no CORS support triggers readyState of [2,4] both with status 0.
// In IE10, no CORS support triggers readyState of just 4 with status 0.
anzovinmod.AsyncRequestObject.prototype._requestObjectStateChangeCallback = function() {
  if (this._onStateChangeCallback != null) {
    this._onStateChangeCallback(this);
  }
  if (this.isDone()) {
    if (this.isSuccess()) {
      if (this._onSuccessCallback != null) {
        this._onSuccessCallback(this);
      }
    } else {
      if (this._onFailCallback != null) {
        this._onFailCallback(this);
      }
    }
  }
};

// This function analyzes the state of the request object, and determines
// whether the request has succeeded or failed. This function will only
// return an accurate value once the request is completed, eg .isDone() returns
// true.
// 
// Returns: True if the request succeeded, false if it failed or still in
// progress or is otherwise not active such as not yet being sent.
anzovinmod.AsyncRequestObject.prototype.isSuccess = function() {
  if (!this._requestSent) {
    return false;
  }
  var state = this._requestObject.readyState;
  var status = this._requestObject.status;
  if (state == 4) {
    if (status == 200) {
      return true;
    }
  }
  return false;
};

// Similar to .isSuccess(), this function will look at the state of the
// request without requiring the request to be completed yet. Eg, this
// can determine if the request is "looking good" while still in progress, eg
// the response headers are 200-OK and the response body is being loaded.
// 
// Returns: True if the request "looks good", false if it does not, or is not
// in progress, or is otherwise not active such as not having been sent yet.
anzovinmod.AsyncRequestObject.prototype.isGood = function() {
  if (!this._requestSent) {
    return false;
  }
  var state = this._requestObject.readyState;
  var status = this._requestObject.status;
  if (state == 2 || state == 3 || state == 4) {
    // todo: add in 2XX codes for cached content for possible type=GET requests?
    if (status == 200) {
      return true;
    }
  }
  return false;
};

// This function reports on whether the requets object is finished sending
// and receiving data. This will only be true when the request has completely
// finished, eg if it is still in progress downloading the response body this
// would still return false.
// 
// Returns: True if the request is finished (either success or failure) or
// false otherwise. If the request has not yet been sent, then this will
// also return false.
anzovinmod.AsyncRequestObject.prototype.isDone = function() {
  if (!this._requestSent) {
    return false;
  }
  var state = this._requestObject.readyState;
  // readyState can = 0 if there is a CORS connection problem, eg the remote
  // server does not support OPTIONS than this will be 0
  return (state == 0 || state == 4);
};

// Returns the response data from a finished (either succeeded or failed) call.
// 
// Returns: A string, or NULL if there is no response data or the response
// is not yet finished.
anzovinmod.AsyncRequestObject.prototype.getResponseData = function() {
  if (this.isDone()) {
    return this._requestObject.responseText;
  }
  return null;
};

// Sends the request with the given content, if applicable for the request
// type. Requests can be reused by simply calling this function whenever one
// wishes for the request to happen.
anzovinmod.AsyncRequestObject.prototype.send = function(content) {
  if (this._requestSent) {
    this._requestObject.abort();
  }
  this._requestSent = true;
  content = anzovinmod.Utilities.defaultParam(content, null);
  this._requestObject.send(content);
};

// Simply sets the request header, exactly like one would do if manually
// interacting with the request object.
// 
// k: The key value for the header.
// v: The value of the header.
anzovinmod.AsyncRequestObject.prototype.setRequestHeader = function(k, v) {
  this._requestObject.setRequestHeader(k, v);
  this._setRequestHeaders.push([k,v]);
};

// Simply returns the ready state of the request object.
// 
// Returns: The 0-5 value of readyState that the internal request object is
// currently within.
anzovinmod.AsyncRequestObject.prototype.getReadyState = function() {
  return this._requestObject.readyState;
};

// Simply returns the status code of the request object, such as 200 or 404. The
// code returned will be 0 if a status code is not yet determined or the
// connection is not yet established, etc.
// 
// Returns: The status code of the request object.
anzovinmod.AsyncRequestObject.prototype.getStatusCode = function() {
  return this._requestObject.status;
};

// Simply returns the user-friendly textual description of the status code
// of the request object. If the status code would be 0, then this is
// an empty string.
// 
// Returns: A string that is the readible version of the status code.
anzovinmod.AsyncRequestObject.prototype.getStatusText = function() {
  return this._requestObject.statusText;
};


anzovinmod.AsyncRequestObject.prototype._timeoutSend = function() {
  this._send();
};

// Just a blank function to override event handlers on the request object, as
// an attempted workaround around IE8 XMLHttpRequest issues.
anzovinmod.AsyncRequestObject._blankFunction = function() {};

}());
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
/*! SimpleMovingArray.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// Constructor. This class contains an internal array and stores a number of
// data elements. Old data elements are overwritten when no longer needed, and
// new data elements are added in their place. Manually doing all this in a
// custom class allows us to more easily avoid having to always push() and
// shift() an a standard array in order to have a simple moving array.
anzovinmod.SimpleMovingArray = function() {
  // The array size.
  this._arraySize = 10;
  // The array data.
  this._arrayData = new Array(10);
  // A pointer to the current write-position in the array.
  this._arrayDataWrite = 0;
  // An indicator of whether or not the array write pointer has wrapped around
  // the array or not. This lets us calculate the number of actual data
  // elements in the array without needing to increment a single value on
  // every iteration.
  this._arrayPointerHasLooped = false;
};

// Resizes the number of elements that the internal array can hold. This
// creates a new array and may reorder the elements in the array so that all
// overwrites occur on the oldest data record.
// 
// size: The new desired size of the array. Must be > 0.
anzovinmod.SimpleMovingArray.prototype.setSize = function(size) {
  size = anzovinmod.Utilities.defaultParam(size, null);
  if (size == null || size <= 0) {
    return;
  }
  if (!this._arrayPointerHasLooped && this._arrayDataWrite == 0) {
    this._arraySize = size;
    this._arrayData = new Array(size);
    return;
  }
  var newArray = new Array(size);
  var sCPc1c2 = this._getSCPc1c2(size);
  var j = 0;
  for (var i = 0; i < sCPc1c2[1]; ++i) {
    newArray[j++] = this._arrayData[i+sCPc1c2[0]];
  }
  for (var i = 0; i < sCPc1c2[2]; ++i) {
    newArray[j++] = this._arrayData[i];
  }
  this._arrayData = newArray;
  this._arraySize = size;
  this._arrayDataWrite = j;
  this._arrayPointerHasLooped = false;
  if (j == size) {
    this._arrayDataWrite = 0;
    this._arrayPointerHasLooped = true;
  }
};

// Simply adds the given data to the moving array.
// 
// d: The data to add.
anzovinmod.SimpleMovingArray.prototype.addData = function(d) {
  this._arrayData[this._arrayDataWrite++] = d;
  if (this._arrayDataWrite == this._arraySize) {
    this._arrayPointerHasLooped = true;
    this._arrayDataWrite = 0;
  }
};

// Returns an average of the data. For this to make sense, the data stored
// must be numerical in nature.
// 
// x: The number of most recent elements to get the average of. If null or
// unspecified, then use all data points gathered.
// 
// Returns: An average of the data set. If there are no elements to the data
// set, then 0 is returned.
anzovinmod.SimpleMovingArray.prototype.calculateAverage = function(x) {
  x = anzovinmod.Utilities.defaultParam(x, null);
  if (x == null) {
    x = this._arraySize;
  }
  var runningTotal = 0;
  var sCPc1c2 = this._getSCPc1c2(x);
  for (var i = 0; i < sCPc1c2[1]; ++i) {
    runningTotal += this._arrayData[i+sCPc1c2[0]];
  }
  for (var i = 0; i < sCPc1c2[2]; ++i) {
    runningTotal += this._arrayData[i];
  }
  if (this._arrayPointerHasLooped) {
    return runningTotal / this._arraySize;
  } else if (this._arrayDataWrite > 0) {
    return runningTotal / this._arrayDataWrite;
  }
  return 0;
};

// In order to copy the internal array into another data structure or iterate
// over the X most recent data entries, we need to know where in the array
// to start iteration from, how many elements to look at until the end of
// the array or X is reached, and how many elements to start at from position
// zero.
// 
// This function returns all three of these numbers, so you can simply
// use them to iterate over the interested data points.
// 
// x: The number of most-recent data points to return sCP/c1/c2 for. If null or
// unspecified, then use all data points.
// 
// Returns: An array with [0]=sCP, [1]=c1, [2]=c2.
anzovinmod.SimpleMovingArray.prototype._getSCPc1c2 = function(x) {
  x = anzovinmod.Utilities.defaultParam(x, null);
  if (x == null) {
    x = this._arraySize;
  }
  var startCopyPosition = 0;
  var copyFirst = 0;
  var copySecond = 0;
  if (!this._arrayPointerHasLooped) {
    // size=10 arraySize=5  write=2 sCP=0 c1=2
    // size=5  arraySize=10 write=2 sCP=0 c1=2
    // size=5  arraySize=10 write=7 sCP=2 c1=5
    copyFirst = this._arrayDataWrite;
    if (x < this._arraySize && this._arrayDataWrite > x) {
      startCopyPosition = this._arrayDataWrite - x;
      copyFirst = this._arrayDataWrite - startCopyPosition;
    }
  } else {
    // size=10 arraySize=5  write=2 sCP=2    c1=3       c2=2
    // size=5  arraySize=10 write=2 sCP=7    c1=5(12)=3 c2=2
    // size=5  arraySize=10 write=7 sCP=12=2 c1=5       c2=0
    startCopyPosition = this._arrayDataWrite;
    copyFirst = this._arraySize - this._arrayDataWrite;
    copySecond = this._arrayDataWrite;
    if (x < this._arraySize) {
      startCopyPosition = this._arraySize + this._arrayDataWrite - x;
      if (startCopyPosition > this._arraySize) {
        startCopyPosition -= this._arraySize;
      }
      copyFirst = x;
      copySecond = 0;
      if (startCopyPosition + x > this._arraySize) {
        copyFirst = this._arraySize - startCopyPosition;
        copySecond = x - copyFirst;
      }
    }
  }
  return [startCopyPosition, copyFirst, copySecond];
};

}());
/*! Utilities.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// No constructor, this should prevent initialization. Also provides an object
// to attach static methods/properties to.
anzovinmod.Utilities = function() {
  throw "cannot instantiate anzovinmod.Utilities";
};

// Holds instance information about createjs objects, including default
// instances of objects and lists of non-object properties.
// In the case of createjs not being defined when this script executes, the
// object instances will just be null instead.
anzovinmod.Utilities.CreateJSObjects = {};

// CreateJS information about the createjs.MovieClip type.
anzovinmod.Utilities.CreateJSObjects.MovieClip = {
  // An instance of a brand new createjs.MovieClip object. This is used in
  // places where the properties of a default object need to be compared to
  // those of an in-use object to determine if properties are display object
  // instances or not.
  "defaultInstance": (typeof createjs !== "undefined" ? new createjs.MovieClip() : null),
  // These properties are attached to the MovieClip object, but are used
  // internally by createjs so should be skipped when iterating over other
  // properties of the MovieClip.
  "nonObjectAttributes": [
    "tweenjs_count",
    "_off",
    "_anzovinmod"
  ]
};

// CreateJS information about the createjs.Container type.
anzovinmod.Utilities.CreateJSObjects.Container = {
  "defaultInstance": (typeof createjs !== "undefined" ? new createjs.Container() : null),
  "nonObjectAttributes": [
    "tweenjs_count",
    "_off",
    "_anzovinmod"
  ]
};

// This function takes a movieclip or container object, and returns a list of
// all the display objects that are attached to the element. This includes
// those objects that are not actually children, but are attached as properties
// to the movieclip/container.
// 
// This function take a default created instance of the type of object passed
// in, and strips out any on-object properties that are a part of the default
// object. This effectively strips out any internally used variables, such as
// "x", "y", etc.
// 
// e: The element to search for children in.
// 
// Return: An array of properties that are child display objects.
anzovinmod.Utilities.listChildDisplayObjectProperties = function(e) {
  e = anzovinmod.Utilities.defaultParam(e, null);
  var ret = [];
  if (e == null) {
    return ret;
  }
  // determine information necessary for recursive calls
  var objectInformation = null;
  if (typeof createjs !== "undefined" && e instanceof createjs.MovieClip) {
    objectInformation = anzovinmod.Utilities.CreateJSObjects.MovieClip;
  } else if (typeof createjs !== "undefined" && e instanceof createjs.Container) {
    objectInformation = anzovinmod.Utilities.CreateJSObjects.Container;
  }
  // if a container type, then recursively call on properties
  if (objectInformation != null) {
    for (var eElement in e) {
      // initial check for default properties. only properties that are not
      // a part of the prototype could possible be display objects, so all the
      // rest can just be discarded
      if (!e.hasOwnProperty(eElement)) {
        continue;
      }
      // is a default property, so skip. these properties are a part of the
      // default object instance, assigned to the object instance during
      // creation, and no longer contain the default prototype value
      if (objectInformation.defaultInstance.hasOwnProperty(eElement)) {
        continue;
      }
      // enhanced search for default properties that have yet to be modified
      // from the prototype's initial value of it. anything that is a part of
      // the base element's properties are not going to be display objects that
      // we are interested in, so we can skip them
      if (eElement in objectInformation.defaultInstance) {
        continue;
      }
      // is a non-object attribute, so skip. these are manually defined
      // properties that may not show up in the default property checks above,
      // but should be skipped. maybe these are used by createjs, or maybe
      // these are used for our own purposes
      if (objectInformation.nonObjectAttributes.indexOf(eElement) != -1) {
        continue;
      }
      // is a function, so skip it. this takes into account all the "frame_#"
      // functions that maybe defined on the root movieclip object or others
      if (typeof e[eElement] === "function") {
        continue;
      }
      // is a valid property, add it
      ret.push(eElement);
    }
  }
  return ret;
};

// A helper function to re-scope function calls cleanly.
// 
// Executed like: (this == obj)
//  bind(obj, function() {console.log(this);})
// 
// scope: The object to "own" the function call.
// fn: The function to call under a bound scope.
// 
// Returns: A function that can be immediatelly called with any arguments as
// desired, that will execute under the desired scope.
anzovinmod.Utilities.bind = function(scope, fn) {
  return function () {
      fn.apply(scope, arguments);
  };
};

// A helper function to use default values in functions.
// 
// A parameter to a function, if left undefined, will be equal to 'undefined'
// type when the function executes. Using this function, you can set up a
// guarantee that parameters will be set to sensicle default values.
// 
// Example:
// var x = function(a) {console.log(a);};
// y();    // undefined
// y(123); // 123
// 
// 'undefined' is not actually a keyword (like null). As such, strange behaviors
// can occur with using the 'undefined' variable word. As such, it may be more
// appropriate to use 'null' as an indicator that a parameter should be a
// default value. For example, in a function(a,b,c), the parameter b might have
// a default of 12 and c might have a default of 50. If someone wished to use
// the default value for b but specify a value manually for c, one could call
// the function as: f(1,undefined,22).
// 
// If you wish to allow for f(1,null,22) to accomplish the same thing, then
// use the 'dOnNull' parameter to this utility function. It will use the default
// value in the case of undefined and null, so as to let you be a bit more
// secure in your use of 'undefined' property.
// 
// v: The parameter that may or may not be defined (usually function input).
// d: The default value to use if the parameter v is not defined.
// dOnNull: Also returns d if v is null, not just undefined.
// 
// Returns: If v is undefined, then d is returned. If v is null and dOnNull is
// true, then d is returned. Otherwise, v is returned.
anzovinmod.Utilities.defaultParam = function(v, d, dOnNull) {
  // optimize for more speed
  if (dOnNull !== true) {
    return (typeof(v) == "undefined" ? d : v);
  } else {
    return (typeof(v) == "undefined" ? d : (v === null ? d : v));
  }
};

// Deep clones an array.
// 
// source: The array to clone.
// deep: Whether to deep clone. Defaults false.
// arrayLength: If the cloned object is an array, then only clone this number
// of elements from the beginning of the array. If unset, then clone the
// entire array. If greater than the array length, then clones the entire array.
// If less than zero, then return an empty array.
// 
//
anzovinmod.Utilities.cloneArray = function(source, deep, arrayLength) {
  deep = anzovinmod.Utilities.defaultParam(deep, false, true);
  arrayLength = anzovinmod.Utilities.defaultParam(arrayLength, null);
  if (source instanceof Array) {
    var ret = [];
    var iter = source.length;
    if (arrayLength != null) {
      if (arrayLength >= 0 && arrayLength <= source.length) {
        iter = arrayLength;
      }
    }
    if (deep) {
      while (iter--) {
        ret[iter] = anzovinmod.Utilities.cloneObject(source[iter], deep);
      }
    } else {
      while (iter--) {
        ret[iter] = source[iter];
      }
    }
    return ret;
  }
  return null;
};

// Deep clones an object. Note that this is not tested for complex objects,
// but should work fine for basic types such as those being used as
// associative arrays created with {}.
// 
// see: http://stackoverflow.com/questions/728360/most-elegant-way-to-clone-a-javascript-object
// see: http://stackoverflow.com/questions/122102/what-is-the-most-efficient-way-to-clone-an-object
// 
// source: The object to clone.
// deep: Whether to deep clone. Defaults false.
// 
// Returns: A new cloned instance of the object, not a reference to any other
// existing objects.
anzovinmod.Utilities.cloneObject = function(source, deep) {
  deep = anzovinmod.Utilities.defaultParam(deep, false, true);
  if (source == null) {
    return source;
  }
  if (source instanceof Array) {
    return anzovinmod.Utilities.cloneArray(source, deep);
  }
  if (source instanceof Object) {
    var ret = {};
    if (deep) {
      for (var i in source) {
        if (!source.hasOwnProperty(i)) {
          continue;
        }
        ret[i] = anzovinmod.Utilities.cloneObject(source[i], deep);
      }
    } else {
      for (var i in source) {
        if (!source.hasOwnProperty(i)) {
          continue;
        }
        ret[i] = source[i];
      }
    }
    return ret;
  }
  return source;
};

// Just convert the given object into a string reproducible facsimile.
// Will safely convert any NULL objects into the string "NULL". This is
// really just so that we can say log(pnt(x)) instead of having to include
// any special code for nullability checks and the like, even though JS seems
// to be good about handling that sort of thing automatically and often has
// its own handling of things like expanding objects onto the DOM when logged.
// 
// Note that for non-simple Object types (eg objects not created with {} but
// with a function constructor) they are simply returned as .toString() and not
// iterated over all elements. This is to help ensure that recursion does not
// take place in custom objects acting as classes, though this may still occur
// in simple Object types.
// 
// The more root reason for this, is that many of the types used in this project
// have .parent nodes that themselves have .children nodes, which creates an
// easy recursion.
// 
// Additionally, in order to help prevent infinite recursion in standard
// Objects (and Arrays and anything else that may have a recursive element in
// it), a level-limit can be used to prevent this sort of output. The second
// parameter to this function can optionally be specified to control the number
// of recursive printing to perform.
// 
// x: The object to print.
// maxRecursion: The maximum number of levels to print on objects and arrays.
// Defaults to 5.
// 
// Returns:
// A non-null string instance that describes the object.
anzovinmod.Utilities.pnt = function(x, maxRecursion) {
  maxRecursion = anzovinmod.Utilities.defaultParam(maxRecursion, 5);
  if (maxRecursion == 0) {
    return "...";
  }
  if (typeof(x) === "undefined") {
    return "UNDEFINED";
  } else if (x === null) {
    return "NULL";
  } else if (x instanceof Array) {
    var s = "[";
    for (var i = 0; i < x.length; ++i) {
      if (i != 0) {
        s += ",";
      }
      s += anzovinmod.Utilities.pnt(x[i], maxRecursion-1);
    }
    s += "]";
    return s;
  } else if (x instanceof Object && typeof(x) == "object") {
    // if the object constructor is "Object", then this was created with {} and
    // has a generally lower liklihood of being recursive as it likely does not
    // have .parent and .children elements that could easily cause recursion
    if (!(x && x.constructor && x.constructor.name && x.constructor.name == "Object")) {
      return x.toString();
    }
    var s = (x && x.constructor ? x.constructor.name : "");
    if (s == null || s == "") {
      s = "unknown";
    }
    s += "{";
    var count = 0;
    for (var i in x) {
      if (!x.hasOwnProperty(i)) {
        continue;
      }
      if (count != 0) {
        s += ",";
      }
      s += i + ":" + anzovinmod.Utilities.pnt(x[i], maxRecursion-1);
      ++count;
    }
    s += "}";
    return s;
  }
  return x.toString();
};

// Convert the given Matrix2D object into a CSS transform "matrix(...)" string.
// 
// m: Matrix2D instance to convert
// 
// Returns: A string that can be used in CSS transform properties.
anzovinmod.Utilities.cssTransformMatrix2D = function(m) {
  return "matrix(" + m.a + "," + m.b + "," + m.c + "," +
    m.d + "," + m.tx + "px," + m.ty + "px)";
};

// This function attempts to get the window size of the current display.
// 
// The method of obtaining the window width/height is left up to this function.
// The width/height returned ideally will be the exact pixels of the screen area
// available under the current display.
// 
// Common ways to calculate the width/height include: directly use the
// window.innerWidth value; use the document.body.clientWidth value after
// hiding all other content and temporarily setting its styles manually to have
// 100% width, no borders, margins, or padding, etc; etc.
// 
// This function may temporarily apply CSS to the document or body or child
// DOM elements in order to get a good calculation. Whatever changes are made
// will be only temporary and should ideally cause as little interference in
// the page content as possible.
// 
// Returns: An object that contains the properties "width" and "height".
anzovinmod.Utilities.getWindowSize = function() {
  var originalStyles = {
    "visibility": document.body.style.visibility,
    "overflow": document.body.style.overflow
  };
  document.body.style.visibility = "hidden";
  document.body.style.overflow = "hidden";
  var ret = {
    //"width":  window.innerWidth,
    //"height": window.innerHeight
    "width":  document.documentElement.clientWidth,
    "height": document.documentElement.clientHeight
  };
  document.body.style.visibility = originalStyles["visibility"];
  document.body.style.overflow = originalStyles["overflow"];
  return ret;
};

// This function is used to help resize elements in this project. It takes in
// a desired width/height combination that may be keyworded in particular ways,
// such as by referring to the window size or the containing element size,
// and calculates the final pixel size of the element in question.
// 
// The arguments of width/height can take on several forms. The exact form of
// which determines how the element is resized.
// 
// "window" / "100%" / 800
// 
// "window": Resizes based on the window size.
// "100%": Resizes based on the containing parent element size.
// 800: Just resizes to the given pixel size.
// 
// parent: The parent of the object node that is being resized.
// width: The desired width of the object.
// height: The desired height of the object.
// 
// Returns: An object that contains "width"/"height" attributes that are the
// calculated width/height based on the inputs.
anzovinmod.Utilities.simplifyPotentialResizeValues = function(parent, width, height) {
  parent = anzovinmod.Utilities.defaultParam(parent, null);
  width = anzovinmod.Utilities.defaultParam(width, null);
  height = anzovinmod.Utilities.defaultParam(height, null);
  // return null on all, require all parameters
  if (parent == null || width == null || height == null) {
    return null;
  }
  // determine window size if that is desired
  if (width == "window" || height == "window") {
    var windowSize = anzovinmod.Utilities.getWindowSize();
    if (width == "window") {
      width = windowSize["width"];
    }
    if (height == "window") {
      height = windowSize["height"];
    }
  }
  // if size is a percent, determine pixel size
  var isPercentW = false;
  var isPercentH = false;
  if (typeof(width) == "string" && width.substr(width.length-1, 1) == "%") {
    isPercentW = true;
  }
  if (typeof(height) == "string" && height.substr(height.length-1, 1) == "%") {
    isPercentH = true;
  }
  if (isPercentW || isPercentH) {
    // calculate available space
    var originalDisplays = [];
    for (var i = 0; i < parent.children.length; ++i) {
      var child = parent.children[i];
      if (child && child.style && child.style.display) {
        originalDisplays.push({"i":i,"d":child.style.display});
        child.style.display = "none !important";
      }
    }
    var parentWidth = parent.clientWidth;
    var parentHeight = parent.clientHeight;
    for (var i = 0; i < originalDisplays.length; ++i) {
      var j = originalDisplays[i];
      parent.children[j["i"]].style.display = j["d"];
    }
    // simplify corresponding values
    if (isPercentW) {
      var percent = parseFloat(width);
      if (isNaN(percent)) {
        percent = 100.0;
      }
      width = parentWidth * percent / 100.0;
    }
    if (isPercentH) {
      var percent = parseFloat(height);
      if (isNaN(percent)) {
        percent = 100.0;
      }
      height = parentHeight * percent / 100.0;
    }
  }
  // return calculated values
  return {
    "width": width,
    "height": height
  };
};

// Determines the presentation scale of an object when given its original and
// max potential scaled dimensions. The final scale is limited to the
// smaller of the resulting horizontal and vertical scaling, to keep the
// presented object within a bounding box and maintain aspect ratio.
// 
// x: The current/desired width of the object, in px.
// y: The current/desired height of the object, in px.
// baseX: The original width of the object.
// baseY: The original height of the object.
// resizeBehaviors: An object contain properties relating to resize behaviors.
// 
// Returns: The final scale value. Will return 0.0 if there was a problem with
// inputs or calculations. Will return 0.0 if the base inputs are zero.
anzovinmod.Utilities.calculateBoundScale = function(x, y, baseX, baseY, resizeBehaviors) {
  x = anzovinmod.Utilities.defaultParam(x, null);
  y = anzovinmod.Utilities.defaultParam(y, null);
  baseX = anzovinmod.Utilities.defaultParam(baseX, null);
  baseY = anzovinmod.Utilities.defaultParam(baseY, null);
  resizeBehaviors = anzovinmod.Utilities.defaultParam(resizeBehaviors, {}, true);
  if (x == null || y == null || baseX == null || baseY == null) {
    return 0.0;
  }
  if (!resizeBehaviors.hasOwnProperty("canScaleUp"))
    resizeBehaviors["canScaleUp"] = true;
  if (!resizeBehaviors.hasOwnProperty("canScaleDown"))
    resizeBehaviors["canScaleDown"] = true;
  if (baseX == 0 || baseY == 0) {
    return 0.0;
  }
  var scaleWidth = x / baseX;
  var scaleHeight = y / baseY;
  var finalScale = (scaleHeight > scaleWidth ? scaleWidth : scaleHeight);
  if (!resizeBehaviors["canScaleUp"] && finalScale > 1.0) {
    finalScale = 1.0;
  }
  if (!resizeBehaviors["canScaleDown"] && finalScale < 1.0) {
    finalScale = 1.0;
  }
  return finalScale;
};

// This function removes a part of a string. This can be used for a number of
// purposes, but one specific purpose is to add and remove classes from class
// definitions easily.
// 
// Note that if the string part is not found in the whole string,
// then the original string should be returned. Also note that this will
// search for all instances of the definition, and will not stop at a single
// instance of the part.
// 
// whole: The current string to remove a part from.
// part: The string part to remove from the whole.
// 
// Returns: The new string. Will return an empty string if inputs were null.
anzovinmod.Utilities.removeStringComponent = function(whole, part, sep) {
  whole = anzovinmod.Utilities.defaultParam(whole, "", true);
  part = anzovinmod.Utilities.defaultParam(part, "", true);
  sep = anzovinmod.Utilities.defaultParam(sep, " ", true);
  if (whole == "") return "";
  if (part == "") return whole;
  var wholePieces = whole.split(sep);
  for (var i = 0; i < wholePieces.length; ++i) {
    if (wholePieces[i] == part) {
      wholePieces.splice(i, 1);
      --i;
    }
  }
  return wholePieces.join(sep);
};

// The opposite of the removeStringComponent() function, this one adds the
// string part to the whole class definition, if it is not already contained
// within it.
// 
// whole: The current string to add a part to.
// part: The string part to add.
// 
// Returns: The new string.
anzovinmod.Utilities.addStringComponent = function(whole, part, sep) {
  whole = anzovinmod.Utilities.defaultParam(whole, "", true);
  part = anzovinmod.Utilities.defaultParam(part, "", true);
  sep = anzovinmod.Utilities.defaultParam(sep, " ", true);
  if (part == "") {
    return whole;
  }
  var wholePieces = whole.split(sep);
  for (var i = 0; i < wholePieces.length; ++i) {
    if (wholePieces[i] == part) {
      return whole;
    }
  }
  wholePieces.push(part);
  return wholePieces.join(sep);
};

// Returns whether or not the whole string component contains the partial string
// component. One use of this, is to check whether a CSS class exists in a
// single definition.
// 
// whole: The current class definition string.
// part: The class definition to check for.
// 
// Returns: True if the string is a part of the whole, else false.
anzovinmod.Utilities.hasStringComponent = function(whole, part, sep) {
  whole = anzovinmod.Utilities.defaultParam(whole, "", true);
  part = anzovinmod.Utilities.defaultParam(part, "", true);
  sep = anzovinmod.Utilities.defaultParam(sep, " ", true);
  if (whole == "" || part == "") {
    return false;
  }
  var wholePieces = whole.split(sep);
  for (var i = 0; i < wholePieces.length; ++i) {
    if (wholePieces[i] == part) {
      return true;
    }
  }
  return false;
};

// This helper function sets a DOM element property. This is a helper function
// because there is specific behaviors that must be followed when setting this
// data on IE8 systems.
// 
// In short, when on >IE8 or another browser, setting a property on a DOM
// element is different than setting an attribute. Attributes are of the form
// 'class="some-css-class"', while properties are more intangible. IE8 just
// converts the property into an attribute, so it must be converted to a string
// instead.
// 
// However, this doesn't resolve the issue of references to other elements being
// resolved, as not all property values are simple JSON values: Some may have
// references to other objects or DOM elements that should be maintained. The
// solution of this Utilities file, is to create an array of
// elements/names/properties so as to not have the properties directly attached
// to the objects in question. This way, the properties can still be referenced
// outside of the DOM element in question.
// 
// The only big thing about this, is that memory leaks may occur unless you
// manually remove the element from this kept list. So, when a DOM element is
// removed, it should be removed from this list as well.
// 
// elem: The DOM element that is getting a property set.
// name: The name of the property.
// prop: The property itself (can be anything).
anzovinmod.Utilities.setDomElementProperty = function(elem, name, prop) {
  elem = anzovinmod.Utilities.defaultParam(elem, null);
  name = anzovinmod.Utilities.defaultParam(name, null);
  if (elem == null || name == null) return;
  if (!anzovinmod.Utilities.IsIE8()) {
    elem[name] = prop;
    return;
  }
  var deps = anzovinmod.Utilities._DomElementProperties;
  for (var i = 0; i < deps.length; ++i) {
    if (deps[i]["elem"] == elem) {
      deps[i]["props"][name] = prop;
      return;
    }
  }
  var o = {"elem":elem, "props":{}};
  o["props"][name] = prop;
  deps.push(o);
};

// Obtains the element property from the given DOM element.
// 
// elem: The DOM element to obtain a property of.
// name: The name of the property to get.
// 
// Returns: The value of the property, or UNDEFINED if not one set.
anzovinmod.Utilities.getDomElementProperty = function(elem, name) {
  elem = anzovinmod.Utilities.defaultParam(elem, null);
  name = anzovinmod.Utilities.defaultParam(name, null);
  if (elem == null || name == null) return;
  if (!anzovinmod.Utilities.IsIE8()) {
    return elem[name];
  }
  var deps = anzovinmod.Utilities._DomElementProperties;
  for (var i = 0; i < deps.length; ++i) {
    if (deps[i]["elem"] == elem) {
      return deps[i]["props"][name];
    }
  }
  return;
};

// Removes the given DOM element from the internal list of stored DOM element
// properties. This is called when the DOM element is to be removed from the
// DOM graph and should be destroyed/garbage collected/whatever.
// 
// elem: The DOM element to remove all listings for.
anzovinmod.Utilities.clearDomElementProperties = function(elem) {
  elem = anzovinmod.Utilities.defaultParam(elem, null);
  if (elem == null) return;
  if (!anzovinmod.Utilities.IsIE8()) {
    return;
  }
  var deps = anzovinmod.Utilities._DomElementProperties;
  for (var i = 0; i < deps.length; ++i) {
    if (deps[i]["elem"] == elem) {
      deps.splice(i, 1);
      return;
    }
  }
};

// A list of the DOM element properties that have been set through calls to
// setDomElementProperty(). Clear out a DOM element with a call to
// clearDomElementProperties() when done with a DOM element.
// 
// Each value of this array is an object with the following format:
// "elem": A reference to the DOM element.
// "props": An object that stores name:value pairs of properties.
anzovinmod.Utilities._DomElementProperties = [];

// An indicator of whether this is IE8 or not. This can easily be set in the
// calling HTML file through something like the following:
// 
// <!--[if IE 8]>
// <script type="text/javascript">
// var anzovinmod = anzovinmod || {};
// anzovinmod.instance = anzovinmod.instance || {};
// anzovinmod.instance.IsIE8 = true;
// </script>
// <![endif]-->
// 
// These functions just use the anzovinmod.instance properties to make the
// final determinations.
anzovinmod.Utilities.IsIE8 = function() {
  return anzovinmod && anzovinmod.instance && anzovinmod.instance.IsIE8;
};

// An indicator of whether this is IE9.
anzovinmod.Utilities.IsIE9 = function() {
  return anzovinmod && anzovinmod.instance && anzovinmod.instance.IsIE9;
};

}());
/*! Logging.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// Constructor, should prevent instantiation. Also provides an object to
// attach static methods and properties to.
anzovinmod.Logging = function() {
  throw "cannot instantiate anzovinmod.Logging";
};

// The current loglevel.
anzovinmod.Logging._LogLevel = 4;

anzovinmod.Logging.LOG_ERROR  = 0;
anzovinmod.Logging.LOG_WARN   = 1;
anzovinmod.Logging.LOG_NOTICE = 2;
anzovinmod.Logging.LOG_INFO   = 3;
anzovinmod.Logging.LOG_DEBUG  = 4;
anzovinmod.Logging.LOG_TRACE  = 5;
anzovinmod.Logging.LOG_TRACE_1  = 5;
anzovinmod.Logging.LOG_TRACE_2  = 6;
anzovinmod.Logging.LOG_TRACE_3  = 7;
anzovinmod.Logging.LOG_TRACE_4  = 8;
anzovinmod.Logging.LOG_TRACE_5  = 9;
anzovinmod.Logging.LOG_TRACE_6  = 10;
anzovinmod.Logging.LOG_TRACE_7  = 11;
anzovinmod.Logging.LOG_TRACE_8  = 12;
anzovinmod.Logging.LOG_TRACE_9  = 13;
anzovinmod.Logging.MAX_LOG_LEVEL = 13;

// Simply returns true/false if the current logging level supports the
// given log level. This is useful for when constructing even the pieces of
// a log message are tedious or long-winded, so you can wrap the creation
// of the log message in a condition.
// 
// l: The log level to test.
// 
// Returns:
// True if the current log level would write a message with level 'l',
// False if it would not write a message.
anzovinmod.Logging.canLog = function(l) {
  return !(l > anzovinmod.Logging._LogLevel);
};

// Just returns the string component of the log message that corresponds to the
// log level, like "-warn--".
// 
// In the case of an invalid log level, then "INVALID" is a code used to
// indicate as such.
// 
// l: The log level to get the string component of.
// 
// Returns: A string for use in logging with.
anzovinmod.Logging._getLogLevelString = function(l) {
  if (typeof(l) != "number") l = -1;
  if (l > anzovinmod.Logging.MAX_LOG_LEVEL) l = -1;
  if (l < 0) l = -1;
  l = parseInt(l);
  switch (l) {
    case -1:                            return "INVALID--- ";
    case anzovinmod.Logging.LOG_ERROR:  return "error----- ";
    case anzovinmod.Logging.LOG_WARN:   return "-warn----- ";
    case anzovinmod.Logging.LOG_NOTICE: return "--notice-- ";
    case anzovinmod.Logging.LOG_INFO:   return "---info--- ";
    case anzovinmod.Logging.LOG_DEBUG:  return "----debug- ";
    default:                            return "-----trace ";
  }
  return "INVALID--- ";
};

// Traces the given statement. Only traces if the current log level is
// equal to or greater than indicated.
// 
// l: The log level of this statement.
// s: The object to trace out as a string. This can be specified multiple times
// in the function call.
// 
// Returns:
// True if traced, false if not traced.
anzovinmod.Logging.logit = function(l) {
  if (arguments.length <= 1) return false;
  if (typeof console === "undefined") return false;
  if (l > anzovinmod.Logging._LogLevel) return false;
  var logout = anzovinmod.Logging._getLogLevelString(l);
  logout += ": ";
  for (var i = 1; i < arguments.length; ++i) {
    logout += anzovinmod.Utilities.pnt(arguments[i]);
  }
  console.log(logout);
  return true;
};

// Traces the given statement. Only traces if the current log level is
// equal to or greater than indicated. This version accepts an additional
// parameter, which is the method that called this log function. This is used
// to handle formatting of log messages.
// 
// l: The log level of this statement.
// m: The method that called this log level (or other identifier).
// s: The object to trace out as a string. This can be specified multiple times
// in the function call.
// 
// Returns:
// True if traced, false if not traced.
anzovinmod.Logging.logm = function(l, m) {
  if (arguments.length <= 2) return false;
  if (typeof console === "undefined") return false;
  if (l > anzovinmod.Logging._LogLevel) return false;
  var logout = anzovinmod.Logging._getLogLevelString(l);
  logout += ": " + m + "() ";
  for (var i = 2; i < arguments.length; ++i) {
    logout += anzovinmod.Utilities.pnt(arguments[i]);
  }
  console.log(logout);
  return true;
};

// Traces the given statement. Only traces if the current log level is
// equal to or greater than indicated. This version only traces once per
// argument passed in to this function, and each argument/object is on its own
// trace line with no other content. This is useful for tracing objects directly
// to the console, so you can interact with them. Additionally, no processing
// is performed on these arguments/objects being logged.
// 
// l: The log level of this statement.
// o: The object to trace out as a string. This can be specified multiple times
// in the function call.
// 
// Returns:
// True if traced, false if not traced.
anzovinmod.Logging.logo = function(l) {
  if (arguments.length <= 1) return false;
  if (typeof console === "undefined") return false;
  if (l > anzovinmod.Logging._LogLevel) return false;
  for (var i = 1; i < arguments.length; ++i) {
    console.log(arguments[i]);
  }
  return true;
}

// This function sets the global logging level. Levels that are below this
// level will not be printed to the console log, but levels that match or are
// greater than this will be printed.
// 
// l: The log level to set to.
anzovinmod.Logging.setLogLevel = function(l) {
  anzovinmod.Logging._LogLevel = l;
};

// Just returns the current logging level.
// 
// Returns:
// The current log level.
anzovinmod.Logging.getLogLevel = function() {
  return anzovinmod.Logging._LogLevel;
};

}());
/*! MainTimeline_Sound.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// This class is just a manager for sound interactions with the mtl.
anzovinmod.MainTimeline_Sound = function(mtl) {
  // A reference back to the originating mtl instance.
  this._mtl = mtl;
  // A list of all the currently paused sounds. Upon resuming, these sounds
  // are resumed then cleared from this list.
  this._pausedSounds = [];
};

// This function simply plays the given sound. Options are
// available to help control exactly how the sound is played.
// 
// A note about the startPaused option: This option causes the sound to start
// playing in a muted state, pause it, and it is added to the internal list of
// paused sounds. A call to unpause all sounds will resume it, along with any
// other sounds that were playing. This can be used to great affect in order to
// start a bunch of sounds at once moment, such as when changing position in an
// animation or even just starting it for the first time.
// 
// Options are specified as a configuration object that can contain any or all
// of the option parameters:
// 
// offset: Defaults 0. The time offset to play the sound.
// addTickerTimeOffset: Defaults True. If True, adds the ticker time offset to
// the given offset when playing the sound. This helps ensure that the sound
// being played is played in proper sync, taking into account recommended
// skipped frames, etc.
// startPaused: Defaults False. If True, then the sound is started in as much of
// a paused state as is possible. The sound is started (as its calculated
// offset) and immediately paused and added to the paused sounds list.
// 
// Actual function parameters follow:
// 
// id: The string id of the sound to play.
// config: The configuration object or null for all defaults. Has properties
// equal to the previously mentioned properties.
anzovinmod.MainTimeline_Sound.prototype.playSoundConfig = function(id, config) {
  id = anzovinmod.Utilities.defaultParam(id, null);
  if (id == null) return;
  config = anzovinmod.Utilities.defaultParam(config, {}, true);
  if (!config.hasOwnProperty("offset"))
    config["offset"] = 0;
  if (!config.hasOwnProperty("addTickerTimeOffset"))
    config["addTickerTimeOffset"] = true;
  if (!config.hasOwnProperty("startPaused"))
    config["startPaused"] = false;
  // adjust the offset if necessary
  var offset = config["offset"];
  // note that adjusting the offset time here will apply the offset (+/-) to
  // whatever offset value was passed in to this function. if the resulting
  // offset is positive, then the offset is used to play the sound. if the
  // resulting offset is negative, it is just treated as zero
  if (config["addTickerTimeOffset"])
    offset += this._mtl._ticker.getCurrentFrameOffsetTimeFromNow();
  if (offset < 0) offset = 0;
  // check that the sound is able to be played, based on duration and offset
  var audioManifest = this._mtl._mtl_scenes.getSoundAudioManifest(id);
  if (audioManifest != null) {
    if (offset >= audioManifest["duration"] * 1000) {
      anzovinmod.Logging.logm(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.MainTimeline_Sound.playSoundConfig", "not playing sound, offset >= duration: ", id, " with final offset: ", offset);
      return;
    }
  }
  // log and play sound
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Sound.playSoundConfig", "playing sound: ", id, " with final offset: ", offset);
  var sound = createjs.Sound.play(id, {
    "interrupt": createjs.Sound.INTERRUPT_NONE,
    "offset": offset,
    "volume": (config["startPaused"] ? 0.0 : 1.0)
  });
  if (sound["playState"] != createjs.Sound.PLAY_SUCCEEDED) {
    if (sound.hasOwnProperty("duration") && sound["duration"] != null) {
      if (offset > sound["duration"]) {
        anzovinmod.Logging.logm(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.MainTimeline_Sound.playSoundConfig", "sound did not play, but offset > length so this is likely normal: ", sound["playState"], " ", sound);
      } else {
        anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Sound.playSoundConfig", "sound did not play, but offset < duration: ", sound["playState"], " ", sound);
      }
    } else {
      anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Sound.playSoundConfig", "sound did not play, but cannot tell sound duration: ", sound["playState"], " ", sound);
    }
  } else {
    if (config["startPaused"]) {
      sound.pause();
      this._pausedSounds.push(sound);
      sound.volume = 1.0;
    }
  }
};

// Simply clears out any paused sounds so they do not resume themselves.
anzovinmod.MainTimeline_Sound.prototype.clearPausedSounds = function() {
  for (var i = 0; i < this._pausedSounds.length; ++i) {
    this._pausedSounds[i].stop();
  }
  this._pausedSounds = [];
};

// This function pauses all sounds being played from the createjs.Sound
// class. This is not restricted to any scene or any instance of the mtl:
// This will stop ALL sounds, regardless of their origin.
anzovinmod.MainTimeline_Sound.prototype.pauseSounds = function() {
  for (var i = 0; i < createjs.Sound._instances.length; ++i) {
    var instance = createjs.Sound._instances[i];
    if (instance.paused == false) {
      instance.pause();
      this._pausedSounds.push(instance);
    }
  }
};

// This function resumes all sounds that were previously paused through a call
// to pauseSounds(), or paused through playing with the appropriate config
// option to start them "paused".
anzovinmod.MainTimeline_Sound.prototype.resumeSounds = function() {
  for (var i = 0; i < this._pausedSounds.length; ++i) {
    var instance = this._pausedSounds[i];
    instance.resume();
  }
  this._pausedSounds = [];
};

// This function plays all SoundsAtNodes that are able to be played, using
// the given ProgressNodes as informational parameters to the nodes and
// the current state of the current nodes.
// 
// The sounds played via this function are paused by default. Play them with a
// call to resumeSounds().
// 
// The sounds played via this function do not take into account ticker real-time
// offset. This is because this function assumes that the sounds will be resumed
// on nearly the same instant as a ticker reset. This provides the best
// quality of sound starting.
// 
// currentNodeFrame: The frame of the current node that is playing. Usually,
// when jumping to a node, this is 0.
// currentNodes: The current node stack. Oldest is closer to index 0.
// soundsAtNodes: A collection of SoundsAtNodes that represent the sounds that
// could possibly be playing right now.
anzovinmod.MainTimeline_Sound.prototype.startPlaythroughNodeSoundsPaused = function(currentNodeFrame, currentNodes, soundsAtNodes) {
  // no node stack means there can't be any node sounds playing
  if (currentNodes.length == 0) return;
  var currentNode = currentNodes[currentNodes.length - 1];
  var framerate = 1000.0 / this._mtl.getFPS();
  for (var i = 0; i < soundsAtNodes.length; ++i) {
    var soundAtNode = soundsAtNodes[i];
    // an indicator of whether we should play the sound or not
    var play = false;
    // the final offset to play the sound at, in milliseconds
    var offset = 0;
    // iterate over the nodes, attempting to find the ones that tell it to
    // play and get the final offset. check the current node first (most likely)
    // and go through the other nodes in order if the current node is not match
    do {
      // if the current node is the same as the sound, we can play it if we are
      // on the right frame (or after it)
      if (soundAtNode["node"] == currentNode["name"]) {
        if (soundAtNode["nodeFrame"] <= currentNodeFrame) {
          play = true;
          offset += framerate * (currentNodeFrame - soundAtNode["nodeFrame"]);
        }
        break;
      }
      // iterate over the nodes, starting at the earliest one. skip the last one,
      // because we've dealt with that one specifically already
      for (var j = 0; j < currentNodes.length - 1; ++j) {
        var stackNode = currentNodes[j];
        // if the sound is played in the stack node, mark it and get the offset
        if (soundAtNode["node"] == stackNode["name"]) {
          play = true;
          offset += framerate * (stackNode["length"] - soundAtNode["nodeFrame"]);
          continue;
        }
        // if we are playing the sound, then the entire node length is a part of
        // the offset
        if (play) {
          offset += framerate * stackNode["length"];
        }
      }
    } while (false);
    // play the sound if it is marked as being played in this frame. start it
    // paused, so that it can be resumed in a single instant after all these
    // calculations are performed (by the callee). we assume that the sound
    // nodes being played are going to be started on the ticker restart
    if (play) {
      this.playSoundConfig(soundAtNode["id"], {
        "offset":offset,
        "startPaused":true,
        "addTickerTimeOffset":false
      });
    }
  }
};

// This function starts playing any node sounds that are supposed to start on
// the current progress node at the current frame. Sounds are played
// immediately, and with any ticker time offset.
// 
// currentNodeFrame: The frame being played in the current node.
// currentNodeName: The string name of the current node, or null if there is
// no current node, such as can happen near the beginning of an animation.
// soundsAtNodes: An array of SoundAtNode that exist for the current scene,
// that could possibly be started playing at the current node.
anzovinmod.MainTimeline_Sound.prototype.startPlayingCurrentNodeSounds = function(currentNodeFrame, currentNodeName, soundsAtNodes) {
  for (var i = 0; i < soundsAtNodes.length; ++i) {
    var soundAtNode = soundsAtNodes[i];
    if (!(soundAtNode["node"] == currentNodeName))
      continue;
    if (!(soundAtNode["nodeFrame"] == currentNodeFrame))
      continue;
    this.playSoundConfig(soundAtNode["id"], {});
  }
};

// Sets the main volume to the given value.
// 
// v: A value between 0.0 and 1.0 inclusive.
anzovinmod.MainTimeline_Sound.prototype.setVolume = function(v) {
  v = anzovinmod.Utilities.defaultParam(v, null);
  if (v == null) return;
  // note that this is setting the volume globally, not tied to a specific
  // main timeline instance. no real way to do this otherwise unless we want to
  // iterate over every sound instance being played and manually adjust the
  // sound levels ourselves. doable, but would take a bit of work
  if (typeof createjs !== "undefined") {
    createjs.Sound.setVolume(v);
  }
};

// Sets or unsets the sound muting for the animation.
// 
// m: True/False if the sound should be muted.
anzovinmod.MainTimeline_Sound.prototype.setMute = function(m) {
  m = anzovinmod.Utilities.defaultParam(m, null);
  if (m == null) return;
  if (typeof createjs !== "undefined") {
    createjs.Sound.setMute(m);
  }
};

// Just stop all sounds permanently.
anzovinmod.MainTimeline_Sound.prototype.stopAllSounds = function() {
  createjs.Sound.stop();
};

}());
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
/*! MainTimeline_Nodes.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// This class handles node related logic.
// 
// mtl: The mtl instance that owns this class.
anzovinmod.MainTimeline_Nodes = function(mtl) {
  // The reference to the owning mtl instance.
  this._mtl = mtl;
  // The current frame number of the current node. If there are no nodes, then
  // non have been reached yet. Otherwise, if there is a node, then this is the
  // current frame within that node. This is irrespective of the actual scene
  // timeline: It is just how many frames have ticked while a part of the node.
  this._currentNodeFrame = 0;
  // This is a stack of nodes that have been traversed. Initially empty, which
  // simply signifies that the node position (or name) is null, and unset.
  // As nodes are reached during the animation, they are pushed onto the top of
  // this stack, as nodes are backtracked, they are popped from the stack, and
  // as nodes are foretracked, they are pushed onto the top of this stack in
  // order.
  this._currentNodes = [];
  // The last playbacl percent number called back.
  this._lastPlaybackPercent = 0.0;
};

// This function resumes from a hard pause when jumping to a node. This is
// needed in order to allow the progress bar to have functionality when the
// animation is hard paused.
anzovinmod.MainTimeline_Nodes.prototype._hardPauseResumeJump = function() {
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Nodes._hardPauseResumeJump", "hard-resuming for a jump");
  this._mtl._mtl_sound.clearPausedSounds();
  this._mtl.setState("hardpaused", false);
};

// Goes back one node. If the ticker is less than one second in the current
// node, it will go to the previous node, otherwise it will just snap to the
// beginning of the current node.
// 
// MSC = no. Needs review.
anzovinmod.MainTimeline_Nodes.prototype.goNodeBack = function() {
  if (this._mtl.isState("hardpaused")) this._hardPauseResumeJump();
  if (this._currentNodes.length == 0) return;
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Nodes.goNodeBack", "going back a node");
  if (this._currentNodeFrame < this._mtl.getFPS()) {
    if (this._currentNodes.length > 0)
      this._currentNodes.pop();
  }
  var doPop = false;
  if (this._currentNodes.length > 0) doPop = true;
  this._gotoCurrentNodeStart(doPop);
};

// Goes forward one node.
// 
// MSC = no. This does not handle the case of a scene node forwarding to another
// scene. The progress nodes of the animation would have to be coalesced into
// a single progress node object and used like that. And, the goto would need
// to properly handle nodes on different scenes.
anzovinmod.MainTimeline_Nodes.prototype.goNodeForward = function() {
  if (this._mtl.isState("hardpaused")) this._hardPauseResumeJump();
  if (this._currentNodes.length == 0) return;
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Nodes.goNodeForward", "going forward a node");
  var currentNode = this._currentNodes[this._currentNodes.length - 1];
  if (currentNode["nextNode"] == null) return;
  var scene = this._mtl.getCurrentScene();
  var progressNodes = this._mtl.getProgressNodes();
  for (var k in progressNodes) {
    if (!progressNodes.hasOwnProperty(k)) continue;
    var progressNode = progressNodes[k];
    if (progressNode["name"] == currentNode["nextNode"]) {
      this._currentNodes.push(progressNode);
      this._gotoCurrentNodeStart(true);
      break;
    }
  }
};

// Goes to the indicated node.
// 
// MSC = no. Needs review.
anzovinmod.MainTimeline_Nodes.prototype.goNodeTo = function(node) {
  node = anzovinmod.Utilities.defaultParam(node, null);
  if (node == null) return;
  if (this._mtl.isState("hardpaused")) this._hardPauseResumeJump();
  if (this._currentNodes.length == 0) return;
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Nodes.goNodeTo", "going to node: ", node);
  // check back
  for (var i = 0; i < this._currentNodes.length - 1; ++i) {
    if (this._currentNodes[i]["name"] == node) {
      for (var j = this._currentNodes.length - 1; j > i; --j) {
        this._currentNodes.pop();
      }
      anzovinmod.Logging.logm(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.MainTimeline_Nodes.goNodeTo", "going back");
      this._gotoCurrentNodeStart(true);
      return;
    }
  }
  // check forward
  var scene = this._mtl.getCurrentScene();
  var progressNodes = scene["_anzovinmod"]["properties"]["progressNodes"];
  var currentNode = this.getCurrentNode();
  while (currentNode["nextNode"] != null) {
    if (!progressNodes.hasOwnProperty(currentNode["nextNode"])) break;
    currentNode = progressNodes[currentNode["nextNode"]];
    if (currentNode["name"] == node) {
      currentNode = this._currentNodes[this._currentNodes.length - 1];
      while (currentNode["nextNode"] != null) {
        currentNode = progressNodes[currentNode["nextNode"]];
        this._currentNodes.push(currentNode);
        if (currentNode["name"] == node) {
          anzovinmod.Logging.logm(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.MainTimeline_Nodes.goNodeTo", "going forward");
          this._gotoCurrentNodeStart(true);
          return;
        }
      }
    }
  }
  // node was this node?
  var currentNode = this._currentNodes[this._currentNodes.length - 1];
  if (currentNode["name"] == node) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_DEBUG, "anzovinmod.MainTimeline_Nodes.goNodeTo", "same node");
    this._gotoCurrentNodeStart(true);
    return;
  }
};

// Begins animation playback at the start of the current top-level node.
// 
// MSC = no. Needs review.
// 
// doPop: Whether to pop off the last node after the goto is called. This is
// due to the first frame of the node being re-added to the node list, so
// needing to be removed for an accurate portrayal of the node list.
anzovinmod.MainTimeline_Nodes.prototype._gotoCurrentNodeStart = function(doPop) {
  // start by stopping all sounds. we'll restart them appropriately after the
  // jump
  this._mtl._mtl_sound.stopAllSounds();
  // set the animation as playing, as we are playing during the jump
  if (this._mtl.isState("ended"))
    this._mtl.setStates({"playing":true, "ended":false});
  // jump to the point in time of the node position
  this._currentNodeFrame = 0;
  var config = {"playSounds":false};
  if (this._currentNodes.length == 0) {
    this._mtl.gotoAndPlay(
      "start",
      null,
      config
    );
  } else {
    this._mtl.gotoAndPlay(
      this._currentNodes[this._currentNodes.length - 1]["frame"],
      null,
      config
    );
  }
  if (doPop) this._currentNodes.pop();
  if (this._currentNodes.length == 0) return;
  this._mtl._mtl_sound.startPlaythroughNodeSoundsPaused(
    this._currentNodeFrame,
    this._currentNodes,
    this._mtl._mtl_scenes.getSoundsAtNodes()
  );
  this._mtl._mtl_sound.resumeSounds();
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Nodes._gotoCurrentNodeStart", "node frame: ", this._currentNodeFrame, " nodes: ", this._currentNodes);
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
anzovinmod.MainTimeline_Nodes.prototype.updateStagePlayNodeSounds = function(scene) {
  var sceneLibProps = scene["_anzovinmod"]["properties"];
  var soundsAtNodes = sceneLibProps["soundsAtNodes"];
  var thisNodeName = null;
  if (this._currentNodes.length > 0)
    thisNodeName = this._currentNodes[this._currentNodes.length - 1]["name"];
  this._mtl._mtl_sound.startPlayingCurrentNodeSounds(
    this._currentNodeFrame,
    thisNodeName,
    this._mtl._mtl_scenes.getSoundsAtNodes()
  );
};

// This function is called on each update stage event, when we wish to keep
// track of each frame of the node animation section.
// 
// scene: The scene that is currently playing.
anzovinmod.MainTimeline_Nodes.prototype.updateStageDoNodeFrameTracking = function(scene) {
  if (scene.paused) return;
  ++this._currentNodeFrame;
  this.updatePlaybackPercent();
};

// Calculates the playback percent and sends that data via the mtl callback
// sender, if it has changed.
anzovinmod.MainTimeline_Nodes.prototype.updatePlaybackPercent = function() {
  var percent = this.getPlaybackPercent();
  if (percent != this._lastPlaybackPercent) {
    this._lastPlaybackPercent = percent;
    this._mtl.sendPlaybackPercent(percent);
  }
};

// Calculates the playback percent value and returns it.
// 
// Returns:
// Playback percent value.
anzovinmod.MainTimeline_Nodes.prototype.getPlaybackPercent = function() {
  var currentNode = this.getCurrentNode();
  if (currentNode == null) return 0;
  var percentNodeStart = currentNode["position"];
  var percentNodeEnd = 100;
  if (currentNode["nextNode"] != null) {
    var progressNodes = this._mtl.getProgressNodes();
    if (progressNodes.hasOwnProperty(currentNode["nextNode"])) {
      var nextNode = progressNodes[currentNode["nextNode"]];
      percentNodeEnd = nextNode["position"];
    }
  }
  var percent = percentNodeEnd;
  if (currentNode["length"] != 0)
    percent = percentNodeStart + ((percentNodeEnd - percentNodeStart) * ((this._currentNodeFrame + 1) / currentNode["length"]));
  return percent;
};

// Simply returns the current node on the stack, or null if there is none.
// 
// Returns:
// ProgressNode object that is the current node on the stack.
anzovinmod.MainTimeline_Nodes.prototype.getCurrentNode = function() {
  if (this._currentNodes.length > 0)
    return this._currentNodes[this._currentNodes.length - 1];
  return null;
};

// This function is called on each update stage event, when we wish to check
// for a changed tracking node position and update the mtl's internal stack of
// traversed nodes.
// 
// MSC: yes. The progressnodes of the current scene are the only ones that can
// be found, so there is no need to search other scenes. Current progress node
// information is on the stack, should it be necessary.
// 
// scene: The scene that is currently playing, and to check for nodes against.
anzovinmod.MainTimeline_Nodes.prototype.updateStageDoNodeTracking = function(scene) {
  var sceneLibProps = scene["_anzovinmod"]["properties"];
  var progressNodes = sceneLibProps["progressNodes"];
  for (var k in progressNodes) {
    if (!progressNodes.hasOwnProperty(k)) continue;
    var progressNode = progressNodes[k];
    if (progressNode["frame"] == scene.timeline.position) {
      this._currentNodeFrame = 0;
      this._currentNodes.push(progressNode);
      anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.MainTimeline_Nodes.updateStageDoNodeTracking", "pushed node on frame ", scene.timeline.position, ": ", progressNode);
    }
  }
};

// This function simply resets the nodes, such as when starting (or restarting)
// an animation.
anzovinmod.MainTimeline_Nodes.prototype.resetNodes = function() {
  this._currentNodeFrame = 0;
  this._currentNodes = [];
};

}());
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
/*! TextController.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// Constructor. This class is responsible for managing text fields in the
// animation. This primarily (currently) comprises of two things. The first is
// applying text fixing translations, as the default Canvas text output is
// apparently always some number of pixels off of the correct position. The
// second is to overlay HTML form input fields on top of the animation for
// text fields that are editable by the user.
anzovinmod.TextController = function() {
  // This reference object (like the next one) has a two-level approach:
  // The first index is the scene name. The difference ends there, as this
  // will contain all of the text elements in the scene in its entirety, so as
  // to not require re-parsing the entire scene in order to find them again.
  this._allTextElements = {};
  // The main reference object. This is a list of the createjs.Text elements
  // that are to contain html form inputs. It is an object because the first
  // level index is the scene name. This lets us more easily manage many
  // scenes worth of Text elements, and improve performance when dealing
  // with Text elements from multiple scenes by only requiring comparisons
  // and iterations on a subset of the Text elements instead of all of them
  // on the stage.
  // The Text elements themselves will have two additional on-object properties
  // that we define to help keep track of the references to the HTML input
  // entities and the DOMElement instance attached to each one:
  // {_textFormInputElement}: A reference to the actual HTML input element.
  // {_textFormInputElementDOM}: A reference to the DOMElement entity that may
  // or may not be attached to the Text's parent entity.
  this._textInputs = {};
  // This reference object contains a list of all of the createjs.Text nodes
  // on given scenes that have edited e.y coordinates. These will be looked at
  // on every frame of the animation for changed e.y coordinates, because they
  // have been modified by the CreateJS animations and need to be re-adjusted
  // in order to line up properly with the graphics.
  this._animatedTextNodes_y = {};
  // The HTML form element to use for managing text input in the canvas.
  this._textForm = null;
  // The HTML form will contain a wrapper DIV element that itself will contain
  // the inputs. This is a reference to that DIV element.
  this._textFormDiv = null;
  // The defined width of the stage.
  this._width = 1920;
  // The defined height of the stage.
  this._height = 1080;
  // The current scale of the stage.
  this._scale = 1;
};

// Sets the size of the form element. The size is different from the currently
// displayed scale: The size should always match the canvas element's HTML
// attribute width/height, and not its CSS. If the displayed width of the
// canvas element changes, call setScale() instead of this function. Only call
// this function once during initialization, and if the stage size changes.
// 
// width: The new desired width.
// height: The new desierd height.
anzovinmod.TextController.prototype.setSize = function(width, height) {
  width = anzovinmod.Utilities.defaultParam(width, null);
  height = anzovinmod.Utilities.defaultParam(height, null);
  if (width == null || height == null) {
    return;
  }
  this._width = width;
  this._height = height;
  this._writeSizesToCss();
};

// Sets the scale of the form element to be that of the value provided. This is
// used when resizing the display of the canvas element. The canvas element
// scales itself accordingly depending on the difference between its HTML
// attribute width and its CSS width, but the HTML form needs to have its scale
// set manually.
// 
// Since the form has an overflow style but also a transform, you don't need to
// change the size to change the transform scale, while still having the correct
// overflow behavior. Changing the scale alone is sufficient.
// 
// Note that this function will attempt to set the scale in multiple different
// ways, depending on if the browser engine supports one particular scaling
// method or another. Eg, -moz-transform.
// 
// scale: A floating point value that is the scale to set. A value less than
// one reduces the size of the form.
anzovinmod.TextController.prototype.setScale = function(scale) {
  scale = anzovinmod.Utilities.defaultParam(scale, null);
  if (scale == null) {
    return;
  }
  this._scale = scale;
  this._writeSizesToCss();
};

// Just like the individual setSize() and setScale() functions, this one just
// does them both at the same time. If width or height are null, then they are
// not changed. If scale is null then it is not changed.
// 
// width: The new desired width.
// height: The new desired height.
// scale: The new desired scale.
anzovinmod.TextController.prototype.setSizeScale = function(width, height, scale) {
  width = anzovinmod.Utilities.defaultParam(width, null);
  height = anzovinmod.Utilities.defaultParam(height, null);
  scale = anzovinmod.Utilities.defaultParam(scale, null);
  if (width != null && height != null) {
    this._width = width;
    this._height = height;
  }
  if (scale != null) {
    this._scale = scale;
  }
  this._writeSizesToCss();
};

// This function just takes the width/height/scale definitions and writes the
// appropriate CSS to the appropriate form element or wrappers. It only changes
// the CSS of the form element or children of it, not any parent elements.
anzovinmod.TextController.prototype._writeSizesToCss = function() {
  if (this._textFormDiv != null) {
    this._textFormDiv.style.transform = "scale(" + this._scale + ")";
    this._textFormDiv.style.MozTransform = "scale(" + this._scale + ")";
    this._textFormDiv.style.WebkitTransform = "scale(" + this._scale + ")";
    this._textFormDiv.style.OTransform = "scale(" + this._scale + ")";
    this._textFormDiv.style.MSTransform = "scale(" + this._scale + ")";
  }
};

// Initializes the "_textInputs" and "_allTextElements" reference objects with
// createjs.Text elements from the given timeline instance. All the scenes on
// the main timeline are parsed.
// 
// Note that this can be called after adding new scenes to the mtl and calling
// this function again. It will not parse the same scene name multiple times,
// as the text inputs on scenes are indepentent.
// 
// mtl: A reference to the main timeline instance object.
// scene: The specific scene that is being searched. This is a cjs movieclip
// instance.
anzovinmod.TextController.prototype.initTextNodesFromMtl = function(mtl, scene) {
  mtl = anzovinmod.Utilities.defaultParam(mtl, null);
  scene = anzovinmod.Utilities.defaultParam(scene, null);
  if (mtl == null) return;
  if (scene == null) return;
  anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.TextController.initTextNodesFromMtl", "initializing text nodes for scene ", scene.name);
  // skip this scene if it is already parsed
  if (this._textInputs.hasOwnProperty(scene.name)) return;
  // find all text nodes on the scene
  var nodes = this._findTextNodesOnScene(mtl, scene.name);
  this._textInputs[scene.name] = [];
  this._allTextElements[scene.name] = [];
  for (var j = 0; j < nodes.length; ++j) {
    var node = nodes[j];
    this._allTextElements[scene.name].push(node);
    var nodeName = node.name;
    if (nodeName != null && nodeName.length > 6 && nodeName.lastIndexOf("_input") == nodeName.length - 6) {
      this._textInputs[scene.name].push(node);
    }
  }
  this._createTextFormInputs(scene.name);
  this.hideSceneElements(scene.name);
};

// Attempts to find all of a timeline's createjs.Text elements that are to have
// HTML text inputs associated with them. Note that this may at first return
// all the createjs.Text elements from a stage, but would be extended to
// eventually return only those text elements that have a custom name or
// property or other identifier that indicates they are to have html text
// inputs associated with them.
// 
// Note that this will return all Text elements, not just those that are
// currently visible on the stage, but all of them. This is for several
// reasons. First of all, this allows all of the text elements to be found
// initially upon the first stage load, simplifying the search process so that
// we do not have to search through all elements all the time. Otherwise, we
// would have to search through every element both on and off the stage all
// the time.
// 
// mtl: A reference to the main timeline instance object.
// scene: The name of the scene to search for elements of.
// 
// Returns: An array of createjs.Text elements on the given scene, that are to
// be managed for HTML input entities.
anzovinmod.TextController.prototype._findTextNodesOnScene = function(mtl, scene) {
  mtl = anzovinmod.Utilities.defaultParam(mtl, null);
  scene = anzovinmod.Utilities.defaultParam(scene, null);
  var ret = [];
  if (mtl == null || scene == null) {
    return ret;
  }
  var elements = mtl.findChildren(scene + ".**.*", true);
  for (var i = 0; i < elements.length; ++i) {
    if (elements[i] instanceof createjs.Text) {
      ret.push(elements[i]);
    }
  }
  return ret;
};

// Creates the basic text input form for the timeline, if not already created.
// The form is referenced directly by this instance, so there is no need to
// keep track of IDs or names of the form.
// 
// Any necessary CSS or positioning of the form itself is initialized here.
// The input elements themselves are added elsewhere.
// 
// The new form should be placed as a child of the div wrapper. For best results
// the canvas element should be placed inside of this form wrapper. This is
// because of issues with z-indexing and positioning the canvas and form
// elements as siblings: Things work much better when the canvas element is
// a sibling of the text input elements.
// 
// In short, create a Div element, create a Form, create a Canvas.
// 
// Returns: The HTML Form element that should be added to the div wrapper.
anzovinmod.TextController.prototype.createGetTextForm = function() {
  if (this._textForm != null) {
    return this._textForm;
  }
  this._textForm = document.createElement("form");
  this._textForm.setAttribute("method", "post");
  this._textForm.setAttribute("action", "");
  this._textFormDiv = document.createElement("div");
  this._textFormDiv.setAttribute("class", "formInputs");
  this._textForm.appendChild(this._textFormDiv);
  this._writeSizesToCss();
  return this._textForm;
};

// Creates a text form input for each of the text fields present on the stage.
// They are not positioned or sized here. They are only created, given the
// appropriate justification, hidden by default, then placed on the page.
// 
// This can be called multiple times if the stage changes. As long as the
// canvas text elements still have their custom properties attached to them,
// this function will not create duplicate input elements.
// 
// A separate function, to handle positioning, sizing, and text values of the
// elements, will need to be used elsewhere and called frequently to ensure that
// the text elements are correctly handling frame-by-frame changes to the stage.
// 
// scene: The scene name to create html input elements form.
anzovinmod.TextController.prototype._createTextFormInputs = function(scene) {
  scene = anzovinmod.Utilities.defaultParam(scene, null);
  if (scene == null) {
    return;
  }
  if (this._textForm == null) {
    return;
  }
  if (this._textFormDiv == null) {
    return;
  }
  if (!this._textInputs.hasOwnProperty(scene)) {
    return;
  }
  for (var i = 0; i < this._textInputs[scene].length; ++i) {
    var e = this._textInputs[scene][i];
    // skip if there is an html input already created for this text node
    if (e.hasOwnProperty("_textFormInputElement")) {
      continue;
    }
    // create element
    var newInputElement = document.createElement("input");
    // set font
    newInputElement.style.font = e.font;
    // set color
    newInputElement.style.color = e.color;
    // text align
    switch (e.textAlign) {
      case "left":
      case "start":
        newInputElement.style.textAlign = "left";
        break;
      case "right":
      case "end":
        newInputElement.style.textAlign = "right";
        break;
      case "center":
        newInputElement.style.textAlign = "center";
        break;
    }
    // content
    newInputElement.value = e.text;
    // make invisible until added
    newInputElement.style.display = "none";
    // make canvas element hidden
    e.visible = false;
    // add to form-div
    this._textFormDiv.appendChild(newInputElement);
    // add as custom properties for tracking
    e["_textFormInputElement"] = newInputElement;
    e["_textFormInputElementDOM"] = new createjs.DOMElement(newInputElement);
  }
};

// This function hides all the HTML input elements from the given scene. This
// is important as a scene transition would have the html input elements still
// be visible after the transition unless they are manually made hidden.
// 
// Note that a possible optimization in this function, could be to make the
// html input elements be children of DIVs, and to just show/hide the Div
// element when a scene goes on/off the stage.
// 
// scene: The name of the scene to hide the elements of.
anzovinmod.TextController.prototype.hideSceneElements = function(scene) {
  scene = anzovinmod.Utilities.defaultParam(scene, null);
  if (scene == null) {
    return;
  }
  if (!this._textInputs.hasOwnProperty(scene)) {
    return;
  }
  for (var i = 0; i < this._textInputs[scene].length; ++i) {
    var e = this._textInputs[scene][i];
    if (!e.hasOwnProperty("_textFormInputElement")) {
      continue;
    }
    var htmlInput = e["_textFormInputElement"];
    if (htmlInput.style.display != "none") {
      htmlInput.style.display = "none";
    }
  }
};

// This function is called once per frame of the stage animation. It is
// responsible for handling any sort of logic that needs to be handled on
// createjs.Text elements per frame, such as ensuring that the text
// position fix is in-place.
// 
// Note that this method uses a different array than the all-text-elements
// array to parse through all the nodes to parse. This is because after the
// first pass, text nodes that are not animated do not need to be checked again,
// and only those that are animated need to be checked. Additionally, text
// nodes that are animated are reset on every frame, so this can be used to
// ensure that we do not waste processing cycles by only checking those
// text nodes that we know may change.
// 
// We might add something to the CreateJS library to make it not set the y
// translation on text nodes that have not changed since last, which would
// make this function only necessary on text nodes that actually animate
// when they need to change their transformation, instead of all the time.
// That is not for now though.
// 
// scene: The name of the scene to manage the elements of.
// removeUnmodifiedElements: Default True. If True, then unmodified text nodes
// are removed from the list of text nodes to keep track of. If False, then
// text nodes will remain in the list.
anzovinmod.TextController.prototype.allTextFrameUpdate = function(scene, removeUnmodifiedElements) {
  scene = anzovinmod.Utilities.defaultParam(scene, null);
  removeUnmodifiedElements = anzovinmod.Utilities.defaultParam(removeUnmodifiedElements, true, true);
  if (scene == null) {
    return;
  }
  // if we haven't done this yet, then just use a copy of the 'all elements'
  // array to initialize our list
  if (!this._animatedTextNodes_y.hasOwnProperty(scene)) {
    if (!this._allTextElements.hasOwnProperty(scene)) {
      return;
    }
    this._animatedTextNodes_y[scene] = anzovinmod.Utilities.cloneArray(this._allTextElements[scene], false);
  }
  var nodes = this._animatedTextNodes_y[scene];
  var numberOfTextNodes = nodes.length;
  for (var i = 0; i < numberOfTextNodes; ++i) {
    var e = nodes[i];
    if (!this._frameNodeFixCjsTextPosition(e) && removeUnmodifiedElements) {
      nodes[i--] = nodes[--numberOfTextNodes];
    }
  }
  if (numberOfTextNodes != nodes.length) {
    anzovinmod.Logging.logm(anzovinmod.Logging.LOG_INFO, "anzovinmod.TextController.allTextFrameUpdate", "reducing animated text nodes from ", nodes.length, " to ", numberOfTextNodes);
    this._animatedTextNodes_y[scene] = anzovinmod.Utilities.cloneArray(nodes, false, numberOfTextNodes);
  }
};

// This function is called once per frame of the stage animation. It does
// anything that needs or may need to be done on each frame for HTML input
// text elements, such as watching out for adjustments and changed
// transformation matrices to apply to the HTML form elements, etc.
// 
// The first thing about this function, is that it only operates on a single
// scene's worth of createjs.Text nodes at once. This is to avoid having to
// process nodes that are obviously not visible.
// 
// scene: The name of the scene to manage the elements of.
// onSceneParent: A MovieClip or Container object that, if a Text node's parent
// history contains this element, treate the node as being on the scene, and
// hence visible. Otherwise, the node will be treated as invisible.
anzovinmod.TextController.prototype.inputTextFrameUpdate = function(scene, onSceneParent) {
  scene = anzovinmod.Utilities.defaultParam(scene, null);
  onSceneParent = anzovinmod.Utilities.defaultParam(onSceneParent, null);
  if (scene == null) {
    return;
  }
  if (!this._textInputs.hasOwnProperty(scene)) {
    return;
  }
  var es = this._textInputs[scene];
  for (var i = 0; i < es.length; ++i) {
    var e = es[i];
    // Skip nodes that do not have an HTML form input element associated.
    if (!e.hasOwnProperty("_textFormInputElement") || e["_textFormInputElement"] == null) {
      continue;
    }
    // Ensure that the node is supposed to be visible. If it is supposed to be
    // hidden, then make sure it is hidden and return "true". Otherwise, return
    // "false" but do not yet make it visible. We'll do that at the end to help
    // prevent any amount of jitter.
    if (this._frameNodeHideIfNotOnStage(e, onSceneParent)) {
      continue;
    }
    // Ensure that the node's DOMElement is either attached to the stage or to
    // the node's parent.
    this._frameNodeEnsureDomeIsAttachedToParent(e);
    // Adjust HTML input size/position if necessary.
    this._frameNodeAdjustHtmlInputSizeAndPosition(e);
    // Make sure the HTML text node is visible.
    this._frameNodeEnsureIsVisible(e);
  }
};

// Checks the parent history of e and, if the given scene parent is not found,
// hide the text node and return true to signify that the node is indeed hidden,
// or was already hidden. The node is not made visible if it should be visible,
// it is only hidden if it should be hidden.
// 
// e: The text node to check.
// onSceneParent: The node to check in the history to indicate that e is
// supposed to be visible.
// 
// Returns: True if the node is is hidden or is supposed to be hidden. False if
// e is supposed to be visible.
anzovinmod.TextController.prototype._frameNodeHideIfNotOnStage = function(e, onSceneParent) {
  var textElement = e["_textFormInputElement"];
  var parent = e.parent;
  while (parent != null && parent != onSceneParent) {
    parent = parent.parent;
  }
  if (parent == null || parent != onSceneParent) {
    if (textElement.style.display != "none") {
      textElement.style.display = "none";
    }
    return true;
  }
  return false;
};

// Checks the parent of the text node e and, if it does not have the node e's
// associated DOMElement attached, then attach it to the parent. As this
// function would only need to be called if the text node e is attached to
// an object or on the stage, it assumes that the parent exists and is valid.
// 
// e: The text node to check.
anzovinmod.TextController.prototype._frameNodeEnsureDomeIsAttachedToParent = function(e) {
  var domElement = e["_textFormInputElementDOM"];
  if (e.parent.children.indexOf(domElement) == -1) {
    e.parent.addChild(domElement);
    // fix newly added HTML form input transformation matrices
    this._frameNodeManuallySetHtmlInputTransformationMatrix(e);
  }
};

// Fixes the text positions of the given createjs.Text element. This
// is necessary to obtain a more pixel-perfect representation of the Canvas
// project across devices and from the output of Flash Pro. The default text
// rendering is offset by some degree.
// 
// Note that this function can detect when the position is changed versus when
// it has already been fixed, so it will only re-fix a node if it has been
// itself re-adjusted by the stage or animation.
// 
// e: The text node to check for and fix position of.
// 
// Returns: True if the position of the given node is changed, false if it
// is not changed.
anzovinmod.TextController.prototype._frameNodeFixCjsTextPosition = function(e) {
  // "Number" type means that it hasn't changed since last being "fixed"
  if (e.y instanceof Number) {
    return false;
  }
  var eSize = e.font.split(" ")[0];
  var eSizeNum = 0;
  if (eSize.indexOf("px") > 0) {
    eSizeNum = eSize.slice(0, eSize.indexOf("px"));
    eSizeNum = eSizeNum / 10;
  } else if (eSize.indexOf("pt") > 0) {
    eSizeNum = eSize.slice(0, eSize.indexOf("pt"));
    eSizeNum = (eSizeNum * 4 / 3) / 10;
  }
  e.y = new Number(e.y + 2 + eSizeNum);
  return true;
};

// Adjust the HTML Input element's size (CSS width/height) and position
// (CSS top/left) to match that of the Text node e. This is necessary to
// take into account offsets or animations of the text node.
// 
// e: The text node to adjust associated HTML input size and position of.
anzovinmod.TextController.prototype._frameNodeAdjustHtmlInputSizeAndPosition = function(e) {
  // Notes on Sizes & Positions:
  // 
  // lineWidth and lineHeight are not necessarily appropriate to use in
  // all cases. They just control when the text is supposed to wrap. We also
  // can't just blindly use the lineWidth or lineHeight values without regard
  // to other potential values. There are also many other properties that
  // relate to sizing text elements. This section outlines some of the values
  // that could be used to determine sizes of elements.
  // 
  // --lineWidth: Can be used as a good default value for width. It is defined
  // as the width before text is split to fit across multiple lines, so it is
  // either equal to or larger than the size of text that is displayed in Text
  // elements. This is one of the most likely variables that, if set, is
  // set as an explicit value, eg most likely to be the actual width of the
  // Text element from Flash Pro. This value can be null, in which case text
  // will not wrap at all. Another value will need to be used in this case.
  // Of note, is that text that is longer than this may not wrap if there are
  // no spaces in the text: It will continue outside of these bounds until
  // a breaking character is found.
  // 
  // --maxWidth: The maximum width of a text element. If the text would be
  // wider than this, then the text is compressed horizontally to fit into
  // the defined size limits. This is analogous to After Effects' OpenSesame's
  // "WIDTH #" format specifier. This could be used in place of lineWidth
  // if it is specified, as both place a limit on the maximum horizontal size
  // of an element. Unlike lineWidth though, this value is more aggressive
  // and text will be compressed regardless of the presence of line-breaking
  // spaces or other characters. Of additional note, is that this compression
  // is done per-line. A two-line text element may have the first line
  // compressed while the second line is displayed normally.
  // 
  // --getMeasuredWidth(): The width in pixels that the original, non
  // transformed text, displays as. Unlike getMeasuredHeight(), this value
  // does not take into account line breaks, regardless of whether they are
  // explicitly included via "\n" or implicit via lineWidth values. For
  // single lines of text, then this value could be used to determine the
  // displayed width of text elements.
  // 
  // --lineHeight: The height difference between two lines of text. If set to
  // a number, then when text is wrapped, subsequent lines appear this
  // distance below the first line. If set to zero or null, then the value of
  // getMeasuredHeight() is used instead, which itself performs a calculation
  // based on the font face in use to determine an appropriate value.
  // 
  // --getMeasuredHeight(): The height in pixels that the original, non
  // transformed text, displays as. On standard, non transformed text fields,
  // this is either very closely equal to or exactly equal to the value of
  // lineHeight times the number of lines in the text. If lineHeight is not
  // available, then the value is approximated through calculations on the
  // font face. This value includes any inherit line breaks caused by
  // lineWidth wrapping text. This value can usually be used in determining
  // the displayed height of text elements.
  // 
  // --getMeasuredLineHeight(): An approximation of the height of the font
  // face in question. This is based on the ratio between em and height, and
  // can give a much more accurate definition for the height of a line of text.
  // It should be noted however, that for fixed-width fonts this value can be
  // grossly inaccurate, as it is a static calculation based on the value of em
  // and does not do any actual glyph rendering to determine the actual line
  // height size.
  // 
  // --getBounds(): Returns the width, height, x, and y positions of the
  // element. This can be used for many different drawn elements, but is
  // also supported by Text elements. The values returned by this are the
  // actual display bounds, and are not representative of any possible limits
  // on these positions. Eg, the width component may be smaller than the
  // lineWidth or maxWidth values, and represent only the actual physical
  // width of the actual text being displayed.
  // 
  // Note that the object returned by getBounds() is not persistant, and
  // either the object or its properties should be copied if persistance is
  // needed on these values.
  // 
  // Conclusions:
  // 
  // The most concrete width definition can be obtained via maxWidth or
  // lineWidth. Prefer maxWidth first and lineWidth second, as maxWidth (if
  // defined) is the "absolute" maximum width of an element, while lineWidth
  // is more of a guideline and can be breached. If both of these are
  // unavailable, then getBounds().width will need to be used to determine
  // the current displayed width of an element. getMeasuredWidth() should be
  // used last, as it does not take into account explicit line breaks in the
  // displayed text, though for single-line elements this is not a problem and
  // getMeasuredWidth() should be equivalent to getBounds().width.
  // 
  // The most concrete height definition can be obtained via
  // getMeasuredLineHeight(), as it takes into account the actual size of the
  // font and doesn't mess around with baseline height differences. However,
  // this does not work entirely well for fixed-width fonts, in which case
  // something like getBounds().height or getMeasuredHeight() should be used.
  // These should be equivalent, as they both take into account line breaks in
  // the text. Just use the value that is already easily obtainable, eg if you
  // are already using getBounds() for its width property, then you might as
  // well use it for its height property.
  // 
  // As for the X and Y coordinates of the text elements, the values to use
  // depends on the text alignment and which of the width/height values were
  // used.
  // 
  // Of note first, is that the properties x and y of a text element
  // represent the point of interest of the text element dependent on the
  // text alignment. If the horizontal alignment is left, then the x
  // coordinate is that of the left bounds of the text element. If the
  // alignment is centered, then the x coordinate is that of the midpoint
  // of the text element. And if the alignment is right justified, then the
  // x coordinate is that of the right side of the text element.
  // 
  // getBounds() also has an x and y coordinate value. These values, unlike
  // the text elements', are not affected by justification, and instead
  // mearly represent the offset from the text element's x and y coordinates
  // that the rendered text begins at. For left justified text, x is therefore
  // zero. For center justified text, x is negative one-half the width
  // property of getBounds(). And for right justified text, x is just
  // negative its width property.
  var textElement = e["_textFormInputElement"];
  var left, top, width, height;
  var bounds = null;
  if (e.maxWidth != null) {
    width = e.maxWidth;
  } else if (e.lineWidth != null) {
    width = e.lineWidth;
  } else {
    bounds = e.getBounds();
    if (bounds != null) {
      width = bounds.width;
    } else {
      width = e.getMeasuredWidth();
    }
  }
  if (bounds != null) {
    height = bounds.height;
  } else {
    height = e.getMeasuredLineHeight();
  }
  // left-right
  if (bounds != null) {
    left = e.x + bounds.x;
  } else {
    switch (e.textAlign) {
      case "right":
      case "end":
        left = e.x - width;
        break;
      case "center":
        left = e.x - width/2;
        break;
      case "left":
      case "start":
      default:
        left = e.x;
        break;
    }
  }
  // top-bottom
  if (bounds != null) {
    top = e.y + bounds.y;
  } else {
    switch (e.textBaseline) {
      case "bottom":
      case "ideographic":
      case "alphabetic":
        top = e.y - height;
        break;
      case "middle":
        top = e.y - height/2;
        break;
      case "top":
      case "hanging":
      default:
        top = e.y;
        break;
    }
  }
  // adjustments
  top = top - 2; // -2 ff, +0 ie, probably other browser-specific adjustments
  // finalize written values
  left = left + "px";
  top = top + "px";
  width = width + "px";
  height = height + "px";
  // only adjust if changed
  if (textElement.style.left != left) {
    textElement.style.left = left;
  }
  if (textElement.style.top != top) {
    textElement.style.top = top;
  }
  if (textElement.style.width != width) {
    textElement.style.width = width;
  }
  if (textElement.style.height != height) {
    textElement.style.height = height;
  }
};

// Simply makes sure that the Text node's associated HTML element has the
// necessary CSS in place to make it visible.
// 
// e: The text node to ensure is visible.
anzovinmod.TextController.prototype._frameNodeEnsureIsVisible = function(e) {
  var textElement = e["_textFormInputElement"];
  if (textElement.style.display != "") {
    textElement.style.display = "";
  }
};

// createjs.DOMElements that are just added to a parent have the
// appropriate matrices applied to them, but these are not forwarded to
// the corresponding HTML form input until the next frame of animation.
// This forces the transformation matrix to be applied to the HTML input
// until the next animation frame can do it itself manually from then on.
// 
// e: The text node that had its DOMElement attached to the parent.
anzovinmod.TextController.prototype._frameNodeManuallySetHtmlInputTransformationMatrix = function(e) {
  // Transform Matrix
  // The transform matrix (CSS transform:matrix(a,b,c,d,tx,ty);) is added to
  // each HTML form element node automatically by the stage on each frame of
  // changed animation. It can be retrieved from the corresponding DOMElement by
  // calling its getConcatenatedMatrix() function. This returns a matrix that
  // represents the transformation matrices all the way to the root stage.
  // This is different from getMatrix(), which just returns the transformation
  // matrix of the single element.
  // 
  // Note, that you do not need to clone the returned matrix object. That
  // is only for transform bounds. Not passing an argument to the getMatrix()
  // and getConcatenatedMatrix() functions just returns a new object instance.
  // Passing in an object instance will write values to the object.
  var textElement = e["_textFormInputElement"];
  var domElement = e["_textFormInputElementDOM"];
  var domMatrix = domElement.getConcatenatedMatrix();
  var matrixString = anzovinmod.Utilities.cssTransformMatrix2D(domMatrix);
  textElement.style.transform = matrixString;
  textElement.style.MozTransform = matrixString;
  textElement.style.WebkitTransform = matrixString;
  textElement.style.OTransform = matrixString;
  textElement.style.MsTransform = matrixString;
};

}());
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
/*! LateEaselLoader.js */
var anzovinmod = anzovinmod || {};
(function(){
"use strict";

// Cannot instantiate this class.
// This class is responsible for managing and loading shape objects in a manner
// which does not completely prevent interaction with the browser and allows it
// to continue to download external assets while shape objects are being
// constructed. Shape objects are really the only things that need to be
// managed in this fashion, because they are the most numerous types of createjs
// objects being instantiated.
// 
// The core of this class is the concept of a step. A step is a combination of
// scope plus callback function, and in the case of our easeljs late loader
// class here, is responsible for creating a single object of some form.
anzovinmod.LateEaselLoader = function() {
  throw "cannot instantiate anzovinmod.LateEaselLoader";
};

// An array of all the scope objects and their step and final functions.
// 
// [0] = scope object
// [1] = next index of [2] to do, starts at 0 even if there are no steps
// [2] = array of steps, usually object constructions, but they are called in
//       order, so the last one can be a "finalizer"
anzovinmod.LateEaselLoader._steps = [];

// A delay timeout, to be used to delay late loading until after another script
// has executed. If not null, then a timeout is set.
anzovinmod.LateEaselLoader._delayTimeout = null;

// The number of steps per delay timeout to perform.
anzovinmod.LateEaselLoader._stepsPerDelay = 50;

// An indicator of if new steps have been added since the last/current
// execution iteration. Note that the return value from getIndex() when a new
// index is created, does not need to set this value, as adding a new index to
// the end of the scope list doesn't change the order of existing entries.
anzovinmod.LateEaselLoader._newSteps = false;

// Returns the scope index for the indicated scope object. This is guaranteed
// to be valid for the remainder of any continuous javascript without intervals,
// waits, or do() executions of the late loader, but if there will be a timeout
// or interval or timer or load, then you should get this index value again or
// just use the unindexed versions of the code because cleanup may reindex
// things. Note that doDelay() doesn't count as long as your execution is
// continuous, as that will only begin execution of the steps after the next
// yielded timeout delay.
// 
// scope: The scope object to get the step index of.
// addIndex: Default true. If True, then if the scope object is not found,
// create a new index for it. If False, then "null" is returned, which will
// cause future indexed versions of functions to search for the proper index.
anzovinmod.LateEaselLoader.getIndex = function(scope, addIndex) {
  for (var i = 0, ii = anzovinmod.LateEaselLoader._steps.length; i < ii; ++i) {
    if (anzovinmod.LateEaselLoader._steps[i][0] === scope) {
      return i;
    }
  }
  anzovinmod.LateEaselLoader._steps.push([scope, 0, []]);
  return anzovinmod.LateEaselLoader._steps.length - 1;
};

// Adds a step in the loading process. This can be anything, but is usually just
// a single object attached to a createjs Container or MovieClip instance.
// The object is (usually) created and attached to a specific property of the
// scope object, so there is no other back-and-forth that needs to occur save
// for the construction.
// 
// If the scope is null, the function step is still added. Null scope functions
// will be called (without a scope of course) when all other scopes are
// finished.
// 
// scope: The object that is the scope of the step.
// f: The function that constructs the object. This uses the 'scope'
// parameter for any "this" assignment in the function.
anzovinmod.LateEaselLoader.add = function(scope, f) {
  anzovinmod.LateEaselLoader._newSteps = true;
  for (var i = 0, ii = anzovinmod.LateEaselLoader._steps.length; i < ii; ++i) {
    if (anzovinmod.LateEaselLoader._steps[i][0] === scope) {
      anzovinmod.LateEaselLoader._steps[i][2].push(f);
      return;
    }
  }
  anzovinmod.LateEaselLoader._steps.push([scope, 0, [f]]);
};

// Adds a step in the loading process, using the given scope index. This index
// is assumed to be valid.
// 
// index: The scope index to use, so we don't have to search for it.
// f: The function that constructs the object. This uses the 'scope'
// parameter for any "this" assignment in the function.
anzovinmod.LateEaselLoader.addIndex = function(index, f) {
  anzovinmod.LateEaselLoader._newSteps = true;
  anzovinmod.LateEaselLoader._steps[index][2].push(f);
};

// Actually performs the work of iterating through a certain number of steps,
// keeping track of any newly added steps during the step function, caring about
// ensuring that only a certain number of steps are done (unless unlimited is
// desired).
// 
// limitToCount: Whether to limit iterations (true) or to just do all steps
// at once (false).
anzovinmod.LateEaselLoader._do = function(limitToCount) {
  var countDone = 0;
  // label used to restart the loop when necessary, such as when a new step
  // was added, or break out, such as when the count is reached. while-true
  // so that 'continue' will restart but 'break' will exit the loop
  mainloop: do {
    // reset new steps label
    anzovinmod.LateEaselLoader._newSteps = false;
    // do non-null scopes first
    for (var i = 0; i < anzovinmod.LateEaselLoader._steps.length; ++i) {
      // skip nulls
      if (anzovinmod.LateEaselLoader._steps[i][0] === null) continue;
      // start at the last offset (defaults zero)
      for (var j = anzovinmod.LateEaselLoader._steps[i][1]; j < anzovinmod.LateEaselLoader._steps[i][2].length; ++j) {
        // if we've reached a max count, then end the loop (do this here to
        // save cleanup for when it's able to do so)
        if (limitToCount) {
          if (countDone >= anzovinmod.LateEaselLoader._stepsPerDelay) {
            break mainloop;
          }
          countDone += 1;
        }
        anzovinmod.LateEaselLoader._steps[i][1] += 1;
        anzovinmod.LateEaselLoader._steps[i][2][j].apply(anzovinmod.LateEaselLoader._steps[i][0]);
        anzovinmod.LateEaselLoader._steps[i][2][j] = null;
        // if there are new steps added, restart the loop to take them into
        // account IMMEDIATELY: our current value of "i" is no longer valid,
        // so we can't even rely on delaying to let the cleanup code execute
        // (but it will execute relatively quickly on the next loop anyways)
        if (anzovinmod.LateEaselLoader._newSteps) {
          continue mainloop;
        }
      }
      // reset array and remove it, remember to decrement loop variable
      anzovinmod.LateEaselLoader._steps[i] = [null, 0, []];
      anzovinmod.LateEaselLoader._steps.splice(i, 1);
      --i;
    }
    // do null-scopes on anything remaining (same comment structure as above)
    for (var i = 0; i < anzovinmod.LateEaselLoader._steps.length; ++i) {
      if (anzovinmod.LateEaselLoader._steps[i][0] !== null) continue;
      for (var j = anzovinmod.LateEaselLoader._steps[i][1]; j < anzovinmod.LateEaselLoader._steps[i][2].length; ++j) {
        if (limitToCount) {
          if (countDone >= anzovinmod.LateEaselLoader._stepsPerDelay) {
            break mainloop;
          }
          countDone += 1;
        }
        anzovinmod.LateEaselLoader._steps[i][1] += 1;
        anzovinmod.LateEaselLoader._steps[i][2][j]();
        anzovinmod.LateEaselLoader._steps[i][2][j] = null;
        if (anzovinmod.LateEaselLoader._newSteps) {
          continue mainloop;
        }
      }
      anzovinmod.LateEaselLoader._steps[i] = [null, 0, []];
      anzovinmod.LateEaselLoader._steps.splice(i, 1);
      --i;
    }
    // done forever
    break mainloop;
  } while (true);
};

// Starts loading all scope indices. Non-null scopes will be done first, and
// any null scope step functions will be called after all the rest are done,
// without a scope of course. All loading is done immediately (but in order),
// and no delays will be used.
anzovinmod.LateEaselLoader.doAll = function() {
  if (anzovinmod.LateEaselLoader._delayTimeout != null) {
    clearTimeout(anzovinmod.LateEaselLoader._delayTimeout);
    anzovinmod.LateEaselLoader._delayTimeout = null;
  }
  anzovinmod.LateEaselLoader._do(false);
};

// Starts loading all scope indices, but in a delayed manner. Only a few
// certain number of steps will be done on each delayed iteration, but they
// will be done in order.
anzovinmod.LateEaselLoader.delayDoAll = function() {
  if (anzovinmod.LateEaselLoader._delayTimeout == null) {
    anzovinmod.LateEaselLoader._delayTimeout = setTimeout(anzovinmod.LateEaselLoader._delayTimeoutCallback, 1);
  }
};

// Is called when the delay timeout triggers.
anzovinmod.LateEaselLoader._delayTimeoutCallback = function() {
  anzovinmod.LateEaselLoader._delayTimeout = null;
  anzovinmod.LateEaselLoader._do(true);
  if (anzovinmod.LateEaselLoader._steps.length > 0) {
    anzovinmod.LateEaselLoader._delayTimeout = setTimeout(anzovinmod.LateEaselLoader._delayTimeoutCallback, 1);
  }
};

}());
