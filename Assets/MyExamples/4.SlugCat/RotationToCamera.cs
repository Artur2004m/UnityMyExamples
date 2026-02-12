using UnityEngine;

namespace MyExamples.SlugCat
{
    public class RotationToCamera : MonoBehaviour
    {
        public Transform cam;
        public Transform target;
        public Transform rootTransform;
        public float maxHorizontalAngle = 90f;
        public float maxVerticalAngle = 45f;
        public float rotationSpeed = 180f;

        private void LateUpdate()
        {
            Transform root = rootTransform != null ? rootTransform : transform;
            Vector3 rootForward = Vector3.ProjectOnPlane(root.forward, Vector3.up).normalized;
            Vector3 rootUp = root.up;

            Vector3 camForward = cam.forward.normalized;
            Vector3 camForwardH = Vector3.ProjectOnPlane(camForward, rootUp).normalized;

            // √оризонтальный угол (рыскание)
            float horizontalAngle = Vector3.SignedAngle(rootForward, camForwardH, rootUp);
            float targetHorizontalAngle;

            if (Mathf.Abs(horizontalAngle) <= maxHorizontalAngle)
            {
                targetHorizontalAngle = horizontalAngle;
            }
            else
            {
                float excess = (Mathf.Abs(horizontalAngle) - maxHorizontalAngle) / (180f - maxHorizontalAngle);
                excess = Mathf.Clamp01(excess);
                targetHorizontalAngle = Mathf.Sign(horizontalAngle) * maxHorizontalAngle * (1f - excess);
            }

            // Ќаправление после горизонтального поворота
            Vector3 forwardAfterYaw = Quaternion.AngleAxis(targetHorizontalAngle, rootUp) * rootForward;
            Quaternion horizontalRot = Quaternion.LookRotation(forwardAfterYaw, rootUp);

            // ¬ертикальный угол (тангаж) Ц подъЄм камеры относительно горизонтали
            float verticalAngle = Mathf.Asin(Vector3.Dot(camForward, rootUp)) * Mathf.Rad2Deg;
            verticalAngle = Mathf.Clamp(verticalAngle, -maxVerticalAngle, maxVerticalAngle);

            // ќсь, вокруг которой делаем наклон Ц права€ ось после горизонтального поворота
            Vector3 rightAfterYaw = horizontalRot * Vector3.right;

            // ѕоворот вверх/вниз (отрицательный знак, т.к. положительный угол вокруг правой оси наклон€ет вниз)
            Quaternion pitchRot = Quaternion.AngleAxis(-verticalAngle, rightAfterYaw);

            // »тоговое целевое вращение
            Quaternion targetRotation = pitchRot * horizontalRot;

            target.rotation = Quaternion.RotateTowards(target.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}