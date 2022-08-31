using System;
using System.Collections.Generic;
using _Prefab.Popup;
using _Scripts.HUD;
using _Scripts.qtLib;
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

        private qtObject.Scene _currentSceneData;
        public hudBase currentHUD
        {
            get;
            private set;
        }
        
        #region ----- VARIABLE -----

        private GameObject _canvasOnTop;
        private GameObject _canvas;
        private GameObject _ignoreCast;
        private RectTransform _loadingIndicator;
        private Image _fading;
        private TextMeshProUGUI _txtDetail;
        private UIDialogWebview _webView;

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
            ShowScene<sceneBase>(startScene);
        }

        private void InitObject()
        {
            _canvasOnTop = FindObjectInRootIncludingInactive("CanvasOnTop");
            _canvas = FindObjectInRootIncludingInactive("MainCanvas");
        }
        
        private void Initialize()
        {
            stackPopup ??= new Stack<popBase>();
        }

        #endregion

        #region ----- UNITY EVENT -----

        protected override void Init()
        {
            base.Init();
            Application.targetFrameRate = 60;
            Input.multiTouchEnabled = false;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                if (stackPopup.Count > 0)
                {
                    stackPopup.Pop().Hide();
                    if (stackPopup.Count > 0)
                    {
                        currentPopup = stackPopup.Peek();
                    }
                    else
                    {
                        currentPopup = null;
                    }
                }
            }
        }

        #endregion

        #region ----- PUBLIC FUNCTION -----

        public UIDialogWebview OpenWebView(string path)
        {
            if (_webView == null)
            {
                _webView = Instantiate(Resources.Load<UIDialogWebview>("UIDialogWebview"), _canvasOnTop.transform);
            }
            _webView.LoadURL(path);
            return _webView;
        }
        public void ShowLoadingIndicator()
        {
            if (_loadingIndicator == null)
            {
                _loadingIndicator = Instantiate(Resources.Load<RectTransform>("imgLoading"), _canvasOnTop.transform);
            }

            _loadingIndicator.SetSiblingIndex(_canvasOnTop.transform.childCount - 1);
            _loadingIndicator.DORotate(-360 * Vector3.forward, 2, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
            _loadingIndicator.gameObject.SetActive(true);
        }

        public void HideLoadingIndicator()
        {
            if (_loadingIndicator == null)
            {
                return;
            }

            _loadingIndicator.DOKill();
            _loadingIndicator.gameObject.SetActive(false);
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
            if (!fade)
            {
                _fading.transform.SetSiblingIndex(currentPopup.transform.GetSiblingIndex() - 1);
                if (stackPopup.Count > 1 )
                {
                    return;
                }
            }
            
            _fade = Color.clear;
            _fade.a = 150 / 255f;
            if (_fading == null)
            {
                _fading = Instantiate(Resources.Load<Image>("imgFading"), _canvasOnTop.transform);
                _fading.gameObject.SetActive(false);
                if (_currentSceneData.showHUD)
                {
                    _fading.transform.SetSiblingIndex(1);
                }
                else
                {
                    _fading.transform.SetSiblingIndex(0);
                }
                _fading.color = Color.clear;
            }
            if (fade)
            {
                if (_fading.gameObject.activeSelf)
                {
                    return;
                }
                
                _fading.transform.SetSiblingIndex(currentPopup.transform.GetSiblingIndex() - 1);
                _fading.gameObject.SetActive(true);
                _fading.DOColor(_fade, 0.25f);
            }
            else
            {
                _fading.DOColor(Color.clear, 0.25f).OnComplete(() => _fading.gameObject.SetActive(false));
            }
        }

        public T ShowPopup<T>(qtScene.EPopup popupId) where T : popBase
        {
            _popups ??= new Dictionary<qtScene.EPopup, popBase>();
            popBase tempPopup = null;
            if (!_popups.ContainsKey(popupId))
            {
                var popup = qtScene.sceneData[popupId];
                tempPopup = Instantiate(popup.prefab, _canvasOnTop.transform).GetComponent<popBase>();
                tempPopup.name = popup.prefab.name;
                _popups.Add(popupId, tempPopup);
            }
            else
            {
                tempPopup = _popups[popupId];
            }
            tempPopup.transform.SetSiblingIndex(_canvasOnTop.transform.childCount - 1);
            stackPopup.Push(tempPopup);
            currentPopup = tempPopup;
            return (T)tempPopup.Show();
        }

        public T ShowScene<T>(qtScene.EScene scene) where T : sceneBase
        {
            _scenes ??= new Dictionary<qtScene.EScene, sceneBase>();
            
            sceneBase tempScene = null;
            var showScene = qtScene.sceneData[scene];
            _currentSceneData = showScene;
            if (!_scenes.ContainsKey(scene))
            {
                var temp = FindObjectInChildren(_canvas, showScene.scene.name);
                if (temp == null)
                {
                    var sceneConfig = qtScene.sceneData[scene];
                    tempScene = Instantiate(sceneConfig.scene, _canvas.transform).GetComponent<sceneBase>();
                    tempScene.gameObject.name = sceneConfig.scene.name;
                    tempScene.OnInit();
                    _scenes.Add(scene, tempScene);
                }
                else
                {
                    tempScene = temp.GetComponent<sceneBase>();
                    tempScene.OnInit();
                    _scenes.Add(scene, tempScene);
                }
            }
            else
            {
                tempScene = _scenes[scene];
                if (tempScene.gameObject.activeSelf)
                {
                    return (T)tempScene;
                }
            }
            tempScene.gameObject.SetActive(false);

            tempScene.Initialize();

            if (currentScene != null)
            {
                var oldScene = currentScene;
                qtCoroutiner.ExcuteAfterDelayTime(showScene.fadingIn, () => oldScene.Hide());
                FadingScene(tempScene, showScene.fadingOut, showScene.fadingIn, showScene.showHUD, showScene.hudId);
            }
            else
            {
                currentScene = tempScene;
                if (showScene.showHUD && showScene.hudId != qtScene.EHud.None)
                {
                    if (currentHUD == null || currentHUD.id != showScene.hudId)
                    {
                        currentHUD = ShowHUD(showScene.hudId);
                    }
                }
                tempScene.Show();
            }
            
            return (T)tempScene;
        }

        public T GetScene<T>(qtScene.EScene scene) where T : sceneBase
        {
            _scenes ??= new Dictionary<qtScene.EScene, sceneBase>();
            if (_scenes.ContainsKey(scene))
            {
                return (T)_scenes[scene];
            }
            var showScene = qtScene.sceneData[scene];
            var temp = FindObjectInChildren(_canvas, showScene.scene.name);
            sceneBase tempScene = null; 
            if (temp == null)
            {
                tempScene = Instantiate(showScene.scene, _canvas.transform).GetComponent<sceneBase>();
                tempScene.gameObject.name = showScene.scene.name;
                tempScene.OnInit();
                _scenes.Add(scene, tempScene);
            }
            else
            {
                tempScene = temp.GetComponent<sceneBase>();
                tempScene.OnInit();
                _scenes.Add(scene, tempScene);
            }
            tempScene.gameObject.SetActive(false);

            return (T)tempScene;
        }
        
        #endregion

        #region ----- PRIVATE FUNCTION -----

        private hudBase ShowHUD(qtScene.EHud hud)
        {
            if (currentHUD != null)
            {
                if (currentHUD.id == hud)
                {
                    return currentHUD;
                }
                currentHUD.Hide();
                currentHUD = null;
            }
            _huds ??= new Dictionary<qtScene.EHud, hudBase>();

            hudBase tempHud = null;
            if (!_huds.ContainsKey(hud))
            {
                var hudConfig = qtScene.sceneData[hud];
                var hudName = hudConfig.prefab.name;
                var temp = FindObjectInChildren(_canvasOnTop, hudName);
                if (temp == null)
                {
                    tempHud = Instantiate(hudConfig.prefab, _canvasOnTop.transform).GetComponent<hudBase>();
                    tempHud.name = hudConfig.prefab.name;
                    tempHud.OnInit();
                    _huds.Add(hud, tempHud);
                }
                else
                {
                    tempHud = temp.GetComponent<hudBase>();
                    tempHud.OnInit();
                    _huds.Add(hud, tempHud);
                }
            }
            else
            {
                tempHud = _huds[hud];
            }
            tempHud.transform.SetSiblingIndex(0);
            tempHud.Initialize();
            tempHud.id = hud;
            currentHUD = tempHud;
            return tempHud.Show();
        }

        private void FadingScene(sceneBase newScene, float fadeIn, float fadeOut, bool showHUD = false, qtScene.EHud hud = qtScene.EHud.None)
        {
            Fading(true, Color.black, fadeIn)
                .OnStart(() =>
                {
                    ignoreCast = true;
                    if (currentHUD != null && currentHUD.id != hud)
                    {
                        currentHUD.Hide();
                        currentHUD = null;
                    }
                })
                .OnComplete(() =>
            {
                stackPopup ??= new Stack<popBase>(); 
                while (stackPopup.Count > 0)
                {
                    stackPopup.Pop().Hide();
                }
                currentPopup = null;
                currentScene = newScene;
                newScene.Show();
                if (showHUD && hud != qtScene.EHud.None)
                {
                    if (currentHUD == null || currentHUD.id != hud)
                    {
                        currentHUD = ShowHUD(hud);
                    }
                }
                Fading(false, Color.black, fadeOut);
            });
        }
        
        private void FadingScene(sceneBase newScene, Action callback)
        {
            Fading(true, Color.black).OnComplete(() =>
            {
                callback?.Invoke();
                Fading(false, Color.black);
            });
        }

        private Tween Fading(bool fade, Color color, float time = 0.5f)
        {
            if (_fading == null)
            {
                _fading = Instantiate(Resources.Load<GameObject>("imgFading"), _canvasOnTop.transform).GetComponent<Image>();
                _fading.gameObject.SetActive(false);
                _fading.color = Color.clear;
            }
            
            _fading.transform.SetSiblingIndex(_canvasOnTop.transform.childCount - 1);
            _fading.DOKill();
            _fade = color;
            _fade.a = 1;

            if (fade)
            {
                _fading.gameObject.SetActive(true);
                return _fading.DOColor(_fade, time);
            }
            else
            {
                if (time == 0)
                {
                    return _fading.DOColor(Color.clear, time).OnComplete(() => _fading.gameObject.SetActive(false));
                }
                return _fading.DOColor(Color.clear, time).SetDelay(0.25f).OnComplete(() =>
                {
                    _fading.gameObject.SetActive(false);
                    ignoreCast = false;
                });
            }
        }

        #endregion
    }
}
