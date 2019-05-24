using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab;

    private float attackCooldown = 0f;
    [SerializeField]
    private float attackSpeed = 1f;

    void Start()
    {
        
    }

    void Update()
    {
        float attacksPerSecond = 1f / attackSpeed;
        if (Input.GetButton("Fire1") && attackCooldown <= 0f)
        {
            float2 playerPos = ((float3)transform.position).xy;
            float2 mouseWorldPostion = ((float3)Camera.main.ScreenToWorldPoint(Input.mousePosition)).xy;

            float2 dir = math.normalize(mouseWorldPostion - playerPos);
            attackCooldown = attacksPerSecond;
            Shoot(dir);
        }
        if (attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;
    }

    private void Shoot(float2 dir)
    {
        float angle = math.degrees(math.atan2(dir.y, dir.x));
        GameObject arrow = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(new Vector3(0,0, angle)));
    }
}
