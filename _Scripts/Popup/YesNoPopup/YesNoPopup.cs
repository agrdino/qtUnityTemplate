using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Prefab.Popup.YesNoPopup
{
    public class YesNoPopup : popBase
    {
        [SerializeField] private Button btnYes;
        [SerializeField] private Button btnNo;
    
        private Action _evtYes;
        private Action _evtNo;
        protected override void OnEnable()
        {
            base.OnEnable();
            btnYes.onClick.AddListener(OnButtonYesClick);
            btnNo.onClick.AddListener(OnButtonNoClick);
        }

        public YesNoPopup Initialize(string content, Action yes, Action no = null)
        {
            _evtYes = yes;
            _evtNo = no;
            txtContent.text = content;
            return this;
        }
    

        private void OnButtonYesClick()
        {
            _evtYes?.Invoke();
            Hide();
        }

        private void OnButtonNoClick()
        {
            _evtNo.Invoke();
            Hide();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            btnYes.onClick.RemoveAllListeners();
            btnNo.onClick.RemoveAllListeners();
        }
    }
}