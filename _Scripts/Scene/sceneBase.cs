using System.Collections;
using System.Collections.Generic;
using _Scripts.Scene.MainMenuScene;
using _Scripts.System;
using DG.Tweening;
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
        public abstract void InitEvent();
        public abstract void InitObject();
        public abstract IEnumerator OnExit();

        public virtual void OnBack()
        {
            //Temp
            UIManager.Instance.ShowScene<MenuScene>(qtScene.EScene.MenuScene);
        }
        public virtual Tween Show()
        {
            return null;
        }

        public virtual Tween Hide()
        {
            return null;
        }
    }
}
