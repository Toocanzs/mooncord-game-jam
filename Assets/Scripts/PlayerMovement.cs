using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
[RequireComponent(typeof(PlayerVelocity))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;
    [SerializeField]
    private LayerMask layerMask;
    private PlayerVelocity playerVelocity;
    void Start()
    {
        playerVelocity = GetComponent<PlayerVelocity>();
    }

    void FixedUpdate()
    {
        float2 input = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (math.dot(input, input) != 0f)
        {
            input = math.normalize(input);
            playerVelocity.velocity = input * moveSpeed;
        }
        else
        {
            playerVelocity.velocity = Vector2.zero;
        }
    }
}
