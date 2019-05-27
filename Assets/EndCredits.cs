using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCredits : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private float time = 0f;
    [SerializeField]
    private AudioClip[] audioClips;

    private AudioSource audioSource;
    private int played = 0;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
        time += Time.deltaTime;
        if(time > 12f && played < 1)
        {
            audioSource.PlayOneShot(audioClips[played]);
            played++;
        }

        if (time > 17f && played < 2)
        {
            audioSource.PlayOneShot(audioClips[played]);
            played++;
        }
    }
}
