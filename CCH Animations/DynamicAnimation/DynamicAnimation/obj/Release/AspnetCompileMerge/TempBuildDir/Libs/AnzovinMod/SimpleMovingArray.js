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
