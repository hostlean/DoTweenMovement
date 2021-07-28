using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoTweenMovement.Scripts
{
    public class DoTweenMove : MonoBehaviour
    {
        [SerializeField] private float closeDistance = 0.1f;
        [SerializeField] private float speed = 1.0f;
        [SerializeField] private float minValue = 0.0f;
        [SerializeField] private float maxValue = 1.0f;

        public SpeedType speedType;
        public PathType pathType;


        [SerializeField] private List<PathPoint> pathPoints;

        [SerializeField] private bool showGizmosInPlayMode;
        [SerializeField] private Color pointGizmoColor;
        [SerializeField, Range(.1f, 10.0f)] private float gizmoSize = 1.0f;
        [SerializeField] private GizmoMovementType pointMoveInEditor;

        public GizmoMovementType PointMoveInEditor => pointMoveInEditor;

        private List<Vector3> _positions = new List<Vector3>();

        private int positionCount;
        private int _currentIndex;

        //Properties
        public List<PathPoint> PathPoints => pathPoints;
        public float GizmoSize => gizmoSize;
        public Color PointGizmoColor => pointGizmoColor;
        public bool ShowGizmosInPlayMode => showGizmosInPlayMode;

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

        private void Awake()
        {
            for (int i = 0; i < pathPoints.Count; i++)
            {
                _positions.Add(transform.position + pathPoints[i].posiiton);
            }
            
            positionCount = _positions.Count;
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
            transform.position =
                Vector3.MoveTowards(
                    transform.position, 
                    _positions[_currentIndex], 
                    Time.fixedDeltaTime * speed);

            if (Vector3.Distance(transform.position, _positions[_currentIndex]) < closeDistance)
            {
                var nextIndex = _currentIndex + 1;
                _currentIndex = nextIndex == positionCount ? 0 : nextIndex;
            }
            
        }
    }

    [Serializable]
    public class PathPoint
    {
        public Vector3 posiiton = Vector3.zero;

    }
}
