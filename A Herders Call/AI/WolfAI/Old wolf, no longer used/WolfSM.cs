using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfSM : StateMachine
{
    [HideInInspector] public MeshRenderer Renderer;
    [HideInInspector] public UnityEngine.AI.NavMeshAgent agent;
    public LayerMask collisionMask;
    public LayerMask visionMask;
    public PhysicsComponent ownerPhysics;
    public WolfPack pack;
    public float neighbor;
    public Vector3 destination;
    public Vector3 velocity;


    protected override void Awake()
    {
        ownerPhysics = GetComponent<PhysicsComponent>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        pack = GetComponentInParent<WolfPack>();
        // player = GetComponent<GameComponents>().player;
        base.Awake();
    }
    public UnityEngine.AI.NavMeshAgent GetNavMesh()
    {
        return agent;
    }
}
