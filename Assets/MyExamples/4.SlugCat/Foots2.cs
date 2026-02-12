using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace MyExamples.SlugCat
{
    public class Foots2 : MonoBehaviour
    {
        public float maxDistance = 5f;

        public Transform bone;
        public Transform center;
        public Transform constraint;

        public float rayLenght;
        public Vector3 rayOffset;
        public Vector3 centerOffset;

        public bool gizmos;
        private Dictionary<string, (Vector3, Vector3, Color)> drawRays = new();
        private Dictionary<string, (Vector3, float)> drawSpheres = new();


        private Vector3 point;
        private bool updated;


        public void LateUpdate()
        {
            var playerOrigin = center.transform.position + centerOffset;
            var origin = bone.transform.position + rayOffset;
            var direction = Vector3.down;
            Ray ray = new Ray(origin, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayLenght))
            {     
                
                drawRays["1"] = (origin, direction * Vector3.Distance(hit.point, origin), Color.green);

                if (!updated)point = hit.point;
                updated = true;
            }
            else
            {
                drawRays["1"] = (origin, direction * rayLenght, Color.red);
            }

            if (Vector3.Distance(playerOrigin, point) > maxDistance)
            {
                drawRays["2"] = (playerOrigin, point - playerOrigin, Color.green);
                updated = false;
            }
            else
            {
                drawRays["2"] = (playerOrigin, point - playerOrigin, Color.red);
            }

            drawSpheres["1"] = (point, 0.2f);
            constraint.position = point;
        }
        private void OnDrawGizmos()
        {
            if (!gizmos) return;
            if (drawRays == null) return;

            foreach (var drawRay in drawRays.Values)
            {
                Debug.DrawRay(drawRay.Item1, drawRay.Item2, drawRay.Item3);
            }
            foreach (var drawSphere in drawSpheres.Values)
            {
                Gizmos.DrawSphere(drawSphere.Item1, drawSphere.Item2);
            }
        }
    }
}