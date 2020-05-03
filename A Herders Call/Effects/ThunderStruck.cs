using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson
public class ThunderStruck : MonoBehaviour
{

    [SerializeField] private GameObject particleHolder;
    [SerializeField] private AudioClip thunderSound;
    [SerializeField] private AudioSource giantAudioSource;
    
    void Start()
    {
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.StunGiant, CreateThunder);
    }

    /// <summary>
    /// Trigger a particle effect that shows a lightning above a ginat when a stungiant event is called
    /// </summary>
    /// <param name="eventInfo"></param>
    private void CreateThunder(EventInfo eventInfo)
    {
        var allaminaps = particleHolder.GetComponentsInChildren<ParticleSystem>();
        giantAudioSource.clip = thunderSound;
        giantAudioSource.Play();
        foreach (ParticleSystem ps in allaminaps)
        {
            ps.Play();
        }
    }
}
