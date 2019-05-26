using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D),typeof(StateMachine))]
public class BasicEnemy : MonoBehaviour
{
    private StateMachine stateMachine;
    new private Rigidbody2D rigidbody;
    [SerializeField]
    private float wanderRange = 3f;
    [SerializeField]
    private float wanderSpeed = 3f;
    [SerializeField]
    private float wanderResetTime = 3f;
    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var states = new Dictionary<Type, BaseState>
        {
            { typeof(WanderState), new WanderState(gameObject, rigidbody, wanderRange, wanderSpeed, wanderResetTime)},
        };
        stateMachine.SetStates(states);
    }
}
