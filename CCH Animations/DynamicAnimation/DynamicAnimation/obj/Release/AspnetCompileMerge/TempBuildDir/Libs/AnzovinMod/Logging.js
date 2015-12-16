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
