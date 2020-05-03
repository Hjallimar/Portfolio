using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

public class UnlockableByQuest : MonoBehaviour
{
    [SerializeField] private int requiredQuestID;
    // Start is called before the first frame update
    void Start()
    {
        if (QuestComponent.IsCompleted(requiredQuestID))
            gameObject.SetActive(false);
        else
        {
            EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.CompleteQuest, RemoveDoor);
        }
    }

    /// <summary>
    /// deactivates the game object the script is held by when a quest with the correct index is completed
    /// </summary>
    /// <param name="eventInfo"></param>
    private void RemoveDoor(EventInfo eventInfo)
    {
        CompleteQuestEventInfo cqei = (CompleteQuestEventInfo)eventInfo;

        if(cqei.eventQuestID == requiredQuestID)
        {
            gameObject.SetActive(false);
        }
    }
}
