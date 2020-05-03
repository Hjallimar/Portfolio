using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Wolf/WolfAttackPatrolState")]
public class WolfAttackPatrolState : WolfBaseState
{
    Vector3 centerPosition;
    private float angle = -1;
    public bool attack;

    public override void Enter()
    {
        attack = false;
        Debug.Log(owner.name + " having the angle " + angle);
    }

    // Update is called once per frame
    public override void Update()
    {
        centerPosition = WolfPack.prey.transform.position;
        CalculateNewPatrol();

        //Debug.Log(owner.name + AIagent.destination);
        //AIagent.SetDestination(patrolDestination);
        if (attack)
            owner.TransitionTo<WolfAttackState>();
        if (!WolfPack.prayFound)
            owner.TransitionTo<WolfPatrolState>();
        base.Update();
    }

    private void CalculateNewPatrol()
    {
        /*
        float distance = Vector3.Distance(Position.position, WolfPack.prey.transform.position);
        Vector3 direction = WolfPack.prey.transform.position - Position.position;
        Vector3 newPath = Vector3.zero;
        if ( distance > 10)
        {
            distance -= 10f;
            newPath = direction.normalized * distance;
            //move in direction of angle

            
            //move closer
        }
        else if(distance < 9)
        {
            distance = 10- distance;
            newPath = direction.normalized * distance;
            //move in direction of angle
            //move further away
        }*/

        
        Vector3 offset = Position.position - centerPosition;
        Vector3 direction = Vector3.Cross(offset, Vector3.up);
        direction *= angle;
        //ApplyFlockBehaviour();
        AIagent.SetDestination(centerPosition + direction);
        Destination = AIagent.destination;
        
    }

    protected void CalculateAngle()
    {
        angle = Vector3.Angle(Position.position, WolfPack.prey.transform.position) - 90;
    }
}
