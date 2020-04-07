using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main Author: Marcus Lundqvist
//Secondary Author: Hjalmar Andersson

public class EventHandeler : MonoBehaviour
{
    private void Awake()
    {
        current = this;

    }

    static private EventHandeler current;
    static public EventHandeler Current
    {
        get
        {
            if (current == null)
            {
                current = GameObject.FindObjectOfType<EventHandeler>();
            }
            return current;
        }
    }

    /// <summary>
    /// Declarations of various kinds of event types.
    /// </summary>
    public enum EVENT_TYPE { Sound, SwapToNight, ParticleTrigger, SwapToDay, CallCow, StopCow, StunGiant,
                            LocateCow, TorchActivation, Gnomekick, TorchPickup, TorchDepleted, SwedAwayCow,
                            Song, CalmCow, PetCow, ChangeRune, CollectCow, Damage, ScareCow,
                            Navigation, ActivateQuest, CompleteQuest, ProgressQuest, QuestReward, Interact, QuestDialog, QuestGoalReached, AvailableQuest,
                            CowIsCollected, AudioSound, Save, Load, LoadCompletedQuests, PlayerInteracting, CinematicFreeze, CinematicResume}
    public delegate void EventListener(EventInfo ei);
    Dictionary<EVENT_TYPE, List<EventListener>> eventListeners;
    Dictionary<EVENT_TYPE, List<EventListener>> removeListeners;

    /// <summary>
    /// Takes an <see cref="EVENT_TYPE"/> and a Method to create a connection between them using a delegate.
    /// The registered method will react to the event it is connected to.
    /// </summary>
    /// <param name="eventType"> The type of event that the method will react to</param>
    /// <param name="listener"> The method that will react to the event activation</param>
    public void RegisterListener(EVENT_TYPE eventType, EventListener listener)
    {
        if (eventListeners == null)
        {
            eventListeners = new Dictionary<EVENT_TYPE, List<EventListener>>();
        }

        if (eventListeners.ContainsKey(eventType) == false || eventListeners[eventType] == null)
        {
            eventListeners[eventType] = new List<EventListener>();
        }
        eventListeners[eventType].Add(listener);
    }

    /// <summary>
    /// Removes the connection between an <see cref="EVENT_TYPE"/> and a Method.
    /// </summary>
    /// <param name="eventType">The type of event that the method will react to.</param>
    /// <param name="listener">The method that will react to the event activation.</param>
    public void UnregisterListener(EVENT_TYPE eventType, EventListener listener)
    {

        //lägger till i ny lista, precis likadant som vi gör när vi registrerar en lyssnare
        if (removeListeners == null)
        {
            removeListeners = new Dictionary<EVENT_TYPE, List<EventListener>>();
        }

        if (removeListeners.ContainsKey(eventType) == false || removeListeners[eventType] == null)
        {
            removeListeners[eventType] = new List<EventListener>();
        }
        removeListeners[eventType].Add(listener);

        /*
        if (eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType].Remove(listener);
        }
         */
    }

    /// <summary>
    /// Receives an <see cref="EVENT_TYPE"/> and information that the event will pass forward to the listening methods. 
    /// </summary>
    /// <param name="eventType"> The event type that will be activated.</param>
    /// <param name="eventInfo"> Information that the events can pass forward to the listening methods.</param>
    public void FireEvent(EVENT_TYPE eventType, EventInfo eventInfo)
    {
        if (eventListeners == null || !eventListeners.ContainsKey(eventType) || eventListeners[eventType] == null)
        {
            return;
        }
        foreach (EventListener el in eventListeners[eventType])
        {
            el(eventInfo);
        }

        RemovalOfListerners();
    }

    /// <summary>
    /// Removes the listeners in the <see cref="removeListeners"/> list.
    /// </summary>
    private void RemovalOfListerners()
    {
        //nästlad forloop för att kolla både typ och value samt lite extra säkerhet
        if (removeListeners != null)
        {
            foreach (EVENT_TYPE et in removeListeners.Keys)
            {
                if (eventListeners.ContainsKey(et))
                {
                    foreach (EventListener rel in removeListeners[et])
                        if (eventListeners[et].Contains(rel))
                        {
                            eventListeners[et].Remove(rel);
                        }
                }
            }
            //rensar listan så att den inte försöker ta bort fler saker.
            removeListeners.Clear();
        }
    }
}

