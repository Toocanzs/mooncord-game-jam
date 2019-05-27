using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
[RequireComponent(typeof(SpriteRenderer))]
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
    private bool destroyGameObject = false;
    [SerializeField]
    private AudioClip onHitClip;
    [SerializeField]
    private GameObject[] disableOnHit;

    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Material reflectedMaterial;

    float damage = 1f;

    public event Action<RaycastHit2D> OnHit = delegate { };

    void Start()
    {
        if(destroyTimeout)
            Destroy(gameObject, 6f);
        spriteRenderer = GetComponent<SpriteRenderer>();
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
                        if (GetComponent<DamagePlayerOnTouch>() == null)
                        {
                            spriteRenderer.material = reflectedMaterial;
                            gameObject.AddComponent<DamagePlayerOnTouch>();
                            var col = gameObject.AddComponent<BoxCollider2D>();
                            col.isTrigger = true;
                            col.size = new Vector2(0.8f, 0.2f);
                            gameObject.layer = LayerMask.NameToLayer("Enemy Projectile");
                            destroyGameObject = true;
                        }
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
                        AudioPlayer.Instance.PlayOneShot(onHitClip, 0.2f);
                    if (destroyOnHit)
                    {
                        if (destroyGameObject)
                            Destroy(gameObject);
                        else
                            Destroy(this);
                    }
                }
            }
        }
    }
}
