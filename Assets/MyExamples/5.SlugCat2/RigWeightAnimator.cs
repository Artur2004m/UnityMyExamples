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
        private Dictionary<string, CancellationTokenSource> activeTokens = new(); // токены для каждого id

        public void Init()
        {
            foreach (var rigpath in rigs)
            {
                rigsMap[rigpath.id] = rigpath.rig;
            }
        }

        public void Animate(string id, AnimationCurve curve, float speedMultiplier = 1f)
        {
            // Отменяем предыдущую анимацию для этого id, если она есть
            if (activeTokens.TryGetValue(id, out var existingCts))
            {
                existingCts.Cancel();
                existingCts.Dispose();
                activeTokens.Remove(id);
            }

            // Создаём новый токен для этой анимации
            var cts = new CancellationTokenSource();
            activeTokens[id] = cts;

            // Запускаем анимацию
            AnimateTask(id, curve, cts.Token, speedMultiplier).Forget();
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

                float curveLength = GetCurveLength(curve);
                float elapsed = 0f;

                while (elapsed < curveLength)
                {
                    token.ThrowIfCancellationRequested();
                    rig.weight = curve.Evaluate(elapsed);
                    elapsed += Time.deltaTime * speedMultiplier;
                    await UniTask.Yield(cancellationToken: token);
                }

                rig.weight = curve.Evaluate(curveLength);
            }
            catch (OperationCanceledException)
            {
                // Анимация отменена – это нормально
            }
            finally
            {
                // После завершения (успешно или отмены) удаляем токен из словаря,
                // но только если он всё ещё тот же (не заменён новым)
                if (activeTokens.TryGetValue(id, out var cts) && cts.IsCancellationRequested)
                {
                    activeTokens.Remove(id);
                    cts.Dispose();
                }
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

        public void Cleanup()
        {
            foreach (var cts in activeTokens.Values)
            {
                cts.Cancel();
                cts.Dispose();
            }
            activeTokens.Clear();
        }
    }
}
