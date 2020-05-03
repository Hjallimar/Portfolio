using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wolf/WolfAttackState")]
public class WolfAttackState : WolfBaseState
{
    public Vector3 charge;
    public float chargeDistance = 15f;
    // Start is called before the first frame update
    public override void Enter()
    {
        AIagent.SetDestination(Position.position);
        charge = owner.transform.position - WolfPack.prey.transform.position;
        moveSpeed = 10f;
        Debug.Log(owner.name + " entered AttackState");
        AIagent.speed = moveSpeed;
        charge = charge.normalized * chargeDistance;
        //ApplyFlockBehaviour();
        AIagent.SetDestination(owner.transform.position - charge);
        Destination = AIagent.destination;

    }

    // Update is called once per frame
    public override void Update()
    {

        if (!WolfPack.prayFound)
            owner.TransitionTo<WolfPatrolState>();
        if (AIagent.remainingDistance < 1f)
            owner.TransitionTo<WolfChaseState>();

        setVelocity(); 
    }
}
