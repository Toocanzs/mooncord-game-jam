using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
[RequireComponent(typeof(PlayerVelocity))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    private PlayerVelocity playerVelocity;
    void Start()
    {
        playerVelocity = GetComponent<PlayerVelocity>();
    }

    void FixedUpdate()
    {
        float2 input = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input = math.normalizesafe(input, float2.zero);
        playerVelocity.velocity = input * moveSpeed;
    }
}
