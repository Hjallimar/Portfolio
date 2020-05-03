using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : ScriptableObject
{
    public int Index { get; set; }

    public virtual void Initialize(StateMachine owner) { }

    public virtual void Enter() { }

    public virtual void Update() { }

    public virtual void Exit() { }

}
