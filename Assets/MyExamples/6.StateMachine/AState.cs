using UnityEngine;


namespace MyExamples.StateMachine
{
    public abstract class AState
    {
        protected StateMachine sm;
        public AState(StateMachine sm = null)
        {
            this.sm = sm;
        }
        public void SetStateMachine(StateMachine sm)
        {
            this.sm = sm;
        }
        public virtual void Enter(EnterStateArgs enterArgs) { }
        public virtual void Exit(ExitStateArgs exitArgs) { }
        public virtual void Update() { }
    }
}