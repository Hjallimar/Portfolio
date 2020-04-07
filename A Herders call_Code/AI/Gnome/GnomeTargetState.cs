using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

[CreateAssetMenu(menuName = "Gnome/GnomeTargetState")]
public class GnomeTargetState : GnomeBaseState
{

    private float timer = 0f;
    [SerializeField] private float speed;
    /// <summary>
    /// Sets the navmesh agents speed upon entry.
    /// </summary>
    public override void Enter()
    {
        Audio.PlayOneShot(Sounds[GetSoundIndex(4, 7.9f )]);
        timer = 0;
        AIagent.speed = speed;
    }
    
    /// <summary>
    /// Sets the navmesh agents destination to the targets position and swaps to <see cref="GnomeAnnoyState"/> when within 1 units distance of the target.
    /// </summary>
    public override void Update()
    {
        timer += Time.deltaTime;
        if (Target != null)
            AIagent.SetDestination(Target.transform.position);//.x, 0, Target.transform.position.z));

        if (AIagent.remainingDistance < 1f)
        {
            owner.TransitionTo<GnomeAnnoyState>();
        }

        if (timer >= 20)
        {
            owner.UnregisterListeners();
            Destroy(owner.gameObject);
        }


    }
}
