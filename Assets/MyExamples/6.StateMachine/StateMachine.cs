using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace MyExamples.StateMachine
{

    public class StateMachine
    {
        public bool logCurrentState;

        private Dictionary<Type, AState> statesDict = new();
        private AState currentState;
        private AState previousState;

        private CancellationTokenSource cts;

        public StateMachine()
        {

        }
        public StateMachine(params AState[] states)
        {
            foreach (var state in states)
            {
                AddState(state);
            }
        }

        public void AddState(AState state)
        {
            statesDict[state.GetType()] = state;
        }
        public void SetState<T>()
        {
            CancelSetStateAsync();//????

            if (currentState != null)
            {
                previousState = currentState;
                var previousStateType = previousState.GetType();
                currentState?.Exit(new ExitStateArgs(previousStateType, typeof(T)));
                currentState = statesDict[typeof(T)];
                currentState.Enter(new EnterStateArgs(previousStateType));
            }
            else
            {
                currentState?.Exit(new ExitStateArgs(default, typeof(T)));
                currentState = statesDict[typeof(T)];
                currentState.Enter(new EnterStateArgs(default));
            }
            if (logCurrentState)
            {
                Debug.Log($"STATEMACHINE: CURRENTSTATE - {currentState}");
            }
        }
        public async UniTask SetStateWithDelay<T>(float delayInSeconds)
        {
            cts = new CancellationTokenSource();

            try
            {
                await UniTask.WaitForSeconds(delayInSeconds, cancellationToken: cts.Token);
                SetState<T>();
            }
            finally
            {
                cts.Dispose();
                cts = null;
            }
        }
        public void Update()
        {
            currentState?.Update();
        }
        private void CancelSetStateAsync()
        {
            cts?.Cancel();
        }
    }


}