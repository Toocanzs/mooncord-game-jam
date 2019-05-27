using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    public static AudioSource Instance;
    void Start()
    {
        if(Instance == null)
        {
            Instance = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(this);
        }
    }

    void Update()
    {
        
    }
}
