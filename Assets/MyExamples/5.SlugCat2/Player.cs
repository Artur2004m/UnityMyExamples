using MyExamples.Movement;
using MyExamples.SlugCat2;
using MyExamples.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public PlayerInput playerInput = new();
    public RigWeightAnimator rigWeightAnimator = new();
    public bool logCurrentState;
    public IdleState idleState = new();
    public IdleCrouchState idleCrouchState = new(); 
    public MoveCrouchState moveCrouchState = new();
    public MoveState moveState = new();
    public StateMachine sm = new();
    private void Start()
    {
        rigWeightAnimator.Init();

        APlayerState[] states = new APlayerState[]
        {
            idleState,
            idleCrouchState,
            moveState, 
            moveCrouchState
        };

        foreach (var state in states)
        {
            state.SetStateMachine(sm);
            state.Init(rigWeightAnimator, playerMovement, playerInput);
        }


        if (logCurrentState)
        {
            sm.logCurrentState = true;
        }
        sm.AddState(idleState);
        sm.AddState(moveState);
        sm.AddState(idleCrouchState);
        sm.AddState(moveCrouchState);

        sm.SetState<IdleState>();
    }
    private void Update()
    {
        playerInput.Update();
        sm.Update();
    }
}
