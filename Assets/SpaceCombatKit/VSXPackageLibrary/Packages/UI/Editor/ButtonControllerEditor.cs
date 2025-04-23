using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace VSX.UI
{
    [CustomEditor(typeof(ButtonController))]
    public class ButtonControllerEditor : UnityEditor.UI.ButtonEditor
    {
        protected SerializedProperty imagesProperty;
        protected SerializedProperty textsProperty;
        protected SerializedProperty selectManuallyProperty;
        protected SerializedProperty resetStateOnEnableProperty;

        protected SerializedProperty normalStateProperty;
        protected SerializedProperty highlightedStateProperty;
        protected SerializedProperty selectedStateProperty;
        protected SerializedProperty deepSelectedStateProperty;

        protected SerializedProperty transitionDurationProperty;
        protected SerializedProperty transitionCurveProperty;


        protected override void OnEnable()
        {
            base.OnEnable();
            
            imagesProperty = serializedObject.FindProperty("images");
            textsProperty = serializedObject.FindProperty("texts");
            selectManuallyProperty = serializedObject.FindProperty("selectManually");
            resetStateOnEnableProperty = serializedObject.FindProperty("resetStateOnEnable");

            normalStateProperty = serializedObject.FindProperty("normalState");
            highlightedStateProperty = serializedObject.FindProperty("highlightedState");
            selectedStateProperty = serializedObject.FindProperty("selectedState");
            deepSelectedStateProperty = serializedObject.FindProperty("deepSelectedState");

            transitionDurationProperty = serializedObject.FindProperty("transitionDuration");
            transitionCurveProperty = serializedObject.FindProperty("transitionCurve");
            
            
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(imagesProperty);
            EditorGUILayout.PropertyField(textsProperty);
            EditorGUILayout.PropertyField(selectManuallyProperty);
            EditorGUILayout.PropertyField(resetStateOnEnableProperty);

            EditorGUILayout.PropertyField(normalStateProperty);
            EditorGUILayout.PropertyField(highlightedStateProperty);
            EditorGUILayout.PropertyField(selectedStateProperty);
            EditorGUILayout.PropertyField(deepSelectedStateProperty);

            EditorGUILayout.PropertyField(transitionDurationProperty);
            EditorGUILayout.PropertyField(transitionCurveProperty);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Default Button Properties", EditorStyles.boldLabel);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();

        }
    }
}
