using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaSearchState : AlphaBaseState
{
    public override void Enter()
    {
        AIagent.speed = MoveSpeed * 1.5f;
        AIagent.SetDestination(SearchPosition);
    }

    public override void Update()
    {
        base.Update();

        if (CanSeePrey())
        {
            PreyLocated = true;
            owner.TransitionTo<AlphaChaseState>();
        }
        else if (AIagent.remainingDistance < 2)
            owner.TransitionTo<AlphaPatrolState>();
    }
}
