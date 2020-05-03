using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticFootsteps : MonoBehaviour
{
    private AudioSource footSource;
    public AudioClip footClip;

    private double time;
    private float filterTime;
    
    void Start()
    {
        footSource = GetComponent<AudioSource>();
        time = AudioSettings.dspTime;
        filterTime = 0.2f;
    }

    /// <summary>
    /// plays the footstep sounds matched with the animation speed
    /// </summary>
    void PlayStaticFootstepSound()
    {
        if (AudioSettings.dspTime < time + filterTime)
        {
            return;
        }
        footSource.PlayOneShot(footClip);

        time = AudioSettings.dspTime;
    }
}
