using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.qtLib
{
    public class qtCoroutiner
    {
        #region ----- Variable -----

        private static GameObject _coroutineObject;
        private static CoroutineInstance _coroutineInstance;
        private static Dictionary<string, IEnumerator> _dictionaryRoutine;

        #endregion

        #region ----- Public Method -----

        /// <summary>
        /// Excute a function after a delay.
        /// </summary>
        public static void ExcuteAfterEndOfFrame(Action action)
        {
            _Initialize();

            _coroutineInstance.StartCoroutine(_DelayActionUntilEndOfFrame(action));
        }

        /// <summary>
        /// Excute a function after a delay.
        /// </summary>
        public static void ExcuteAfterDelayTime(float delayTime, Action action)
        {
            _Initialize();

            if (delayTime > 0)
            {
                _coroutineInstance.StartCoroutine(_DelayActionByTime(delayTime, action));
            }
            else
            {
                if (action != null)
                {
                    action();
                }
            }
        }

        /// <summary>
        /// Excute a function after a delay.
        /// </summary>
        public static void ExcuteAfterDelayTime(string key, float delayTime, Action action)
        {
            if (delayTime > 0)
            {
                Start(key, _DelayActionByTime(delayTime, action));
            }
            else
            {
                if (action != null)
                {
                    action();
                }
            }
        }

        /// <summary>
        /// Excute a function after a delay.
        /// </summary>
        public static void ExcuteAfterDelayFrame(int delayFrame, Action action)
        {
            _Initialize();

            if (delayFrame > 0)
            {
                _coroutineInstance.StartCoroutine(_DelayActionByFrame(delayFrame, action));
            }
            else
            {
                if (action != null)
                {
                    action();
                }
            }
        }

        /// <summary>
        /// Excute a function after a delay.
        /// </summary>
        public static void ExcuteAfterDelayFrame(string key, int delayFrame, Action action)
        {
            if (delayFrame > 0)
            {
                Start(key, _DelayActionByFrame(delayFrame, action));
            }
            else
            {
                if (action != null)
                {
                    action();
                }
            }
        }

        /// <summary>
        /// Start a coroutine.
        /// </summary>
        public static void Start(IEnumerator routine)
        {
            _Initialize();

            _coroutineInstance.StartCoroutine(routine);
        }

        /// <summary>
        /// Start a coroutine.
        /// </summary>
        public static void Start(string key, IEnumerator routine)
        {
            _Initialize();

            if (_dictionaryRoutine.ContainsKey(key))
            {
                if (_dictionaryRoutine[key] != null)
                {
                    _coroutineInstance.StopCoroutine(_dictionaryRoutine[key]);
                }
                _dictionaryRoutine.Remove(key);
            }

            _coroutineInstance.StartCoroutine(routine);
            _dictionaryRoutine.Add(key, routine);
        }

        /// <summary>
        /// Stop a coroutine.
        /// </summary>
        public static void Stop(string key)
        {
            _Initialize();

            if (_dictionaryRoutine.ContainsKey(key))
            {
                if (_dictionaryRoutine[key] != null)
                {
                    _coroutineInstance.StopCoroutine(_dictionaryRoutine[key]);
                }
                _dictionaryRoutine.Remove(key);
            }
        }

        /// <summary>
        /// Stop all coroutines.
        /// </summary>
        public static void StopAll()
        {
            _Initialize();

            _coroutineInstance.StopAllCoroutines();
        }

        #endregion

        #region ----- Private Method -----

        /// <summary>
        /// Initialize.
        /// </summary>
        private static void _Initialize()
        {
            // if (_coroutineObject == null)
            // {
            //     string objName = typeof(qtCoroutiner).Name;
            //
            //     _coroutineObject = qtHelper.FindObjectInRoot(objName);
            //
            //     if (_coroutineObject == null)
            //     {
            //         _coroutineObject = new GameObject(objName);
            //     }
            //
            //     GameObject.DontDestroyOnLoad(_coroutineObject);
            //
            //     _dictionaryRoutine = new Dictionary<string, IEnumerator>();
            // }

            if (_coroutineInstance == null)
            {
                _coroutineInstance = new GameObject("qtCoroutiner").AddComponent<CoroutineInstance>();
            }
        }

        /// <summary>
        /// Delay a action.
        /// </summary>
        private static IEnumerator _DelayActionUntilEndOfFrame(Action action)
        {
            yield return new WaitForEndOfFrame();

            if (action != null)
            {
                action();
            }
        }

        /// <summary>
        /// Delay a action.
        /// </summary>
        private static IEnumerator _DelayActionByTime(float delayTime, Action action)
        {
            yield return new WaitForSeconds(delayTime);

            if (action != null)
            {
                action();
            }
        }

        /// <summary>
        /// Delay a action.
        /// </summary>
        private static IEnumerator _DelayActionByFrame(int delayFrame, Action action)
        {
            for (int i = 0; i < delayFrame; i++)
            {
                yield return null;
            }

            if (action != null)
            {
                action();
            }
        }

        #endregion

        #region ----- Internal Class -----

        public class CoroutineInstance : MonoBehaviour
        {
        }

        #endregion
    }
}