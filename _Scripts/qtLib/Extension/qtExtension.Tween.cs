using System.Collections;
using System.Collections.Generic;
using _Scripts.Scene;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.qtLib.Extension
{
    public partial class qtExtension
    {
        public static IEnumerator<T> WaitForCompletion<T>(this Tween tween) where T: Component
        {
            if (tween.IsActive() && !tween.IsComplete())
            {
                yield return null;
            }
        }
    }
}