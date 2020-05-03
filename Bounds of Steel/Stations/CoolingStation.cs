using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolingStation : StationBase
{
    [SerializeField] private float cooling = 10;
    private bool eventSent = false;

    new void Start()
    {
        base.Start();
        stationActivated += ActivateCoolantRefill;
        stationDeactivated += DeactivateCoolantRefill;
    }

    new void Update()
    {
        base.Update();

        //if (stationActive && !eventSent)
        //{
        //    CoolantRefillStatusEventInfo crsei = new CoolantRefillStatusEventInfo(gameObject, true, cooling);
        //    EventCoordinator.ActivateEvent(crsei);
        //    eventSent = true;
        //}else if(!stationActive && eventSent)
        //{
        //    CoolantRefillStatusEventInfo crsei = new CoolantRefillStatusEventInfo(gameObject, false, 0);
        //    EventCoordinator.ActivateEvent(crsei);
        //    eventSent = false;
        //}
    }

    private void ActivateCoolantRefill()
    {
        CoolantRefillStatusEventInfo crsei = new CoolantRefillStatusEventInfo(gameObject, true, cooling);
        EventCoordinator.ActivateEvent(crsei);
    }
    private void DeactivateCoolantRefill()
    {
        CoolantRefillStatusEventInfo crsei = new CoolantRefillStatusEventInfo(gameObject, false, 0);
        EventCoordinator.ActivateEvent(crsei);
    }

    public void PrintDeathTestText()
    {
        Debug.Log("AHHHHHHHHHHHH im no longer cool, im lame :C");
    }
}
