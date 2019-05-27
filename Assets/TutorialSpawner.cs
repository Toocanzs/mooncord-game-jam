using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private float shotsPerSecond = 10f;
    private float time = 0f;
    [SerializeField]
    private bool opening = false;
    public int count = 0;
    void Start()
    {
        time = 0f;
    }
    void Update()
    {
        time += Time.deltaTime;
        if(time > 1f/shotsPerSecond)
        {
            time = 0f;
            if(!opening || count > 6)
                Instantiate(prefab,transform.position, transform.rotation);
            count++;
            if (count > 35)
                count = 0;
        }
    }
}
