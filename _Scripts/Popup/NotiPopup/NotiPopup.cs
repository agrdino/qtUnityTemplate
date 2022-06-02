using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Prefab.Popup.NotiPopup
{
    public class NotiPopup : popBase
    {
        [SerializeField] private Button btnOk;
        private Action _evtConfirm;
        public NotiPopup Initialize(string content, Action action = null)
        {
            _evtConfirm = action;
            txtContent.text = content;
            return this;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            btnOk.onClick.AddListener(OnButtonConfirmClick);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            btnOk.onClick.RemoveAllListeners();
        }

        private void OnButtonConfirmClick()
        {
            _evtConfirm?.Invoke();
            Hide();
        }
    }
}