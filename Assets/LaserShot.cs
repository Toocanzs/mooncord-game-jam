using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LaserShot : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private GameObject colliderObject;

    [SerializeField]
    private AnimationCurve animationCurve;
    [SerializeField]
    private AudioClip shot;
    private AudioSource audioSource;

    private float time = 0f;
    [SerializeField]
    private float colliderOnThreshold = 0.8f;
    [SerializeField]
    private float maxTime = 1f;
    [SerializeField]
    private float laserWidth = 4f;

    private bool playedSound = false;
    void OnEnable()
    {
        colliderObject.SetActive(false);
        time = 0f;
        audioSource = GetComponent<AudioSource>();
        playedSound = false;
    }

    void Update()
    {
        var hit = Physics2D.Raycast(transform.position, transform.right, float.PositiveInfinity, layerMask);
        time += Time.deltaTime;
        float y = animationCurve.Evaluate(time / maxTime);
        if (time > maxTime)
            Destroy(gameObject);
        if (y > colliderOnThreshold)
        {
            if(!playedSound)
            {
                audioSource.PlayOneShot(shot);
                playedSound = true;
            }
            colliderObject.SetActive(true);
        }
        else
        {
            colliderObject.SetActive(false);
        }
        if(hit.transform != null)
        {
            float dist = math.distance(((float3)transform.position).xy, hit.point);
            transform.localScale = new Vector3(dist*2, y* laserWidth, 1);
        }
    }
}
