using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;
    void Start()
    {
        
    }

    void Update()
    {
        float2 input = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (math.dot(input, input) != 0f)
            input = math.normalize(input);
        transform.position += (Vector3)new float3(input * Time.deltaTime * moveSpeed, 0);
    }
}
