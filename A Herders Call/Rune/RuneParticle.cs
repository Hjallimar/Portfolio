using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

public class RuneParticle : MonoBehaviour
{

    [SerializeField] private Color particleColor;
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private AudioSource myAudio;
    private bool active = false;
    private Vector3 rotationSpeed = new Vector3(-90,10,0);
    private Vector3 rotatorAdjustment;

    void Start()
    {
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.SwapToNight, StartParticleEffect);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.SwapToDay, StopParticleEffect);
        rotatorAdjustment.x = -90;
    }

    /// <summary>
    /// Starts a particle effect when it turns to night
    /// </summary>
    /// <param name="eventInfo"></param>
    private void StartParticleEffect(EventInfo eventInfo)
    {
        ps.Play();// = 0.5f;
        myAudio.Play();
        active = true;
        StartCoroutine(SpinMyParticles());
    }
    /// <summary>
    /// Stops the particle effect when it turns to day
    /// </summary>
    /// <param name="eventInfo"></param>
    private void StopParticleEffect(EventInfo eventInfo)
    {
        ps.Stop();//.emissionRate = 0f;
        active = false;
    }

    /// <summary>
    /// Rotates the particle system so that the particles flies in a circle
    /// </summary>
    /// <returns></returns>
private IEnumerator SpinMyParticles()
    {
        float volume = 1f;
        while (active)
        {
            rotatorAdjustment.y += rotationSpeed.y * Time.deltaTime;
            ps.transform.localRotation = Quaternion.Euler(rotatorAdjustment);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        while(volume >= 0f){
            volume -= 5 / Time.deltaTime;
            myAudio.volume = volume;
        }
    }
}
