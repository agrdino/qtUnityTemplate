using System;
using System.Collections.Generic;
using _Prefab.Popup;
using _Scripts.Scene;
using qtObject;
using UnityEngine;

[CreateAssetMenu(fileName = "New PopupData", menuName = "Popup Data", order = 51)]
public class SceneData : ScriptableObject
{
    public List<qtObject.Popup> popups = new List<qtObject.Popup>();
    public List<qtObject.Scene> sences = new List<qtObject.Scene>();
    public List<qtObject.HUD> huds = new List<HUD>();
}

namespace qtObject
{
    [Serializable]
    public class Popup
    {
        public string name;
        public qtScene.EPopup id;
        public GameObject prefab;
        [NonSerialized] public popBase handler;
    }

    [Serializable]
    public class Scene
    {
        public string name;
        public qtScene.EScene id;
        public GameObject scene;
        public bool showHUD;
        public qtScene.EHud hudId;
        [NonSerialized] public sceneBase handler;
    }

    [Serializable]
    public class HUD
    {
        public string name;
        public qtScene.EHud id;
        public GameObject prefab;
        [NonSerialized] public hudBase handler;
    }
}