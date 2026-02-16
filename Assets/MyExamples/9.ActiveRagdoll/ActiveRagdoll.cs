using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyExamples.ActiveRagdoll
{
    public class ActiveRagdoll : MonoBehaviour
    {
        public Transform ghostBone;
        public Rigidbody rb;

        public bool followPosition = false;
        public bool followRotation = true;

        public float followPositionForce;
        public float followTorqueForce;

        private void OnValidate()
        {
            //this.gameObject.name = $"[{ghostBone}] Target";
        }
        private void FixedUpdate()
        {
            if (followPosition)
            {
                var deltaPosition = ghostBone.position - rb.position;
                rb.AddForce(followPositionForce * deltaPosition, ForceMode.Acceleration);
            }
            if (followRotation)
            {
                var deltaRotQ = ghostBone.rotation * Quaternion.Inverse(rb.rotation);
                var deltaRot = deltaRotQ.eulerAngles;
                ClampDeltaRotation(ref deltaRot);
                rb.AddTorque(followTorqueForce * deltaRot, ForceMode.Acceleration);
            }
        }
        private void ClampDeltaRotation(ref Vector3 deltaRot)
        {
            if (deltaRot.x > 180) deltaRot.x = deltaRot.x - 360;
            if (deltaRot.x < -180) deltaRot.x = 360 - deltaRot.x;

            if (deltaRot.y > 180) deltaRot.y = deltaRot.y - 360;
            if (deltaRot.y < -180) deltaRot.y = 360 - deltaRot.y;

            if (deltaRot.z > 180) deltaRot.z = deltaRot.z - 360;
            if (deltaRot.z < -180) deltaRot.z = 360 - deltaRot.z;
        }
    }
}