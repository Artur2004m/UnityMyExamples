using System;
using UnityEngine;


namespace MyExamples.StateMachine
{
    public struct EnterStateArgs
    {
        public Type previousStateType;

        public EnterStateArgs(Type previousStateType)
        {
            this.previousStateType = previousStateType;
        }
    }
}