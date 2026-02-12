using UnityEngine;

namespace MyExamples.SlugCat
{
    public class RotationToCamera2 : MonoBehaviour
    {
        [Header("References")]
        public Transform cam;
        public Transform target;
        public Transform rootTransform;

        [Header("Base Offset (Euler angles relative to root)")]
        public Vector3 baseOffset = Vector3.zero;

        [Header("Horizontal (Yaw)")]
        public bool enableYaw = true;
        public float yawMultiplier = 1f;
        public bool invertYaw = false;
        public float maxYawAngle = 90f;

        [Header("Vertical (Pitch)")]
        public bool enablePitch = false;
        public float pitchMultiplier = 1f;
        public bool invertPitch = false;
        public float maxPitchAngle = 30f;

        [Header("Return when exceeding limit")]
        [Range(0f, 1f)]
        public float returnStrength = 0.5f;

        [Header("Speed")]
        public float rotationSpeed = 180f;

        private void LateUpdate()
        {
            if (cam == null || target == null) return;

            Transform root = rootTransform != null ? rootTransform : transform;
            Vector3 rootUp = root.up;
            Vector3 rootForward = Vector3.ProjectOnPlane(root.forward, rootUp).normalized;

            // Neutral rotation: root * baseOffset
            Quaternion neutral = root.rotation * Quaternion.Euler(baseOffset);

            // ---- Yaw ----
            float yawAngle = 0f;
            if (enableYaw)
            {
                Vector3 camForwardH = Vector3.ProjectOnPlane(cam.forward, rootUp).normalized;
                float raw = Vector3.SignedAngle(rootForward, camForwardH, rootUp);
                raw *= yawMultiplier;
                if (invertYaw) raw *= -1f;
                yawAngle = ApplyLimit(raw, maxYawAngle, returnStrength);
            }

            // ---- Pitch ----
            float pitchAngle = 0f;
            if (enablePitch)
            {
                float raw = Mathf.Asin(Vector3.Dot(cam.forward, rootUp)) * Mathf.Rad2Deg;
                raw *= pitchMultiplier;
                if (invertPitch) raw *= -1f;
                pitchAngle = ApplyLimit(raw, maxPitchAngle, returnStrength);
            }

            // Build world rotation: neutral * yaw * pitch
            Quaternion yawRot = Quaternion.AngleAxis(yawAngle, rootUp);
            Vector3 forwardAfterYaw = yawRot * (neutral * Vector3.forward);
            Quaternion horizontal = Quaternion.LookRotation(forwardAfterYaw, rootUp);

            Vector3 rightAfterYaw = horizontal * Vector3.right;
            Quaternion pitchRot = Quaternion.AngleAxis(-pitchAngle, rightAfterYaw);

            Quaternion desired = pitchRot * horizontal;
            target.rotation = Quaternion.RotateTowards(target.rotation, desired, rotationSpeed * Time.deltaTime);
        }

        private float ApplyLimit(float angle, float limit, float returnStrength)
        {
            if (Mathf.Abs(angle) <= limit) return angle;
            float sign = Mathf.Sign(angle);
            float excess = (Mathf.Abs(angle) - limit) / (180f - limit);
            excess = Mathf.Clamp01(excess);
            float factor = Mathf.Lerp(1f, 1f - excess, returnStrength);
            return sign * limit * factor;
        }
    }
}