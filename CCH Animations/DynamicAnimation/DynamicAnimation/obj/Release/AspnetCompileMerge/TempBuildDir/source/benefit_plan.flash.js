var anzovinmod = anzovinmod || {};
anzovinmod.instance = anzovinmod.instance || {};
anzovinmod.instance.Scenes = anzovinmod.instance.Scenes || {};
anzovinmod.instance.Scenes["benefit_plan"] = anzovinmod.instance.Scenes["benefit_plan"] || {"properties":{}, "instance":{}, "manifest":{}, "loadedCallbacks":[]};
anzovinmod.instance.Scenes["benefit_plan"]["properties"] = {
  "name":"benefit_plan",
  "width":1920,
  "height":1080,
  "fps":24,
  "color":0xECE7D1,
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
      "id":"sfx1",
      "node":"start",
      "nodeFrame":0
    },{
      "id":"JustaThought_24_bit_480001_01",
      "node":"start",
      "nodeFrame":0
    },{
      "id":"_100_sound",
      "node":"start",
      "nodeFrame":14
    },{
      "id":"_200_sound",
      "node":"start",
      "nodeFrame":215
    },{
      "id":"_300_spent",
      "node":"ytdspent",
      "nodeFrame":5
    },{
      "id":"sfx3",
      "node":"deductible",
      "nodeFrame":8
    },{
      "id":"_400_sound",
      "node":"deductible",
      "nodeFrame":0
    },{
      "id":"_500_sound",
      "node":"oopmax",
      "nodeFrame":302
    }
  ],
  "progressNodes": {
    "start": {
      "frame":1,
      "length":417,
      "name":"start",
      "description":"Introduction",
      "position":0,
      "nextNode":"ytdspent"
    },
    "ytdspent": {
      "frame":418,
      "length":259,
      "name":"ytdspent",
      "description":"Year-to-Date Spent",
      "position":15,
      "nextNode":"deductible"
    },
    "deductible": {
      "frame":677,
      "length":300,
      "name":"deductible",
      "description":"Deductible",
      "position":30,
      "nextNode":"coinsurance"
    },
    "coinsurance": {
      "frame":977,
      "length":331,
      "name":"coinsurance",
      "description":"Coinsurance",
      "position":50,
      "nextNode":"oopmax"
    },
    "oopmax": {
      "frame":1308,
      "length":304,
      "name":"oopmax",
      "description":"Out-of-Pocket Maximum",
      "position":70,
      "nextNode":"ending"
    },
    "ending": {
      "frame":1612,
      "length":731,
      "name":"ending",
      "description":"Ending",
      "position":90,
      "nextNode":null
    }
  }
};