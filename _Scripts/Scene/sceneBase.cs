using UnityEngine;

namespace _Scripts.Scene
{
    public abstract class sceneBase : MonoBehaviour
    {
        public virtual void Initialize()
        {
            InitEvent();
        }
        protected abstract void InitEvent();
        public abstract void InitObject();

        protected abstract void RemoveEvent();

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            RemoveEvent();
        }
    }
}
