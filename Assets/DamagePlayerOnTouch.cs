using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayerOnTouch : MonoBehaviour
{
    [SerializeField]
    private bool dodgeable = true;
    void Start()
    {
        
    }
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        OnTouch(collision.collider);
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        OnTouch(collider);
    }

    void OnTouch(Collider2D collider)
    {
        Player player = collider.gameObject.GetComponent<Player>();
        if (player != null)
        {
            if (player.GetComponent<PlayerDash>().dashing == true && dodgeable)
                return;
            else
                player.Kill();
        }
    }
}
