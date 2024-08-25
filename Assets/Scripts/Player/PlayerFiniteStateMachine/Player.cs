using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStateMachine StateMachine { get; private set; }

    public Animator AnimatorController { get; private set; }

    private void Awake()
    {
        StateMachine = new PlayerStateMachine();
    }

    private void Start()
    {
        AnimatorController = GetComponentInChildren<Animator>();

        //TODO: Init stateMachine

    }

    private void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }
}
