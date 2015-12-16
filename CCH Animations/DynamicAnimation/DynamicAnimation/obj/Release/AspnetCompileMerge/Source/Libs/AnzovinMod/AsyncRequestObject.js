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
