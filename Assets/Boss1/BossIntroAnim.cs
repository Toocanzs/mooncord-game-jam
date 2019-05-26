using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class BossIntroAnim : MonoBehaviour
{
    [SerializeField]
    private bool playSound = false;
    [SerializeField]
    private int soundNum = 0;
    [SerializeField]
    private float volume = 1f;
    private bool lastPlaySoundBool = false;

    [SerializeField]
    private AudioClip[] audioClips;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if(playSound && lastPlaySoundBool == false)
        {
            audioSource.PlayOneShot(audioClips[soundNum], volume);
        }
        lastPlaySoundBool = playSound;
    }
}
