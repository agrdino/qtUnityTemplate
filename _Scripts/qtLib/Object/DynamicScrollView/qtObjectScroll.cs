using System;
using UnityEngine;

namespace _Scripts.qtLib
{
    [DisallowMultipleComponent]
    public abstract class qtObjectScroll : MonoBehaviour
    {
        public enum ObjectScrollState
        {
            InView,
            Cache,
        }

        public ObjectScrollState state;

        public int index;
        public RectTransform rect;
        public virtual qtObjectScroll Initialize(qtObjectModelScroll target, int index, Vector3 position)
        {
            rect = GetComponent<RectTransform>();
            this.index = index;
            return this;
        }
    }
}
