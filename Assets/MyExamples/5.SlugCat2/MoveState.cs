using MyExamples.Movement;
using MyExamples.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace MyExamples.SlugCat2
{
    [Serializable]
    public class MoveState : APlayerState
    {
        public AnimationCurve rigWeightCurveEnterState;
        public float rigWeightCurveSpeedEnterState = 1f;
        public AnimationCurve rigWeightCurveExitState;
        public float rigWeightCurveSpeedExitState = 1f;
        public override void Enter(EnterStateArgs enterArgs)
        {
            playerMovement.rotateTowardsCamera = true;
            rigWeightAnimator.Animate("MoveRig", rigWeightCurveEnterState, rigWeightCurveSpeedEnterState);
        }
        public override void Exit(ExitStateArgs exitArgs)
        {
            rigWeightAnimator.Animate("MoveRig", rigWeightCurveExitState, rigWeightCurveSpeedExitState);
        }
        public override void Update()
        {
            if (playerMovement.inputDirectionCached == Vector3.zero)
            {
                sm.SetState<IdleState>();
            }
        }
    }
}