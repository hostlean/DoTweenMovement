using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace DoTweenMovement.Scripts
{
    public class DoTweenMove : MonoBehaviour
    {
        [SerializeField] private float closeDistance = 0.1f;
        [SerializeField] private float speed = 1.0f;
        [SerializeField] private float minValue = 0.0f;
        [SerializeField] private float maxValue = 1.0f;
        [SerializeField] private int sampleRate = 10;

        public SpeedType speedType;
        public PathType pathType;
        public FollowType followType;


        [SerializeField] private List<PathPoint> pathPoints;
        [SerializeField] private bool showGizmosInPlayMode;
        [SerializeField] private Color pointGizmoColor;
        [SerializeField, Range(.1f, 10.0f)] private float gizmoSize = 1.0f;
        [SerializeField] private GizmoMovementType pointMoveInEditor;
        [SerializeField] private int startPoint;
        [SerializeField] private bool setStartPoint;
        [SerializeField] private AnimationCurve speedCurve = 
            AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField] private AnimationCurve calculatingCurve;

        private readonly List<Vector3> positions = new List<Vector3>();
        private int positionCount;
        private int currentIndex;
        private bool goBack;
        private float alteredSpeed;
        private float increasingTime;
        private Vector3 awakePos;
        [HideInInspector] public float testSpeed;
        

        //Properties
        public GizmoMovementType PointMoveInEditor => pointMoveInEditor;
        public List<Vector3> Positions => positions;
        public List<PathPoint> PathPoints => pathPoints;
        public int PositionCount => positionCount;
        public int PathCount => pathPoints.Count;
        public float GizmoSize => gizmoSize;
        public bool SetStartPoint => setStartPoint;
        public Vector3 AwakePos => awakePos;
        public Color PointGizmoColor
        {
            get => pointGizmoColor;
            set => pointGizmoColor = value;
        }
        public int StartPoint
        {
            get => startPoint;
            set => startPoint = value;
        }

        public bool ShowGizmosInPlayMode => showGizmosInPlayMode;

        public enum FollowType
        {
            Loop,
            BackAndForth
        }
        
        public enum SpeedType
        {
            Constant,
            Curve
        }
        
        public enum PathType
        {
            Line,
            Bezier
        }

        public enum GizmoMovementType
        {
            Free,
            Transform
        }


        private float totalTime;
        
        private void Awake()
        {
            awakePos = transform.position;
            for (int i = 0; i < pathPoints.Count; i++)
            {
                positions.Add(transform.position + pathPoints[i].position);
            }

            positionCount = positions.Count;
           

            if (setStartPoint)
            {
                currentIndex = startPoint;
                transform.position = positions[startPoint];
            }
               
            
            
            
            calculatingCurve = speedCurve;
            alteredSpeed = speed;
        }

        private void FixedUpdate()
        {
            switch (pathType)
            {
                case PathType.Line:
                    MoveWithLine();
                    break;
                case PathType.Bezier:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
           
        }

        private void MoveWithLine()
        {
            if (speedType == SpeedType.Curve)
            {
                alteredSpeed = speedCurve.Evaluate(increasingTime) * (maxValue - minValue) + minValue;
                    //calculatingCurve.Evaluate(increasingTime);
                    //speedCurve.Evaluate(increasingTime) * (maxValue - minValue) + minValue;
                increasingTime += Time.fixedDeltaTime / totalTime;
            }
            else
            {
                alteredSpeed = speed;
            }

            testSpeed = alteredSpeed;
            transform.position =
                Vector3.MoveTowards(
                    transform.position, 
                    positions[currentIndex], 
                    Time.fixedDeltaTime * alteredSpeed);
            
            if (Vector3.Distance(transform.position, positions[currentIndex]) < closeDistance)
            {
                var nextIndex = currentIndex + 1;
                var previousIndex = currentIndex - 1;
                ChangeIndexByFollowType(nextIndex, previousIndex);
              
            }
        }

        private void ChangeIndexByFollowType(int nextIndex, int previousIndex)
        {
            switch (followType)
            {
                case FollowType.Loop:
                    CalculateLoopIndex(nextIndex);
                    break;
                case FollowType.BackAndForth:
                    CalculateBackAndForthIndex(nextIndex, previousIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CalculateLoopIndex(int nextIndex)
        {
            var oldIndex = currentIndex;
            currentIndex = nextIndex == positionCount ? 0 : nextIndex;
            if (speedType == SpeedType.Curve)
            {
                CalculateTimeForPathPoints(
                    positions[oldIndex],
                    positions[currentIndex],
                    sampleRate);
            }
        }

        private void CalculateBackAndForthIndex(int nextIndex, int previousIndex)
        {
            var oldIndex = currentIndex;
            if (goBack)
            {
                currentIndex = previousIndex == -1 ? nextIndex : previousIndex;
                if (currentIndex == nextIndex)
                    goBack = false;
            }
            else
            {
                currentIndex = nextIndex == positionCount ? previousIndex : nextIndex;
                if (currentIndex == previousIndex)
                    goBack = true;
            }
            
            if (speedType == SpeedType.Curve)
            {
                CalculateTimeForPathPoints(
                    positions[oldIndex],
                    positions[currentIndex],
                    sampleRate);
            }
        }

        private void CalculateTimeForPathPoints(Vector3 firstPoint, Vector3 secondPoint, int sampleRateParam)
        {
            increasingTime = 0;
            var distance = Vector3.Distance(firstPoint, secondPoint);
            float totalSpeedValue = 0;
            for (int i = 0; i < sampleRateParam+1; i++)
            {
                var sampleValue = 
                    speedCurve.Evaluate((float)i/(sampleRateParam)) * 
                    (maxValue - minValue) + minValue;
                
                totalSpeedValue += sampleValue;
            }

            var time = distance / (totalSpeedValue / (sampleRateParam));
            totalTime = time;

            //calculatingCurve = CreateNewSpeedCurve(time);

            // for (int i = 0; i < speedCurve.keys.Length; i++)
            // {
            //     speedCurve.keys[i].time *= time;
            // }


        }

        private AnimationCurve CreateNewSpeedCurve(float timeValue)
        {
            AnimationCurve animationCurve = new AnimationCurve();
            Keyframe[] newKeys = new Keyframe[speedCurve.length];
           //animationCurve = speedCurve;
            for (int i = 0; i < speedCurve.length; i++)
            {
                // var diff = maxValue - minValue;
                // float value = minValue + speedCurve.keys[i].value * diff;
                // float time = speedCurve.keys[i].time * timeValue;
                //animationCurve.MoveKey(i, new Keyframe(time, value));
                // float inTangent = speedCurve.keys[i].inTangent * (timeValue / 2) + 
                //                   (speedCurve.keys[i].inTangent * (minValue) + speedCurve.keys[i].inTangent * (diff));
                // float outTangent = speedCurve.keys[i].outTangent * (timeValue / 2) + 
                //                    (speedCurve.keys[i].outTangent * (minValue) + speedCurve.keys[i].outTangent * (diff));
                // float inWeight = speedCurve.keys[i].inWeight * (timeValue / 2);
                // float outWeight = speedCurve.keys[i].outWeight * (timeValue / 2);
                //                                   
                // newKeys[i] = new Keyframe(time, value, 
                //     speedCurve.keys[i].inTangent, 
                //     speedCurve.keys[i].outTangent,
                //     inWeight,
                //     outWeight);
                // animationCurve.AddKey(newKeys[i]);
                //animationCurve.MoveKey(i)
                // animationCurve.keys[i].value = (animationCurve.keys[i].value * (maxValue - minValue) + minValue);
                // animationCurve.keys[i].time = animationCurve.keys[i].time * timeValue;

            }

            return animationCurve;
        }
        
        
    }

    [Serializable]
    public class PathPoint
    {
        public int index;
        public DoTweenMove doTweenMove;
        public Vector3 position = Vector3.zero;
        public float closeDistance = .1f;

    }
}
