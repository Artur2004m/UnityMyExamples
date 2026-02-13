using UnityEngine;

namespace MyExamples.SlugCat2
{
    public class TailAnimator : MonoBehaviour
    {
        public Vector3 finalRotationL;
        public Vector3 finalRotationMid;
        public Vector3 finalRotationR;
        public Transform ikTargetTip;
        public Transform playerForward;
        public Transform cam;

        public float maxAngleY = 90f;          
        public float rotationSpeed = 270f;      
        public bool invertDirection = false;    

        private Quaternion rotL, rotMid, rotR;
        private Quaternion currentRotation;
        private float maxSin;

        void Start()
        {
            rotL = Quaternion.Euler(finalRotationL);
            rotMid = Quaternion.Euler(finalRotationMid);
            rotR = Quaternion.Euler(finalRotationR);
            currentRotation = rotMid;
            maxSin = Mathf.Sin(maxAngleY * Mathf.Deg2Rad);
        }

        void LateUpdate()
        {
            if (!cam || !playerForward || !ikTargetTip) return;

            Vector3 camForward = cam.forward;
            camForward.y = 0;
            if (camForward.sqrMagnitude < 0.001f) return;
            camForward.Normalize();

            Vector3 localDir = playerForward.InverseTransformDirection(camForward);
            float x = localDir.x;
            if (invertDirection) x = -x;

            float t = (maxSin > 0.001f) ? Mathf.Clamp(x / maxSin, -1f, 1f) : 0f;

            Quaternion desiredRot;
            if (t < 0)
            {
                float factor = Mathf.InverseLerp(-1f, 0f, t);
                desiredRot = Quaternion.Lerp(rotL, rotMid, factor);
            }
            else
            {
                float factor = Mathf.InverseLerp(0f, 1f, t);
                desiredRot = Quaternion.Lerp(rotMid, rotR, factor);
            }

            currentRotation = Quaternion.RotateTowards(currentRotation, desiredRot, rotationSpeed * Time.deltaTime);
            ikTargetTip.localRotation = currentRotation;
        }
    }
}