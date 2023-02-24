using System;
using System.Collections.Generic;
using System.Linq;
using _Prefab.Popup;
using _Scripts.HUD;
using _Scripts.Scene;
using qtObject;
using UnityEngine;

[CreateAssetMenu(fileName = "New PopupData", menuName = "Popup Data", order = 51)]
public class SceneData : ScriptableObject
{
    public List<qtObject.Popup> popups = new List<qtObject.Popup>();
    public List<qtObject.Scene> sences = new List<qtObject.Scene>();
    public List<qtObject.HUD> huds = new List<HUD>();
    
    public Popup this[qtScene.EPopup id] => popups.FirstOrDefault(popup => popup.id == id) ?? null;
    public Scene this[qtScene.EScene id] => sences.FirstOrDefault(scene => scene.id == id) ?? null;
    public HUD this[qtScene.EHud id] => huds.FirstOrDefault(hud => hud.id == id) ?? null;
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
        [Range(0, 2)] public float fadingIn = 0.25f;
        [Range(0, 2)] public float fadingOut = 0.5f;
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