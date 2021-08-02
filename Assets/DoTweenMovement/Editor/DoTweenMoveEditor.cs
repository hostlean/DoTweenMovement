using System;
using System.Collections.Generic;
using System.Linq;
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
        private SerializedProperty followType;
        private SerializedProperty speed;
        private SerializedProperty minValue;
        private SerializedProperty maxValue;

        private SerializedProperty pathPoints;

        private SerializedProperty showGizmosInPlayMode;
        private SerializedProperty pointGizmoColor;
        private SerializedProperty gizmoSize;
        private SerializedProperty pointMoveInEditor;

        private readonly GUILayoutOption minWidth = GUILayout.MinWidth(100);
        private readonly GUILayoutOption maxWidth = GUILayout.MaxWidth(200);

        private AnimationCurve _speedCurve = AnimationCurve.Linear(0, 0, 1, 1);
        
        private GUIStyle _boldStyle = GUIStyle.none;
        private GUIStyle _numberStyle = new GUIStyle();

        private bool isDirty = false;
        private Vector3 MyPosition;
        private int CurrentPositionCount { get; set; }
        private int OldPositionCount { get; set; }
        


        private void OnEnable()
        {
            myScript = (DoTweenMove) target;
            closeDistance = serializedObject.FindProperty(nameof(closeDistance));
            speedType = serializedObject.FindProperty(nameof(speedType));
            followType = serializedObject.FindProperty(nameof(followType));
            speed = serializedObject.FindProperty(nameof(speed));
            minValue = serializedObject.FindProperty(nameof(minValue));
            maxValue = serializedObject.FindProperty(nameof(maxValue));
            pathPoints = serializedObject.FindProperty(nameof(pathPoints));
            showGizmosInPlayMode = serializedObject.FindProperty(nameof(showGizmosInPlayMode));
            pointGizmoColor = serializedObject.FindProperty(nameof(pointGizmoColor));
            gizmoSize = serializedObject.FindProperty(nameof(gizmoSize));
            pointMoveInEditor = serializedObject.FindProperty(nameof(pointMoveInEditor));


            _numberStyle.normal.textColor = Color.cyan;
            _numberStyle.fontStyle = FontStyle.Bold;
            _boldStyle.normal.textColor = Color.white;
            _boldStyle.fontStyle = FontStyle.Bold;

            OldPositionCount = myScript.PathCount;
            if(myScript.PointGizmoColor.a == 0)
                myScript.PointGizmoColor = Color.yellow;
        }



        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            CurrentPositionCount = myScript.PathCount;
            
            EditorGUILayout.PropertyField(closeDistance);
            EditorGUILayout.PropertyField(followType);
            EditorGUILayout.PropertyField(pathPoints);
            
            EditorGUILayout.Space(1);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(1);
            EditorGUILayout.LabelField("Speed", _boldStyle);
            EditorGUILayout.PropertyField(speedType);
            switch (myScript.speedType)
            {
                case DoTweenMove.SpeedType.Constant:
                    EditorGUILayout.PropertyField(speed);
                    break;
                case DoTweenMove.SpeedType.Curve:
                    EditorGUILayout.PropertyField(minValue);
                    EditorGUILayout.PropertyField(maxValue);
                    EditorGUILayout.CurveField("Speed Curve", _speedCurve);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorGUILayout.Space(1);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(1);
            //Reminder new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Bold}
            EditorGUILayout.LabelField("Point Editor", _boldStyle);
            EditorGUILayout.PropertyField(showGizmosInPlayMode);
            EditorGUILayout.PropertyField(pointMoveInEditor);
            EditorGUILayout.PropertyField(gizmoSize);
            EditorGUILayout.PropertyField(pointGizmoColor);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();


            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();

            
            var positionList = myScript.Positions;
            
            if(myScript.Positions != null)
                isDirty = myScript.Positions.Any(p => positionList.Any(a => a == p));
            
            if(OldPositionCount != CurrentPositionCount || isDirty)
                EditorUtility.SetDirty(myScript.gameObject);
            
        }

        protected virtual void OnSceneGUI()
        {
            //myScript = (DoTweenMove) target;
            if (myScript.PathPoints == null) return;
            if(!Application.isPlaying)
                MyPosition = myScript.transform.position;
            var myPathPoints = myScript.PathPoints;
            var pointCount = myScript.PathPoints.Count;
            Handles.color = myScript.PointGizmoColor;
            var fontSize = (int) myScript.GizmoSize * 10;
            if(fontSize > 25)
                _numberStyle.fontSize = (int) myScript.GizmoSize * 10;
            else
                _numberStyle.fontSize = 25;
            

            if (!myScript.ShowGizmosInPlayMode && Application.isPlaying) return;
            
            for (int i = 0; i < pointCount; i++)
            {
                EditorGUI.BeginChangeCheck();
                
                Vector3 oldPoint = MyPosition + myScript.PathPoints[i].posiiton;

                //Draw handles for points
                
                Handles.DrawWireDisc(
                    MyPosition + myScript.PathPoints[i].posiiton,
                    Vector3.forward, 
                    myScript.GizmoSize, myScript.GizmoSize);
                
                //Draw numbers for path points
                Handles.Label(
                    MyPosition + myScript.PathPoints[i].posiiton + (Vector3.down * 0.4f) +
                    (Vector3.right * 0.4f), "" + i, _numberStyle);
                
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
                        Handles.DrawLine(MyPosition + myPathPoints[i].posiiton, MyPosition + myPathPoints[i+1].posiiton, myScript.GizmoSize);
                    else if(myScript.followType == DoTweenMove.FollowType.Loop)
                        Handles.DrawLine(MyPosition + myPathPoints[i].posiiton, MyPosition + myPathPoints[0].posiiton, myScript.GizmoSize);
                } 
            }
            else
                Handles.DrawLine(MyPosition, MyPosition + myPathPoints[0].posiiton, myScript.GizmoSize);
        }
    }
}