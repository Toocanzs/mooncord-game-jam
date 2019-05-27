using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField]
    private RoomLaser roomLaser;

    public float currentHp;
    [SerializeField]
    public float maxHp = 600;
    public GameObject hpBarObject;

    private int totalPhases = 6;
    private float nextHpTick;
    [SerializeField]
    private AudioClip[] phaseChanges;

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
        currentHp = maxHp;
        nextHpTick = maxHp - (maxHp / (totalPhases+1));
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire2") && !hitSnapState)
        {
            stateMachine.SwitchToNextState(typeof(SnapState));
            var snapstate = ((SnapState)states[typeof(SnapState)]);
            snapstate.time = snapstate.totalWaitTime - 1f;
            Destroy(scanAudioSource.gameObject);
        }
        if(currentHp < nextHpTick)
        {
            AudioPlayer.Instance.PlayOneShot(phaseChanges[math.min(phase, totalPhases)]);
            phase++;
            nextHpTick -= (maxHp / (totalPhases + 1));
        }
        if(currentHp <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
    }

    private float FireChain()
    {
        StartCoroutine(Chain());
        return 4f;
    }

    private float FireShotgun()
    {
        var go = Instantiate(shotgunPrefab, transform.position, Quaternion.identity, null);
        float2 dir = ((float3)(Player.Instance.transform.position - transform.position)).xy;
        go.transform.right = new Vector3(dir.x, dir.y, 0);
        go.GetComponent<SmgBurst>().parent = transform;
        return 0f;
    }

    public void Hit(float damage)
    {
        currentHp -= damage;
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
        return 0f;
    }

    IEnumerator Lasers(int num, List<float> rand)
    {
        for (int i = 0; i < num; i++)
        {
            var go = Instantiate(laserPrefab, transform.position, Quaternion.identity, transform);
            float2 dir = ((float3)(Player.Instance.transform.position - transform.position)).xy;
            go.transform.right = new Vector3(dir.x, dir.y, 0);
            go.transform.rotation = Quaternion.Euler(go.transform.rotation.eulerAngles + new Vector3(0, 0, rand[i]));
            go.GetComponent<LaserShot>().targetRotation = go.transform.rotation;
            go.GetComponent<LaserShot>().trauma = 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private float FireLasers()
    {
        int x = UnityEngine.Random.Range(4 + (phase / 2), 8 + (phase/4));
        List<float> rand = new List<float>();
        for(int i = 0; i < x; i++)
        {
            if (i == 0)
                rand.Add(0f);
            else
                rand.Add(UnityEngine.Random.Range(-45f, 45f));
        }
        rand.Sort();
        if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
            rand.Reverse();
        StartCoroutine(Lasers(x, rand));
        return x * 0.2f;
    }

    IEnumerator Wall()
    {
        var go = Instantiate(wallPrefab, transform.position, Quaternion.identity, null);
        float2 dir = ((float3)(Player.Instance.transform.position - transform.position)).xy;
        go.transform.right = new Vector3(dir.x, dir.y, 0);
        go.GetComponent<Arrow>().velocity = bossMoveSpeed * 1.4f;
        yield return new WaitForSeconds(1f);
        FireRandomQuickAttack();
    }

    private float FireWall()
    {
        StartCoroutine(Wall());
        //fire another attack durring this
        return 1f;
    }

    IEnumerator Gravity()
    {
        Player.Instance.GetComponent<PlayerVelocity>().influence = transform;
        GetComponent<StateMachine>().enabled = false;
        AudioPlayer.Instance.PlayOneShot(gravitySound, 3f);
        yield return new WaitForSeconds(2f);
        FireRandomQuickAttack();
        yield return new WaitForSeconds(2f);
        GetComponent<StateMachine>().enabled = true;
        Player.Instance.GetComponent<PlayerVelocity>().influence = null;
    }

    private void FireRandomQuickAttack()
    {
        switch (UnityEngine.Random.Range(0, 4))
        {
            case 0:
                FireLasers();
                break;
            case 1:
                FireSmg();
                break;
            case 2:
                FireShotgun();
                break;
            case 3:
                FireExplode();
                break;
            default:
                break;
        }
    }

    private float FireGravity()
    {
        StartCoroutine(Gravity());
        return 4f;
    }

    public float FireRoomLaser()
    {
        switch (UnityEngine.Random.Range(0, 2 + math.max(phase - 2, 0)))
        {
            case 0:
                roomLaser.FireHorizontal();
                break;
            case 1:
                roomLaser.FireVertical();
                break;
            case 2:
                roomLaser.FireHorizontal();
                roomLaser.FireVertical();
                break;
            case 3:
                roomLaser.FireHorizontalSplit();
                break;
            case 4:
                roomLaser.FireVerticalSplit();
                break;
            default:
                roomLaser.FireVerticalSplit();
                roomLaser.FireHorizontalSplit();
                break;
        }
        return 4f;
    }

    private float FireDodgeableWall()
    {
        roomLaser.FireDodgebable();
        return 4f;
    }

    public float ChooseAttack()
    {
        switch (UnityEngine.Random.Range(0, 8 + (phase > 1 ? 1 : 0)))
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
                return FireChain();
            case 5:
                return FireExplode();
            case 6:
                return FireGravity();
            case 7:
                return FireDodgeableWall();
            case 8:
                return FireRoomLaser();
            default:
                return 0f;
        }

    }

    public float GetTimeBetweenAttacks()
    {
        switch (phase)
        {
            case 0:
                return 3f;
            case 1:
                return 3f;
            case 2:
                return 2.5f;
            case 3:
                return 2f;
            default:
                return 2f;
        }
    }
}
