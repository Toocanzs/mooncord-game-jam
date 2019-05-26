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
        Destroy(gameObject, 6f);
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
                if (hit.transform.GetComponent<ReflectionShield>() != null)
                {
                    var reflected = Vector2.Reflect(transform.right, hit.normal);
                    var angle = Vector2.Angle(Vector2.right, reflected) *  Mathf.Rad2Deg;
                    transform.right = reflected;

                    transform.position = hit.transform.position 
                        + (Vector3.Normalize(transform.position - hit.transform.position) * hit.collider.bounds.size.x / 2f) 
                        + new Vector3(reflected.x, reflected.y, 0) * velocity * Time.deltaTime;
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
}
