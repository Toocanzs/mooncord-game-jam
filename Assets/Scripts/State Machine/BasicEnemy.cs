using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D),typeof(StateMachine))]
public class BasicEnemy : MonoBehaviour
{
    private StateMachine stateMachine;
    [HideInInspector]
    new public Rigidbody2D rigidbody;
    public float wanderRange = 3f;
    public float wanderSpeed = 3f;
    public float wanderResetTime = 3f;
    private Dictionary<Type, BaseState> states;
    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        rigidbody = GetComponent<Rigidbody2D>();
        states = new Dictionary<Type, BaseState>
        {
            { typeof(WanderState), new WanderState(this)},
        };
        stateMachine.SetStates(states);
    }

    void Update()
    {
        
    }
}
