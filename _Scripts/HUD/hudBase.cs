using UnityEngine;

namespace _Scripts.HUD
{
    [DisallowMultipleComponent]
    public abstract class hudBase : MonoBehaviour
    {
        [HideInInspector] public qtScene.EHud id;
        protected float _width;
        protected float _height;

        protected bool _isInit;
        public virtual void Initialize()
        {
            InitEvent();
        }
        public abstract void InitEvent();
        public abstract void InitObject();

        protected abstract void OnExit();

        public virtual hudBase Show()
        {
            gameObject.SetActive(true);
            return this;
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
