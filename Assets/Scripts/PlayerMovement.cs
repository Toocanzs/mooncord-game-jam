using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;
    [SerializeField]
    private LayerMask layerMask;

    private BoxCollider2D boxCollider;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        float2 input = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (math.dot(input, input) != 0f)
        {
            input = math.normalize(input);
            float movementDelta = Time.deltaTime * moveSpeed;
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, input, movementDelta, layerMask);
            Debug.Log("norm: " + hit.normal);
            if(hit.transform == null)
            {
                transform.position += (Vector3)new float3(input * movementDelta, 0);
            }
            else
            {
                transform.position += (Vector3)new float3(input * movementDelta * hit.fraction, 0);
            }
        }
    }
}
