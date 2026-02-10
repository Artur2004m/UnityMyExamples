using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace MyExamples.FootIK
{
    public class FootIK : MonoBehaviour
    {
        public TwoBoneIKConstraint LeftLegConstraint;
        public TwoBoneIKConstraint RightLegConstraint;
        public TwoBoneIKConstraint LeftToeConstraint;
        public TwoBoneIKConstraint RightToeConstraint;

        public Transform LeftLegBone;
        public Transform LeftToeBone;
        public Transform RightLegBone;
        public Transform RightToeBone;

        public float legRayLenght;
        public float toeRayLenght;
        public Vector3 legRayOffset;
        public Vector3 toeRayOffset;

        void Update()
        {
            ThrowRay(LeftLegConstraint, LeftLegBone, legRayLenght, legRayOffset);
            ThrowRay(RightLegConstraint, RightLegBone, legRayLenght, legRayOffset);
            ThrowRay(LeftToeConstraint, LeftToeBone, toeRayLenght, toeRayOffset);
            ThrowRay(RightToeConstraint, RightToeBone, toeRayLenght, toeRayOffset);
        }
        void ThrowRay(TwoBoneIKConstraint constraint, Transform bone, float rayLenght, Vector3 rayOffset)
        {
            Ray ray = new Ray(bone.position + rayOffset, Vector3.down);
            RaycastHit hit;
            Color rayColor;

            rayColor = Color.red;

            if (Physics.Raycast(ray, out hit, rayLenght, LayerMask.GetMask("Walkable")))
            {
                constraint.weight = 1f;

                constraint.transform.position = new Vector3(
                    constraint.transform.position.x,
                    hit.point.y,
                    constraint.transform.position.z
                    );

                rayColor = Color.green;
            }
            else
            {
                constraint.weight = 0f;
            }

            Debug.DrawRay(bone.position + rayOffset, Vector3.down * rayLenght, rayColor);
        }
    }

}