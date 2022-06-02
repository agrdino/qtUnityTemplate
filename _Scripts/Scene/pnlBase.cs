using UnityEngine;

namespace _Scripts.Scene
{
    public abstract class pnlBase : MonoBehaviour
    {
        protected bool isInit;
        protected bool isIn;
        protected float h;
        protected float w;

        public void Show()
        {
            gameObject.SetActive(true);
            if (!isInit)
            {
                InitObject();
            }

            InitEvent();
            Initialize();
            MoveIn();
        }

        public abstract void RefreshUI();

        protected abstract void Initialize();
        
        public abstract pnlBase InitObject();

        protected abstract void InitEvent();
        
        protected abstract void MoveIn();

        protected virtual void OnDisable()
        {
            RemoveEvent();
        }

        public virtual void Hide()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            MoveOut();
        }
        protected abstract void MoveOut();

        protected abstract void RemoveEvent();
    }
}
