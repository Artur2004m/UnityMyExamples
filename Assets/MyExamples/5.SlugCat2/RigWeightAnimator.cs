using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace MyExamples.SlugCat2
{
    [Serializable]
    public class RigPath
    {
        public string id;
        public Rig rig;
    }

    [Serializable]
    public class RigWeightAnimator
    {
        public RigPath[] rigs;
        private Dictionary<string, Rig> rigsMap = new();
        private CancellationTokenSource animateCts;

        public void Init()
        {
            foreach (var rigpath in rigs)
            {
                rigsMap[rigpath.id] = rigpath.rig;
            }
        }

        public void Animate(string id, AnimationCurve curve, float speedMultiplier = 1f)
        {
            // Отменяем предыдущую анимацию
            animateCts?.Cancel();
            animateCts?.Dispose();
            animateCts = new CancellationTokenSource();

            AnimateTask(id, curve, animateCts.Token, speedMultiplier).Forget();
        }

        private async UniTask AnimateTask(string id, AnimationCurve curve, CancellationToken token, float speedMultiplier)
        {
            try
            {
                // Проверяем существование rig
                if (!rigsMap.TryGetValue(id, out var rig))
                {
                    Debug.LogError($"Rig with id {id} not found");
                    return;
                }

                // Получаем реальную длину кривой
                float curveLength = GetCurveLength(curve);
                float elapsed = 0f;

                while (elapsed < curveLength)
                {
                    // Проверяем отмену
                    token.ThrowIfCancellationRequested();

                    rig.weight = curve.Evaluate(elapsed);
                    elapsed += Time.deltaTime * speedMultiplier;

                    await UniTask.Yield(cancellationToken: token);
                }

                // Убеждаемся, что в конце точное значение
                rig.weight = curve.Evaluate(curveLength);
            }
            catch (OperationCanceledException)
            {
                // Анимация была отменена - это нормально
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in rig animation: {e}");
            }
        }

        private float GetCurveLength(AnimationCurve curve)
        {
            if (curve == null || curve.length == 0) return 0f;

            float minTime = curve[0].time;
            float maxTime = curve[0].time;

            for (int i = 1; i < curve.length; i++)
            {
                if (curve[i].time < minTime) minTime = curve[i].time;
                if (curve[i].time > maxTime) maxTime = curve[i].time;
            }

            return maxTime - minTime;
        }

        // Не забудьте очистить ресурсы при уничтожении объекта
        public void Cleanup()
        {
            animateCts?.Cancel();
            animateCts?.Dispose();
            animateCts = null;
        }
    }
}
