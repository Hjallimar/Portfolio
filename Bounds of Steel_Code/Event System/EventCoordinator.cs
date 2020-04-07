using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//Author: Victor Siönäs
//Secondary: Jonathan Rawet
//Secondary: Hjalmar Andersson

/// <summary>
/// <see cref="EventCoordinator"/> is the callbacks system that handles events triggered by objects so that other objects can react or listen to those events.
/// Events are grouped by different <see cref="EventInfo"/> types.
/// </summary>
public class EventCoordinator
{
    private static EventCoordinator currentEventCoordinator = null;
    private static EventCoordinator CurrentEventCoordinator
    {
        get
        {
            if (currentEventCoordinator == null)
            {
                currentEventCoordinator = new EventCoordinator();
            }
            return currentEventCoordinator;
        }
    }

    public delegate void EventListener(EventInfo eventInfo);
    public Dictionary<Type, EventListener> EventListeners { get; private set; }

    /// <summary>
    /// Register a method to listen to a type of events, so that when <see cref="ActivateEvent(EventInfo)"/> runs with the same <see cref="EventInfo"/> type the method will run.
    /// </summary>
    /// <typeparam name="T">Subtype of <see cref="EventInfo"/></typeparam>
    /// <param name="eventListener">A method that returns void and has an <see cref="EventInfo"/> as a parameter.</param>
    public static void RegisterEventListener<T>(EventListener eventListener) where T : EventInfo
    {
        if (CurrentEventCoordinator.EventListeners == null)
        {
            CurrentEventCoordinator.EventListeners = new Dictionary<Type, EventListener>();
        }

        if (!CurrentEventCoordinator.EventListeners.ContainsKey(typeof(T)) || CurrentEventCoordinator.EventListeners[typeof(T)] == null)
        {
            CurrentEventCoordinator.EventListeners[typeof(T)] = eventListener;
            return;
        }

        CurrentEventCoordinator.EventListeners[typeof(T)] += eventListener;
    }

    /// <summary>
    /// Unregister a method to stop listening to a type of events, so that it will no longer be triggered.
    /// </summary>
    /// <typeparam name="T">Subtype of <see cref="EventInfo"/></typeparam>
    /// <param name="eventListener">A method that returns void and has an <see cref="EventInfo"/> as a parameter.</param>
    public static void UnregisterEventListener<T>(EventListener eventListener) where T : EventInfo
    {
        if (CurrentEventCoordinator.EventListeners == null || !CurrentEventCoordinator.EventListeners.ContainsKey(typeof(T)) || CurrentEventCoordinator.EventListeners[typeof(T)] == null)
        {
            return;
        }

        CurrentEventCoordinator.EventListeners[typeof(T)] -= eventListener;
    }

    /// <summary>
    /// Activate events/methods already registered with the type of the <see cref="EventInfo"/> parameter to run on the parameter.
    /// </summary>
    /// <param name="eventInfo">A subclass of <see cref="EventInfo"/> representing what triggered the event.</param>
    public static void ActivateEvent(EventInfo eventInfo)
    {
        Type t = eventInfo.GetType();

        if (CurrentEventCoordinator.EventListeners == null || !CurrentEventCoordinator.EventListeners.ContainsKey(t) || CurrentEventCoordinator.EventListeners[t] == null)
        {
            return;
        }

        CurrentEventCoordinator.EventListeners[t](eventInfo);
    }
}