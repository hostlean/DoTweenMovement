using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DoTweenMovement.Scripts;

namespace DoTweenMovement.Editor
{
    [CustomEditor(typeof(DTMovement))]
    public class DoTweenMovementEditor : UnityEditor.Editor
    {
        private DTMovement myScript;

        private SerializedProperty closeDistance;
        private SerializedProperty speed;

        private SerializedProperty pathPoints;

        private SerializedProperty gizmoColor;
        private SerializedProperty gizmoSize;
        private SerializedProperty gizmoType;
        
        private readonly GUILayoutOption minWidth = GUILayout.MinWidth(100);
        private readonly GUILayoutOption maxWidth = GUILayout.MaxWidth(200);
        

        private void OnEnable()
        {
            myScript = (DTMovement) target;
            closeDistance = serializedObject.FindProperty(nameof(closeDistance));
            speed = serializedObject.FindProperty(nameof(speed));
            pathPoints = serializedObject.FindProperty(nameof(pathPoints));
            gizmoColor = serializedObject.FindProperty(nameof(gizmoColor));
            gizmoSize = serializedObject.FindProperty(nameof(gizmoSize));
            gizmoType = serializedObject.FindProperty(nameof(gizmoType));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(closeDistance);
            EditorGUILayout.PropertyField(speed);
            EditorGUILayout.PropertyField(pathPoints);
            
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("Gizmos");
            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(gizmoSize);
            EditorGUILayout.PropertyField(gizmoType);

            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            EditorGUILayout.PropertyField(gizmoColor, minWidth, maxWidth);
            EditorGUILayout.PropertyField(gizmoColor, minWidth, maxWidth);
            //EditorGUILayout.PropertyField(gizmoColor);

            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}