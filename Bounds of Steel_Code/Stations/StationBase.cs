using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(StationHealthSystem))]
public abstract class StationBase : MonoBehaviour
{

    //[SerializeField] private float maxHealth = 1000.0f;
    //[SerializeField] protected UnityEvent stationDestroyedEvent;

    private StationHealthSystem healthSystem;
    //private Collider coll;
    //protected float currentHealth = 0.0f;
    public bool stationActive { get; protected set; } = false;
    
    //protected float MaxHealth { get { return maxHealth; } }

    public delegate void StationActiveChanged();
    public StationActiveChanged stationActivated;
    public StationActiveChanged stationDeactivated;

    protected void Awake()
    {
        //coll = GetComponent<Collider>();
        healthSystem = GetComponent<StationHealthSystem>();
        //currentHealth = maxHealth;
        stationActive = false;
        //EventCoordinator.RegisterEventListener<DamageEventInfo>(ReceiveDirectDamage);
        //EventCoordinator.RegisterEventListener<AreaDamageEventInfo>(ReceiveAreaDamage);
    }

    //protected void OnDestroy()
    //{
    //    EventCoordinator.UnregisterEventListener<DamageEventInfo>(ReceiveDirectDamage);
    //    EventCoordinator.UnregisterEventListener<AreaDamageEventInfo>(ReceiveAreaDamage);
    //}

    protected void Start() { }

    protected void Update() { }

    //private void ReceiveDirectDamage(EventInfo ei)
    //{
    //    DamageEventInfo dei = (DamageEventInfo)ei;
    //    if(dei.ObjectHit.GetInstanceID() != gameObject.GetInstanceID())
    //    {
    //        return;
    //    }

    //    //CalculateDamageFromType(dei.DamageType, dei.Damage);
    //}

    //private void ReceiveAreaDamage(EventInfo ei)
    //{
    //    AreaDamageEventInfo adei = (AreaDamageEventInfo)ei;

    //    Vector3 closestPoint = coll.ClosestPoint(adei.OriginPoint);
    //    Vector3 positionDiff = closestPoint - adei.OriginPoint;

    //    if(positionDiff.sqrMagnitude > adei.Range * adei.Range || Vector3.Angle(positionDiff.normalized,  adei.Direction) > adei.MaxAngleDegrees)
    //    {
    //        return;
    //    }

    //    CalculateDamageFromType(adei.DamageType, adei.Damage);
    //}

    //private void CalculateDamageFromType(DamageType damageType, float damage)
    //{
    //    if (currentHealth > 0.0f && Mathf.Approximately(damageType.HealthModifier, 0.0f) == false)
    //    {
    //        damage *= damageType.HealthModifier; ;
    //        currentHealth -= damage;

    //        OnDamageTaken();

    //        if(currentHealth <= 0.0f)
    //        {
    //            currentHealth = 0.0f;
    //            RunStationDestroyedEvent();
    //        }
    //    }
    //}

    //private void RunStationDestroyedEvent()
    //{
    //    stationDestroyedEvent.Invoke();
    //}

    public void SetStationActive(bool toSet)
    {
        bool oldActive = stationActive;
        stationActive = toSet;

        if(oldActive != stationActive)
        {
            if (stationActive)
            {
                stationActivated.Invoke();
            }
            else
            {
                stationDeactivated.Invoke();
            }
        }
        // add some check for whether the station is destroyed or not
    }
}
