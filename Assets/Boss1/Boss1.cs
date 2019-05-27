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

    public BatteryManager batteryManager;
    [HideInInspector]
    public bool hitSnapState = false;

    public float bossMoveSpeed = 1f;

    public int phase = 0;

    [SerializeField]
    private GameObject chainPrefab;

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
            { typeof(ChaseState), new ChaseState(this)},
        };
        stateMachine.SetStates(states);
    }

    void Update()
    {
        if(Input.GetButtonDown("Fire2") && !hitSnapState)
        {
            stateMachine.SwitchToNextState(typeof(SnapState));
            var snapstate = ((SnapState)states[typeof(SnapState)]);
            snapstate.time = snapstate.totalWaitTime - 1f;
            Destroy(scanAudioSource.gameObject);
        }
    }

    public void UpdatePhase()
    {

    }

    private float FireChain()
    {
        var go = Instantiate(chainPrefab, transform.position, transform.rotation);
        go.GetComponent<ChainSpawner>().boss = gameObject;
        return 3f;
    }

    public float ChooseAttack()
    {
        return FireChain();
    }

    public float GetTimeBetweenAttacks()
    {
        switch(phase)
        {
            case 0:
                return 3f;
            case 1:
                return 2.5f;
            case 2:
                return 2f;
            case 3:
                return 1.5f;
            default:
                return 1f;
        }
    }
}
