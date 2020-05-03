using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

[CreateAssetMenu(menuName = "Alpha/AlphaIdleState")]
public class AlphaIdleState : AlphaBaseState
{

    public float waitTime;
    private float timer = 0;

    /// <summary>
    /// Sets values upon entry.
    /// </summary>
    public override void Enter()
    {
        waitTime = Random.Range(1, 4);
    }

    /// <summary>
    /// Adds <see cref="Time.deltaTime"/> to a timer as a condition to transition to <see cref="AlphaPatrolState"/>
    /// </summary>
    public override void Update()
    {
        timer += Time.deltaTime;
        base.Update();

        if (CanSeePrey())
            owner.TransitionTo<AlphaChaseState>();
        else if(timer >= waitTime)
        {
            owner.TransitionTo<AlphaPatrolState>();
        }
    }
    

}
