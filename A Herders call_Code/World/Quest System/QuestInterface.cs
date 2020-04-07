using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Author: Hjalmar ANdersson

public class QuestInterface : MonoBehaviour
{
    //This is used to show the player that an interact is happening;
    [SerializeField] private Image interact;
    [SerializeField] private Sprite pcInteract;
    [SerializeField] private Sprite consolInteract;
    [SerializeField] private Image descriptionPanel; // 
    [SerializeField] private Text questDescription;

    [SerializeField] private GameObject questPanel; // The entire questLog panel.
    [SerializeField] private Text questLogText; // description for the 

    [SerializeField] private GameObject book;

    private bool firstInput = false;
    private bool isPC = true;

    private bool activePanel = false;
    private int currentIndex = 0;
    private List<int> questIndex = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        //Activate Quest Listerner
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.ActivateQuest, ActivateQuest);
        //Complete Quest Listerner
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.CompleteQuest, CompleteQuest);
        //Interact avalible/ not avalible
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.Interact, InteractAvalible);
        //QuestDialog;
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.QuestDialog, ShowQuestDialog);
        WhatPlatform();
    }

    /// <summary>
    /// Checks what platform is currently beeing used so that the interface shows the correct image
    /// </summary>
    private void WhatPlatform()
    {
        interact.sprite = pcInteract;
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.LinuxPlayer)
            return;
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.LinuxEditor)
            return;
        interact.sprite = consolInteract;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Quest log"))
        {
            book.SetActive(false);
            if (questIndex.Count != 0)
                questLogText.text = QuestComponent.GetQuestName(questIndex[currentIndex]) + "\n" + QuestComponent.GetQuestDescription(questIndex[currentIndex]);
            else
                questLogText.text = "You have no quests at the moment";

            if (questPanel.active == true)
                questPanel.SetActive(false);
            else
                questPanel.SetActive(true);

        }else if (Input.GetButtonDown("Flip pages keys") || Input.GetAxisRaw("Flip pages axis") > 0)
        {
            currentIndex++;
            if (questIndex.Count == 0)
            {
                currentIndex = 0;
                return;
            }
            else if (currentIndex > questIndex.Count - 1)
                currentIndex = 0;
            questLogText.text = QuestComponent.GetQuestName(questIndex[currentIndex]) + "\n" + QuestComponent.GetQuestDescription(questIndex[currentIndex]);
        }
        else if (Input.GetButtonDown("Flip pages keys") || Input.GetAxisRaw("Flip pages axis") < 0)
        {
            currentIndex--;
            if (questIndex.Count == 0)
            {
                currentIndex = 0;
                return;
            }
            else if (currentIndex < 0)
                currentIndex = questIndex.Count - 1;
            questLogText.text = QuestComponent.GetQuestName(questIndex[currentIndex]) + "\n" + QuestComponent.GetQuestDescription(questIndex[currentIndex]);
        }
    }

    /// <summary>
    /// Reacts to an InteractTriggerEvent callback;
    /// This enables an Image that gives feedback to the player that somthing can be interacted with;
    /// </summary>
    /// <param name="eventInfo">InteractTriggerEvent</param>
    private void InteractAvalible(EventInfo eventInfo)
    {
        InteractTriggerEventInfo itei = (InteractTriggerEventInfo)eventInfo;
        if(itei.isInteractable)
            interact.gameObject.SetActive(true);
        else
            interact.gameObject.SetActive(false);
    }

    /// <summary>
    /// Reacts to a ActivateQuestEvent callback;
    /// Adds a quest name to the UI questLog so the player can see it;
    /// </summary>
    /// <param name="eventInfo">ActivateQuestEvent</param>
    private void ActivateQuest(EventInfo eventInfo)
    {
        ActivateQuestEventInfo aqei = (ActivateQuestEventInfo)eventInfo;
        questIndex.Add(aqei.eventQuestID);
    }

    /// <summary>
    /// Reacts to a CompleteQuestEventInfo callback;
    /// This uppdates the UI questLog so no old information is displayed;
    /// </summary>
    /// <param name="eventInfo">CompleteQuestEventInfo</param>
    private void CompleteQuest(EventInfo eventInfo)
    {
        CompleteQuestEventInfo cqei = (CompleteQuestEventInfo)eventInfo;
        questIndex.Remove(cqei.eventQuestID);
    }

    /// <summary>
    /// This will decide if the player is starting an interact or leaving a area where the player had interacted.
    /// Either showing or hiding an interact dialog box that gives the player information via text. 
    /// </summary>
    /// <param name="eventInfo"></param>
    private void ShowQuestDialog(EventInfo eventInfo)
    {
        QuestDialogEventInfo questDialog = (QuestDialogEventInfo)eventInfo;
        if(questDialog.show == true)
        {
            descriptionPanel.gameObject.SetActive(true);
            questDescription.text = questDialog.questText;
        }
        else
        {
            descriptionPanel.gameObject.SetActive(false);
            questDescription.text = "";
        }
    }
}
