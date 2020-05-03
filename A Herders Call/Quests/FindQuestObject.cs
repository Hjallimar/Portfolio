using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

public class FindQuestObject : MonoBehaviour
{

    [SerializeField] private int activationID;
    [SerializeField] private ParticleSystem gatherIndicator;
    private bool active;
    private bool trigger = false;
    // Start is called before the first frame update
    void Start()
    {
        active = false;
        gatherIndicator.emissionRate = 0;
        gatherIndicator.Play();
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.ActivateQuest, TimeToActivate);
    }

    /// <summary>
    /// Trigger that the player has interacted with the object when the objects quest was active
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (active == true && other.tag == "Player")
        {
            ProgressQuestEventInfo pqei = new ProgressQuestEventInfo { type = GoalType.Find };
            EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.ProgressQuest, pqei);
            MissionComplete();
        }
    }


    /// <summary>
    /// Activaes a particle effect and trigger when the player starts the quest that the object is associeted with
    /// </summary>
    /// <param name="eventInfo"></param>
    private void TimeToActivate(EventInfo eventInfo)
    {
        ActivateQuestEventInfo aqei = (ActivateQuestEventInfo)eventInfo;
        if(aqei.eventQuestID == activationID)
        {
        Debug.Log("Time to activate");
            active = true;
            var ps = gatherIndicator;
            ps.emissionRate = 20f;
        }
    }

    /// <summary>
    /// stops the particles and sets the gameobject to active false
    /// </summary>
    private void MissionComplete()
    {
        gatherIndicator.emissionRate = 0;
        gameObject.SetActive(false);
    }

}
