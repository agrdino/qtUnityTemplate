using System;
using System.Collections.Generic;
using _Prefab.Popup;
using _Scripts.Helper;
using _Scripts.Scene;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static qtHelper;

namespace _Scripts.System
{
    public class UIManager : qtSingleton<UIManager>
    {
        [Header("Scene config")] [SerializeField]
        private qtScene.EScene startScene;

        public popBase currentPopup
        {
            get;
            private set;
        }
        public sceneBase currentScene
        {
            get;
            private set;
        }

        public hudBase currentHUD
        {
            get;
            private set;
        }
        
        #region ----- VARIABLE -----

        private GameObject _canvasOnTop;
        private GameObject _canvas;
        private GameObject _ignoreCast;
        private Image _fading;
        private TextMeshProUGUI _txtDetail;

        private Dictionary<qtScene.EPopup, popBase> _popups;
        private Dictionary<qtScene.EScene, sceneBase> _scenes;
        private Dictionary<qtScene.EHud, hudBase> _huds;
        public Stack<popBase> stackPopup; 
        
        #endregion
        
        #region ----- INITIALIZE -----
        
        private void Start()
        {
            InitObject();
            Initialize();
            ShowScene(startScene);
        }

        private void InitObject()
        {
            _canvasOnTop = FindObjectInRootIncludingInactive("CanvasOnTop");
            _canvas = FindObjectInRootIncludingInactive("Canvas");
        }
        
        private void Initialize()
        {
            stackPopup ??= new Stack<popBase>();
        }

        #endregion

        #region ----- UNITY EVENT -----

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                if (stackPopup.Count > 0)
                {
                    stackPopup.Pop().Hide();
                    currentPopup = stackPopup.Peek();
                }
            }
        }

        #endregion

        #region ----- PUBLIC FUNCTION -----
        
        public void ShowDetail(string content, Vector3 position, bool isShow = true)
        {
            if (_txtDetail == null)
            {
                _txtDetail = Instantiate(Resources.Load<GameObject>("pnlDetail"), _canvasOnTop.transform, true)
                    .GetComponentInChildren<TextMeshProUGUI>();
            }

            if (!isShow)
            {
                _txtDetail.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                _txtDetail.text = content;
                _txtDetail.transform.parent.position = position;
                _txtDetail.transform.parent.gameObject.SetActive(true);
            }
        }
        
        public bool ignoreCast
        {
            set {
                if (_ignoreCast == null)
                {
                    _ignoreCast = Instantiate(Resources.Load<GameObject>("imgIgnoreCast"), _canvasOnTop.transform);
                    _ignoreCast.transform.SetSiblingIndex(1);
                }

                _ignoreCast.SetActive(value);
            }
        }

        private Color _fade;
        public void Fading(bool fade)
        {
            _fade = Color.clear;
            _fade.a = 150 / 255f;
            if (_fading == null)
            {
                _fading = Instantiate(Resources.Load<GameObject>("imgFading"), _canvasOnTop.transform).GetComponent<Image>();
                _fading.gameObject.SetActive(false);
                _fading.transform.SetSiblingIndex(1);
                _fading.color = Color.clear;
            }
            if (fade)
            {
                if (_fading.gameObject.activeSelf)
                {
                    return;
                }
                _fading.gameObject.SetActive(true);
                _fading.DOColor(_fade, 0.25f);
            }
            else
            {
                _fading.DOColor(Color.clear, 0.25f).OnComplete(() => _fading.gameObject.SetActive(false));
            }
        }

        public popBase ShowPopup(qtScene.EPopup popupId)
        {
            _popups ??= new Dictionary<qtScene.EPopup, popBase>();
            popBase tempPopup = null;
            if (!_popups.ContainsKey(popupId))
            {
                tempPopup = Instantiate(qtScene.sceneData.popups.Find(x => x.id == popupId).prefab, _canvasOnTop.transform).GetComponent<popBase>();
                _popups.Add(popupId, tempPopup);
            }
            else
            {
                tempPopup = _popups[popupId];
            }
            tempPopup.transform.SetSiblingIndex(_canvasOnTop.transform.childCount - 1);
            currentPopup = tempPopup;
            return tempPopup;
        }

        public sceneBase ShowScene(qtScene.EScene scene)
        {
            _scenes ??= new Dictionary<qtScene.EScene, sceneBase>();
            
            sceneBase tempScene = null;
            if (!_scenes.ContainsKey(scene))
            {
                var sceneName = qtScene.sceneData.sences.Find(x => x.id == scene).scene.name;
                var temp = FindObjectInChildren(_canvas, sceneName);
                if (temp == null)
                {
                    tempScene = Instantiate(qtScene.sceneData.sences.Find(x => x.id == scene).scene, _canvas.transform).GetComponent<sceneBase>();
                    tempScene.InitObject();
                    _scenes.Add(scene, tempScene);
                }
                else
                {
                    tempScene = temp.GetComponent<sceneBase>();
                    tempScene.InitObject();
                    _scenes.Add(scene, tempScene);
                }
            }
            else
            {
                tempScene = _scenes[scene];
            }

            if (currentScene != null)
            {
                currentScene.Hide();
            }
            tempScene.Initialize();
            tempScene.Show();
            currentHUD = ShowHUD(qtScene.sceneData.sences.Find(x => x.id == scene));
            currentScene = tempScene;
            return tempScene;
        }

        #endregion

        #region ----- PRIVATE FUNCTION

        private hudBase ShowHUD(qtObject.Scene scene)
        {
            if (!scene.showHUD) 
            {
                if (currentHUD != null)
                {
                    currentHUD.Hide();
                    currentHUD = null;
                    return currentHUD;
                }
            }
            _huds ??= new Dictionary<qtScene.EHud, hudBase>();

            hudBase tempHud = null;
            if (!_huds.ContainsKey(scene.hudId))
            {
                var hudName = qtScene.sceneData.huds.Find(x => x.id == scene.hudId).prefab.name;
                var temp = FindObjectInChildren(_canvasOnTop, hudName);
                if (temp == null)
                {
                    tempHud = Instantiate(qtScene.sceneData.huds.Find(x => x.id == scene.hudId).prefab, _canvasOnTop.transform).GetComponent<hudBase>();
                    tempHud.InitObject();
                    _huds.Add(scene.hudId, tempHud);
                }
                else
                {
                    tempHud = temp.GetComponent<hudBase>();
                    tempHud.InitObject();
                    _huds.Add(scene.hudId, tempHud);
                }

            }
            else
            {
                tempHud = _huds[scene.hudId];
            }
            tempHud.transform.SetSiblingIndex(0);
            tempHud.Initialize();
            tempHud.Show();
            return tempHud;
        }

        #endregion
    }
}
