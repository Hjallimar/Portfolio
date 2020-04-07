using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoolSystem : MonoBehaviour
{

    [SerializeField] private float dangerLevel = 500;
    [SerializeField] private float currentHeat = 0;
    [SerializeField] private float overheatDamagePerHeatPerSecond = 1.0f;

    [SerializeField] private float maxCoolant = 1500;
    [SerializeField] private float flushTimer = 3f;


    [SerializeField] private HealthComponent healthComp;
    [SerializeField, Tooltip("This should be set to the damage type called \"Internal_Overheat\"")] private DamageType overheatDamageType;

    [SerializeField] private float transferedHeat = 0;
    [SerializeField] private float heatTravelTime = 10;
    [SerializeField] private float passiveHeatReductionPerSecond = 5;

    float drainPerSecond = 0;
    bool overHeatedReactor = false;
    float reactorHeat = 0;

    float currentCoolant = 0;
    bool flushing = false;
    float amountToFlush = 0;
    float flushingLeft = 0;

    bool refillingCoolant = false;
    float coolantPerSecond = 0;


    void Start()
    {
        EventCoordinator.RegisterEventListener<ReactorOverchargeBeginEventInfo>(OverChargeReactorStart);
        EventCoordinator.RegisterEventListener<ReactorOverchargeEndEventInfo>(OverChargeReactorStop);
        EventCoordinator.RegisterEventListener<TransferToMainHeatSinkEventInfo>(AddDrainedHeat);
        EventCoordinator.RegisterEventListener<FlushCoolantEventInfo>(FlushCoolant);
        EventCoordinator.RegisterEventListener<CoolantRefillStatusEventInfo>(RefillCoolant);
        currentCoolant = maxCoolant;
        currentCoolant = 1000;
    }

    private void Update()
    {
        if (healthComp.CurrentHealth <= 0)
        {
            SystemMalfunction();
        }

        if (flushing)
        {
            FlushTheSystem();
        }

        PassiveHeatManipulation();

        if (overHeatedReactor)
            currentHeat += reactorHeat * Time.deltaTime;

        if (refillingCoolant)
            Refill();

        if(currentHeat > dangerLevel)
        {
            //Deal damage to the mech
            //DamageEventInfo dei = new DamageEventInfo(gameObject, gameObject, (currentHeat - dangerLevel) * overheatDamagePerHeatPerSecond * Time.deltaTime, overheatDamageType);
            //EventCoordinator.ActivateEvent(dei);
        }

        //int coolantProcent = (int)((currentCoolant / maxCoolant) * 100);
        //coolantText.text = "Coolant Tank: " + coolantProcent + "%";
        ////heatLevelText.text = "Current Tempurature: " + ((int)currentHeat + 3000);
        //heatLevelText.text = "Current Temperature: " + ((int)(100.0f * currentHeat / dangerLevel)).ToString() + "%";
    }
    
    private void OnDestroy()
    {
        EventCoordinator.UnregisterEventListener<ReactorOverchargeBeginEventInfo>(OverChargeReactorStart);
        EventCoordinator.UnregisterEventListener<ReactorOverchargeEndEventInfo>(OverChargeReactorStop);
        EventCoordinator.UnregisterEventListener<TransferToMainHeatSinkEventInfo>(AddDrainedHeat);
        EventCoordinator.UnregisterEventListener<FlushCoolantEventInfo>(FlushCoolant);
        EventCoordinator.RegisterEventListener<CoolantRefillStatusEventInfo>(RefillCoolant);
    }
    
    private void PassiveHeatManipulation()
    {
        if(transferedHeat > 0)
        {
            currentHeat += drainPerSecond * Time.deltaTime;
            transferedHeat -= drainPerSecond * Time.deltaTime;
        }
        else
        {
            drainPerSecond = 0;
            transferedHeat = 0;
        }

        if(currentHeat > 0)
            currentHeat -= passiveHeatReductionPerSecond * Time.deltaTime;
    }

    private void AddDrainedHeat(EventInfo ei)
    {
        TransferToMainHeatSinkEventInfo ttmhei = (TransferToMainHeatSinkEventInfo)ei;
        transferedHeat += ttmhei.Ammount;
        drainPerSecond = transferedHeat / heatTravelTime;
    }
   
    private void OverChargeReactorStart(EventInfo ei)
    {
        ReactorOverchargeBeginEventInfo robei = (ReactorOverchargeBeginEventInfo)ei;
        //if (robei.GO == gameObject)
        //{
            overHeatedReactor = true;
            reactorHeat = robei.HeatPerSecond;
        //}
    }

    private void OverChargeReactorStop(EventInfo ei)
    {
        ReactorOverchargeEndEventInfo roeei = (ReactorOverchargeEndEventInfo)ei;
        //if(roeei.GO == gameObject)
        //{
            overHeatedReactor = false;
            reactorHeat = 0;
        //}
    }


    private void SystemMalfunction()
    {
        CoolingSystemMalfunctionEventInfo csm = new CoolingSystemMalfunctionEventInfo(gameObject, "Shut it down");
        EventCoordinator.ActivateEvent(csm);
    }

    private void FlushCoolant(EventInfo ei)
    {
        if(currentCoolant > 0 && !flushing)
        {
            if((currentCoolant - currentHeat) > 0)
            {
                flushing = true;
                amountToFlush = currentHeat;
                currentCoolant -= currentHeat;
                flushingLeft = amountToFlush;
            }
            else
            {
                flushing = true;
                amountToFlush = currentCoolant;
                currentCoolant = 0;
                flushingLeft = amountToFlush;
            }
        }
    }

    private void FlushTheSystem()
    {
        currentHeat -= amountToFlush * Time.deltaTime * flushTimer;
        flushingLeft -= amountToFlush * Time.deltaTime * flushTimer;
        if (flushingLeft <= 0)
            flushing = false;
    }


    private void RefillCoolant(EventInfo ei)
    {
        CoolantRefillStatusEventInfo crsei = (CoolantRefillStatusEventInfo)ei;
        refillingCoolant = crsei.Status;
        coolantPerSecond = crsei.Coolant;
    }

    private void Refill()
    {
        currentCoolant += coolantPerSecond * Time.deltaTime;
        if(currentCoolant > maxCoolant)
        {
            currentCoolant = maxCoolant;
        }
    }

}
