using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class FogOfWarCamera : MonoBehaviour
{
    public static Camera Instance;
    void Start()
    {
        if(Instance == null)
        {
            Instance = GetComponent<Camera>();
        }
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
