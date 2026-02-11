using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


namespace MyExamples.SlugCat
{
    public class RotationToCamera : MonoBehaviour
    {
        public Transform playerForward;
        public MultiRotationConstraint targetConstraint;
        public Transform targetTransform;
        public Camera cam;
        public float maxYAngle;
        public float maxZAngle;

        private void LateUpdate()
        {
            targetTransform.rotation = Quaternion.LookRotation(cam.transform.forward);
        }
    }
}