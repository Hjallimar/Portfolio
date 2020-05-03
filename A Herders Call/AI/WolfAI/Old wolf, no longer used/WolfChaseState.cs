using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wolf/WolfChaseState")]
public class WolfChaseState : WolfBaseState
{
    // Start is called before the first frame update
    public override void Enter()
    {
        moveSpeed = 8f;
        Debug.Log(owner.name + " has started a chase");
        AIagent.speed = moveSpeed;
    }

    // Update is called once per frame
    public override void Update()
    {
        Destination = AIagent.destination;

        if (!WolfPack.prayFound)
        {
            owner.TransitionTo<WolfPatrolState>();
        }
        else if(Vector3.Distance(Position.position, WolfPack.prey.transform.position) < 10f)
        {
            owner.TransitionTo<WolfAttackPatrolState>();
        }

        SetDestination();

        base.Update();
        setVelocity();
    }

    private void SetDestination()
    {
        GameObject followWolf = null;
        foreach (GameObject wolf in WolfPack.wolves)
        {
            if (Vector3.Distance(wolf.transform.position, WolfPack.prey.transform.position) < Vector3.Distance(Position.position, WolfPack.prey.transform.position))
                followWolf = wolf;
        }

        if (followWolf != null)
        {
            float rand = Random.Range(0.8f, 1.2f);
            AIagent.SetDestination(followWolf.transform.position * rand);
        }
        else
            AIagent.SetDestination(WolfPack.prey.transform.position);
    }
}
