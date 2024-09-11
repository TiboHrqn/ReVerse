using UnityEngine;
using System;

//pas encore idéal car on fait beaucoup de record inutile
//pour eviter ca il faudrait ne record que le timeSamples au lancement de la musique, puis lors du replay utiliser le temps de la simulation pour retrouver le bon timeSamples

public class AudioSourceRecorder : PausableComponentRecorder 
{
    public AudioSource audioSource { get; private set; }

    public bool isPlaying { get; private set; }
    public AudioClip audioClip { get; private set; }
    public int timeSamples { get; private set; }
    public float volume { get; private set; }
    public float pitch { get; private set; }

    public AudioSourceRecorder(AudioSource audioSource)
    {
        this.audioSource = audioSource;
        audioClip = audioSource.clip;
        isPlaying = audioSource.isPlaying;
        timeSamples = audioSource.timeSamples;
        volume = audioSource.volume;
        pitch = audioSource.pitch;
    }

    public override bool IsStateDifferent()
    {
        if (isPlaying != audioSource.isPlaying || 
            audioClip != audioSource.clip || 
            timeSamples != audioSource.timeSamples || 
            volume != audioSource.volume || 
            pitch != audioSource.pitch)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override void EvenRecorderState()
    {
        audioClip = audioSource.clip;
        isPlaying = audioSource.isPlaying;
        timeSamples = audioSource.timeSamples;
        volume = audioSource.volume;
        pitch = audioSource.pitch;
    }

    protected override void EvenComponentState()
    {
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        if (!isPlaying && audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        else if (isPlaying && !audioSource.isPlaying)
            {
                audioSource.timeSamples = timeSamples;
                audioSource.Play();
            }
        

    
    }

    protected override ComponentState GetRecorderState()
    {
        return new AudioSourceState(isPlaying, audioClip, timeSamples, volume, pitch);
    }

    protected override void SetRecorderState(ComponentState compState)
    {
        if (compState is AudioSourceState asState)
        {
            isPlaying = asState.isPlaying;
            audioClip = asState.audioClip;
            timeSamples = asState.timeSamples;
            volume = asState.volume;
            pitch = asState.pitch;
        }
        else 
        {
            throw new Exception("Wrong component state type");
        }
    }

    protected override ComponentState LerpCompState(ComponentState stateA, ComponentState stateB, float p)
    {
        if (stateA is AudioSourceState asStateA && stateB is AudioSourceState asStateB)
        {
            return stateA; //A faire lorsqu'il n'y aura plus de record inutile
        }
        else
        {
            throw new Exception("Wrong component state type");
        }
    }

    public override void PauseComponent()
    {
        audioSource.Pause();
    }


}

public class AudioSourceState : ComponentState
{
    public bool isPlaying { get; private set; }
    public AudioClip audioClip { get; private set; }
    public int timeSamples { get; private set;}
    public float volume { get; private set; }
    public float pitch { get; private set; }

    public AudioSourceState(bool isPlaying, AudioClip audioClip, int timeSamples, float volume, float pitch)
    {
        this.isPlaying = isPlaying;
        this.audioClip = audioClip;
        this.timeSamples = timeSamples;
        this.volume = volume;
        this.pitch = pitch;
    }
}

