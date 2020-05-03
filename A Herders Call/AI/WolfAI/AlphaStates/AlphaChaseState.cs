using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

[CreateAssetMenu(menuName = "Alpha/AlphaChaseState")]
public class AlphaChaseState : AlphaBaseState
{
    private float newPathTimer;

    /// <summary>
    /// Sets values upon entry.
    /// </summary>
    public override void Enter()
    {
        Audio.PlayOneShot(Sounds[GetSoundIndex(1, 2.9f)]);
        newPathTimer = 0;
        AIagent.speed = MoveSpeed * 2.2f;
    }

    /// <summary>
    /// Starts a timer for when the object is gonna transition to <see cref="AlphaObserveState"/> and handles conditions for state swapping.
    /// </summary>
    public override void Update()
    {
        newPathTimer += Time.deltaTime;
        Destination = AIagent.destination;
        base.Update();


        if (PlayerHasFire)
        {
            owner.TransitionTo<AlphaFleeState>();
        }
        else if (!PreyLocated)
        {
            owner.TransitionTo<AlphaPatrolState>();
        }
        else if (newPathTimer > 0.35 && CheckRemainingDistance(Prey.transform.position, 8f) && CanSeePrey())
        {
            owner.TransitionTo<AlphaObserveState>();
        }
        else if (newPathTimer > 0.35)
        {
            if(CanSeePrey())
                AIagent.SetDestination(Prey.transform.position);
            newPathTimer = 0;
        }

    }
}
