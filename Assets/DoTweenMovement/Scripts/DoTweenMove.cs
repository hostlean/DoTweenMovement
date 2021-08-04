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
        public FollowType followType;


        [SerializeField] private List<PathPoint> pathPoints;

        [SerializeField] private bool showGizmosInPlayMode;
        [SerializeField] private Color pointGizmoColor;
        [SerializeField, Range(.1f, 10.0f)] private float gizmoSize = 1.0f;
        [SerializeField] private GizmoMovementType pointMoveInEditor;
        [SerializeField] private int startPoint;

        private List<Vector3> _positions = new List<Vector3>();
        private int _positionCount;
        private int _currentIndex;
        private bool goBack;
        

        //Properties
        public GizmoMovementType PointMoveInEditor => pointMoveInEditor;
        public List<Vector3> Positions => _positions;
        public List<PathPoint> PathPoints => pathPoints;
        public int PositionCount => _positionCount;
        public int PathCount => pathPoints.Count;
        public float GizmoSize => gizmoSize;
        public Color PointGizmoColor
        {
            get => pointGizmoColor;
            set => pointGizmoColor = value;
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

        private void Awake()
        {
            for (int i = 0; i < pathPoints.Count; i++)
            {
                _positions.Add(transform.position + pathPoints[i].position);
            }
            
            _positionCount = _positions.Count;

            //if(startPoint != 0)
            _currentIndex = startPoint;
            transform.position = _positions[startPoint];

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
                var previousIndex = _currentIndex - 1;
                ChangeIndexByFollowType(nextIndex, previousIndex);
              
            }
            
        }

        private void ChangeIndexByFollowType(int nextIndex, int previousIndex)
        {
            switch (followType)
            {
                case FollowType.Loop:
                    _currentIndex = nextIndex == _positionCount ? 0 : nextIndex;
                    break;
                case FollowType.BackAndForth:
                    if (goBack)
                    {
                        _currentIndex = previousIndex == -1 ? nextIndex : previousIndex;
                        if (_currentIndex == nextIndex)
                            goBack = false;
                    }
                    else
                    {
                        _currentIndex = nextIndex == _positionCount ? previousIndex : nextIndex;
                        if (_currentIndex == previousIndex)
                            goBack = true;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
