﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerVelocity), typeof(CircleCollider2D))]
public class PlayerDash : MonoBehaviour
{
    [SerializeField]
    float maxDashDistance = 1f;
    [SerializeField]
    float dashSpeed = 5f;
    [SerializeField]
    private AudioClip dashSound;
    [SerializeField]
    private float dodgeCooldown = 0.3f;
    private float dodgeCooldownTime = 0f;

    private AudioSource audioSource;

    private PlayerMovement playerMovement;

    public float dashTime;
    public bool dashing = false;
    float2 dashEndPoint = 0;
    float2 dashStartPoint = 0;
    new private CircleCollider2D collider;
    float2 dashDirection = 0;
    PlayerVelocity playerVelocity;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        collider = GetComponent<CircleCollider2D>();
        playerVelocity = GetComponent<PlayerVelocity>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        dodgeCooldownTime = dodgeCooldown;
    }
    void OnDisable()
    {
        dashing = false;
    }

    void Update()
    {
        float2 input = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (math.dot(input, input) != 0f)
        {
            dashDirection = math.normalize(input);
        }
        if (Input.GetButtonDown("Jump") && !dashing && dodgeCooldownTime > dodgeCooldown)
        {
            AudioPlayer.Instance.PlayOneShot(dashSound, 0.5f);
            dashing = true;
            dodgeCooldownTime = 0f;
            playerMovement.enabled = false;
            dashTime = 0f;
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, collider.radius, dashDirection, maxDashDistance, PlayerLayerMask.Instance.layerMask);
            dashStartPoint = ((float3)transform.position).xy;
            if (hit.transform == null)
            {
                dashEndPoint = ((float3)transform.position).xy + dashDirection * maxDashDistance;
            }
            else
            {
                dashEndPoint = ((float3)transform.position).xy + dashDirection * maxDashDistance * hit.fraction;
            }
        }
        if(dashing)
        {
            spriteRenderer.sharedMaterial.SetInt("dashing", 1);
            playerVelocity.velocity = 0;
            float dashTotalTime = math.distance(dashStartPoint, dashEndPoint) / dashSpeed;
            if (dashTotalTime <= 0f)
            {
                dashing = false;
                playerMovement.enabled = true;
            }
            else
            {
                dashTime += Time.deltaTime;
                float2 pos = math.lerp(dashStartPoint, dashEndPoint, math.saturate(dashTime / dashTotalTime));
                transform.position = new Vector3(pos.x, pos.y, 0);
                if (dashTime > dashTotalTime)
                {
                    dashing = false;
                    playerMovement.enabled = true;
                }
            }
        }
        else
        {
            spriteRenderer.sharedMaterial.SetInt("dashing", 0);
        }
        dodgeCooldownTime += Time.deltaTime;
    }
}
