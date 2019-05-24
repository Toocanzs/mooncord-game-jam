using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerDash : MonoBehaviour
{
    [SerializeField]
    float maxDashDistance = 1f;
    [SerializeField]
    float dashSpeed = 5f;
    private PlayerMovement playerMovement;

    float dashDistanceTraveled = 0f;
    bool dashing = false;
    float2 dashDirection = new float2(0,1);

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }
    void Update()
    {
        float2 input = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (math.dot(input, input) != 0f)
        {
            dashDirection = math.normalize(input);
        }
        if (Input.GetButtonDown("Jump") && !dashing)
        {
                dashing = true;
                playerMovement.enabled = false;
        }
        if(dashing)
        {
            float startDist = dashDistanceTraveled;
            dashDistanceTraveled = math.min(dashDistanceTraveled + Time.deltaTime * dashSpeed, maxDashDistance);
            float delta = dashDistanceTraveled - startDist;
            transform.position += new Vector3(dashDirection.x, dashDirection.y, 0) * delta;

            if (dashDistanceTraveled >= maxDashDistance)
            {
                dashing = false;
                playerMovement.enabled = true;
                dashDistanceTraveled = 0f;
            }
        }
    }
}
