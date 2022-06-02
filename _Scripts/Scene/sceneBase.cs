namespace _Scripts.Scene
{
    public abstract class sceneBase : qtSingleton<sceneBase>
    {
        public virtual void Initialize()
        {
            InitEvent();
        }
        protected abstract void InitEvent();
        public abstract void InitObject();

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
