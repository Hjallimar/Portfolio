using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main Author: Hjalmar Andersson
//Secondary Author: Marcus Lundqvist

[CreateAssetMenu(menuName = "Gnome/GnomeKickedState")]
public class GnomeKickedState : GnomeBaseState
{
    [SerializeField] private float stopTimer;
    [SerializeField] private float kickForce;
    private float timeWaited = 0;
    private bool airBorn = false;

    /// <summary>
    /// Stops the navMesh agent's movement and gives the Vector3 EscapeDirection a nomalized direction facing away from the player.
    /// </summary>
    public override void Enter()
    {

        Audio.PlayOneShot(Sounds[GetSoundIndex(0, 3.9f)]);
        AIagent.enabled = false;
        EscapeDirection = Vector3.ProjectOnPlane(Position.position - PlayerKickLocation, Vector3.up).normalized;
        EscapeDirection += new Vector3(0, 0.4f, 0);
        Velocity = Quaternion.Euler(0.0f, Random.Range(-20.0f, 20.0f), 0.0f) * EscapeDirection * kickForce;
        Anim.SetBool("GnomeKicked", true);
        FreezePlayerEventInfo fpei = new FreezePlayerEventInfo { FreezeDuration = stopTimer, InteractionTarget = owner.transform, Tag = owner.gameObject };
        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.PlayerInteracting, fpei);
        owner.UnregisterListeners();

    }

    /// <summary>
    /// Stops the game object for a set amount of time and then adds force on the Navmesh agent's velocity. 
    /// </summary>
    public override void Update()
    {
        timeWaited += Time.deltaTime;
        if (timeWaited >= stopTimer)
        {
            CollisionCheck(Velocity * Time.deltaTime);
            ApplyGravity();
            if (GroundCheck() && timeWaited > 1.5f)
            {
                owner.TransitionTo<GnomeFleeState>();
            }
        }
    }

    /// <summary>
    /// Resets the timer upon exit.
    /// </summary>
    public override void Exit()
    {
        AIagent.enabled = true;
        timeWaited = 0;
    }
}
