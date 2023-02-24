using System.Collections;
using _Scripts.qtLib;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.Scene.MenuScene
{
    public class MenuSceneView : sceneBase
    {
        #region ----- VARIABLE -----

        [SerializeField] private qtButton _btnTop;
        [SerializeField] private qtButton _btnBot;
        
        #endregion
        
        #region ----- INITIALIZE -----

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void InitEvent()
        {
        }
        
        public override void InitObject()
        {
        }

        public override IEnumerator OnExit()
        {
            yield break;
        }

        #endregion

        #region ----- ANIMATION -----

        public override void SetUpAnim()
        {
            base.SetUpAnim();
            _btnBot.transform.localScale = Vector3.zero;
            _btnTop.transform.localScale = Vector3.zero;
        }

        public override Tween Show()
        {
            return _btnBot.transform.DOScale(Vector3.one, 3).OnStart(() =>
            {
                _btnTop.transform.DOScale(Vector3.one, 3);
            });
        }

        #endregion

        #region ----- BUTTON EVENT -----

        #endregion

        #region ----- PUBLIC FUNCTION -----

        #endregion

        #region ----- PRIVATE FUNCTION -----

        #endregion
    }
}
