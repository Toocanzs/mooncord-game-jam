using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(StateMachine), typeof(AudioSource))]
public class Boss1 : MonoBehaviour
{
    [HideInInspector]
    public StateMachine stateMachine;
    [HideInInspector]
    new public Rigidbody2D rigidbody;
    private Dictionary<Type, BaseState> states;
    [HideInInspector]
    public AudioSource audioSource;
    public AudioSource scanAudioSource;
    public AnimationCurve scanAudioCurve;
    public AnimationCurve scanSnapCurve;
    public AudioClip scanSnapClip;
    public AudioClip scanTargetFound;

    public GameObject shieldGameObject;
    [HideInInspector]
    public bool hitSnapState = false;

    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        rigidbody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        states = new Dictionary<Type, BaseState>
        {
            { typeof(PauseState), new PauseState(gameObject, 4f, typeof(ScanState))},
            { typeof(ScanState), new ScanState(this, 10f)},
            { typeof(SnapState), new SnapState(this, 8.5f)},
        };
        stateMachine.SetStates(states);
    }

    void Update()
    {
        if(Input.GetButtonDown("Fire2") && !hitSnapState)
        {
            stateMachine.SwitchToNextState(typeof(SnapState));
        }
    }
}
