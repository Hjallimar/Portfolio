using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

public class AlphaStateMachine : StateMachine
{
    #region Get and Set properties
    public MeshRenderer Renderer { get; set; }
    public UnityEngine.AI.NavMeshAgent Agent { get; set; }
    public PhysicsComponent OwnerPhysics { get; set; }
    public GameObject Prey { get; set; }
    public Vector3 Destination { get; set; }
    public Vector3 Velocity { get; set; }
    public Vector3 DenLocation { get; set; }
    public Vector3 SearchPosition { get; set; }
    public bool PreyLocated { get; set; }
    public bool PlayerHasFire { get; set; }
    public bool IsNight { get; set; }
    #endregion

    #region Properties using private values
    public LayerMask CollisionMask { get { return collisionMask; } }
    public LayerMask VisionMask { get { return visionMask; } }
    public GameObject GameComponent { get { return gameComponent; } }
    public float MoveSpeed { get { return moveSpeed; } }
    public float Damage { get { return damage; } }
    public float HearingRange { get { return hearingRange; } }
    public Animator Anim { get; set; }
    #endregion

    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private LayerMask visionMask;
    [SerializeField] private GameObject gameComponent;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float damage;
    [SerializeField] private float hearingRange;


    #region
    [SerializeField] private AudioSource audio;
    public AudioSource Audio { get { return audio; } set { audio = value; } }

    [SerializeField] private List<AudioClip> sounds = new List<AudioClip>();
    public List<AudioClip> Sounds { get { return sounds; } }
    #endregion

    /// <summary>
    /// Registers listeners.
    /// </summary>
    protected void Start()
    {
        Anim = GetComponent<Animator>();
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.TorchActivation ,TorchIsLit);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.SwapToDay, DayEvent);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.SwapToNight, NightEvent);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.Save, SaveWolf);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.Load, LoadWolf);
        PlayerHasFire = false;
    }

    /// <summary>
    /// Saves the wolf
    /// </summary>
    /// <param name="eventInfo"></param>
    public void SaveWolf(EventInfo eventInfo)
    {
        GameDataController.SetWolfState(this);
    }

    /// <summary>
    /// Loads a wolf after the player has loaded a saved file
    /// </summary>
    /// <param name="eventInfo"></param>
    public void LoadWolf(EventInfo eventInfo)
    {
        WolvesData myInfo = GameDataController.GetWolfState(this);
        float[] pos = myInfo.Position;
        float[] rotation = myInfo.Rotation;
        float[] start = myInfo.SpawnLocation;
        Vector3 loadPos;
        Vector3 loadRotation;
        Vector3 loadStartPosition;

        loadPos.x = pos[0];
        loadPos.y = pos[1];
        loadPos.z = pos[2];

        loadRotation.x = rotation[0];
        loadRotation.y = rotation[1];
        loadRotation.z = rotation[2];

        loadStartPosition.x = start[0];
        loadStartPosition.y = start[1];
        loadStartPosition.z = start[2];

        transform.position = loadPos;
        DenLocation = loadStartPosition;
        transform.localRotation = Quaternion.Euler(loadRotation);
        int stateIndex = myInfo.StateIndex;

        TransitionTo(stateIndex);
    }

    /// <summary>
    /// Assignes values to variables.
    /// </summary>
    protected override void Awake()
    {
        OwnerPhysics = GetComponent<PhysicsComponent>();
        Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        // player = GetComponent<GameComponents>().player;
        base.Awake();
    }

    /// <summary>
    /// Returns the NavMesh agent attached to the object
    /// </summary>
    /// <returns></returns>
    public UnityEngine.AI.NavMeshAgent GetNavMesh()
    {
        return Agent;
    }

    /// <summary>
    /// Swaps value of <see cref="PlayerHasFire"/> and transitions to <see cref="AlphaFleeState"/>
    /// </summary>
    /// <param name="eventInfo"> Contains information from <see cref="EventInfo"/></param>
    private void TorchIsLit(EventInfo eventInfo)
    {
        PlayerHasFire = !PlayerHasFire;

        TorchEventInfo torchInfo = (TorchEventInfo)eventInfo;
        if (Vector3.Distance(torchInfo.playerPosition, transform.position) < 30f && PlayerHasFire)
            TransitionTo<AlphaFleeState>();
    }

    /// <summary>
    /// Sets <see cref="IsNight"/> to false and transitions to <see cref="AlphaReturnToDenState"/>
    /// </summary>
    /// <param name="eventinfo"> Contains information from <see cref="EventInfo"/></param>
    private void DayEvent(EventInfo eventinfo)
    {
        IsNight = false;
        TransitionTo<AlphaReturnToDenState>();
    }

    /// <summary>
    /// Sets <see cref="IsNight"/> to true and transitions to <see cref="AlphaPatrolState"/>
    /// </summary>
    /// <param name="eventinfo"> Contains information from <see cref="EventInfo"/></param>
    private void NightEvent(EventInfo eventinfo)
    {
        IsNight = true;
        TransitionTo<AlphaPatrolState>();
    }

    /// <summary>
    /// Transitions to <see cref="AlphaSearchState"/> if the owner is within <see cref="hearingRange"/> distance of the player.
    /// </summary>
    /// <param name="eventInfo"> Contains information from <see cref="EventInfo"/></param>
    public void SearchForPlayer(EventInfo eventInfo)
    {
        ShoutEventInfo se = (ShoutEventInfo)eventInfo;

        if (IsNight && Vector3.Distance(transform.position, se.playerPosition) < hearingRange && Prey != null)
        {
            SearchPosition = se.playerPosition;
            TransitionTo<AlphaSearchState>();
        }
    }

}
