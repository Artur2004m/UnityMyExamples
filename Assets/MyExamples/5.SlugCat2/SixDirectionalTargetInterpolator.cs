using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyExamples.SlugCat2
{
    public enum CameraDirection
    {
        Forward,
        Backward,
        Left,
        Right,
        Up,
        Down
    }

    [Serializable]
    public class Target
    {
        public Transform targetTransform;
        public CameraDirection direction;
        public Vector3 localPosition;
        public Vector3 localRotation;
    }

    public class SixDirectionalTargetInterpolator : MonoBehaviour
    {
        [Header("Settings")]
        public bool enable = true;
        public Transform cameraTransform;
        public Transform rootTransform;
        public Target[] targets;

        [Header("Interpolation Toggles")]
        public bool interpolatePosition = true;
        public bool interpolateRotation = true;

        [Header("Smoothing")]
        public float positionSmoothTime = 0.1f;
        public float rotationSpeed = 360f;

        [Header("Debug")]
        public bool gizmos = true;
        public float gizmosSphereSize = 0.1f;
        public bool drawLines = true;

        private Dictionary<Transform, TargetState> targetStates;

        private class TargetState
        {
            public Vector3 currentLocalPos;
            public Quaternion currentLocalRot;
            public Vector3 posVelocity;
        }

        private void OnValidate()
        {
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
        }

        private void Start()
        {
            InitializeStatesFromCurrentPositions();
        }

        private void InitializeStatesFromCurrentPositions()
        {
            targetStates = new Dictionary<Transform, TargetState>();

            if (targets == null || rootTransform == null) return;

            foreach (var target in targets)
            {
                if (target?.targetTransform == null) continue;

                if (!targetStates.ContainsKey(target.targetTransform))
                {
                    // Преобразуем текущее мировое положение таргета в локальное относительно rootTransform
                    Vector3 currentLocalPos = rootTransform.InverseTransformPoint(target.targetTransform.position);
                    Quaternion currentLocalRot = Quaternion.Inverse(rootTransform.rotation) * target.targetTransform.rotation;

                    targetStates[target.targetTransform] = new TargetState
                    {
                        currentLocalPos = currentLocalPos,
                        currentLocalRot = currentLocalRot,
                        posVelocity = Vector3.zero
                    };
                }
            }
        }

        private void LateUpdate()
        {
            if (!enable || cameraTransform == null || rootTransform == null || targets == null || targets.Length == 0)
                return;

            if (targetStates == null || targetStates.Count == 0)
                InitializeStatesFromCurrentPositions();

            var (directions, weights) = GetBlendedDirections();

            foreach (var kvp in targetStates)
            {
                Transform targetTransform = kvp.Key;
                TargetState state = kvp.Value;

                // --- Вычисление целевой локальной позиции (средневзвешенное) ---
                Vector3 targetLocalPos = Vector3.zero;
                float totalWeightPos = 0f;

                // --- Вычисление целевого локального поворота (усреднение кватернионов) ---
                Vector4 avgRot = Vector4.zero;
                float totalWeightRot = 0f;
                Quaternion referenceRot = Quaternion.identity;

                for (int i = 0; i < directions.Length; i++)
                {
                    CameraDirection dir = directions[i];
                    float weight = weights[i];
                    if (weight <= 0.001f) continue;

                    Target setting = FindTargetSetting(targetTransform, dir);
                    if (setting == null) continue;

                    // Позиция
                    targetLocalPos += setting.localPosition * weight;
                    totalWeightPos += weight;

                    // Поворот
                    Quaternion q = Quaternion.Euler(setting.localRotation);

                    // Приводим все кватернионы к одному полушарию относительно первого ненулевого
                    if (totalWeightRot == 0)
                    {
                        referenceRot = q;
                    }
                    else
                    {
                        if (Quaternion.Dot(q, referenceRot) < 0)
                            q = new Quaternion(-q.x, -q.y, -q.z, -q.w);
                    }

                    avgRot.x += q.x * weight;
                    avgRot.y += q.y * weight;
                    avgRot.z += q.z * weight;
                    avgRot.w += q.w * weight;
                    totalWeightRot += weight;
                }

                // Нормализация позиции
                if (totalWeightPos > 0.001f)
                {
                    targetLocalPos /= totalWeightPos;
                }
                else
                {
                    // Нет активных настроек для этого таргета в текущем направлении — пропускаем обновление
                    continue;
                }

                // Нормализация поворота
                Quaternion targetLocalRot;
                if (totalWeightRot > 0.001f)
                {
                    avgRot /= totalWeightRot;
                    targetLocalRot = new Quaternion(avgRot.x, avgRot.y, avgRot.z, avgRot.w).normalized;
                }
                else
                {
                    targetLocalRot = state.currentLocalRot;
                }

                // --- Применение интерполяции ---
                if (interpolatePosition)
                {
                    state.currentLocalPos = Vector3.SmoothDamp(
                        state.currentLocalPos,
                        targetLocalPos,
                        ref state.posVelocity,
                        positionSmoothTime
                    );
                }
                else
                {
                    state.currentLocalPos = targetLocalPos;
                }

                if (interpolateRotation)
                {
                    state.currentLocalRot = Quaternion.RotateTowards(
                        state.currentLocalRot,
                        targetLocalRot,
                        rotationSpeed * Time.deltaTime
                    );
                }
                else
                {
                    state.currentLocalRot = targetLocalRot;
                }

                // --- Применение к трансформу ---
                if (interpolatePosition)
                {
                    targetTransform.position = rootTransform.TransformPoint(state.currentLocalPos);
                }
                if (interpolateRotation)
                {
                    targetTransform.rotation = rootTransform.rotation * state.currentLocalRot;
                }
            }
        }

        private Target FindTargetSetting(Transform targetTransform, CameraDirection direction)
        {
            if (targets == null) return null;
            return Array.Find(targets, t => t != null && t.targetTransform == targetTransform && t.direction == direction);
        }

        private (CameraDirection[] directions, float[] weights) GetBlendedDirections()
        {
            // Направление камеры в локальном пространстве персонажа
            Vector3 localCamDir = rootTransform.InverseTransformDirection(cameraTransform.forward).normalized;

            // Горизонтальная проекция
            Vector3 horizontalDir = localCamDir;
            horizontalDir.y = 0;
            float horizontalMagnitude = horizontalDir.magnitude;

            // Почти строго вверх/вниз
            if (horizontalMagnitude < 0.001f)
            {
                CameraDirection verticalDir = localCamDir.y > 0 ? CameraDirection.Up : CameraDirection.Down;
                return (new[] { verticalDir, verticalDir }, new[] { 1f, 0f });
            }

            horizontalDir /= horizontalMagnitude;

            // Угол между Forward (0,0,1) и горизонтальным направлением
            float angle = Vector3.SignedAngle(Vector3.forward, horizontalDir, Vector3.up);
            if (angle < 0) angle += 360f;

            // Определяем два соседних горизонтальных направления и вес t
            CameraDirection leftDir, rightDir;
            float t;

            if (angle < 90f)
            {
                leftDir = CameraDirection.Forward;
                rightDir = CameraDirection.Right;
                t = angle / 90f;
            }
            else if (angle < 180f)
            {
                leftDir = CameraDirection.Right;
                rightDir = CameraDirection.Backward;
                t = (angle - 90f) / 90f;
            }
            else if (angle < 270f)
            {
                leftDir = CameraDirection.Backward;
                rightDir = CameraDirection.Left;
                t = (angle - 180f) / 90f;
            }
            else
            {
                leftDir = CameraDirection.Left;
                rightDir = CameraDirection.Forward;
                t = (angle - 270f) / 90f;
            }

            // Вертикальная составляющая
            float verticalWeight = Mathf.Abs(localCamDir.y);
            float horizontalWeight = 1f - verticalWeight;

            if (verticalWeight < 0.01f)
            {
                return (new[] { leftDir, rightDir }, new[] { 1f - t, t });
            }
            else
            {
                CameraDirection verticalDir = localCamDir.y > 0 ? CameraDirection.Up : CameraDirection.Down;
                return (
                    new[] { leftDir, rightDir, verticalDir },
                    new[] { (1f - t) * horizontalWeight, t * horizontalWeight, verticalWeight }
                );
            }
        }

        private void OnDrawGizmos()
        {
            if (!gizmos || targets == null || rootTransform == null) return;

            foreach (var target in targets)
            {
                if (target?.targetTransform == null) continue;

                Vector3 worldPos = rootTransform.TransformPoint(target.localPosition);

                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(worldPos, gizmosSphereSize);

                if (drawLines)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(target.targetTransform.position, worldPos);
                }

#if UNITY_EDITOR
                Quaternion worldRot = rootTransform.rotation * Quaternion.Euler(target.localRotation);
                UnityEditor.Handles.color = Color.green;
                UnityEditor.Handles.ArrowHandleCap(
                    0,
                    worldPos,
                    worldRot,
                    gizmosSphereSize * 2f,
                    EventType.Repaint
                );

                UnityEditor.Handles.color = Color.white;
                UnityEditor.Handles.Label(worldPos + Vector3.up * 0.2f,
                    $"{target.targetTransform.name}: {target.direction}");
#endif
            }
        }
    }
}