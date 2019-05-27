using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Explode : MonoBehaviour
{
    [SerializeField]
    private GameObject shotgunGameObject;
    [SerializeField]
    private float explodeDistance = 5f;

    private Vector3 startPos;
    private Vector3 lastPos;
    void OnEnable()
    {
        startPos = transform.position;
    }

    void Start()
    {
        
    }
    void Update()
    {
        if (math.distancesq(startPos, transform.position) > explodeDistance * explodeDistance)
        {
            Destroy(gameObject);
            return;
        }
        lastPos = transform.position;
    }

    void OnDisable()
    {
        Vector3 dif = math.normalize(lastPos - transform.position);
        GameObject go = Instantiate(shotgunGameObject, transform.position + dif*0.1f, transform.rotation);
        var burst = go.GetComponent<SmgBurst>();
        burst.angle = 360;
        burst.shotNum = 50;
        burst.totalShotTime = 0f;
        burst.totalTrauma = 0.1f;
    }
}
