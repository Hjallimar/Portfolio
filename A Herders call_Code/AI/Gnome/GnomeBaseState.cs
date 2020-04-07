using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Author: Hjalmar Andersson
//Secondary: Marcus Lundqvist

public class GnomeBaseState : State
{
    protected LayerMask CollisionMask { get { return owner.CollisionMask; } }
    protected Transform Position { get { return owner.transform; } }
    protected NavMeshAgent AIagent { get { return owner.Agent; } set { owner.Agent = value; } }
    protected Vector3 Velocity { get { return owner.Velocity; } set { owner.Velocity = value; } }
    protected GameObject Target { get { return owner.Target; } }
    protected Vector3 Gnomergan { get { return owner.Gnomergan; } set { owner.Gnomergan = value; } }
    protected float Speed { get { return owner.Speed; } }
    protected Vector3 PlayerKickLocation { get { return owner.PlayerKickLocation; } }
    protected Vector3 EscapeDirection { get { return owner.EscapeDirection; } set { owner.EscapeDirection = value; } }
    protected Animator Anim { get { return owner.Anim; } }


    //values that can change from state to state
    protected GnomeStateMachine owner;
    protected RaycastHit capsuleRaycast;
    protected float skinWidth = 0.1f;
    protected Vector3 pointUp;
    protected Vector3 pointDown;
    protected CapsuleCollider capsule;

    //Sound components
    protected AudioSource Audio { get { return owner.Audio; } set { owner.Audio = value; } }
    protected List<AudioClip> Sounds { get { return owner.Sounds; } }

    /// <summary>
    /// Assigns values to variables on initialization.
    /// </summary>
    /// <param name="owner"> Reference to the object the <see cref="GnomeStateMachine"/> is attached to</param>
    public override void Initialize(StateMachine owner)
    {
        this.owner = (GnomeStateMachine)owner;
        AIagent.speed = Speed;
        capsule = owner.GetComponent<CapsuleCollider>();
    }

    /// <summary>
    /// Assignes a direction in which the game object will move towards.
    /// </summary>
    protected void FleeToGnomergan()
    {
        Gnomergan = EscapeDirection * Random.Range(10, 20);
    }


    /// <summary>
    /// Checks the remaining distance to the target
    /// </summary>
    /// <param name="destination">The targets position</param>
    /// <param name="acceptableRange">How close the target has to be</param>
    /// <returns></returns>
    public bool CheckRemainingDistance(Vector3 destination, float acceptableRange)
    {
        return (destination - Position.position).sqrMagnitude < acceptableRange * acceptableRange;
    }

    /// <summary>
    /// Checks the collision so that the gnome doesn't get stuck or go somewhere it shouldn't when it gets kicked
    /// </summary>
    /// <param name="frameMovement"></param>
    public void CollisionCheck(Vector3 frameMovement)
    {

        pointUp = Position.position + (capsule.center + Vector3.up * (capsule.height / 2 - capsule.radius));
        pointDown = Position.position + (capsule.center + Vector3.down * (capsule.height / 2 - capsule.radius));
        if (Physics.CapsuleCast(pointUp, pointDown, capsule.radius, frameMovement.normalized, out capsuleRaycast, Mathf.Infinity, CollisionMask))
        {

            float angle = (Vector3.Angle(capsuleRaycast.normal, frameMovement.normalized) - 90) * Mathf.Deg2Rad;
            float snapDistanceFromHit = skinWidth / Mathf.Sin(angle);

            Vector3 snapMovementVector = frameMovement.normalized * (capsuleRaycast.distance - snapDistanceFromHit);

            //float snapdistance = capsuleRaycast.distance + skinWidth / Vector3.Dot(frameMovement.normalized, capsuleRaycast.normal);

            //Vector3 snapMovementVector = frameMovement.normalized * snapdistance;
            snapMovementVector = Vector3.ClampMagnitude(snapMovementVector, frameMovement.magnitude);
            Position.position += snapMovementVector;
            frameMovement -= snapMovementVector;

            Vector3 frameMovementNormalForce = HelpClass.NormalizeForce(frameMovement, capsuleRaycast.normal);
            frameMovement += frameMovementNormalForce;

            if (frameMovementNormalForce.magnitude > 0.001f)
            {
                Vector3 velocityNormalForce = HelpClass.NormalizeForce(Velocity, capsuleRaycast.normal);
                Velocity += velocityNormalForce;

            }

            if (frameMovement.magnitude > 0.001f)
            {
                CollisionCheck(frameMovement);
            }
            return;
        }

        else
        {
            Position.position += frameMovement;
        }
    }

    /// <summary>
    /// Applies gravity to the gnome after the player has kicked the gnome
    /// </summary>
    public void ApplyGravity()
    {
        Velocity += Vector3.down * 9f * Time.deltaTime;
    }

    /// <summary>
    /// returns a int between the values that you send as parameters
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public int GetSoundIndex(float start, float end)
    {
        int index = (int)Random.Range(start, end);
        return index;
    }

    /// <summary>
    /// Checks the ground distance so that the gnome knows when it has contact with the ground. 
    /// </summary>
    /// <returns>True if it has collided with the ground</returns>
    public bool GroundCheck()
    {
       
        if (Physics.Raycast(Position.position, Vector3.down, out capsuleRaycast, (0.3f), CollisionMask)) // ändrade 0,8 till 0,6
        {
            return true;
        }
        else
            return false;
    }
}
