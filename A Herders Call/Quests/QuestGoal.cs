using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

[System.Serializable]
public class QuestGoal
{
    [SerializeField] private GoalType goalType;
    public GoalType TypeOfGoal { get { return goalType; } }
    public int CurrentAmount { get { return currentAmount; } }

    [SerializeField] private int requieredAmount;
    [SerializeField] private int currentAmount;

    /// <summary>
    /// Creates a new GoalType;
    /// </summary>
    /// <param name="type">One out of three types;</param>
    /// <param name="goal">Ammount that the player has to achive to reach the goal;</param>
    /// <param name="start">Start ammount if the quests lets you start on another progress;</param>
    public QuestGoal(GoalType type, int goal, int start)
    {
        goalType = type;
        requieredAmount = goal;
        currentAmount = start;
    }

    /// <summary>
    /// Checks if the progress of the quest has reached the goal;
    /// </summary>
    /// <returns></returns>
    public bool IsGoalReached()
    {
        return (currentAmount >= requieredAmount);
    }

    /// <summary>
    /// Increases the current progress of the quest;
    /// </summary>
    public void increaseProgress()
    {
        currentAmount++;
    }

    public void IncreaseRequired()
    {
        requieredAmount++;
    }

    /// <summary>
    /// Called if a new quest is called as active and the player has collected some cows already;
    /// The progress will fill up with the sama ammount of cows as the player had collected;
    /// </summary>
    /// <param name="ammount">Ammount of cows the player has collected;</param>
    public void AddCollectedCows(int ammount)
    {
        currentAmount = ammount;
        Debug.Log("I have " + currentAmount + " cows at the start");
    }

    public override string ToString()
    {
        return "current: " + currentAmount + ", needed: " + requieredAmount;
    }
}

public enum GoalType
{
    Gather,
    Escort,
    Find,
    Song
}
