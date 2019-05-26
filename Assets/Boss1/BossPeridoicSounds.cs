using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class BossPeridoicSounds : MonoBehaviour
{
    float time = 0f;
    public float maxTime = 0f;
    int plays = 0;
    private AudioSource audioSource;
    [SerializeField]
    private List<AudioClip> audioClips;
    float startPlayTime = 1f;
    void OnEnable()
    {
        time = 0f;
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (time > maxTime)
            Destroy(this);
        time += Time.deltaTime;
        if(time > startPlayTime)
        {
            AudioClip clip = audioClips[Random.Range(0, audioClips.Count)];
            startPlayTime += clip.length;
            audioClips.Remove(clip);
            audioSource.PlayOneShot(clip);
        }
    }
}
