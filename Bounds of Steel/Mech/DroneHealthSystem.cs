using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DroneHealthSystem : HealthSystem
{
    [SerializeField] protected UnityEvent droneDestroyedEvent;
    new protected void Awake()
    {
        base.Awake();
    }

    new protected void Start() { }

    // Update is called once per frame
    new protected void Update()
    {
        base.Update();
    }

    protected override void OnArmourDamageTaken(HealthComponent bodyPart, float damageTaken, Transform damageOriginPosition) { }

    protected override void OnHealthDamageTaken(HealthComponent bodyPart, float damageTaken, Transform damageOriginPosition)
    {
        if (CurrentHealth <= 0.0f)
        {
            droneDestroyedEvent.Invoke();
        }
    }

}
