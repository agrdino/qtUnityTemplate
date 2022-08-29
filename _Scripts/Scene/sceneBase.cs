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
        protected abstract void OnEnter();
        public abstract void OnInit();

        protected abstract void OnExit();

        public virtual void Show()
        {
            gameObject.SetActive(true);
            OnEnter();
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            OnExit();
        }
    }
}
