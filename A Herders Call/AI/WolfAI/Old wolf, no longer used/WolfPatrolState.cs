using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wolf/WolfPatrolState")]
public class WolfPatrolState : WolfBaseState
{
    public float patrolArea = 20f;

    public override void Enter()
    {
        Debug.Log("WolfPatrol");
        AIagent.speed = moveSpeed;
        SetNewDestination();
    }

    // Update is called once per frame
    public override void Update()
    {
        if (AIagent.remainingDistance < 1f)
        {
            owner.TransitionTo<WolfIdleState>();
        }
        if(!CloseToPack())
        {
            SetNewDestination();
        }
        setVelocity();
        if (WolfPack.prayFound)
            owner.TransitionTo<WolfChaseState>();
        base.Update();
    }

    private void SetNewDestination()
    {
        Vector3 position;
        int currentTry = 0;
        do
        {
            position = new Vector3(Random.Range(-patrolArea, patrolArea), 0, Random.Range(-patrolArea, patrolArea));
            currentTry++;
        } while (Vector3.Dot(position - owner.transform.position, WolfPack.transform.position - owner.transform.position) < 1 && currentTry < 100);
        AIagent.SetDestination(position);
        Destination = AIagent.destination;

    }
}
