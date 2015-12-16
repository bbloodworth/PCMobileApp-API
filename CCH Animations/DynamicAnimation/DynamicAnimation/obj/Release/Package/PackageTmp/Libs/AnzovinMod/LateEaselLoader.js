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
