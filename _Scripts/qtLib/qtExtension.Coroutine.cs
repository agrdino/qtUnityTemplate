using System;
using System.Collections;
using UnityEngine;

namespace _Scripts.qtLib
{
    public partial class qtExtension
    {
        public void ExecuteAfter(float time, Action action)
        {
        }
        
        private static IEnumerator Execute(float time, Action action)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }
    }
}