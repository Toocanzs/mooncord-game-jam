using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CircleCollider2D))]
public class ReflectionShield : MonoBehaviour
{
    CircleCollider2D circleCollider;
    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform != null)
        {
            if (collision.transform.GetComponent<Player>() != null)
            {
                collision.transform.position = transform.position + 
                    Vector3.Normalize(collision.transform.position - transform.position)
                    *(circleCollider.radius + collision.collider.bounds.size.x*0.5f);
            }
        }
    }
}
