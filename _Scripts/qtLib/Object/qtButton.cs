using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.qtLib
{
    public class qtButton : Button
    {
        public Text text;
        public TextMeshProUGUI tmpText;
        
        protected override void Reset()
        {
            base.Reset();
            text = GetComponentInChildren<Text>(true);
            tmpText = GetComponentInChildren<TextMeshProUGUI>(true);
        }
    }
}