using MyExamples.Movement;
using MyExamples.SlugCat2;
using MyExamples.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;

    public RigWeightAnimator rigWeightAnimator = new();
    public bool logCurrentState;
    public IdleState idleState = new();
    public MoveState moveState = new();
    public StateMachine sm = new();
    private void Start()
    {
        rigWeightAnimator.Init();

        APlayerState[] states = new APlayerState[]
        {
            idleState,
            moveState
        };

        foreach (var state in states)
        {
            state.SetStateMachine(sm);
            state.Init(rigWeightAnimator, playerMovement);
        }


        if (logCurrentState)
        {
            sm.logCurrentState = true;
        }
        sm.AddState(idleState);
        sm.AddState(moveState);

        sm.SetState<IdleState>();
    }
    private void Update()
    {
        sm.Update();
    }
}
