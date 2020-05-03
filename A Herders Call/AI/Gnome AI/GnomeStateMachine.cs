using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

public class GnomeStateMachine : StateMachine
{
    #region Get and Set properties
    [HideInInspector] public MeshRenderer Renderer { get; set; }
    [HideInInspector] public UnityEngine.AI.NavMeshAgent Agent { get; set; }
    public PhysicsComponent OwnerPhysics { get; set; }
    public Vector3 Velocity { get; set; }
    public Vector3 EscapeDirection { get; set; }
    public float Speed { get { return speed; } }
    public Vector3 Gnomergan { get; set; }
    public Vector3 PlayerKickLocation { get; set; }
    public GameObject Target { get; set; }
    #endregion

    #region Properties returning private values
    public LayerMask CollisionMask { get { return collisionMask; } }
    public LayerMask VisionMask { get { return visionMask; } }
    public float KickRange { get { return kickRange; } }
    public Animator Anim { get { return anim; } }
    #endregion

    [SerializeField] private float speed;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private LayerMask visionMask;
    [SerializeField] private float kickRange = 5;
    [SerializeField] private Animator anim;
    
    #region Audio Components
    [SerializeField] private AudioSource audioSource;
    public AudioSource Audio { get { return audioSource; } set { audioSource = value; } }

    [SerializeField] private List<AudioClip> sounds = new List<AudioClip>();
    public List<AudioClip> Sounds { get { return sounds; } }
    #endregion

    /// <summary>
    /// assignes values at awake
    /// </summary>
    protected override void Awake()
    {
        OwnerPhysics = GetComponent<PhysicsComponent>();
        Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        base.Awake();
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Registers listerners for events.
    /// </summary>
    private void Start()
    {
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.SwapToNight, SwapToNight);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.Gnomekick, KickTheGnome);
    }

    /// <summary>
    /// Makes the object flee if it becomes night.
    /// </summary>
    /// <param name="evenInfo"> Contains information from <see cref="EventInfo"/></param>
    private void SwapToNight(EventInfo evenInfo)
    {
        UnregisterListeners();
        TransitionTo<GnomeFleeState>();
    }

    /// <summary>
    /// Unregisters listeners.
    /// </summary>
    public void UnregisterListeners()
    {
        EventHandeler.Current.UnregisterListener(EventHandeler.EVENT_TYPE.SwapToNight, SwapToNight);
        EventHandeler.Current.UnregisterListener(EventHandeler.EVENT_TYPE.Gnomekick, KickTheGnome);
    }

    /// <summary>
    /// Returns the navmesh agent attached to the object.
    /// </summary>
    /// <returns></returns>
    public UnityEngine.AI.NavMeshAgent GetNavMesh()
    {
        return Agent;
    }

    /// <summary>
    /// Receives the players position, assignes it to the variable <see cref="PlayerKickLocation"/> and transitions to <see cref="GnomeKickedState"/>
    /// </summary>
    /// <param name="eventInfo"> Contains information from <see cref="EventInfo"/></param>
    private void KickTheGnome(EventInfo eventInfo)
    {
        GnomeKickEventInfo gkei = (GnomeKickEventInfo)eventInfo;
        if (Vector3.Distance(gkei.playerPosition, transform.position) < kickRange)
        {
            PlayerKickLocation = gkei.playerPosition;
            TransitionTo<GnomeKickedState>();
        }
    }

    /// <summary>
    /// Sets the target to the gnome, Thetarget must be a Cow object.
    /// </summary>
    /// <param name="cow"></param>
    public void SetGnomeTarget(GameObject cow)
    {
        Target = cow;
    }
}
