using System;
using UnityEngine;

namespace MyExamples.StateMachine
{

    public struct ExitStateArgs
    {
        public Type nextStateType;
        public Type previousStateType;

        public ExitStateArgs(Type previousStateType = default, Type nextStateType = default)
        {
            this.previousStateType = previousStateType;
            this.nextStateType = nextStateType;
        }
    }
}