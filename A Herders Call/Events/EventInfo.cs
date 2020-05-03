using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main Author: Marcus Lundqvist
//Secondary Author: Hjalmar Andersson

public abstract class EventInfo 
{

}
//Time Events
public class NightEventInfo : EventInfo
{
}
public class DayEventInfo : EventInfo
{
}
public class AudioSoundEventInfo : EventInfo
{
    public bool Ambient;
    public int soundIndex;
}

//Cow Events
public class CallCowEventInfo : EventInfo
{
    public Vector3 playerPosition;
    public GameObject player;
}
public class LocateCowEventInfo : EventInfo
{
    public Vector3 playerPosition;
}
public class CalmCowEvent : EventInfo
{
    public Vector3 playerPosition;
}
public class PetCowEventInfo : EventInfo
{
    public Vector3 playerPosition;
}
public class SendAwayCowEvent : EventInfo
{
    public Vector3 destination;
}
public class StopCowEventInfo : EventInfo
{
    public Vector3 playerPosition;
}
public class CollectCowEventInfo : EventInfo
{
    public GameObject closestPen;
}
public class ScareCowEventInfo : EventInfo
{
    public Vector3 scarySoundLocation;
}
public class CowIsCapturedEventInfo : EventInfo
{

}

//Torch Events
public class TorchEventInfo : EventInfo
{
    public Vector3 playerPosition;
    //skrämma vargar
    //tar bort från GUI
}
public class TorchPickup : EventInfo
{
    //lägga till på gui
}
public class TorchDepleted : EventInfo
{

}
public class NavigationEventInfo : EventInfo
{
    public float navFloat;
}

//Gui Events
public class ChangeRuneEventInfo : EventInfo
{
    public Rune newRune; 
    //ändra runa på gui
}
public class InteractTriggerEventInfo : EventInfo
{
    public bool isInteractable;
}
public class QuestDialogEventInfo : EventInfo
{
    public string questText;
    public bool show;
}


//Giant Events
public class StunGiantEventInfo : EventInfo
{
    public Vector3 playerPosition;
    public float stunDistance;
}

//Gnome Events
public class GnomeKickEventInfo : EventInfo
{
    public Vector3 playerPosition;
}

//Shout Events
public class ShoutEventInfo : EventInfo
{
    public Vector3 playerPosition;
    public float shoutDuration;
    public int songId;
}

//AI Event
public class DamageEventInfo : EventInfo
{
    public Vector3 position;
    public float damage;
}

//Quest Event
public class ActivateQuestEventInfo : EventInfo
{
    public int eventQuestID;
}
public class CompleteQuestEventInfo : EventInfo
{
    public int eventQuestID;
}
public class ProgressQuestEventInfo : EventInfo
{
    public GoalType type;
    //public int eventQuestID;
}
public class RewardQuestInfo : EventInfo
{
    public int rewardNumber;
}
public class GoalReachedEventInfo: EventInfo
{
    public int completedQuestID;
}
public class AvailableQuestEventInfo : EventInfo
{
    public int availableQuestID;
}

//Save Events
public class SaveEventInfo : EventInfo
{
}
public class LoadEventInfo : EventInfo
{
}

public class LoadQuestEventInfo : EventInfo
{
}

//Player Specific Events
public class FreezePlayerEventInfo : EventInfo
{
    public float FreezeDuration;
    public Transform InteractionTarget;
    public GameObject Tag;
}

public class CinematicEventInfo : EventInfo
{

}

