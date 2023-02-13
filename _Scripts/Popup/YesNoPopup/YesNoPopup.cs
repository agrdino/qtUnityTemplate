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
        protected override void OnEnable()
        {
            base.OnEnable();
            btnYes.onClick.AddListener(OnButtonYesClick);
            btnNo.onClick.AddListener(OnButtonNoClick);
        }

        public YesNoPopup Initialize(string title, string content, Action yes)
        {
            txtTitle.text = title;
            _evtYes = yes;
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