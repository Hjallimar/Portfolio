using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WolfBaseState : State
{
    //values that will stay the same regardless of state
    private CapsuleCollider CapsuleCollider { get { return owner.GetComponent<CapsuleCollider>(); } }
    protected LayerMask CollisionMask { get { return owner.collisionMask; } }
    protected Transform Position { get { return owner.transform; } }
    protected Quaternion Rotation { get { return owner.transform.rotation; } set { owner.transform.rotation = value; } }
    protected UnityEngine.AI.NavMeshAgent AIagent { get { return owner.agent; } set { owner.agent = value; } }
    protected WolfPack WolfPack { get { return owner.pack; } set { owner.pack = value; } }
    protected PhysicsComponent OwnerPhysics { get { return owner.GetComponent<PhysicsComponent>(); } }
    protected Vector3 Velocity { get { return owner.velocity; } set { owner.velocity = value; } }

    //values that can change from state to state
    protected WolfSM owner;
    protected RaycastHit capsuleRaycast;
    protected float skinWidth = 0.1f;
    [SerializeField] protected Vector3 Destination { get { return owner.destination; } set { owner.destination = value; } }
    [SerializeField] protected float hearingRange = 50f;
    [SerializeField] protected float seeingRange = 50f;
    [SerializeField] protected float followRange = 20f;

    protected float Neighbor { get { return owner.neighbor; } set { owner.neighbor = value; } }

    protected float moveSpeed = 1.5f;
    protected float security = 0;

    public override void Initialize(StateMachine owner)
    {
        this.owner = (WolfSM)owner;
        OwnerPhysics.SetAirResistance(0.95f);
    }

    public override void Enter()
    {
        
    }

    public override void Update()
    {
        CollisionCheck();
        Debug.DrawLine(Position.position, Position.position + AIagent.velocity, Color.blue);
        security = 0;
    }

    public void CollisionCheck()
    {
        if (security > 25)
            return;
        security++;
        Vector3 pointUp = Position.position + (CapsuleCollider.center + Vector3.up * (CapsuleCollider.height / 2 - CapsuleCollider.radius));
        Vector3 pointDown = Position.position + (CapsuleCollider.center + Vector3.down * (CapsuleCollider.height / 2 - CapsuleCollider.radius));
        if (!Physics.CapsuleCast(pointUp, pointDown, CapsuleCollider.radius, AIagent.velocity.normalized, out capsuleRaycast, Mathf.Infinity, CollisionMask) || AIagent.velocity.magnitude < 0.001f)
            return;
        float snapdistance = capsuleRaycast.distance + skinWidth / Vector3.Dot(AIagent.velocity.normalized, capsuleRaycast.normal);

        if (snapdistance < AIagent.velocity.magnitude * Time.deltaTime)
        {
            if (snapdistance > 0)
                Position.position += AIagent.velocity.normalized * snapdistance;

            Vector3 normalForce = HelpClass.NormalizeForce(AIagent.velocity, capsuleRaycast.normal);
            InheritVelocity(capsuleRaycast.transform, ref normalForce);
            AIagent.velocity += normalForce;
        }
        CollisionCheck();
        return;
    }

    private void InheritVelocity(Transform collideObject, ref Vector3 normalForce)
    {
        PhysicsComponent physics = collideObject.GetComponent<PhysicsComponent>();
        if (physics == null)
            return;
        normalForce = normalForce.normalized * (normalForce.magnitude + Vector3.Project(physics.GetVelocity(), normalForce.normalized).magnitude);
        Vector3 forceInDirection = Vector3.ProjectOnPlane(AIagent.velocity - physics.GetVelocity(), normalForce.normalized);
        Vector3 friction = -forceInDirection.normalized * normalForce.magnitude * OwnerPhysics.GetStaticFriction();

        if (friction.magnitude > forceInDirection.magnitude)
            friction = friction.normalized * forceInDirection.magnitude;
        AIagent.velocity += friction;
    }

    protected bool CanSeePrey()
    {
        return !Physics.Linecast(owner.transform.position, WolfPack.GetComponent<WolfPack>().prey.transform.position, owner.visionMask);
    }

    protected void SearchForPray()
    {
        //ifall en ko eller spelaren är innom sniffRange
        //ifall vargen är innom Kulnings range
    }

    protected void setVelocity()
    {
        OwnerPhysics.SetVelocity(AIagent.velocity);
    }

    protected Vector3 CalculateAlignment()
    {
        Vector3 v = Vector3.zero;
        foreach(GameObject wolf in WolfPack.wolves)
        {
            if(wolf.GetComponent<WolfSM>().agent != AIagent)
            {
                Vector3 temp = Position.position - wolf.transform.position;
                if ( temp.magnitude < WolfPack.radius)
                {
                    v.x += wolf.GetComponent<PhysicsComponent>().GetVelocity().x;
                    v.z += wolf.GetComponent<PhysicsComponent>().GetVelocity().z;
                    Neighbor++;
                }
            }
        }

        if (Neighbor == 0)
            return v;
        v.x /= Neighbor;
        v.z /= Neighbor;
        return v.normalized;
    }

    protected Vector3 CalculateCohesion()
    {
        Vector3 v = Vector3.zero;
        foreach (GameObject wolf in WolfPack.wolves)
        {
            if (wolf.GetComponent<WolfSM>().GetNavMesh() != AIagent)
            {
                Vector3 temp = Position.position - wolf.transform.position;
                if (temp.magnitude < 1f)
                {
                    v.x += wolf.transform.position.x;
                    v.z += wolf.transform.position.z;
                    Neighbor++;
                }
            }
        }
        if (Neighbor == 0)
            return v;
        v.x /= Neighbor;
        v.z /= Neighbor;
        v = new Vector3(v.x - AIagent.transform.position.x, v.y - AIagent.transform.position.y);
        return v.normalized;

    }

    protected Vector3 CalculateSeperation()
    {
        Vector3 v = Vector3.zero;
        foreach (GameObject wolf in WolfPack.wolves)
        {
            if (wolf.GetComponent<WolfSM>().GetNavMesh() != AIagent)
            {
                Vector3 temp = Position.position - wolf.transform.position;
                if (temp.magnitude < 2f)
                {
                    v.x += wolf.transform.position.x - AIagent.transform.position.x;
                    v.z += wolf.transform.position.z - AIagent.transform.position.z;
                    Neighbor++;
                }
            }
        }
        if (Neighbor == 0)
            return v;
        v.x /= Neighbor;
        v.z /= Neighbor;
        v.x *= -1;
        v.z *= -1;
        v = new Vector3(v.x - AIagent.transform.position.x, v.y - AIagent.transform.position.y);
        return v.normalized;
    }

    protected void ApplyFlockBehaviour()
    {
        Vector3 alignment = CalculateAlignment();
        Vector3 cohesion = CalculateCohesion();
        Vector3 seperation = CalculateSeperation();

        Vector3 temp = new Vector3(alignment.x + cohesion.x + seperation.x, 0, alignment.z + cohesion.z + seperation.z);
        AIagent.velocity += temp * Time.deltaTime;
        //AIagent.velocity = AIagent.velocity.normalized * moveSpeed;
    }

    protected float GetWaitTime()
    {
        float rand = Random.Range(0.5f, 5f);
        return rand;
    }

    protected void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    protected float GetPatrolSpeed()
    {
        float rand = Random.Range(1f, 3f);
        return rand;
    }

    protected bool CloseToPack()
    {
        if (Vector3.Distance(owner.transform.position, WolfPack.transform.position) > 20)
            return false;
        return true;
    }

    protected void Accelerate()
    {
        Vector3 direction = AIagent.nextPosition.normalized;
        if (direction.magnitude > 1)
                Velocity += direction.normalized * AIagent.acceleration * Time.deltaTime;
        else
            Velocity += direction * AIagent.acceleration * Time.deltaTime;

        if (Velocity.magnitude > moveSpeed)
            Velocity = Velocity.normalized * moveSpeed;
        ApplyFlockBehaviour();
        AIagent.velocity = Velocity;
    }
}
