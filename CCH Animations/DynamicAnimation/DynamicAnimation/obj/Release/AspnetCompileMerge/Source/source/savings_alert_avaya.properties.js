var anzovinmod = anzovinmod || {};
anzovinmod.instance = anzovinmod.instance || {};
anzovinmod.instance.Scenes = anzovinmod.instance.Scenes || {};
anzovinmod.instance.Scenes["savings_alert_avaya"] = anzovinmod.instance.Scenes["savings_alert_avaya"] || {"properties":{}, "instance":{}, "manifest":{}, "loadedCallbacks":[]};
anzovinmod.instance.Scenes["savings_alert_avaya"]["properties"] = {
  name: "savings_alert_avaya",
  width: 1920,
  height: 1080,
  fps: 24,
  color: "#ECE7D1",
  manifest: [
    {src:"../sounds/savings_alert_avaya/cch_button_bop.ogg", id:"cch_button_bop", data:5},
    {src:"../sounds/savings_alert_avaya/cch_button_pop.ogg", id:"cch_button_pop", data:5},
    {src:"../sounds/savings_alert_avaya/all.avaya.ogg", id:"all", data:1}
  ],
  "loginButtonConfig": {
    "imageSizeWidth": 1334,
    "imageSizeHeight": 750,
    "x": 1185,
    "y": 5,
    "width": 143,
    "height": 63,
    "url": "http://www.clearcosthealth.com/"
  },
  "soundsAtNodes": [
    {
      "id":"all",
      "node":"start",
      "nodeFrame":0
    }
  ],
  "progressNodes": {
    "start": {
      "frame":1,
      "length":171,
      "name":"start",
      "description":"Introduction",
      "position":0,
      "nextNode":"couldhavesaved"
    },
    "couldhavesaved": {
      "frame":172,
      "length":206,
      "name":"couldhavesaved",
      "description":"You Could Have\nSaved",
      "position":10,
      "nextNode":"whatiscch"
    },
    "whatiscch": {
      "frame":378,
      "length":309,
      "name":"whatiscch",
      "description":"What is\nClearCost Health",
      "position":20,
      "nextNode":"howtousecch"
    },
    "howtousecch": {
      "frame":687,
      "length":720,
      "name":"howtousecch",
      "description":"How to Use\nClearCost Health",
      "position":40,
      "nextNode":"findfuturesavings"
    },
    "findfuturesavings": {
      "frame":1407,
      "length":879,
      "name":"findfuturesavings",
      "description":"Find\nFuture Savings",
      "position":75,
      "nextNode":null
    }
  }
};
