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
    void Start()
    {
        startPosition = transform.position;
        arrow = GetComponent<Arrow>();
        arrow.OnHit += OnHitHandler;
    }

    void Update()
    {
        chain.transform.localScale = new Vector3(math.distance(startPosition, transform.position)*2, 1, 1);
        if(pulling)
        {
            if (pullingType == PullingType.PLAYER)
            {
                transform.position = Vector3.MoveTowards(transform.position, startPosition, pullSpeed * Time.deltaTime);
                pulledPlayer.transform.position = transform.position;
                if (math.distancesq(startPosition, pulledPlayer.transform.position) < chainReleaseDistance * chainReleaseDistance)
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
                transform.parent.position = Vector3.MoveTowards(transform.parent.position, transform.position, pullSpeed * Time.deltaTime);
                startPosition = transform.parent.position;
                transform.position = finalHitPoint;
                if (math.distancesq(transform.parent.position, transform.position) < chainReleaseDistance * chainReleaseDistance)
                {
                    Destroy(gameObject);
                }
            }
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
