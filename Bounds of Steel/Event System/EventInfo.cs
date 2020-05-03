using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

//Author: Victor Siönäs
//Secondary: Jonathan Rawet
//Secondary: Hjalmar Andersson

/// <summary>
/// Base class for alll types of events or <see cref="EventInfo"/>. All events needs to use an EventInfo class that inherits from this class.
/// </summary>
/// 

public abstract class EventInfo
{
    public GameObject GO { get; private set; }
    public string EventDescription { get; private set; }
    
    public EventInfo(GameObject gO, string description)
    {
        GO = gO;
        EventDescription = description;
    }
}

public class DebugEventInfo : EventInfo
{
    public byte Severity { get; private set; }
    public DebugEventInfo(GameObject gO, string description, byte severity = 0) : base(gO, description)
    {
        Severity = severity;
    }
}
public class DamageEventInfo : EventInfo
{
    public GameObject ObjectHit { get; private set; }
    public float Damage { get; private set; }
    public DamageType DamageType { get; private set; }
    public Transform AttackerPosition { get; private set; }
    public bool CanHarmSelf { get; private set; }

    public DamageEventInfo(GameObject gO, GameObject objectHit, float damage, DamageType damageType, Transform attackerPosition, bool canHarmSelf = false, string description = "DamageEvent") : base(gO, description)
    {
        ObjectHit = objectHit;
        Damage = damage;
        DamageType = damageType;
        AttackerPosition = attackerPosition;
        CanHarmSelf = canHarmSelf;
    }
}

public class AreaDamageEventInfo : EventInfo
{
    public Vector3 OriginPoint { get; private set; }
    public float Damage { get; private set; }
    public float Range { get; private set; }
    public DamageType DamageType { get; private set; }
    public Vector3 Direction { get; private set; }
    public float MaxAngleDegrees { get; private set; }
    public Transform AttackerPosition { get; private set; }
    public bool CanHarmSelf { get; private set; }

    public AreaDamageEventInfo(GameObject gO, Vector3 originPoint, float damage, float range, DamageType damageType, Vector3 direction, float maxAngleDegrees, Transform attackerPosition, bool canHarmSelf = false, string description = "AreaDamageEvent") : base(gO, description)
    {
        OriginPoint = originPoint;
        Damage = damage;
        Range = range;
        DamageType = damageType;
        Direction = direction.normalized;
        if(Mathf.Approximately(Direction.sqrMagnitude, 0.0f))
        {
            Direction = (OriginPoint - gO.transform.position).normalized;
            if(Mathf.Approximately(Direction.sqrMagnitude, 0.0f))
            {
                Direction = gO.transform.forward;
            }
        }
        MaxAngleDegrees = maxAngleDegrees;
        AttackerPosition = attackerPosition;
        CanHarmSelf = canHarmSelf;
    }
}

public class PlayerRotateEventInfo : EventInfo
{
    public Quaternion Quad { get; private set; }
    public PlayerRotateEventInfo(GameObject gO, Quaternion quad, string description) : base(gO, description)
    {
        Quad = quad;
    }
}
public class UpdateUIDamageEvent : EventInfo
{
    public float Change { get; private set; }

    public UpdateUIDamageEvent(GameObject gO, string description, float change) : base(gO, description)
    {
        Change = change;
    }
}

public class UpdateUIArmorEvent : EventInfo
{
    public float Change { get; private set; }

    public UpdateUIArmorEvent(GameObject gO, string description, float change) : base(gO, description)
    {
        Change = change;
    }
}


public class PickUpEventInfo : EventInfo
{
    public SteamVR_Input_Sources Hand { get; }
    public PickUpEventInfo(GameObject gO, string description, SteamVR_Input_Sources hand) : base(gO, description)
    {
        Hand = hand;
    }
}

public class DropItemEventInfo : EventInfo
{
    public SteamVR_Input_Sources Hand { get; }
    public DropItemEventInfo(GameObject gO, string description, SteamVR_Input_Sources hand) : base(gO, description)
    {
        Hand = hand;
    }
}

public class ReceiverPowerChangedEvent : EventInfo
{
    public float PowerChange { get; private set; }

    public ReceiverPowerChangedEvent(GameObject gO, float powerChange, string description = "power has changed") : base(gO, description)
    {
        PowerChange = powerChange;
    }
}

public class DetatchArmorEventInfo : EventInfo
{
    public DetatchArmorEventInfo(GameObject gO, string description) : base(gO, description)
    {
    }
}

public class AutomaticArtilleryFireEventInfo : EventInfo
{
    public Vector3 Target { get; private set; }
    public AutomaticArtilleryFireEventInfo(GameObject gO, string description, Vector3 target) : base(gO, description)
    {
        Target = target;
    }
}

public class ManualArtilleryFireEventInfo : EventInfo
{
    public ManualArtilleryFireEventInfo(GameObject gO, string description) : base(gO, description)
    {
    }
}

public class ActivateArtilleryEventInfo : EventInfo
{
    public bool Active { get; private set; }
    public ActivateArtilleryEventInfo(GameObject gO, string description, bool active) : base(gO, description)
    {
        Active = active;
    }
}

public class GateDestroyedEventInfo : EventInfo
{
    public GateDestroyedEventInfo(GameObject gO, string description) : base(gO, description)
    {

    }
}

public class AdjustDoorEventInfo : EventInfo
{
    public AdjustDoorEventInfo(GameObject gO, string description) : base(gO, description)
    {

    }
}

public class EnemyDiedEventInfo: EventInfo
{
    public EnemyDiedEventInfo(GameObject gO, string description) : base(gO, description)
    {
    }
}

public class AttackTargetEventInfo : EventInfo
{
    public GameObject Target { get; private set; }
    public AttackTargetEventInfo(GameObject gO, string description, GameObject target) : base(gO, description)
    {
        Target = target;
    }
}

public class ReactorOverchargeBeginEventInfo : EventInfo
{
    public float HeatPerSecond { get; private set; }
    public ReactorOverchargeBeginEventInfo(GameObject gO, float heatPerSecond, string description = "Reactor overcharge started") : base(gO, description)
    {
        HeatPerSecond = heatPerSecond;
    }
}

public class ReactorOverchargeEndEventInfo : EventInfo
{
    public ReactorOverchargeEndEventInfo(GameObject gO, string description = "Reactor overcharge ended") : base(gO, description)
    {

    }
}

public class TransferToMainHeatSinkEventInfo : EventInfo
{
    public float Ammount { get; private set; }

    public TransferToMainHeatSinkEventInfo(GameObject gO, float ammount, string description = "Transfer heatt to sink") : base(gO, description)
    {
        Ammount = ammount;
    }
}

public class FlushCoolantEventInfo : EventInfo
{
    public FlushCoolantEventInfo(GameObject gO, string description = "Flushing the system") : base(gO, description)
    {
    }
}

public class CoolantRefillStatusEventInfo : EventInfo
{
    public bool Status { get; private set; }
    public float Coolant { get; private set; }
    public CoolantRefillStatusEventInfo(GameObject gO, bool status, float coolant, string description = "Flushing the system") : base( gO, description)
    {
        Status = status;
        Coolant = coolant;
    }
}

public class StartDroneRepairEventInfo : EventInfo
{
    public GameObject Target { get; private set; }
    public float RepairSpeed { get; private set; }
    public StartDroneRepairEventInfo(GameObject gO, GameObject target, float repairSpeed, string description = "Repairing") : base(gO, description)
    {
        Target = target;
        RepairSpeed = repairSpeed;
    }
}

public class RepairingDoneEventInfo : EventInfo
{
    public RepairingDoneEventInfo(GameObject gO, string description = "Done with repair") : base(gO, description)
    {
    }
}

public class AssignTargetToRepairEventInfo : EventInfo
{
    public GameObject Target { get; private set; }
    public AssignTargetToRepairEventInfo(GameObject gO, GameObject target, string description = "Repair this target") : base(gO, description)
    {
        Target = target;
    }
}


public class DroneDiedEventInfo : EventInfo
{
    public GameObject Target { get; private set; }
    public float RepairSpeed { get; private set; }
    public DroneDiedEventInfo(GameObject gO, GameObject target, float repairSpeed, string description = "I Died") : base(gO, description)
    {
        Target = target;
        RepairSpeed = repairSpeed;
    }
}

public class GameWinEventInfo : EventInfo
{
    public GameWinEventInfo(GameObject gO, string description = "Player won") : base(gO, description)
    {

    }
}

public class TriggerRangedEventFromAnimEventInfo : EventInfo
{
    public TriggerRangedEventFromAnimEventInfo(GameObject gO, string description) : base(gO, description)
    {

    }
}

public class AddScoreEventInfo : EventInfo
{
    public float Score { get; private set; }
    public AddScoreEventInfo(GameObject gO, float score, string description = "Adding Score") : base(gO, description)
    {
        Score = score;
    }
}

public class HandleCheckEvent : EventInfo
{
    public SteamVR_Input_Sources Hand { get; private set; }
    public HandleTutorialStep HandleStep { get; private set; }
    public HandleCheckEvent(HandleTutorialStep handleTutorialStep, SteamVR_Input_Sources hand, string description = "Checking if handle is held") : base(null, description)
    {
        Hand = hand;
        HandleStep = handleTutorialStep;
    }
}