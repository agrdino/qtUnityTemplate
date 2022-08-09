using System.Collections.Generic;
using _Prefab.Popup;
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

        public void ShowLoadingIndicator()
        {
            if (_loadingIndicator == null)
            {
                _loadingIndicator = Instantiate(Resources.Load<GameObject>("imgLoading"), _canvasOnTop.transform).GetComponent<RectTransform>();
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
            _fade = Color.clear;
            _fade.a = 150 / 255f;
            if (_fading == null)
            {
                _fading = Instantiate(Resources.Load<GameObject>("imgFading"), _canvasOnTop.transform).GetComponent<Image>();
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
                tempPopup = Instantiate(qtScene.sceneData.popups.Find(x => x.id == popupId).prefab, _canvasOnTop.transform).GetComponent<popBase>();
                tempPopup.name = qtScene.sceneData.popups.Find(x => x.id == popupId).prefab.name;
                _popups.Add(popupId, tempPopup);
            }
            else
            {
                tempPopup = _popups[popupId];
            }
            tempPopup.transform.SetSiblingIndex(_canvasOnTop.transform.childCount - 1);
            currentPopup = tempPopup;
            return (T)tempPopup.Show();
        }

        public T ShowScene<T>(qtScene.EScene scene) where T : sceneBase
        {
            _scenes ??= new Dictionary<qtScene.EScene, sceneBase>();
            
            sceneBase tempScene = null;
            var showScene = qtScene.sceneData.sences.Find(x => x.id == scene);
            _currentSceneData = showScene;
            if (!_scenes.ContainsKey(scene))
            {
                var temp = FindObjectInChildren(_canvas, showScene.scene.name);
                if (temp == null)
                {
                    tempScene = Instantiate(qtScene.sceneData.sences.Find(x => x.id == scene).scene, _canvas.transform).GetComponent<sceneBase>();
                    tempScene.gameObject.name = qtScene.sceneData.sences.Find(x => x.id == scene).scene.name;
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
            tempScene.gameObject.SetActive(false);

            tempScene.Initialize();

            if (currentScene != null)
            {
                currentScene.Hide();
                FadingScene(tempScene);
            }
            else
            {
                currentScene = tempScene;
                tempScene.Show();
            }

            currentHUD = ShowHUD(showScene);

            return (T)tempScene;
        }

        public T GetScene<T>(qtScene.EScene scene) where T : sceneBase
        {
            _scenes ??= new Dictionary<qtScene.EScene, sceneBase>();
            if (_scenes.ContainsKey(scene))
            {
                return (T)_scenes[scene];
            }
            var showScene = qtScene.sceneData.sences.Find(x => x.id == scene);
            var temp = FindObjectInChildren(_canvas, showScene.scene.name);
            sceneBase tempScene = null; 
            if (temp == null)
            {
                tempScene = Instantiate(qtScene.sceneData.sences.Find(x => x.id == scene).scene, _canvas.transform).GetComponent<sceneBase>();
                tempScene.gameObject.name = qtScene.sceneData.sences.Find(x => x.id == scene).scene.name;
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
        
        #endregion

        #region ----- PRIVATE FUNCTION -----

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

                return null;
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
                    tempHud.name = qtScene.sceneData.huds.Find(x => x.id == scene.hudId).prefab.name;
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

        private void FadingScene(sceneBase newScene)
        {
            Fading(true, Color.black).OnComplete(() =>
            {
                stackPopup ??= new Stack<popBase>(); 
                while (stackPopup.Count > 0)
                {
                    stackPopup.Pop().Hide();
                }
                currentPopup = null;
                currentScene = newScene;
                newScene.Show();
                Fading(false, Color.black);
            });
        }

        private Tween Fading(bool fade, Color color)
        {
            _fading.DOKill();
            _fade = color;
            _fade.a = 1;
            if (_fading == null)
            {
                _fading = Instantiate(Resources.Load<GameObject>("imgFading"), _canvasOnTop.transform).GetComponent<Image>();
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
                _fading.gameObject.SetActive(true);
                return _fading.DOColor(_fade, 0.5f);
            }
            else
            {
                return _fading.DOColor(Color.clear, 0.25f).SetDelay(0.25f).OnComplete(() => _fading.gameObject.SetActive(false));
            }
        }

        #endregion
    }
}
