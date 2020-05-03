using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Author: Hjalmar Andersson

public class AlphaBaseState : State
{
    #region Values that are the same regardless of state
    protected NavMeshAgent AIagent { get { return owner.Agent; } set { owner.Agent = value; } }
    private CapsuleCollider CapsuleCollider { get { return owner.GetComponent<CapsuleCollider>(); } }
    private LayerMask CollisionMask { get { return owner.CollisionMask; } }
    protected Transform Position { get { return owner.transform; } }
    protected Quaternion Rotation { get { return owner.transform.rotation; } set { owner.transform.rotation = value; } }
    protected GameObject Prey { get { return owner.Prey; } set { owner.Prey = value; } }
    protected RaycastHit capsuleRaycast;
    protected GameObject GameComponent { get { return owner.GameComponent; } }
    protected Vector3 Velocity { get { return owner.Velocity; } set { owner.Velocity = value; } }
    protected Vector3 Destination { get { return owner.Destination; } set { owner.Destination = value; } }
    protected Vector3 DenLocation { get { return owner.DenLocation; } set { owner.DenLocation = value; } }
    protected Animator Anim { get { return owner.Anim; } }
    #endregion

    #region Values that can change between states
    protected Vector3 SearchPosition { get { return owner.SearchPosition; } }
    protected bool PreyLocated { get { return owner.PreyLocated; } set { owner.PreyLocated = value; } }
    protected bool PlayerHasFire { get { return owner.PlayerHasFire; } }
    protected bool IsNight { get { return owner.IsNight; } set { owner.IsNight = value; } }
    protected float MoveSpeed { get { return owner.MoveSpeed; } }
    protected float Damage { get { return owner.Damage; } }
    protected float HearingRange { get { return owner.HearingRange; } }
    [SerializeField] protected float seeingRange = 40f;
    [SerializeField] protected float followRange = 20f;

    //values that can change from state to state
    #endregion

    //Audio Components
    protected AudioSource Audio { get { return owner.Audio; } set { owner.Audio = value; } }
    protected List<AudioClip> Sounds { get { return owner.Sounds; } }

    protected AlphaStateMachine owner;


    protected float security = 0;

    public override void Initialize(StateMachine owner)
    {
        this.owner = (AlphaStateMachine)owner;
        DenLocation = Position.position;
    }

    public override void Update()
    {
        Debug.DrawLine(Position.position, Position.position + AIagent.velocity, Color.blue);
        security = 0;
    }
   

    /// <summary>
    /// Returns a random float between 0.5f and 5.0f
    /// </summary>
    /// <returns></returns>
    protected float GetWaitTime()
    {
        float rand = Random.Range(0.5f, 5f);
        return rand;
    }

    /// <summary>
    /// Returns a random float between 1 and <see cref="MoveSpeed"/>
    /// </summary>
    /// <returns></returns>
    protected float GetPatrolSpeed()
    {
        float rand = Random.Range(1f, MoveSpeed);
        return rand;
    }

    /// <summary>
    /// Calculates a new position that the object should go to whem its entering <see cref="AlphaFleeState"/>
    /// </summary>
    /// <returns></returns>
    protected Vector3 GetDistanceToRun()
    {
        Vector3 destinationToRun = Vector3.zero;
        int check = 0;
        while(destinationToRun.magnitude <= 30 && check <= 10)
        {
            check++;
        float tempCoordinate = Random.Range(Position.position.x - 40, Position.position.x + 40);
        destinationToRun.x = tempCoordinate;
        tempCoordinate = Random.Range(Position.position.z - 40, Position.position.z + 40);
        destinationToRun.z = tempCoordinate;
        }
        return destinationToRun;
    }

    /// <summary>
    /// Sends a linecast if a <see cref="Prey"/> was found through <see cref="GetClosestTarget"/> and returns true if the linecast isnt interupted by terrain.
    /// </summary>
    /// <returns></returns>
    protected bool CanSeePrey()
    {
        Prey = GetClosestTarget();

        if (Prey != null)
            return !Physics.Linecast(owner.transform.position, Prey.transform.position, owner.VisionMask);
        return false;
    }

    /// <summary>
    /// Gets a random int from the sounds list with the intervall between start and end
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    protected int GetSoundIndex(float start, float end)
    {
        int index = (int)Random.Range(start, end);
        return index;
    }

    /// <summary>
    /// Returns the closest <see cref="GameObject"/> in <see cref="GameComponents.FairGameList"/> that is within <see cref="HearingRange"/>. 
    /// </summary>
    /// <returns></returns>
    public GameObject GetClosestTarget()
    {
        GameObject closestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = Position.position;
        foreach (GameObject potentialTarget in GameComponents.FairGameList)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float distanceSqrToTarget = directionToTarget.sqrMagnitude;
            if (distanceSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqrToTarget;
                closestTarget = potentialTarget;
            }
        }

        if (closestDistanceSqr < seeingRange * seeingRange)
        {
            return closestTarget;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Calculates the distance between the <see cref="GameObject"/> holding the script and the destination received
    /// and returns true if the distance is less than the acceptableRange.
    /// </summary>
    /// <param name="destination"> The position of the current destination of the <see cref="NavMeshAgent"/></param>
    /// <param name="acceptableRange"> The maximum distance that can be between the two compared targets for the condition to be true</param>
    /// <returns></returns>
    public bool CheckRemainingDistance(Vector3 destination, float acceptableRange)
    {
        return (destination - Position.position).sqrMagnitude < acceptableRange * acceptableRange;
    }

}

