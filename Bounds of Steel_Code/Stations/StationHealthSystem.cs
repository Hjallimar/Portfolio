using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StationHealthSystem : HealthSystem
{
    [SerializeField] private bool isMainBase = false;
    [SerializeField] protected GameObject DestroyedParticles = null;
    [SerializeField] protected UnityEvent stationDestroyedEvent;

    [SerializeField] protected UnityEvent stationLightDamage;
    [SerializeField] protected UnityEvent stationModerateDamage;
    [SerializeField] protected UnityEvent stationHeavyDamage;
    [SerializeField] protected UnityEvent stationTargetedEvent;

    [SerializeField] private AudioClip deathVoiceline = null;

    new protected void Awake()
    {
        base.Awake();
        EventCoordinator.RegisterEventListener<LaneTargetUpdatedEventInfo>(LaneTargetUpdatedListener);
    }

    new protected void Start()
    {
        BalanceFileHolder.BalanceVariableNames healthVarName = BalanceFileHolder.BalanceVariableNames.barrier_health;
        BalanceFileHolder.BalanceVariableNames armourVarName = BalanceFileHolder.BalanceVariableNames.barrier_armour;
        if(isMainBase)
        {
            healthVarName = BalanceFileHolder.BalanceVariableNames.main_base_health;
            armourVarName = BalanceFileHolder.BalanceVariableNames.main_base_armour;
        }

        SetCustomBalanceHealth(GameController.GetBalanceVariable(healthVarName));
        SetCustomBalanceArmour(GameController.GetBalanceVariable(armourVarName));
        
    }
    
    // Update is called once per frame
    new protected void Update()
    {
        base.Update();
    }

    protected override void OnArmourDamageTaken(HealthComponent bodyPart, float damageTaken, Transform damageOriginPosition)
    {  
    }

    protected override void OnHealthDamageTaken(HealthComponent bodyPart, float damageTaken, Transform damageOriginPosition)
    {
        if (CurrentHealth < (MaxHealth * 0.75f) && CurrentHealth > (MaxHealth * 0.5f))
        {
            stationLightDamage.Invoke();
        }
        else if (CurrentHealth < (MaxHealth * 0.5f) && CurrentHealth > (MaxHealth * 0.25f))
        {
            stationModerateDamage.Invoke();
        }
        else if (CurrentHealth < (MaxHealth * 0.25f) && CurrentHealth > 0)
        {
            stationHeavyDamage.Invoke();
        }
        else if (CurrentHealth <= 0.0f)
        {
            stationDestroyedEvent.Invoke();
            LaneTargetDestroyedEventInfo ltdei = new LaneTargetDestroyedEventInfo(gameObject, this);
            EventCoordinator.ActivateEvent(ltdei);
            ParticleEventInfo psei = new ParticleEventInfo(gameObject, "Particles", DestroyedParticles, transform.position, transform.rotation);
            EventCoordinator.ActivateEvent(psei);
            TextLogEventInfo tlei = new TextLogEventInfo(gameObject, name + " has been destroyed!");
            EventCoordinator.ActivateEvent(tlei);
            VoiceLineEventInfo vlei = new VoiceLineEventInfo(gameObject, deathVoiceline);
            EventCoordinator.ActivateEvent(vlei);
        }
    }

    private void LaneTargetUpdatedListener(EventInfo ei)
    {
        LaneTargetUpdatedEventInfo ltuei = (LaneTargetUpdatedEventInfo)ei;

        if(ltuei.NewTarget == gameObject)
        {
            stationTargetedEvent.Invoke();
            EventCoordinator.UnregisterEventListener<LaneTargetUpdatedEventInfo>(LaneTargetUpdatedListener);
        }
    }

    private void OnDisable()
    {
        EventCoordinator.UnregisterEventListener<LaneTargetUpdatedEventInfo>(LaneTargetUpdatedListener);
    }
}
