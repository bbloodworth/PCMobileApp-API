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
