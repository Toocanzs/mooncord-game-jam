using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Chain : MonoBehaviour
{
    private enum PullingType
    {
        PLAYER,
        SELF
    }

    [SerializeField]
    private GameObject chain;
    private Vector3 startPosition;
    private Arrow arrow;
    private float time = 0f;
    private bool pulling;
    [SerializeField]
    private float pullSpeed = 20f;
    [SerializeField]
    private float chainReleaseDistance = 1f;

    private List<MonoBehaviour> reenableAfterPull;
    private Player pulledPlayer;
    private PullingType pullingType;
    private Vector3 finalHitPoint;

    public Transform parent;
    void Start()
    {
        startPosition = transform.position;
        arrow = GetComponent<Arrow>();
        arrow.OnHit += OnHitHandler;
        Destroy(gameObject, 6f);
    }

    void Update()
    {
        if(pulling)
        {
            if (pullingType == PullingType.PLAYER)
            {
                transform.position = Vector3.MoveTowards(transform.position, parent.position, pullSpeed * Time.deltaTime);
                pulledPlayer.transform.position = transform.position;
                chain.transform.position = parent.position;
                chain.transform.rotation = chain.transform.RotationTo(Player.Instance.transform.position, 180f);
                if (math.distancesq(parent.position, pulledPlayer.transform.position) < chainReleaseDistance * chainReleaseDistance)
                {
                    foreach (var mono in reenableAfterPull)
                    {
                        mono.enabled = true;
                    }
                    Destroy(gameObject);
                }
            }
            else if(pullingType == PullingType.SELF)
            {
                parent.position = Vector3.MoveTowards(parent.position, finalHitPoint, pullSpeed * Time.deltaTime);
                startPosition = parent.position;
                transform.position = finalHitPoint;
                parent.GetComponent<StateMachine>().enabled = false;
                chain.transform.position = parent.position;
                chain.transform.rotation = chain.transform.RotationTo(finalHitPoint, 180f);
                if (math.distancesq(parent.position, transform.position) < chainReleaseDistance * chainReleaseDistance)
                {
                    parent.GetComponent<StateMachine>().enabled = true;
                    Destroy(gameObject);
                }
            }
            chain.transform.localScale = new Vector3(math.distance(parent.position, transform.position) * 2, 1, 1);
        }
        else
        {
            chain.transform.localScale = new Vector3(math.distance(parent.position, transform.position) * 2, 1, 1);
        }
    }

    void OnHitHandler(RaycastHit2D hit)
    {
        Player player = hit.collider.gameObject.GetComponent<Player>();
        if (player != null)
        {
            pulling = true;
            pulledPlayer = player;
            var playerWeapon = player.gameObject.GetComponent<PlayerWeapon>();
            var playerVelocity = player.gameObject.GetComponent<PlayerVelocity>();
            var playerMovement = player.gameObject.GetComponent<PlayerMovement>();
            var playerDash = player.gameObject.GetComponent<PlayerDash>();
            reenableAfterPull = new List<MonoBehaviour>() { playerWeapon, playerVelocity, playerMovement, playerDash};
            foreach (var mono in reenableAfterPull)
            {
                mono.enabled = false;
            }
            pullingType = PullingType.PLAYER;
            CameraShakeData.Instance.AddTrauma(0.4f);
        }
        else
        {
            pullingType = PullingType.SELF;
            pulling = true;
            finalHitPoint = transform.position;
            CameraShakeData.Instance.AddTrauma(0.2f);
        }
    }
}

public static class TransformExtension
{
    public static Quaternion RotationTo(this Transform transform, Vector3 other)
    {
        float2 dir = math.normalize(((float3)other).xy - ((float3)transform.position).xy);
        float angle = math.degrees(math.atan2(dir.y, dir.x));
        return Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public static Quaternion RotationTo(this Transform transform, Vector3 other, float offset)
    {
        float2 dir = math.normalize(((float3)other).xy - ((float3)transform.position).xy);
        float angle = math.degrees(math.atan2(dir.y, dir.x));
        return Quaternion.Euler(new Vector3(0, 0, angle + offset));
    }
}