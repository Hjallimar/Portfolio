using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

[CreateAssetMenu(menuName = "Alpha/AlphaReturnToDenState")]
public class AlphaReturnToDenState : AlphaBaseState
{
    bool isInDen;

    /// <summary>
    /// Makes the wolf run back towards it's starting position.
    /// </summary>
    public override void Enter()
    {

        Audio.PlayOneShot(Sounds[0]);
        isInDen = false;
        AIagent.speed = MoveSpeed * 1.5f;
        AIagent.SetDestination(DenLocation);
    }

    public override void Update()
    {
       if( AIagent.remainingDistance<2f && !isInDen)
        {
            isInDen = true;

        }
            
    }

    public override void Exit()
    {
        Audio.PlayOneShot(Sounds[0]);
    }
}
