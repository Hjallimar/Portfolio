using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Author: Hjalmar Andersson

[CreateAssetMenu(menuName = "Alpha/AlphaFleeState")]
public class AlphaFleeState : AlphaBaseState
{
    /// <summary>
    /// Changes values upon entry.
    /// </summary>
    public override void Enter()
    {
        AIagent.SetDestination(GetDistanceToRun());
        Prey = null;
        PreyLocated = false;
        AIagent.speed = MoveSpeed;

    }


    /// <summary>
    /// keeps track of the <see cref="NavMeshAgent.remainingDistance"/> <see langword="async"/>a condition to transition to <see cref="AlphaPatrolState"/>
    /// </summary>
    public override void Update()
    {
        if(AIagent.remainingDistance < 2f)
        {
            owner.TransitionTo<AlphaPatrolState>();
        }
    }
}
