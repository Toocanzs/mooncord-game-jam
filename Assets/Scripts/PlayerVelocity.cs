using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerVelocity : MonoBehaviour
{
    [HideInInspector]
    public float2 velocity;

    public Transform influence;

    new private Rigidbody2D rigidbody;
    private PlayerMovement playerMovement;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, math.normalize(velocity), math.length(velocity) * Time.deltaTime, PlayerLayerMask.Instance.layerMask);
        if(hit.transform != null)
        {
            velocity = 0;
        }
        if (influence != null)
        {
            float2 influenceDir = math.normalize(((float3)influence.position).xy - ((float3)transform.position).xy);
            rigidbody.velocity = velocity + (influenceDir * playerMovement.moveSpeed * 0.7f);
        }
        else
        {
            rigidbody.velocity = velocity;
        }
        
    }
}
