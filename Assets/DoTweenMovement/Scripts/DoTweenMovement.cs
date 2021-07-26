using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoTweenMovement.Scripts
{
    public class DoTweenMovement : MonoBehaviour
    {
        [SerializeField] private float closeDistance = 0.1f;
        [SerializeField] private float speed = 1.0f;

        [SerializeField] private SpeedType speedType;
        [SerializeField] private PathType pathType;


        [SerializeField] private List<PathPoint> points;
        
        [Header("Gizmos")]
        [SerializeField] private Color gizmoColor;

        [SerializeField, Range(.1f, 10.0f)] private float gizmoSize = 1.0f;

        [SerializeField] private GizmoType gizmoType;

        private List<Vector3> _positions = new List<Vector3>();



        private int positionCount;
        private int _currentIndex;

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

        public enum GizmoType
        {
            Sphere,
            Cube,
            WiredSphere,
            WiredCube
        }

        private void Awake()
        {
            for (int i = 0; i < points.Count; i++)
            {
                _positions.Add(transform.position + points[i].posiiton);
            }
            
            positionCount = _positions.Count;
        }

        private void FixedUpdate()
        {
            MoveWithLine();
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


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            for (int i = 0; i < points.Count; i++)
            {
                Gizmos.color = gizmoColor;

                switch (gizmoType)
                {
                    case GizmoType.Sphere:
                        Gizmos.DrawSphere(transform.position + points[i].posiiton, gizmoSize);
                        break;
                    case GizmoType.Cube:
                        Gizmos.DrawCube(transform.position + points[i].posiiton, Vector3.one * gizmoSize);
                        break;
                    case GizmoType.WiredSphere:
                        Gizmos.DrawWireSphere(transform.position + points[i].posiiton, gizmoSize);
                        break;
                    case GizmoType.WiredCube:
                        Gizmos.DrawWireCube(transform.position + points[i].posiiton, Vector3.one * gizmoSize);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
            }
        }
#endif
        
    }

    [Serializable]
    public class PathPoint
    {
        public Vector3 posiiton;
        
    }
}
