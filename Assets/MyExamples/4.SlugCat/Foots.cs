

namespace MyExamples.SlugCat
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Animations.Rigging;

    public class Foots : MonoBehaviour
    {
        public TwoBoneIKConstraint constraint;
        public Transform bone;
        public float rayLenght;
        public Vector3 rayOffset;

        public void LateUpdate()
        {
            Place(constraint, bone, rayLenght, rayOffset);
        }
        void Place(TwoBoneIKConstraint constraint, Transform bone, float rayLenght, Vector3 rayOffset)
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
