namespace qtUnityTemplate._Scripts.Scene
{
    public class mediatorBase
    {
        public bool uiRequir = true;
        public object param;

        public mediatorBase(bool uiRequir = true)
        {
            this.uiRequir = uiRequir; 
            
        }

        public mediatorBase(object Param = null)
        {
            this.param = Param;
        }

        protected mediatorBase()
        {
        }

        public mediatorBase SetParam(object p)
        {
            param = p; return this;
        }

    }
}