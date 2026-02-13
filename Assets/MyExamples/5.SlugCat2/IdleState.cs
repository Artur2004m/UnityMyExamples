using MyExamples.StateMachine;
using MyExamples.Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyExamples.SlugCat2
{
    [Serializable]
    public class IdleState : APlayerState
    {
        public AnimationCurve rigWeightCurveEnterState;
        public float rigWeightCurveSpeedEnterState = 1f;
        public AnimationCurve rigWeightCurveExitState;
        public float rigWeightCurveSpeedExitState = 1f;
        public override void Enter(EnterStateArgs enterArgs)
        {
            playerMovement.rotateTowardsCamera = false;
            rigWeightAnimator.Animate("IdleRig", rigWeightCurveEnterState, rigWeightCurveSpeedEnterState);

        }
        public override void Exit(ExitStateArgs exitArgs)
        {
            rigWeightAnimator.Animate("IdleRig", rigWeightCurveExitState, rigWeightCurveSpeedExitState);
        }
        public override void Update()
        {
            if (playerMovement.inputDirectionCached != Vector3.zero)
            {
                sm.SetState<MoveState>();
                
            }
        }
    }
}