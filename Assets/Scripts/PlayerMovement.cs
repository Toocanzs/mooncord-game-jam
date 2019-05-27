using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
[RequireComponent(typeof(PlayerVelocity))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;
    private PlayerVelocity playerVelocity;
    public Transform influence;
    void Start()
    {
        playerVelocity = GetComponent<PlayerVelocity>();
    }

    void FixedUpdate()
    {
        float2 input = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input = math.normalizesafe(input, float2.zero);
        playerVelocity.velocity = input * moveSpeed;
        if (influence != null)
        {
            float2 influenceDir = math.normalize(((float3)influence.position).xy - ((float3)transform.position).xy);
            playerVelocity.velocity += influenceDir * moveSpeed * 0.7f;
        }
    }
}
