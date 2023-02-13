using UnityEditor;
using UnityEngine;

namespace _Scripts.qtLib
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(qtButton))]
    [CanEditMultipleObjects]
    public class qtButtonEditor : Editor
    {
        private qtButton qtButton;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            qtButton = target as qtButton;

            serializedObject.Update();
            switch (qtButton.TransitionType)
            {
                case BtnTransitionType.Image:
                    SerializedProperty selectImage = serializedObject.FindProperty("selectImage");
                    selectImage.objectReferenceValue = EditorGUILayout.ObjectField("selectImage", qtButton.selectImage,typeof(Sprite), true) as Sprite;

                    SerializedProperty norImage = serializedObject.FindProperty("norImage");
                    norImage.objectReferenceValue = EditorGUILayout.ObjectField("norImage", qtButton.norImage,typeof(Sprite), true) as Sprite;

                    SerializedProperty disableImage = serializedObject.FindProperty("disableImage");
                    disableImage.objectReferenceValue = EditorGUILayout.ObjectField("disableImage", qtButton.disableImage,typeof(Sprite), true) as Sprite;

                    SerializedProperty isUsedImage = serializedObject.FindProperty("isUsedImage");
                    isUsedImage.objectReferenceValue = EditorGUILayout.ObjectField("isUsedImage", qtButton.isUsedImage,typeof(Sprite), true) as Sprite;
                    
                    SerializedProperty textSelectColor = serializedObject.FindProperty("textSelectColor");
                    textSelectColor.colorValue = EditorGUILayout.ColorField("textSelectColor", qtButton.textSelectColor);
                    
                    SerializedProperty textDisableColor = serializedObject.FindProperty("textDisableColor");
                    textDisableColor.colorValue = EditorGUILayout.ColorField("textDisableColor", qtButton.textDisableColor);

                    break;
                case BtnTransitionType.Color:
                    SerializedProperty selectColor = serializedObject.FindProperty("selectColor");
                    selectColor.colorValue = EditorGUILayout.ColorField("selectColor", qtButton.selectColor);
                    
                    SerializedProperty norColor = serializedObject.FindProperty("norColor");
                    norColor.colorValue = EditorGUILayout.ColorField("norColor", qtButton.norColor);

                    SerializedProperty disableColor = serializedObject.FindProperty("disableColor");
                    disableColor.colorValue = EditorGUILayout.ColorField("disableColor", qtButton.disableColor);
                    
                    SerializedProperty isUsedColor = serializedObject.FindProperty("isUsedColor");
                    isUsedColor.colorValue = EditorGUILayout.ColorField("isUsedColor", qtButton.isUsedColor);

                    
                    textSelectColor = serializedObject.FindProperty("textSelectColor");
                    textSelectColor.colorValue = EditorGUILayout.ColorField("textSelectColor", qtButton.textSelectColor);
                    
                    textDisableColor = serializedObject.FindProperty("textDisableColor");
                    textDisableColor.colorValue = EditorGUILayout.ColorField("textDisableColor", qtButton.textDisableColor);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}