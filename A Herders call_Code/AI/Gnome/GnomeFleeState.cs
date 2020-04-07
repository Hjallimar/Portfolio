using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

[CreateAssetMenu(menuName = "Gnome/GnomeFleeState")]
public class GnomeFleeState : GnomeBaseState
{
   
    private bool despawned;
    private float animationTimer = 0;

    /// <summary>
    /// Sets values upon entry.
    /// </summary>
    public override void Enter()
    {

        Audio.PlayOneShot(Sounds[GetSoundIndex(4, 7.9f)]);
        AIagent.speed = Speed * 1.5f;
        FleeToGnomergan();
        AIagent.SetDestination(Position.position + Gnomergan);
        despawned = false;
        AIagent.isStopped = true;
        
    }

    /// <summary>
    /// Destroys the game object once it reaches it's destination.
    /// </summary>
    public override void Update()
    {
        animationTimer += Time.deltaTime;
        if(animationTimer >= 4.8f)
        {
            AIagent.isStopped = false;
            Anim.SetBool("GnomeKicked", false);
        }

        if (AIagent.remainingDistance < 1f || animationTimer >= 6f && !despawned)
        {
            despawned = true;
            //Target.GetComponent<CowSM>().SetGnomes(-1);
            //GameComponetns.gnomeList.Remove(owner.gameObject);
            Destroy(owner.gameObject, 1f);

        }
    }
}
