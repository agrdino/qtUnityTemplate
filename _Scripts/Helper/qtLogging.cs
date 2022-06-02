using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace _Scripts.Helper
{
    public class qtLogging : MonoBehaviour
    {
        private static Text _prefab;
        private static Transform _content;

        private void OnEnable()
        {
            _prefab = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
            _content = transform.GetChild(0).GetChild(0).GetChild(0);
        }

        public static void Log(Object message, bool show = true)
        {
            Debug.Log($"{"[LOG]:",-17}{message}");
            ShowLog($"{"[LOG]:",-17}{message}", show);
        }

        public static void LogWarning(Object message, bool show = true)
        {
            Debug.Log($"{"<color=#FFFF00>[WARNING]</color>:",-9}{message}");
            ShowLog($"{"<color=#FFFF00>[WARNING]</color>:",-9}{message}", show);
        }

        public static void LogError(Object message, bool show = true)
        {
            Debug.Log($"{"<color=#FF0000>[ERROR]</color>:",-11}{message}");
            ShowLog($"{"<color=#FF0000>[ERROR]</color>:",-11}{message}", show);
        }

        private static void ShowLog(string msg, bool show = true)
        {
            return;
        
            if (!show)
            {
                return;
            }
            var temp = Instantiate(_prefab.gameObject, _content).GetComponent<Text>();
            temp.text = msg;
            temp.transform.SetSiblingIndex(0);
            temp.gameObject.SetActive(true);
        }
    }
}
