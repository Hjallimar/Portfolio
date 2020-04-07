using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

//Author: Hjalmar Andersson

[CreateAssetMenu(menuName = "Alpha/AlphaAttackState")]
public class AlphaAttackState : AlphaBaseState
{

    private bool attacked;
    /// <summary>
    /// Assigns values upon entry.
    /// </summary>
    public override void Enter()
    {
        attacked = false;
        AIagent.velocity = Vector3.zero;
        Debug.Log(owner.name + " entered AttackState");
        AIagent.speed = MoveSpeed * 2.2f;
    }

    /// <summary>
    /// Checks if the Player has a torch out, if it has lost it's target and if it is close enough to attack.
    /// Then swaps state depending on what condition is true
    /// </summary>
    public override void Update()
    {

        AIagent.SetDestination(Prey.transform.position);

        if (PlayerHasFire)
        {
            owner.TransitionTo<AlphaFleeState>();
        }
        else if (!PreyLocated) { 
            owner.TransitionTo<AlphaPatrolState>();
        }
        else if(CheckRemainingDistance(Prey.transform.position, 3f) && attacked == false)
        {
            attacked = true;
            if(Prey.gameObject.tag == "Cow")
            {
                Audio.PlayOneShot(Sounds[4]);
                Prey.GetComponent<CowSM>().AttackTheCow((int)Damage);
                Anim.SetBool("wolfAttacking", true);
            }
            else if(Prey.gameObject.tag == "Player")
            {
                Anim.SetBool("wolfAttacking", true);
                DamageEventInfo dei = new DamageEventInfo { damage = Damage };
                EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.Damage, dei);
                Debug.Log("Damage is done by wolf, dmg " + Damage);
            }
            if(Prey == null)
            {
                PreyLocated = false;
            }
            
            owner.TransitionTo<AlphaObserveState>();
        }
        base.Update();


    }

    /// <summary>
    /// Disables the sprite upon exit.
    /// </summary>
    public override void Exit()
    {
        Anim.SetBool("wolfAttacking", false);
    }
}
