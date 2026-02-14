using MyExamples.Movement;
using MyExamples.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyExamples.SlugCat2
{
    public class APlayerState : AState
    {
        protected RigWeightAnimator rigWeightAnimator;
        protected PlayerMovement playerMovement;
        protected PlayerInput playerInput;
        public void Init(RigWeightAnimator rigWeightAnimator, PlayerMovement playerMovement, PlayerInput playerInput)
        {
            this.rigWeightAnimator = rigWeightAnimator;
            this.playerMovement = playerMovement;
            this.playerInput = playerInput; 
        }
    }
}