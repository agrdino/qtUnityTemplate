using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using _Prefab.Popup;
using _Prefab.Popup.YesNoPopup;
using _Scripts.HUD;
using _Scripts.qtLib;
using _Scripts.qtLib.Corountine;
using _Scripts.Scene;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
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

        private Dictionary<qtScene.EPopup, popBase> _popups;
        private Dictionary<qtScene.EScene, sceneBase> _scenes;
        private Dictionary<qtScene.EHud, hudBase> _huds;
        public List<popBase> stackPopup; 
        // public CultureInfo culture = new CultureInfo("ja-JP", true);
        public const string DayFormat = "MM/dd の声かけ";
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
            _loadingIndicator = Instantiate(Resources.Load<RectTransform>("imgLoading"), _canvasOnTop.transform);
            _loadingIndicator.gameObject.SetActive(false);

            if (_fading == null)
            {
                _fading = Instantiate(Resources.Load<Image>("imgFading"), _canvasOnTop.transform);
                _fading.gameObject.SetActive(false);
                _fading.color = Color.clear;
            }
            
            if (_ignoreCast == null)
            {
                _ignoreCast = Instantiate(Resources.Load<GameObject>("imgIgnoreCast"), _canvasOnTop.transform);
                _ignoreCast.SetActive(false);
                _ignoreCast.transform.SetSiblingIndex(1);
            }
        }
        
        private void Initialize()
        {
            stackPopup ??= new List<popBase>();
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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (stackPopup.Count > 0)
                {
                    stackPopup[^1].Hide();
                    if (stackPopup.Count > 0)
                    {
                        currentPopup = stackPopup[^1];
                    }
                    else
                    {
                        currentPopup = null;
                    }
                    return;
                }
                if (currentPopup == null)
                {
                    if (currentScene != null)
                    {
                        currentScene.OnBack();
                    }
                }
            }
        }

        #endregion

        #region ----- PUBLIC FUNCTION -----

        public void ShowLoadingIndicator()
        {
            _loadingIndicator.transform.eulerAngles = Vector3.zero;
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
                _ignoreCast.SetActive(value);
            }
        }

        private Color _fade;
        public void Fading(bool fade)
        {
            if (!fade)
            {
                if (currentPopup != null)
                {
                    _fading.transform.SetSiblingIndex(currentPopup.transform.GetSiblingIndex() - 1);
                }
                if (stackPopup.Count >= 1 )
                {
                    return;
                }
            }
            
            _fade = Color.clear;
            _fade.a = 150 / 255f;
            if (fade)
            {
                if (currentPopup != null)
                {
                    _fading.transform.SetSiblingIndex(currentPopup.transform.GetSiblingIndex() - 1);
                }
                _fading.gameObject.SetActive(true);
                _fading.DOColor(_fade, 0.25f);
            }
            else
            {
                _fading.DOColor(Color.clear, 0.25f).OnComplete(() => _fading.gameObject.SetActive(false));
            }
        }

        //Popup
        public IEnumerator<T> ShowPopupWithoutWait<T>(qtScene.EPopup popupId, Action beforeShow = null, Action afterShow = null) where T : popBase
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
                if (tempPopup.gameObject.activeSelf)
                {
                    yield return tempPopup.KeepShowing() as T;
                }
            }
            tempPopup.transform.SetSiblingIndex(_canvasOnTop.transform.childCount - 1);
            stackPopup.Add(tempPopup);
            currentPopup = tempPopup;
            Fading(true);
            yield return tempPopup.Show() as T;
        }
        
        public T ShowPopup<T>(qtScene.EPopup popupId, Action beforeShow = null, Action afterShow = null) where T : popBase
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
                if (tempPopup.gameObject.activeSelf)
                {
                    return tempPopup.KeepShowing() as T;
                }
            }
            tempPopup.transform.SetSiblingIndex(_canvasOnTop.transform.childCount - 1);
            stackPopup.Add(tempPopup);
            currentPopup = tempPopup;
            Fading(true);
            return tempPopup.Show() as T;
        }

        public void HidePopup(popBase popup)
        {
            stackPopup.Remove(popup);
            if (stackPopup.Count > 0)
            {
                currentPopup = stackPopup[^1];
            }
            else
            {
                currentPopup = null;
            }
        }

        //Scene
        public IEnumerator<T> ShowSceneWithAnimation<T>(qtScene.EScene scene) where T : sceneBase
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
                if (tempScene.gameObject.activeSelf)
                {
                    yield return (T)tempScene;
                }
            }
            tempScene.gameObject.SetActive(false);


            if (currentScene != null)
            {
                var oldScene = currentScene;
                qtPrivateCoroutiner.Start(oldScene.OnExit());
                var anim = oldScene.Hide();
                while (anim.IsActive() && !anim.IsComplete())
                {
                    yield break;
                }
                tempScene.InitEvent();
                tempScene.Initialize();
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
                tempScene.Initialize();
                var anim = tempScene.Show();
                while (anim.IsActive() && !anim.IsComplete())
                {
                    yield break;
                }
            }
            
            yield return (T)tempScene;
        }
        
        public T ShowScene<T>(qtScene.EScene scene, Action beforeShow = null, Action afterShow = null) where T : sceneBase
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
                if (tempScene.gameObject.activeSelf)
                {
                    return (T)tempScene;
                }
            }
            tempScene.gameObject.SetActive(false);


            if (currentScene != null)
            {
                var oldScene = currentScene;
                FadingScene(tempScene, showScene.fadingIn, showScene.fadingOut, showScene.showHUD, showScene.hudId,
                    () =>
                    {
                        qtPrivateCoroutiner.Start(oldScene.OnExit());
                        oldScene.gameObject.SetActive(false);
                    }, () =>
                    {
                        tempScene.InitEvent();
                        tempScene.Initialize();
                    });
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
                tempScene.Initialize();
                tempScene.gameObject.SetActive(true);
            }
            
            return (T)tempScene;
        }

        public T GetScene<T>(qtScene.EScene scene, Action beforeShow = null, Action afterShow = null) where T : sceneBase
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
                tempScene.InitObject();
                _scenes.Add(scene, tempScene);
            }
            else
            {
                tempScene = temp.GetComponent<sceneBase>();
                tempScene.InitObject();
                _scenes.Add(scene, tempScene);
            }
            tempScene.gameObject.SetActive(false);

            return (T)tempScene;
        }
        
        public IEnumerator<bool> ShowConfirmPopup(string title, string message)
        {
            var result = false;
            var popup = ShowPopupWithoutWait<YesNoPopup>(qtScene.EPopup.YesNo).Current
                .Initialize(title, message, () =>
                {
                    result = true;
                });
            yield return result;
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
                    tempHud.InitObject();
                    _huds.Add(hud, tempHud);
                }
                else
                {
                    tempHud = temp.GetComponent<hudBase>();
                    tempHud.InitObject();
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

        private void FadingScene(sceneBase newScene, float fadeIn, float fadeOut, bool showHUD = false, qtScene.EHud hud = qtScene.EHud.None, Action fadeInAction = null, Action onComplete = null)
        {
            Fading(true, Color.black, fadeIn)
                .OnStart(() =>
                {
                    ignoreCast = true;
                    if (currentHUD != null && (currentHUD.id == qtScene.EHud.None || !showHUD || currentHUD.id != hud))
                    {
                        currentHUD.Hide();
                        currentHUD = null;
                    }
                })
                .OnComplete(() =>
            {
                stackPopup ??= new List<popBase>(); 
                while (stackPopup.Count > 0)
                {
                    stackPopup[^1].Hide();
                }
                currentPopup = null;
                currentScene = newScene;
                fadeInAction?.Invoke();
                newScene.gameObject.SetActive(true);
                if (showHUD && hud != qtScene.EHud.None)
                {
                    if (currentHUD == null || currentHUD.id != hud)
                    {
                        currentHUD = ShowHUD(hud);
                    }
                }
                onComplete?.Invoke();
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
                if (time == 0)
                {
                    _fade = Color.clear;
                }
                _fading.gameObject.SetActive(true);
                return _fading.DOColor(_fade, time);
            }
            else
            {
                if (time == 0)
                {
                    ignoreCast = false;
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
