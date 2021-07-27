using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DoTweenMovement.Scripts;

namespace DoTweenMovement.Editor
{
    [CustomEditor(typeof(DoTweenMove))]
    public class DoTweenMoveEditor : UnityEditor.Editor
    {
        private DoTweenMove myScript;

        private SerializedProperty closeDistance;
        private SerializedProperty speedType;
        private SerializedProperty speed;
        private SerializedProperty minValue;
        private SerializedProperty maxValue;

        private SerializedProperty pathPoints;

        private SerializedProperty showGizmosInPlayMode;
        private SerializedProperty pointGizmoColor;
        private SerializedProperty gizmoSize;
        private SerializedProperty gizmoType;
        private SerializedProperty pointMoveInEditor;

        private readonly GUILayoutOption minWidth = GUILayout.MinWidth(100);
        private readonly GUILayoutOption maxWidth = GUILayout.MaxWidth(200);

        AnimationCurve speedCurve = AnimationCurve.Linear(0, 0, 1, 1);
        
        private GUIStyle boldStyle = GUIStyle.none;
        GUIStyle numberStyle = new GUIStyle();

        private Camera _cam;


        private void OnEnable()
        {
            var cam = SceneView.GetAllSceneCameras();
            _cam = cam[0];
            myScript = (DoTweenMove) target;
            closeDistance = serializedObject.FindProperty(nameof(closeDistance));
            speedType = serializedObject.FindProperty(nameof(speedType));
            speed = serializedObject.FindProperty(nameof(speed));
            minValue = serializedObject.FindProperty(nameof(minValue));
            maxValue = serializedObject.FindProperty(nameof(maxValue));
            pathPoints = serializedObject.FindProperty(nameof(pathPoints));
            showGizmosInPlayMode = serializedObject.FindProperty(nameof(showGizmosInPlayMode));
            pointGizmoColor = serializedObject.FindProperty(nameof(pointGizmoColor));
            gizmoSize = serializedObject.FindProperty(nameof(gizmoSize));
            gizmoType = serializedObject.FindProperty(nameof(gizmoType));
            pointMoveInEditor = serializedObject.FindProperty(nameof(pointMoveInEditor));


            numberStyle.normal.textColor = Color.cyan;
            numberStyle.fontStyle = FontStyle.Bold;
            boldStyle.normal.textColor = Color.white;
            boldStyle.fontStyle = FontStyle.Bold;
        }



        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(closeDistance);
            EditorGUILayout.PropertyField(pathPoints);
            
            EditorGUILayout.Space(1);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(1);
            EditorGUILayout.LabelField("Speed", boldStyle);
            EditorGUILayout.PropertyField(speedType);
            switch (myScript.speedType)
            {
                case DoTweenMove.SpeedType.Constant:
                    EditorGUILayout.PropertyField(speed);
                    break;
                case DoTweenMove.SpeedType.Curve:
                    EditorGUILayout.PropertyField(minValue);
                    EditorGUILayout.PropertyField(maxValue);
                    EditorGUILayout.CurveField("Speed Curve", speedCurve);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorGUILayout.Space(1);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(1);
            //Reminder new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Bold}
            EditorGUILayout.LabelField("Point Editor", boldStyle);
            EditorGUILayout.PropertyField(showGizmosInPlayMode);
            EditorGUILayout.PropertyField(pointMoveInEditor);
            EditorGUILayout.PropertyField(gizmoSize);
            EditorGUILayout.PropertyField(gizmoType);
            EditorGUILayout.PropertyField(pointGizmoColor);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();


            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnSceneGUI()
        {
            myScript = (DoTweenMove) target;
            var position = myScript.transform.position;
            var myPathPoints = myScript.PathPoints;
            var pointCount = myScript.PathPoints.Count;
            Handles.color = myScript.PointGizmoColor;
            var fontSize = (int) myScript.GizmoSize * 10;
            if(fontSize > 25)
                numberStyle.fontSize = (int) myScript.GizmoSize * 10;
            else
                numberStyle.fontSize = 25;
            

            if (!myScript.ShowGizmosInPlayMode && Application.isPlaying) return;
           

            for (int i = 0; i < pointCount; i++)
            {
                EditorGUI.BeginChangeCheck();
                
                Vector3 oldPoint = position + myScript.PathPoints[i].posiiton;

                //Draw handles for points
                
                Handles.DrawWireDisc(
                    position + myScript.PathPoints[i].posiiton,
                    Vector3.forward, 
                    myScript.GizmoSize, myScript.GizmoSize);
                
                //Draw numbers for path points
                Handles.Label(
                    position + myScript.PathPoints[i].posiiton + (Vector3.down * 0.4f) +
                    (Vector3.right * 0.4f), "" + i, numberStyle);
                
                Vector3 newPoint = Vector3.zero;

                switch (myScript.PointMoveInEditor)
                {
                    case DoTweenMove.GizmoMovementType.Free:
                        //Draw free move Handle
                        newPoint = Handles.FreeMoveHandle(oldPoint, Quaternion.identity, .5f,
                            new Vector3(.25f, .25f, .25f), Handles.CircleHandleCap);
                        break;
                    case DoTweenMove.GizmoMovementType.Transform:
                        //Draw transform arrows
                        newPoint = Handles.PositionHandle(oldPoint, Quaternion.identity);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                // Records changes and undo them
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Free Move Handle");
                    myScript.PathPoints[i].posiiton = newPoint - myScript.transform.position;
                }
            }
            
            //Draw lines for each point
            Handles.color = Color.white;
            if (pointCount > 1)
            {
                for (int i = 0; i < pointCount; i++)
                {
                    if(i + 1 != pointCount)
                        Handles.DrawLine(position + myPathPoints[i].posiiton, position + myPathPoints[i+1].posiiton, myScript.GizmoSize);
                    else
                        Handles.DrawLine(position + myPathPoints[i].posiiton, position + myPathPoints[0].posiiton, myScript.GizmoSize);
                } 
            }
            else
                Handles.DrawLine(position, position + myPathPoints[0].posiiton, myScript.GizmoSize);
        }
    }
}