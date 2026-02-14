using MyExamples.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyExamples.SlugCat2
{ 
    public class MoveCrouchState : APlayerState
    {
        public AnimationCurve rigWeightCurveEnterState;
        public float rigWeightCurveSpeedEnterState = 1f;
        public AnimationCurve rigWeightCurveExitState;
        public float rigWeightCurveSpeedExitState = 1f;

        public override void Enter(EnterStateArgs enterArgs)
        {
            playerMovement.rotateTowardsCamera = true;
            rigWeightAnimator.Animate("MoveCrouchRig", rigWeightCurveEnterState, rigWeightCurveSpeedEnterState);
        }
        public override void Exit(ExitStateArgs exitArgs)
        {
            rigWeightAnimator.Animate("MoveCrouchRig", rigWeightCurveExitState, rigWeightCurveSpeedExitState);
        }
        public override void Update()
        {
            bool crouchDown = playerInput.crouch_down;
            bool crouchHeld = playerInput.crouch_held;
            Vector3 moveInputVector = playerMovement.inputDirectionCached;

            if (crouchDown)
            {
                if (moveInputVector == Vector3.zero)
                {
                    sm.SetState<IdleState>();
                }
                else
                {
                    sm.SetState<MoveState>();
                }
            }

            if (moveInputVector == Vector3.zero)
            {
                if (crouchHeld) return;
                sm.SetState<IdleCrouchState>();
            }
        }
    }
}
