using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
public class Arrow : MonoBehaviour
{
    [SerializeField]
    private float velocity = 5f;
    private bool hitWall = false;
    [SerializeField]
    private LayerMask layerMask;
    void Start()
    {
        
    }

    void Update()
    {
        if (!hitWall)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, velocity * Time.deltaTime, layerMask);
            if (hit.transform == null)
            {
                transform.position += transform.right * velocity * Time.deltaTime;
            }
            else
            {
                transform.position = hit.point;
                hitWall = true;
                Destroy(this);
            }
        }
    }
}
