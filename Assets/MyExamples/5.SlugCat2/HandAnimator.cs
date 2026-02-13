using UnityEngine;

namespace MyExamples.SlugCat2
{
    public class HandAnimator : MonoBehaviour
    {
        public Transform cam;
        public Transform playerForward;
        public Transform ikTargetL;
        public Transform ikTargetR;
        public Transform ikHintL;
        public Transform ikHintR;

        [Header("TwoBoneIK targets")]
        public Vector3 leftTarget_camForwardPos;
        public Vector3 leftTarget_camForwardRot;
        public Vector3 leftTarget_camRightPos;
        public Vector3 leftTarget_camRightRot;
        public Vector3 leftTarget_camLeftPos;
        public Vector3 leftTarget_camLeftRot;
        public Vector3 leftTarget_camBackPos;
        public Vector3 leftTarget_camBackRot;

        public Vector3 rightTarget_camForwardPos;
        public Vector3 rightTarget_camForwardRot;
        public Vector3 rightTarget_camRightPos;
        public Vector3 rightTarget_camRightRot;
        public Vector3 rightTarget_camLeftPos;
        public Vector3 rightTarget_camLeftRot;
        public Vector3 rightTarget_camBackPos;
        public Vector3 rightTarget_camBackRot;

        [Header("TwoBoneIK hints")]
        public Vector3 leftHint_camForwardPos;
        public Vector3 leftHint_camForwardRot;
        public Vector3 leftHint_camRightPos;
        public Vector3 leftHint_camRightRot;
        public Vector3 leftHint_camLeftPos;
        public Vector3 leftHint_camLeftRot;
        public Vector3 leftHint_camBackPos;
        public Vector3 leftHint_camBackRot;

        public Vector3 rightHint_camForwardPos;
        public Vector3 rightHint_camForwardRot;
        public Vector3 rightHint_camRightPos;
        public Vector3 rightHint_camRightRot;
        public Vector3 rightHint_camLeftPos;
        public Vector3 rightHint_camLeftRot;
        public Vector3 rightHint_camBackPos;
        public Vector3 rightHint_camBackRot;

        [Header("Smoothing")]
        public float positionSmoothTime = 0.1f;
        public float rotationSpeed = 360f;

        // Текущие сглаженные локальные значения
        private Vector3 currentLeftTargetPos, currentRightTargetPos;
        private Quaternion currentLeftTargetRot, currentRightTargetRot;
        private Vector3 currentLeftHintPos, currentRightHintPos;
        private Quaternion currentLeftHintRot, currentRightHintRot;

        // Скорости для SmoothDamp
        private Vector3 leftTargetPosVel, rightTargetPosVel;
        private Vector3 leftHintPosVel, rightHintPosVel;

        void Start()
        {
            // Инициализируем текущие значения центральным положением (forward)
            if (ikTargetL != null)
            {
                currentLeftTargetPos = leftTarget_camForwardPos;
                currentLeftTargetRot = Quaternion.Euler(leftTarget_camForwardRot);
            }
            if (ikTargetR != null)
            {
                currentRightTargetPos = rightTarget_camForwardPos;
                currentRightTargetRot = Quaternion.Euler(rightTarget_camForwardRot);
            }
            if (ikHintL != null)
            {
                currentLeftHintPos = leftHint_camForwardPos;
                currentLeftHintRot = Quaternion.Euler(leftHint_camForwardRot);
            }
            if (ikHintR != null)
            {
                currentRightHintPos = rightHint_camForwardPos;
                currentRightHintRot = Quaternion.Euler(rightHint_camForwardRot);
            }
        }

        void LateUpdate()
        {
            if (cam == null || playerForward == null) return;

            // Горизонтальное направление камеры
            Vector3 camFlat = cam.forward;
            camFlat.y = 0;
            if (camFlat.sqrMagnitude < 0.001f) return;
            camFlat.Normalize();

            Vector3 playerFlat = playerForward.forward;
            playerFlat.y = 0;
            playerFlat.Normalize();

            // Угол между playerForward и camFlat в диапазоне [-180, 180]
            float signedAngle = Vector3.SignedAngle(playerFlat, camFlat, Vector3.up);
            float angle = signedAngle >= 0 ? signedAngle : signedAngle + 360f; // [0,360)

            // Определяем два соседних индекса (0: вперед, 1: вправо, 2: назад, 3: влево)
            int leftIdx, rightIdx;
            float t;
            if (angle < 90f)
            {
                leftIdx = 0; rightIdx = 1; t = angle / 90f;
            }
            else if (angle < 180f)
            {
                leftIdx = 1; rightIdx = 2; t = (angle - 90f) / 90f;
            }
            else if (angle < 270f)
            {
                leftIdx = 2; rightIdx = 3; t = (angle - 180f) / 90f;
            }
            else
            {
                leftIdx = 3; rightIdx = 0; t = (angle - 270f) / 90f;
            }

            // Функция получения позиции и поворота по индексу для левой руки Target
            (Vector3 pos, Quaternion rot) GetLeftTarget(int idx)
            {
                switch (idx)
                {
                    case 0: return (leftTarget_camForwardPos, Quaternion.Euler(leftTarget_camForwardRot));
                    case 1: return (leftTarget_camRightPos, Quaternion.Euler(leftTarget_camRightRot));
                    case 2: return (leftTarget_camBackPos, Quaternion.Euler(leftTarget_camBackRot));
                    case 3: return (leftTarget_camLeftPos, Quaternion.Euler(leftTarget_camLeftRot));
                    default: return (Vector3.zero, Quaternion.identity);
                }
            }

            (Vector3, Quaternion) GetRightTarget(int idx)
            {
                switch (idx)
                {
                    case 0: return (rightTarget_camForwardPos, Quaternion.Euler(rightTarget_camForwardRot));
                    case 1: return (rightTarget_camRightPos, Quaternion.Euler(rightTarget_camRightRot));
                    case 2: return (rightTarget_camBackPos, Quaternion.Euler(rightTarget_camBackRot));
                    case 3: return (rightTarget_camLeftPos, Quaternion.Euler(rightTarget_camLeftRot));
                    default: return (Vector3.zero, Quaternion.identity);
                }
            }

            (Vector3, Quaternion) GetLeftHint(int idx)
            {
                switch (idx)
                {
                    case 0: return (leftHint_camForwardPos, Quaternion.Euler(leftHint_camForwardRot));
                    case 1: return (leftHint_camRightPos, Quaternion.Euler(leftHint_camRightRot));
                    case 2: return (leftHint_camBackPos, Quaternion.Euler(leftHint_camBackRot));
                    case 3: return (leftHint_camLeftPos, Quaternion.Euler(leftHint_camLeftRot));
                    default: return (Vector3.zero, Quaternion.identity);
                }
            }

            (Vector3, Quaternion) GetRightHint(int idx)
            {
                switch (idx)
                {
                    case 0: return (rightHint_camForwardPos, Quaternion.Euler(rightHint_camForwardRot));
                    case 1: return (rightHint_camRightPos, Quaternion.Euler(rightHint_camRightRot));
                    case 2: return (rightHint_camBackPos, Quaternion.Euler(rightHint_camBackRot));
                    case 3: return (rightHint_camLeftPos, Quaternion.Euler(rightHint_camLeftRot));
                    default: return (Vector3.zero, Quaternion.identity);
                }
            }

            // Получаем данные для левого и правого индексов
            var leftTargetL = GetLeftTarget(leftIdx);
            var leftTargetR = GetLeftTarget(rightIdx);
            var rightTargetL = GetRightTarget(leftIdx);
            var rightTargetR = GetRightTarget(rightIdx);
            var leftHintL = GetLeftHint(leftIdx);
            var leftHintR = GetLeftHint(rightIdx);
            var rightHintL = GetRightHint(leftIdx);
            var rightHintR = GetRightHint(rightIdx);

            // Интерполяция
            Vector3 targetLeftPos = Vector3.Lerp(leftTargetL.Item1, leftTargetR.Item1, t);
            Quaternion targetLeftRot = Quaternion.Lerp(leftTargetL.Item2, leftTargetR.Item2, t);
            Vector3 targetRightPos = Vector3.Lerp(rightTargetL.Item1, rightTargetR.Item1, t);
            Quaternion targetRightRot = Quaternion.Lerp(rightTargetL.Item2, rightTargetR.Item2, t);
            Vector3 hintLeftPos = Vector3.Lerp(leftHintL.Item1, leftHintR.Item1, t);
            Quaternion hintLeftRot = Quaternion.Lerp(leftHintL.Item2, leftHintR.Item2, t);
            Vector3 hintRightPos = Vector3.Lerp(rightHintL.Item1, rightHintR.Item1, t);
            Quaternion hintRightRot = Quaternion.Lerp(rightHintL.Item2, rightHintR.Item2, t);

            // Плавное движение позиций
            currentLeftTargetPos = Vector3.SmoothDamp(currentLeftTargetPos, targetLeftPos, ref leftTargetPosVel, positionSmoothTime);
            currentRightTargetPos = Vector3.SmoothDamp(currentRightTargetPos, targetRightPos, ref rightTargetPosVel, positionSmoothTime);
            currentLeftHintPos = Vector3.SmoothDamp(currentLeftHintPos, hintLeftPos, ref leftHintPosVel, positionSmoothTime);
            currentRightHintPos = Vector3.SmoothDamp(currentRightHintPos, hintRightPos, ref rightHintPosVel, positionSmoothTime);

            // Плавное вращение
            float maxRotDelta = rotationSpeed * Time.deltaTime;
            currentLeftTargetRot = Quaternion.RotateTowards(currentLeftTargetRot, targetLeftRot, maxRotDelta);
            currentRightTargetRot = Quaternion.RotateTowards(currentRightTargetRot, targetRightRot, maxRotDelta);
            currentLeftHintRot = Quaternion.RotateTowards(currentLeftHintRot, hintLeftRot, maxRotDelta);
            currentRightHintRot = Quaternion.RotateTowards(currentRightHintRot, hintRightRot, maxRotDelta);

            // Применение к трансформам
            if (ikTargetL != null)
            {
                ikTargetL.position = playerForward.TransformPoint(currentLeftTargetPos);
                ikTargetL.rotation = playerForward.rotation * currentLeftTargetRot;
            }
            if (ikTargetR != null)
            {
                ikTargetR.position = playerForward.TransformPoint(currentRightTargetPos);
                ikTargetR.rotation = playerForward.rotation * currentRightTargetRot;
            }
            if (ikHintL != null)
            {
                ikHintL.position = playerForward.TransformPoint(currentLeftHintPos);
                ikHintL.rotation = playerForward.rotation * currentLeftHintRot;
            }
            if (ikHintR != null)
            {
                ikHintR.position = playerForward.TransformPoint(currentRightHintPos);
                ikHintR.rotation = playerForward.rotation * currentRightHintRot;
            }
        }
    }
}