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
        private SerializedProperty sampleRate;

        private SerializedProperty pathPoints;
        private SerializedProperty startPoint;

        private SerializedProperty showGizmosInPlayMode;
        private SerializedProperty pointGizmoColor;
        private SerializedProperty gizmoSize;
        private SerializedProperty pointMoveInEditor;
        private SerializedProperty setStartPoint;
        private SerializedProperty speedCurve;
        private SerializedProperty testSpeed;
        private SerializedProperty calculatingCurve;

        private readonly GUILayoutOption minWidth = GUILayout.MinWidth(100);
        private readonly GUILayoutOption maxWidth = GUILayout.MaxWidth(200);

        //private readonly AnimationCurve speedCurve = AnimationCurve.Linear(0, 0, 1, 1);
        
        private readonly GUIStyle boldStyle = GUIStyle.none;
        private readonly GUIStyle numberStyle = new GUIStyle();

        private bool isDirty = false;
        private Vector3 myPosition;
        private Vector3 playPosition;
        private int selectedPathIndex;
        private int oldPathIndex;
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
            sampleRate = serializedObject.FindProperty(nameof(sampleRate));
            pathPoints = serializedObject.FindProperty(nameof(pathPoints));
            showGizmosInPlayMode = serializedObject.FindProperty(nameof(showGizmosInPlayMode));
            pointGizmoColor = serializedObject.FindProperty(nameof(pointGizmoColor));
            gizmoSize = serializedObject.FindProperty(nameof(gizmoSize));
            pointMoveInEditor = serializedObject.FindProperty(nameof(pointMoveInEditor));
            startPoint = serializedObject.FindProperty(nameof(startPoint));
            setStartPoint = serializedObject.FindProperty(nameof(setStartPoint));
            speedCurve = serializedObject.FindProperty(nameof(speedCurve));
            testSpeed = serializedObject.FindProperty(nameof(testSpeed));
            calculatingCurve = serializedObject.FindProperty(nameof(calculatingCurve));


            numberStyle.normal.textColor = Color.cyan;
            numberStyle.fontStyle = FontStyle.Bold;
            boldStyle.normal.textColor = Color.white;
            boldStyle.fontStyle = FontStyle.Bold;

            if(myScript.PathPoints != null)
                OldPositionCount = myScript.PathCount;
            if(myScript.PointGizmoColor.a == 0)
                myScript.PointGizmoColor = Color.yellow;
        }

        private int[] GetPathIndex(int length)
        {
            int[] indexes = new int[length];
            for (int i = 0; i < length; i++)
            {
                indexes[i] = i;
            }

            return indexes;
        }

        private string[] GetPathNames(int length)
        {
            string[] pathNames = new string[length];
            for (int i = 0; i < length; i++)
            {
                pathNames[i] = $"Position {myScript.PathPoints[i].index}";
            }

            return pathNames;
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            

            CurrentPositionCount = myScript.PathCount;
            
            EditorGUILayout.PropertyField(closeDistance);
            EditorGUILayout.PropertyField(followType);
            EditorGUILayout.PropertyField(setStartPoint);

            if (myScript.SetStartPoint)
            {
                var length = myScript.PathCount;
                string[] dataNames = GetPathNames(length);
                int[] dataIndex = GetPathIndex(length);


                selectedPathIndex = EditorGUILayout.IntPopup("Start Point", startPoint.intValue, dataNames, dataIndex);
                startPoint.intValue = selectedPathIndex;

            }
          





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
                    EditorGUILayout.PropertyField(sampleRate);
                    EditorGUILayout.PropertyField(speedCurve);
                    EditorGUILayout.PropertyField(calculatingCurve);
                    //EditorGUILayout.CurveField("Speed Curve", speedCurve);
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
            if (myScript.PathPoints == null || myScript.PathPoints.Count == 0) return;
            if (!Application.isPlaying)
            {
                playPosition = myScript.transform.position;
                myPosition = playPosition;
            }
            else
            {
                myPosition = myScript.AwakePos;
            }

            
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
                
                Vector3 oldPoint = myPosition + myScript.PathPoints[i].position;

                myScript.PathPoints[i].index = i + 1;

                //Draw handles for path points
                Handles.DrawWireDisc(
                    myPosition + myScript.PathPoints[i].position,
                    Vector3.forward, 
                    myScript.GizmoSize, myScript.GizmoSize);

                if (Application.isPlaying)
                {
                    Handles.Label(myScript.transform.position + Vector3.down, $"{testSpeed.floatValue}", numberStyle);
                }
                
                
                //Draw numbers for path points
                Handles.Label(
                    myPosition + myScript.PathPoints[i].position + (Vector3.down * 0.4f) +
                    (Vector3.right * 0.4f), "" + (i+1), numberStyle);
                
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
                    myScript.PathPoints[i].position = newPoint - myScript.transform.position;
                }
            }
            
            //Draw lines for each point
            Handles.color = Color.white;
            if (pointCount > 1)
            {
                for (int i = 0; i < pointCount; i++)
                {
                    if(i + 1 != pointCount)
                        Handles.DrawLine(myPosition + myPathPoints[i].position, myPosition + myPathPoints[i+1].position, myScript.GizmoSize);
                    else if(myScript.followType == DoTweenMove.FollowType.Loop)
                        Handles.DrawLine(myPosition + myPathPoints[i].position, myPosition + myPathPoints[0].position, myScript.GizmoSize);
                } 
            }
            else
                Handles.DrawLine(myPosition, myPosition + myPathPoints[0].position, myScript.GizmoSize);
        }
    }
}