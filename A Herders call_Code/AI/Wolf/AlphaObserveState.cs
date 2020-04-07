using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

[CreateAssetMenu(menuName = "Alpha/AlphaObserveState")]
public class AlphaObserveState : AlphaBaseState
{
    private Vector3 centerPosition;
    private float attack;
    private float attackTimer;
    private float timeUpdate;

    /// <summary>
    /// Sets values upon entry.
    /// </summary>
    public override void Enter()
    {
        timeUpdate = 0;
        attackTimer = 0;
        attack = Random.Range(10f, 20f);
        CalculateNewPatrol();
        AIagent.speed = MoveSpeed * 2f;
    }

    /// <summary>
    /// Controlls conditions for state transition and updates values and timers.
    /// </summary>
    public override void Update()
    {
        centerPosition = Prey.transform.position;
        timeUpdate += Time.deltaTime;
        attackTimer += Time.deltaTime;
        if(timeUpdate > 0.2)
        {
            CalculateNewPatrol();
            timeUpdate = 0;
        }

        if (PlayerHasFire)
            owner.TransitionTo<AlphaFleeState>();
        else if (attackTimer >= attack)
            owner.TransitionTo<AlphaAttackState>();
        else if (!PreyLocated)
            owner.TransitionTo<AlphaPatrolState>();
        base.Update();
    }

    /// <summary>
    /// Calculates a new path to circle around the current target.
    /// </summary>
    private void CalculateNewPatrol()
    {
        Vector3 offset = Position.position - centerPosition;
        Vector3 direction = Vector3.Cross(offset * 4f, Vector3.up);
        direction *= -1;
        AIagent.SetDestination(centerPosition + (direction));
        Destination = AIagent.destination;
    }

}
