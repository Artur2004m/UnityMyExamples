using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyExamples.SlugCat
{
    public class Foots3 : MonoBehaviour
    {
        public float maxDistance;
        public float t;
        public float cooldownTime = 0.2f;
        public float targetReachedDistance = 0.05f;
        public float liftHeight = 0.3f;

        [Header("references")]
        public Transform centerTarget;
        public Transform ikTargetL;
        public Transform ikTargetR;

        [Header("offsets")]
        public Vector3 centerOffsetR;
        public Vector3 centerOffsetL;
        public float rayLenght = 1f;

        [Header("gizmos")]
        public bool gizmos;
        public float gizmosSphereRadius = 0.2f;
        private Dictionary<int, (Vector3, Color)> drawSpheres = new();
        private Dictionary<int, (Vector3, Vector3, Color)> drawRays = new();

        private Vector3 lastPointR;
        private Vector3 lastPointL;
        private bool stickedToGroundR;
        private bool stickedToGroundL;
        private bool isMovingR;
        private bool isMovingL;
        private float cooldownTimerR;
        private float cooldownTimerL;
        private Vector3 startPosR;
        private Vector3 startPosL;
        private float startDistR;
        private float startDistL;

        private void LateUpdate()
        {
            DrawSphere(1, centerTarget.position, Color.yellow);
            DrawSphere(2, centerTarget.position + centerOffsetR, Color.magenta);
            DrawSphere(3, centerTarget.position + centerOffsetL, Color.magenta);

            var originR = centerTarget.position + centerOffsetR;
            var originL = centerTarget.position + centerOffsetL;
            var direction = Vector3.down;

            Ray rayR = new Ray(originR, direction);
            Ray rayL = new Ray(originL, direction);
            RaycastHit hitR;
            RaycastHit hitL;

            if (Physics.Raycast(rayR, out hitR, rayLenght))
            {
                DrawRay(1, originR, direction * rayLenght, Color.green);
                if (!stickedToGroundR && !isMovingR && cooldownTimerR <= 0f)
                {
                    if (!isMovingL)
                    {
                        lastPointR = hitR.point;
                        stickedToGroundR = true;
                        isMovingR = true;
                        startPosR = ikTargetR.position;
                        startDistR = Vector3.Distance(startPosR, lastPointR);
                        if (startDistR < 0.001f) startDistR = 0.001f;
                    }
                }
            }
            else
            {
                DrawRay(1, originR, direction * rayLenght, Color.red);
            }

            if (Physics.Raycast(rayL, out hitL, rayLenght))
            {
                DrawRay(2, originL, direction * rayLenght, Color.green);
                if (!stickedToGroundL && !isMovingL && cooldownTimerL <= 0f)
                {
                    if (!isMovingR)
                    {
                        lastPointL = hitL.point;
                        stickedToGroundL = true;
                        isMovingL = true;
                        startPosL = ikTargetL.position;
                        startDistL = Vector3.Distance(startPosL, lastPointL);
                        if (startDistL < 0.001f) startDistL = 0.001f;
                    }
                }
            }
            else
            {
                DrawRay(2, originL, direction * rayLenght, Color.red);
            }

            if (stickedToGroundR)
            {
                DrawRay(3, originR, lastPointR - originR, Color.red);
            }
            if (stickedToGroundL)
            {
                DrawRay(4, originL, lastPointL - originL, Color.red);
            }

            if (Vector3.Distance(lastPointR, originR) > maxDistance)
            {
                stickedToGroundR = false;
            }
            if (Vector3.Distance(lastPointL, originL) > maxDistance)
            {
                stickedToGroundL = false;
            }

            DrawSphere(4, lastPointR, Color.cyan);
            DrawSphere(5, lastPointL, Color.cyan);

            if (isMovingR)
            {
                float distToTarget = Vector3.Distance(ikTargetR.position, lastPointR);
                float progress = 1f - (distToTarget / startDistR);
                progress = Mathf.Clamp01(progress);
                float lift = Mathf.Sin(progress * Mathf.PI) * liftHeight;
                Vector3 targetWithLift = lastPointR + Vector3.up * lift;
                ikTargetR.position = Vector3.Lerp(ikTargetR.position, targetWithLift, t);
            }
            else
            {
                ikTargetR.position = Vector3.Lerp(ikTargetR.position, lastPointR, t);
            }

            if (isMovingL)
            {
                float distToTarget = Vector3.Distance(ikTargetL.position, lastPointL);
                float progress = 1f - (distToTarget / startDistL);
                progress = Mathf.Clamp01(progress);
                float lift = Mathf.Sin(progress * Mathf.PI) * liftHeight;
                Vector3 targetWithLift = lastPointL + Vector3.up * lift;
                ikTargetL.position = Vector3.Lerp(ikTargetL.position, targetWithLift, t);
            }
            else
            {
                ikTargetL.position = Vector3.Lerp(ikTargetL.position, lastPointL, t);
            }

            if (isMovingR && Vector3.Distance(ikTargetR.position, lastPointR) < targetReachedDistance)
            {
                isMovingR = false;
                cooldownTimerR = cooldownTime;
                ikTargetR.position = lastPointR;
            }
            if (isMovingL && Vector3.Distance(ikTargetL.position, lastPointL) < targetReachedDistance)
            {
                isMovingL = false;
                cooldownTimerL = cooldownTime;
                ikTargetL.position = lastPointL;
            }

            if (cooldownTimerR > 0f) cooldownTimerR -= Time.deltaTime;
            if (cooldownTimerL > 0f) cooldownTimerL -= Time.deltaTime;
        }

        private void OnDrawGizmos()
        {
            if (!gizmos) return;
            foreach (var ray in drawRays.Values)
            {
                Debug.DrawRay(ray.Item1, ray.Item2, ray.Item3);
            }
            foreach (var sphere in drawSpheres.Values)
            {
                Gizmos.color = sphere.Item2;
                Gizmos.DrawSphere(sphere.Item1, gizmosSphereRadius);
            }
        }

        private void DrawSphere(int id, Vector3 spherePos = default, Color color = default)
        {
            if (!gizmos) return;
            drawSpheres[id] = (spherePos, color);
        }

        private void DrawRay(int id, Vector3 rayPos = default, Vector3 rayDir = default, Color color = default)
        {
            if (!gizmos) return;
            drawRays[id] = (rayPos, rayDir, color);
        }

        private void EraseSphere(int id)
        {
            if (!gizmos) return;
            drawSpheres.Remove(id);
        }

        private void EraseRay(int id)
        {
            if (!gizmos) return;
            drawRays.Remove(id);
        }
    }
}