using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

public class QuestGiver : MonoBehaviour
{

    [SerializeField] private int questID;
    [SerializeField] private int questIDRecuirement;
    [SerializeField] private int alternativeIDRequired;
    [SerializeField] private Sprite avalibleQuest;
    [SerializeField] private Sprite activeQuest;
    [SerializeField] private Sprite completedQuest;

    private bool started = false;
    private bool turnedIn = false;

    [SerializeField] private QuestGiver nextQuestInLine;

    [SerializeField] private SpriteRenderer side;
    [SerializeField] private SpriteRenderer OtherSide;

    
    private bool interacted = false;
    private Quest myQuest;

    private void Awake()
    {
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.QuestGoalReached, QuestCompleted);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.AvailableQuest, MakeQuestAvalible);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.LoadCompletedQuests, LoadQuests);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.Song, SongIncrease);

        if(QuestComponent.IsCompleted(questIDRecuirement) && QuestComponent.IsCompleted(alternativeIDRequired))
        {
            Debug.Log("I am not active" + questID);
            side.sprite = avalibleQuest;
            OtherSide.sprite = avalibleQuest;
        }
        else
        {
            enabled = false;
        }
        if (nextQuestInLine != null)
            nextQuestInLine.enabled = false;
    }

    private void Update()
    {
        if (interacted)
        {
            interacted = false;
        }
    }

    /// <summary>
    /// Loads in the quests if a file is saved and then loaded into the scene
    /// This will make sure that the player can continue on a quest and doesn't need to redo old quests.
    /// </summary>
    /// <param name="eventInfo"></param>
    public void LoadQuests(EventInfo eventInfo)
    {
        if (QuestComponent.IsCompleted(questID) == true)
        {
            CompleteQuestEventInfo cqei = new CompleteQuestEventInfo { eventQuestID = questID };
            EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.CompleteQuest, cqei);

            EventHandeler.Current.UnregisterListener(EventHandeler.EVENT_TYPE.QuestGoalReached, QuestCompleted);
            EventHandeler.Current.UnregisterListener(EventHandeler.EVENT_TYPE.AvailableQuest, MakeQuestAvalible);

            turnedIn = true;
            enabled = false;
            side.sprite = null;
            OtherSide.sprite = null;
        }
    }

    /// <summary>
    /// starts the quest by calling an ActivateQuestEventInfo;
    /// Activates a quest if the event is called with the correct ID;
    /// </summary>
    public void StartTheQuest(){
        ActivateQuestEventInfo aqei = new ActivateQuestEventInfo { eventQuestID = questID};
        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.ActivateQuest, aqei);
        started = true;
        side.sprite = activeQuest;
        OtherSide.sprite = activeQuest;
    }

    /// <summary>
    /// Increases the progress for a quest if the quest requires the player to sing;
    /// </summary>
    /// <param name="eventInfo"></param>
    public void SongIncrease(EventInfo eventInfo)
    {
        ShoutEventInfo sei = (ShoutEventInfo)eventInfo;
        if(myQuest != null && myQuest.QuestType.TypeOfGoal == GoalType.Song)
        {
            Vector3 distance = (sei.playerPosition - transform.position);
            if (distance.sqrMagnitude <= Mathf.Pow(20f, 2f))
                myQuest.IncreaseProgressBySong(sei.songId);
        }
    }

    /// <summary>
    /// While the player stays in contact with the quest Giver and presses E:
    /// if the quest is complete nothing happens;
    /// if the quest is not yet avalible to the player nothing happens;
    /// if the requirements arent met for the quest then nothing hapens;
    /// otherwise the quest starts by calling startTheQuest();
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (enabled == false)
            return;
        if (Input.GetButtonDown("Interact") && interacted == false)
        {
            interacted = true;
            ControllInput();
        }
    }

    /// <summary>
    /// This will trigger if its time for this quest to activate and become avalible for the player.
    /// </summary>
    /// <param name="eventInfo"></param>
    private void TriggerNextQuest(EventInfo eventInfo)
    {
        if(QuestComponent.IsCompleted(questIDRecuirement) == true || QuestComponent.IsCompleted(alternativeIDRequired) == false && started == false)
        {
            side.sprite = avalibleQuest;
            OtherSide.sprite = avalibleQuest;
        }

    }

    /// <summary>
    /// Decides what happens when the player interacts with the quest giver
    /// </summary>
    public void ControllInput()
    {
        if (enabled == true)
        {
            QuestDialogEventInfo questDialogEvent = new QuestDialogEventInfo { questText = CurrentStateDialog(), show = true };
            EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.QuestDialog, questDialogEvent);
        }

        if (QuestComponent.IsCompleted(questID))
        {
            TurnInQuest();
        }
        else if (QuestComponent.IsCompleted(questIDRecuirement) == false || QuestComponent.IsCompleted(alternativeIDRequired) == false && this.enabled == true)
        {
            return;
        }
        else if (started == false )
        {
            StartTheQuest();
            myQuest.IsProgressNeeded();
        }
    }

    /// <summary>
    /// If the player enters contact with the quest giver then a InteractTriggerEvent is called;
    /// </summary>
    /// <param name="other">Gameobjects Collider</param>
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag != "Player")
            return;
        if (enabled == false)
            return;
        
        InteractTriggerEventInfo itei = new InteractTriggerEventInfo{ isInteractable = true };
        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.Interact, itei);
    }

    /// <summary>
    /// If the player exit contact with the quest giver then a InteractTriggerEvent is called;
    /// </summary>
    /// <param name="other">Gameobjects Collider</param>
    private void OnTriggerExit(Collider other)
    {
        if (enabled == false)
            return;
        InteractTriggerEventInfo itei = new InteractTriggerEventInfo { isInteractable = false };
        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.Interact, itei);

        QuestDialogEventInfo questDialogEvent = new QuestDialogEventInfo { questText = CurrentStateDialog(), show = false };
        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.QuestDialog, questDialogEvent);
        
    }

    /// <summary>
    /// Called when the player interacts with the questgiver and the quest is completed;
    /// </summary>
    private void TurnInQuest()
    {
        side.sprite = null;
        OtherSide.sprite = null;
        if (turnedIn != true)
        {
            CompleteQuestEventInfo cqei = new CompleteQuestEventInfo { eventQuestID = questID };
            EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.CompleteQuest, cqei);

            RewardQuestInfo rei = new RewardQuestInfo { rewardNumber = myQuest.RewardID };
            EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.QuestReward, rei);

            EventHandeler.Current.UnregisterListener(EventHandeler.EVENT_TYPE.QuestGoalReached, QuestCompleted);
            EventHandeler.Current.UnregisterListener(EventHandeler.EVENT_TYPE.AvailableQuest, MakeQuestAvalible);
        }
        else if (nextQuestInLine != null && turnedIn == true)
        {
            enabled = false;
            nextQuestInLine.enabled = true;
        }
        turnedIn = true;
    }

    /// <summary>
    /// Reacts to a callback event CompleteQuestEventInfo;
    /// This then changes material for the indicator and proceeds to set the indicator to inactive;
    /// </summary>
    /// <param name="eventInfo">CompleteEventInfo;</param>
    private void QuestCompleted(EventInfo eventInfo)
    {
        GoalReachedEventInfo grei = (GoalReachedEventInfo)eventInfo;
        if (grei.completedQuestID == questID && enabled == true)
        {
            side.sprite = completedQuest;
            OtherSide.sprite = completedQuest;
        }
    }

    
    /// <summary>
    /// Makes a quest avalible to the player if the requiremets are fullfilled
    /// and if the quest hasn't already been completed
    /// </summary>
    /// <param name="eventinfo"></param>
    private void MakeQuestAvalible(EventInfo eventinfo)
    {
        if (turnedIn)
            return;
        if (QuestComponent.IsCompleted(alternativeIDRequired) && QuestComponent.IsCompleted(questIDRecuirement) && started != true)
        {
            side.sprite = avalibleQuest;
            OtherSide.sprite = avalibleQuest;
            enabled = true;
        }
    }

    /// <summary>
    /// Decides what dialog should be shown to the player
    /// </summary>
    /// <returns></returns>
    private string CurrentStateDialog()
    {
        myQuest = QuestComponent.GetQuestFromID(questID);
        if (myQuest == null)
            return "There is no quest";

        if (QuestComponent.IsCompleted(questID))
        {
            return myQuest.QuestCompleteDescription;
        }
        else if (QuestComponent.IsCompleted(questIDRecuirement) == false && QuestComponent.IsCompleted(alternativeIDRequired))
        {
            return "You don't meet the requirements for the quest";
        }
        else if (started != true)
        {
            return myQuest.QuestDescription;
        }
        else if(started == true)
        {
            return myQuest.QuestActiveDescription;
        }
        return "";
    }


}
