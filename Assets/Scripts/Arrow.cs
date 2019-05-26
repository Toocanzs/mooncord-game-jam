using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

[RequireComponent(typeof(AudioSource))]
public class Arrow : MonoBehaviour
{
    [SerializeField]
    private float velocity = 5f;
    private bool hitWall = false;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private bool reflectable = true;
    [SerializeField]
    private bool destroyOnHit = true;
    [SerializeField]
    private bool destroyTimeout = true;
    [SerializeField]
    private bool parentOnHit = true;
    [SerializeField]
    private AudioClip onHitClip;
    private AudioSource audioSource;
    [SerializeField]
    private GameObject[] disableOnHit;

    float damage = 1f;

    public event Action<RaycastHit2D> OnHit = delegate { };

    void Start()
    {
        if(destroyTimeout)
            Destroy(gameObject, 6f);
        audioSource = GetComponent<AudioSource>();
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
                if (reflectable && hit.collider.transform.GetComponent<ReflectionShield>() != null)
                {
                    ReflectionShield shield = hit.collider.transform.GetComponent<ReflectionShield>();
                    if (shield.deletesOnHit)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        var reflected = Vector2.Reflect(transform.right, hit.normal).normalized;
                        transform.right = reflected;

                        transform.position = new Vector3(hit.point.x, hit.point.y, 0)
                            + ((new Vector3(reflected.x, reflected.y, 0) * velocity * Time.deltaTime));
                    }
                }
                else
                {
                    if (hit.collider.transform.GetComponent<Battery>() != null)
                        hit.collider.transform.GetComponent<Battery>().Hit(damage);

                    transform.position = hit.point;
                    hitWall = true;
                    foreach(var go in disableOnHit)
                    {
                        go.SetActive(false);
                    }
                    OnHit(hit);
                    if (parentOnHit)
                        transform.parent = hit.collider.transform;
                    if(onHitClip != null)
                        audioSource.PlayOneShot(onHitClip);
                    if(destroyOnHit)
                        Destroy(this);
                }
            }
        }
    }
}
