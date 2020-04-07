using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class WallHealthSystem : HealthSystem
{
    bool done = true;

    private void FixedUpdate()
    {
        if(base.CurrentHealth <= 0 && done)
        {
            done = !done;
            GateDestroyed();
        }
    }

    protected override void OnArmourDamageTaken(HealthComponent bodyPart, float damageTaken, Transform damageOriginPosition) { }

    protected override void OnHealthDamageTaken(HealthComponent bodyPart, float damageTaken, Transform damageOriginPosition) { }

    private void GateDestroyed()
    {
        GateDestroyedEventInfo gdei = new GateDestroyedEventInfo(gameObject, "Destroyed");
        EventCoordinator.ActivateEvent(gdei);
    }
}
