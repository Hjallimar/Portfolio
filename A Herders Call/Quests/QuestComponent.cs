using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Author: Hjalmar Andersson

public class QuestComponent : MonoBehaviour
{

    public List<int> CompletedQuests { get { return completedQuests; } set { completedQuests = value; } }
    public int CollectedCows { get { return collectedCows; } set { collectedCows = value; } }
    public Dictionary<int, Quest> QuestLog { get { return questLog; } }


    [SerializeField] static private Dictionary<int, Quest> questLog = new Dictionary<int, Quest>();

    [SerializeField] static private List<int> completedQuests = new List<int>();

    [SerializeField] static private Dictionary<int, Quest> quests = new Dictionary<int, Quest>();

    [SerializeField] static private int collectedCows = 0;

    [SerializeField] static private Text dialogText;

    // Start is called before the first frame update
    void Start()
    {
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.ActivateQuest, ActivateQuest);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.CompleteQuest, CompleteQuest);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.QuestGoalReached, ChangeStatusForQuests);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.ProgressQuest, QuestProgress);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.CowIsCollected, CollectCow);

        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.Save, SaveQuests);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.Load, LoadQuests);
    }

    /// <summary>
    /// Saves quests
    /// </summary>
    /// <param name="eventInfo"></param>
    private void SaveQuests(EventInfo eventInfo)
    {
        GameDataController.SetQuestState(this);
    }

    /// <summary>
    /// Loads the states of the quests after a player has stareted a loaded scene
    /// </summary>
    /// <param name="eventInfo"></param>
    private void LoadQuests(EventInfo eventInfo)
    {
        SaveData2 myInfo = GameDataController.GetQuestState(this);

        completedQuests = myInfo.QuestsCompleted;
        collectedCows = myInfo.CowsCollected;



        foreach (KeyValuePair<int, int> activeQuests in myInfo.ActiveQuests)
        {
            foreach (KeyValuePair<int, Quest> quest in quests)
            {
                if (activeQuests.Key == quest.Key)
                {
                    ActivateQuestEventInfo aqei = new ActivateQuestEventInfo { eventQuestID = activeQuests.Key};
                    EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.ActivateQuest, aqei);
                    quest.Value.LoadProgress(activeQuests.Value);
                }
            }
        }

        LoadQuestEventInfo lqei = new LoadQuestEventInfo { };
        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.LoadCompletedQuests, lqei);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)){
            ProgressQuestEventInfo pqei = new ProgressQuestEventInfo { type = GoalType.Gather };
            EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.ProgressQuest, pqei);
        }
    }

    /// <summary>
    /// This method is called when the QuestComponent reacts to a callback event with the type CollectCow.
    /// This method will as a response create a callback that increases progress for a escort quest
    /// </summary>
    /// <param name="eventInfo">CollectCowEventInfo;</param>
    private static void CollectCow(EventInfo eventInfo)
    {
        collectedCows++;
        ProgressQuestEventInfo pqei = new ProgressQuestEventInfo { type = GoalType.Escort };
        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.ProgressQuest, pqei);
    }

    /// <summary>
    /// Called when the QuestComponent is reacting to a callback event that activates a quest; 
    /// If the activated quest is an escort quest, its progress will be filled with cows already captured; 
    /// </summary>
    /// <param name="eventInfo">ActivateQuestEventInfo;</param>
    private static void ActivateQuest(EventInfo eventInfo)
    {
        ActivateQuestEventInfo activate = (ActivateQuestEventInfo)eventInfo;

        if (quests.ContainsKey(activate.eventQuestID)) {
            quests[activate.eventQuestID].SetActiveQuest();
            if (questLog.ContainsKey(activate.eventQuestID))
                questLog.Remove(activate.eventQuestID);
            questLog.Add(activate.eventQuestID,quests[activate.eventQuestID]);
            if(questLog[activate.eventQuestID].QuestType.TypeOfGoal.Equals(GoalType.Escort))
            {
                questLog[activate.eventQuestID].QuestType.AddCollectedCows(collectedCows);
            }
        }
    }

    /// <summary>
    /// Called when the QuestComponent is reacting to a callback event that completes a quest;
    /// Removes the quest from the questLog and adds the questID to completedQuests;
    /// </summary>
    /// <param name="eventInfo">CompleteQuestEventInfo;</param>
    private static void CompleteQuest(EventInfo eventInfo)
    {
        CompleteQuestEventInfo completed = (CompleteQuestEventInfo)eventInfo;
        AvailableQuestEventInfo aqei = new AvailableQuestEventInfo { };
        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.AvailableQuest, aqei);
        if (GetQuestFromID(completed.eventQuestID).QuestType.TypeOfGoal == GoalType.Escort)
        {
            foreach(Quest quest in quests.Values)
            {
                if (completedQuests.Contains(quest.QuestID) || quest.QuestType.TypeOfGoal != GoalType.Escort)
                {
                }
                else
                {
                    quest.QuestGoalIncrease();
                }
            }
        }
    }

    /// <summary>
    /// Reacts to a callback event caused by goalReachedEventInfo
    /// This changes the staus of the quest, marking it as complete if the goal is reached. This will make the quest turn in-able at the questgiver
    /// </summary>
    /// <param name="eventInfo"></param>
    private void ChangeStatusForQuests(EventInfo eventInfo)
    {
        GoalReachedEventInfo completed = (GoalReachedEventInfo)eventInfo;
        Quest completedQuest = null;
        if (questLog.ContainsKey(completed.completedQuestID))
        {
            completedQuest = questLog[completed.completedQuestID];
            completedQuests.Add(completed.completedQuestID);
        }
        if (completedQuest != null)
            questLog.Remove(completed.completedQuestID);
    }

    /// <summary>
    /// Called when the QuestComponent is reacting to a callback event that grats progress to a questType;
    /// It creates a new temporary list with all the quests that has the same questType;
    /// Reason for that is if a quest is completed during the process it will remove itself from the questlog list. Removal during runtime is not wanted so a new temporary list it is.
    /// Then it loops through the new list and calling its progress;
    /// </summary>
    /// <param name="eventInfo">ProgressQuestEventInfo;</param>
    private static void QuestProgress(EventInfo eventInfo)
    {
        ProgressQuestEventInfo pqei = (ProgressQuestEventInfo)eventInfo;
        List<Quest> tempQuestList = new List<Quest>();

        foreach(Quest q in questLog.Values)
        {
            if(q.QuestType.TypeOfGoal == pqei.type)
            {
                tempQuestList.Add(q);
            }
        }
        foreach(Quest qst in tempQuestList)
        {
            qst.IncreaseQuestProgress();
        }
    }

    /// <summary>
    /// Returns if a quest with the entered ID has been completed or not
    /// </summary>
    /// <param name="id">ID for the quest;</param>
    /// <returns>true if the quest exists in completedQuests;</returns>
    static public bool IsCompleted(int id)
    {
        if (id == 0)
        {
            return true;
        }
        else if (completedQuests.Count <= 0)
            return false;
        return completedQuests.Contains(id);
    }

    /// <summary>
    /// Retuns the name of the quest that has the same ID as the one entered;
    /// </summary>
    /// <param name="id">ID for the quest;</param>
    /// <returns>name of the Quest;</returns>
    static public string GetQuestName(int id)
    {
        return quests[id].QuestName;
    }

    /// <summary>
    /// Returns the description text for the quest with the same ID as the one entered;
    /// </summary>
    /// <param name="id">ID for the quest;</param>
    /// <returns>Description for a quest;</returns>
    static public string GetQuestDescription(int id)
    {
        return quests[id].QuestDescription;
    }

    /// <summary>
    /// Returns a list with all the completed quests index.
    /// Used by the save function to determin what quests the player has completed at the time of save.
    /// </summary>
    /// <returns></returns>
    static public List<int> GetCompletedQuestList()
    {
        return completedQuests;
    }

    /// <summary>
    /// Returns the name for all the questst that are active in the questlog;
    /// </summary>
    /// <returns>The names for all the quests in questLog;</returns>
    static public string GetQuestLog()
    {
        string log = "";
        foreach(Quest q in questLog.Values)
        {
            log += q.QuestName + "\n";
        }
        return log;
    }

    /// <summary>
    /// Returns the Quest with the quest ID entered.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    static public Quest GetQuestFromID(int id)
    {
        if (quests.ContainsKey(id))
            return quests[id];
        else
            return null;
    }

    /// <summary>
    /// Changes the for of a quest Text
    /// Belive this one isn't used
    /// </summary>
    /// <param name="name"></param>
    static public void ChangeFontOfDialog(string name){
        dialogText.font = Resources.Load<Font>("Fonts/" + name);
    }

    /// <summary>
    /// Adds a list of quests to the QuestCOmponents
    /// Belive this one shouldn't be used
    /// </summary>
    /// <param name="listOfQuests"></param>
    static public void AddRealQuests(Dictionary<int, Quest> listOfQuests)
    {
        quests = listOfQuests;
    }
}
