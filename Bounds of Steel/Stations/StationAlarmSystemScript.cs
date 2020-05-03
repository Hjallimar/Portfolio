using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationAlarmSystemScript : MonoBehaviour
{
    [Header("Health system for the station")]
    [SerializeField] private StationHealthSystem healthSystem = null;
    [Space]
    [Header("Particle prefabs for damaged and destroyed")]
    [SerializeField] private ParticleSystem[] particleSystems = null;
    [Space]
    [Header("Lights for the damage")]
    [SerializeField] private Light[] lights = null;
    [SerializeField] private float intensity = 1000;
    [SerializeField] private Gradient colorGradient = new Gradient();
    [SerializeField] private float flashSpeed = 2;
    [Space]
    [Header("Audio for the station")]
    [SerializeField] private AudioSource audioSource = null;

    private bool go = false;
    private float intens;
    // Start is called before the first frame update
    void Start()
    {
        intens = intensity;
        flashSpeed = -flashSpeed;
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (go)
        {
            intens += flashSpeed;
            foreach (Light light in lights)
            {
                light.intensity = intens;
            }

            if (intens <= 10)
            {
                flashSpeed = -flashSpeed;
            }
            else if (intens >= intensity)
            {
                flashSpeed = -flashSpeed;
            }
        }
    }

    public void LightDamage()
    {
        go = true;
        if(!audioSource.isPlaying)
            audioSource.Play();
        Color temp = colorGradient.Evaluate(healthSystem.CurrentHealth / healthSystem.MaxHealth);
        foreach (Light light in lights)
        {
            light.color = temp;
        }
    }

    public void ModerateDamage()
    {
        Color temp = colorGradient.Evaluate(healthSystem.CurrentHealth / healthSystem.MaxHealth);
        foreach (Light light in lights)
        {
            light.color = temp;
        }
        foreach(ParticleSystem ps in particleSystems)
        {
            ps.Play();
        }
    }

    public void HeavyDamage()
    {
        Color temp = colorGradient.Evaluate(healthSystem.CurrentHealth / healthSystem.MaxHealth);
        foreach (Light light in lights)
        {
            light.color = temp;
        }
    }

    public void StructureDied()
    {
        go = false;
        audioSource.Stop();
        foreach (Light light in lights)
        {
            light.intensity = 0;
        }
        foreach (ParticleSystem ps in particleSystems)
        {
           // ps.Stop();
        }
    }



}
