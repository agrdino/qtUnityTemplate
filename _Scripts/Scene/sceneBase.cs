using _Scripts.Scene.MainMenuScene;
using _Scripts.System;
using UnityEngine;

namespace _Scripts.Scene
{
    [DisallowMultipleComponent]
    public abstract class sceneBase : MonoBehaviour
    {
        protected float _w, _h;
        public virtual void Initialize()
        {
        }
        protected abstract void InitEvent();
        public abstract void InitObject();
        protected abstract void OnExit();

        public virtual void OnBack()
        {
            //Temp
            UIManager.Instance.ShowScene<MenuScene>(qtScene.EScene.MenuScene);
        }
        public virtual void Show()
        {
            gameObject.SetActive(true);
            InitEvent();
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            OnExit();
        }
    }
}
