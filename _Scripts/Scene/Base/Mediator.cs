using System.Collections.Generic;
using _Scripts.Scene;
using UnityEngine;

namespace qtUnityTemplate._Scripts.Scene
{
    public class Mediator<V, L> : mediatorBase where V : sceneBase where L : logicBase
    {
        public delegate Coroutine fncVLM(V view, L logic, Mediator<V, L> m);

        protected fncVLM _addBeforeUIShow, _addAfterUIShow;

        public Mediator<V, L> AfterUIShow(fncVLM func) { _addAfterUIShow += func; return this; }
        public Mediator<V, L> BeforeUIShow(fncVLM func)
        {
            _addBeforeUIShow += func;
            return this;
        }

    }
}