
namespace MyExamples.SlugCat
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PhysBone : MonoBehaviour
    {
        [Header("Sources")]
        [Tooltip("Цель, к которой физический объект будет стремиться. Обычно – исходная кость анимации.")]
        public Transform source;

        [Tooltip("Смещение относительно источника в его локальном пространстве.")]
        public Vector3 positionOffset = Vector3.zero;

        [Tooltip("Поворот относительно источника.")]
        public Vector3 rotationOffset = Vector3.zero;

        [Header("Spring Settings")]
        [Range(0, 1000)] public float positionSpring = 300f;
        [Range(0, 100)] public float positionDamper = 10f;
        [Range(0, 300)] public float rotationSpring = 100f;
        [Range(0, 30)] public float rotationDamper = 5f;

        [Header("Rigidbody Settings")]
        public bool useGravity = false;
        public bool freezePositionX = false;
        public bool freezePositionY = false;
        public bool freezePositionZ = false;
        public bool freezeRotationX = false;
        public bool freezeRotationY = false;
        public bool freezeRotationZ = false;

        private Rigidbody rb;
        private Quaternion sourceRotationOffset;

        void Reset()
        {
            // Автоматическая настройка Rigidbody при добавлении компонента
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.mass = 1f;
        }

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (source == null)
            {
                Debug.LogWarning($"{name}: источник не задан. Компонент будет отключён.", this);
                enabled = false;
                return;
            }
            sourceRotationOffset = Quaternion.Euler(rotationOffset);
        }

        void FixedUpdate()
        {
            if (source == null || rb == null) return;

            // Целевые позиция и поворот (мировые)
            Vector3 targetPosition = source.TransformPoint(positionOffset);
            Quaternion targetRotation = source.rotation * sourceRotationOffset;

            // --- Силы для позиции ---
            Vector3 positionError = targetPosition - rb.position;
            Vector3 desiredVelocity = Vector3.zero;
            Vector3 positionForce = Vector3.zero;

            if (!freezePositionX) desiredVelocity.x = positionError.x * positionSpring;
            if (!freezePositionY) desiredVelocity.y = positionError.y * positionSpring;
            if (!freezePositionZ) desiredVelocity.z = positionError.z * positionSpring;

            // Сила, пропорциональная ошибке позиции и демпфированию скорости
            positionForce = desiredVelocity - (rb.velocity * positionDamper);
            rb.AddForce(positionForce, ForceMode.Acceleration);

            // --- Моменты для поворота ---
            Quaternion rotationError = targetRotation * Quaternion.Inverse(rb.rotation);
            Vector3 torque = Vector3.zero;

            // Преобразуем ошибку поворота в углы Эйлера (кратчайший путь)
            rotationError.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;
            if (angle != 0)
            {
                Vector3 angularVelocityError = axis * (angle * Mathf.Deg2Rad * rotationSpring);
                if (!freezeRotationX) torque.x = angularVelocityError.x;
                if (!freezeRotationY) torque.y = angularVelocityError.y;
                if (!freezeRotationZ) torque.z = angularVelocityError.z;
            }

            // Демпфирование угловой скорости
            Vector3 angularVelocity = rb.angularVelocity;
            if (freezeRotationX) angularVelocity.x = 0;
            if (freezeRotationY) angularVelocity.y = 0;
            if (freezeRotationZ) angularVelocity.z = 0;
            torque -= angularVelocity * rotationDamper;

            rb.AddTorque(torque, ForceMode.Acceleration);

            // --- Гравитация ---
            rb.useGravity = useGravity;
        }
    }
}
