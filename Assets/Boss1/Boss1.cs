﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    [SerializeField]
    private AudioClip chainAlertSound;
    [SerializeField]
    private GameObject shotgunPrefab;
    [SerializeField]
    private GameObject smgPrefab;
    [SerializeField]
    private GameObject explodePrefab;
    [SerializeField]
    private GameObject laserPrefab;
    [SerializeField]
    private GameObject wallPrefab;
    public AudioSource audioLoopSource;

    [SerializeField]
    private AudioClip gravitySound;

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

    IEnumerator Chain()
    {
        AudioPlayer.Instance.PlayOneShot(chainAlertSound);
        yield return new WaitForSeconds(2.2f);
        var go = Instantiate(chainPrefab, transform.position, Quaternion.identity, null);
        float2 dir = ((float3)(Player.Instance.transform.position - transform.position)).xy;
        go.transform.right = new Vector3(dir.x, dir.y, 0);
        go.GetComponent<Chain>().parent = transform;
        yield return new WaitForSeconds(1f);
        FireRandomQuickAttack();
        //Probably fire more stuff along the way.
    }

    private float FireChain()
    {
        StartCoroutine(Chain());
        return 5f;
    }

    private float FireShotgun()
    {
        var go = Instantiate(shotgunPrefab, transform.position, Quaternion.identity, null);
        float2 dir = ((float3)(Player.Instance.transform.position - transform.position)).xy;
        go.transform.right = new Vector3(dir.x, dir.y, 0);
        go.GetComponent<SmgBurst>().parent = transform;
        return 0f;
    }

    private float FireSmg()
    {
        var go = Instantiate(smgPrefab, transform.position, Quaternion.identity, null);
        float2 dir = ((float3)(Player.Instance.transform.position - transform.position)).xy;
        go.transform.right = new Vector3(dir.x, dir.y, 0);
        go.GetComponent<SmgBurst>().parent = transform;
        return 0f;
    }

    private float FireExplode()
    {
        var go = Instantiate(explodePrefab, transform.position, Quaternion.identity, null);
        float2 dir = ((float3)(Player.Instance.transform.position - transform.position)).xy;
        go.transform.right = new Vector3(dir.x, dir.y, 0);
        return 2f;
    }

    IEnumerator Lasers(int num)
    {
        for (int i = 0; i < num; i++)
        {
            var go = Instantiate(laserPrefab, transform.position, Quaternion.identity, transform);
            float2 dir = ((float3)(Player.Instance.transform.position - transform.position)).xy;
            go.transform.right = new Vector3(dir.x, dir.y, 0);
            if (i != 0)
                go.transform.rotation = Quaternion.Euler(go.transform.rotation.eulerAngles + new Vector3(0,0,UnityEngine.Random.Range(-45f, 45f)));
            go.GetComponent<LaserShot>().targetRotation = go.transform.rotation;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private float FireLasers()
    {
        int x = UnityEngine.Random.Range(4, 8 + phase*3);
        StartCoroutine(Lasers(x));
        return x*0.2f;
    }

    private float FireWall()
    {
        var go = Instantiate(wallPrefab, transform.position, Quaternion.identity, null);
        float2 dir = ((float3)(Player.Instance.transform.position - transform.position)).xy;
        go.transform.right = new Vector3(dir.x, dir.y, 0);
        go.GetComponent<Arrow>().velocity = bossMoveSpeed * 1.4f;

        //fire another attack durring this
        return 1f;
    }

    IEnumerator Gravity()
    {
        Player.Instance.GetComponent<PlayerMovement>().influence = transform;
        GetComponent<StateMachine>().enabled = false;
        AudioPlayer.Instance.PlayOneShot(gravitySound, 3f);
        yield return new WaitForSeconds(2f);
        FireRandomQuickAttack();
        yield return new WaitForSeconds(2.2f);
        GetComponent<StateMachine>().enabled = true;
        Player.Instance.GetComponent<PlayerMovement>().influence = null;
    }

    private void FireRandomQuickAttack()
    {
        switch (UnityEngine.Random.Range(0, 5))
        {
            case 0:
                FireLasers();
                break;
            case 1:
                FireWall();
                break;
            case 2:
                FireSmg();
                break;
            case 3:
                FireShotgun();
                break;
            case 4:
                FireExplode();
                break;
            default:
                break;
        }
    }

    private float FireGravity()
    {
        //influence
        //Fire other attacks probably
        StartCoroutine(Gravity());
        return 5f;
    }


    public float ChooseAttack()
    {
        switch (UnityEngine.Random.Range(0, 7))
        {
            case 0:
                return FireLasers();
            case 1:
                return FireWall();
            case 2:
                return FireSmg();
            case 3:
                return FireShotgun();
            case 4:
                return FireExplode();
            case 5:
                return FireChain();
            case 6:
                return FireGravity();
            default:
                return 0f;
        }

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
