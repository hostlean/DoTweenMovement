using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DoTweenMovement.Editor
{
    public class DoTweenMovementEditor : UnityEditor.Editor
    {
        [SerializeField] private float closeDistance = 0.1f;
        [SerializeField] private float speed = 1.0f;
        [SerializeField] private List<Transform> points;
        private List<Vector3> positions;

        private void Awake()
        {
            throw new NotImplementedException();
        }
    }
}