using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ChainSpawner : MonoBehaviour
{
    [SerializeField]
    private float releaseTime;
    private float time;

    [SerializeField]
    private GameObject chainPrefab;
    public GameObject boss;

    void OnEnable()
    {
        time = 0f;
    }
    void Update()
    {
        if (time > releaseTime)
        {
            var go = Instantiate(chainPrefab, boss.transform.position, Quaternion.identity, null);
            float2 dir = ((float3)(Player.Instance.transform.position - boss.transform.position)).xy;
            go.transform.right = new Vector3(dir.x, dir.y, 0);
            go.GetComponent<Chain>().parent = boss.transform;
            Destroy(gameObject);
        }
        time += Time.deltaTime;
    }
}
