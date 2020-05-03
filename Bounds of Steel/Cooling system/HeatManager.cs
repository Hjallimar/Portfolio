using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatManager : MonoBehaviour
{

    [Header("Heat and cooling")]
    [SerializeField] private float heatSinkCapacity = 200;
    [SerializeField] private float currentHeat = 0;
    [SerializeField] private float passiveDraining = 10f;
    [SerializeField] private float timeBetweenCheck = 0.5f;
    [SerializeField] private float flushingMultiplier = 4f;

    public float CurrentHeatProcent { get { return (currentHeat / heatSinkCapacity); } }
    public float CurrentHeatAmount { get { return currentHeat; } }

    private bool overHeated = false;
    private bool malfunction = false;
    private float ammountDrained = 0;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        EventCoordinator.RegisterEventListener<CoolingSystemMalfunctionEventInfo>(Malfunction);
        EventCoordinator.RegisterEventListener<IncreaseHeatEventInfo>(IncreaseHeat);
    }

    private void OnDestroy()
    {
        if (!malfunction)
        {
            EventCoordinator.UnregisterEventListener<CoolingSystemMalfunctionEventInfo>(Malfunction);
            EventCoordinator.UnregisterEventListener<IncreaseHeatEventInfo>(IncreaseHeat);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(currentHeat > heatSinkCapacity && !overHeated)
        {
            overHeated = true;
            TransferHeatToMainHeatSink();
            OverHeatedEventInfo ohei = new OverHeatedEventInfo(gameObject, "Overheat", true);
            EventCoordinator.ActivateEvent(ohei);
        }
        else if (timer > timeBetweenCheck && !overHeated)
        {
            timer = 0;
            TransferHeatToMainHeatSink();
        }

        if (ammountDrained > 0)
            PassiveDrain();
    }

    void TransferHeatToMainHeatSink()
    {
        float ammount = currentHeat - ammountDrained;
        if(ammount > 0)
        ammountDrained += ammount;
        TransferToMainHeatSinkEventInfo thtmsei = new TransferToMainHeatSinkEventInfo(gameObject, ammount);
        EventCoordinator.ActivateEvent(thtmsei);
    }

    void PassiveDrain()
    {
        float drained = passiveDraining * Time.deltaTime;
        if (overHeated)
        {
            currentHeat -= drained * flushingMultiplier;
            ammountDrained -= drained * flushingMultiplier;
            if(currentHeat <= 0)
            {
                currentHeat = 0;
                ammountDrained = 0;
                overHeated = false;
                OverHeatedEventInfo ohei = new OverHeatedEventInfo(gameObject, "Not Overheat", false);
                EventCoordinator.ActivateEvent(ohei);
            }
        }
        else
        {
            currentHeat -= drained;
            ammountDrained -= drained;
        }
    }

    void IncreaseHeat(EventInfo ei)
    {
        IncreaseHeatEventInfo ihei = (IncreaseHeatEventInfo)ei;
        if (ihei.GO == gameObject)
            currentHeat += ihei.HeatAmmount;
    }

    private void Malfunction(EventInfo ei)
    {
        malfunction = true;
        EventCoordinator.UnregisterEventListener<CoolingSystemMalfunctionEventInfo>(Malfunction);
        EventCoordinator.UnregisterEventListener<IncreaseHeatEventInfo>(IncreaseHeat);

    }

}
