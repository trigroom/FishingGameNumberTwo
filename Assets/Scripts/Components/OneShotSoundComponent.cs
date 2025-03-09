using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OneShotSoundComponent 
{
    public AudioSource audioSource;
    public float time;

    public void Construct(AudioSource audioSource, float time)
    {
        this.audioSource = audioSource;
        this.time = time;
    }
}
