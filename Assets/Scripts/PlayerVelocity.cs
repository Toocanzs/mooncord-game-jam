using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerVelocity : MonoBehaviour
{
    [HideInInspector]
    public float2 velocity;

    new private Rigidbody2D rigidbody;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, math.normalize(velocity), math.length(velocity) * Time.deltaTime, PlayerLayerMask.Instance.layerMask);
        if(hit.transform != null)
        {
            velocity = 0;
        }
        rigidbody.velocity = velocity;
    }
}
