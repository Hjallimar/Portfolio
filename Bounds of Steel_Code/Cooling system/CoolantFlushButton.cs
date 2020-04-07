using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Valve.VR.InteractionSystem.HoverButton))]
public class CoolantFlushButton : MonoBehaviour
{
    public void FlushCoolant()
    {
        FlushCoolantEventInfo fcei = new FlushCoolantEventInfo(gameObject);
        EventCoordinator.ActivateEvent(fcei);
    }
}
