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
