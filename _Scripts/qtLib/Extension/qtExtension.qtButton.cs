using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Scripts.qtLib.Extension
{
    public static partial class qtExtension
    {
        public static void AddListener(this qtButton button, UnityAction action)
        {
            button.onClick.RemoveListener(action);
            button.onClick.AddListener(action);
        }
    }
}