using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VoiceRecord : MonoBehaviour
{
    private AudioClip recordedClip;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartRecord()
    {
        recordedClip = Microphone.Start("", false, 3000, 44100);
    }

    public void EndRecord()
    {
        Microphone.End(null);
    }

    public void StartReplay(float time)
    {
        if (recordedClip != null && time < recordedClip.length)
        {
            audioSource.clip = recordedClip;
            audioSource.time = time;
            audioSource.Play();
        }
    }

    public void PauseReplay()
    {
        audioSource.Pause();
    }
}
