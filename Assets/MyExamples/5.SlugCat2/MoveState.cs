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
        public override void Enter(EnterStateArgs enterArgs)
        {
            playerMovement.rotateTowardsCamera = true;
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