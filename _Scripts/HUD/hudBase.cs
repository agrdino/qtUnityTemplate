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
            OnEnter();
        }
        protected abstract void OnEnter();
        public abstract void OnInit();

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
