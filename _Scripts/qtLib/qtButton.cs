using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.qtLib
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class qtButton : Button
    {
        [HideInInspector] public Button button;

        [HideInInspector] public bool scale;
        
        [HideInInspector] public Sprite selectedSprite;
        [HideInInspector] public Sprite normalSprite;
        [HideInInspector] public bool changeSprite;

        [HideInInspector] public bool showObj;
        [HideInInspector] public GameObject selectedObj;
        protected override void OnEnable()
        {
            base.OnEnable();
            button = GetComponent<Button>();
            normalSprite = button.image.sprite;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (scale)
            {
                transform.DOKill();
                transform.DOScale(1.2f * Vector3.one, 0.25f);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (scale)
            {
                transform.DOKill();
                transform.DOScale(Vector3.one, 0.25f);
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
        }

        public void ResetState()
        {
            button.image.sprite = normalSprite;
            if (selectedObj != null)
            {
                selectedObj.SetActive(false);
            }
        }

        public void Selected()
        {
            if (changeSprite)
            {
                button.image.sprite = selectedSprite;
            }

            if (showObj)
            {
                selectedObj.SetActive(true);
            }
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(qtButton))]
    class qtButtonEditor : Editor
    {
        private qtButton _btn;
        
        public override void OnInspectorGUI()
        {
            _btn = (qtButton)target;
            
            _btn.scale = EditorGUILayout.Toggle("Scale: ", _btn.scale);
            
            _btn.changeSprite = EditorGUILayout.Toggle("Change sprite: ", _btn.changeSprite);
            if (_btn.changeSprite)
            {
                _btn.selectedSprite = (Sprite)EditorGUILayout.ObjectField("Selected: ", _btn.selectedSprite, typeof(Sprite));
            }
            
            _btn.showObj = EditorGUILayout.Toggle("Show obj: ", _btn.showObj);
            if (_btn.showObj)
            {
                _btn.selectedObj = (GameObject)EditorGUILayout.ObjectField("Selected: ", _btn.selectedObj, typeof(GameObject), true);
            }

            GUILayout.Space(20);
            base.OnInspectorGUI();

            EditorUtility.SetDirty(target);
        }
    }
#endif
}