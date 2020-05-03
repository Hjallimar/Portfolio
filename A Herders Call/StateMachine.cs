using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class StateMachine : MonoBehaviour
{
    public State CurrentState { get { return currentState; } }

    [SerializeField] private State[] states;
    private readonly Dictionary<Type, State> stateDictionary = new Dictionary<Type, State>();
    [SerializeField] private State currentState;
    private State[] stateClones;


    protected virtual void Awake()
    {
        stateClones = states;
        int StateIndex = 0;
        foreach (State state in states)
        {
            State instance = Instantiate(state);
            instance.Index = StateIndex;
            stateClones[StateIndex] = instance;
            StateIndex++;
            instance.Initialize(this);
            stateDictionary.Add(instance.GetType(), instance);
            if (currentState != null) 
                continue;
            currentState = instance;
            currentState.Enter();
        }
    }

    public void Update()
    {
        currentState.Update(); 
    }

    public T GetState<T>()
    {
        Type type = typeof(T);
        if (!stateDictionary.ContainsKey(type))
            throw new NullReferenceException("No state of type: " + type + " found");
        return (T)Convert.ChangeType(stateDictionary[type], type);
    }

    public void TransitionTo<T>()
    {
        currentState.Exit();
        currentState = GetState<T>() as State;
        currentState.Enter();
    }

    public void TransitionTo(State newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void TransitionTo(int newstate)
    {
        currentState.Exit();
        currentState = stateClones[newstate];
        currentState.Enter();
    }
}
