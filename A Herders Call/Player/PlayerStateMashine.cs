using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main Author: Debatable

public class PlayerStateMashine : StateMachine
{
    #region Get and Set properties
    public PhysicsComponent OwnerPhysics { get; set; }
    public Vector3 Direction { get; set; }
    public Vector3 LookDirection { get; set; }
    public Vector3 FaceDirection { get; set; }
    public List<string> Inventory { get; set; }
    public Rune[] Runes { get; set; }
    public int RuneNumber { get; set; }
    public Rune CurrentRune { get; set; }
    public float VerticalDirection { get; set; }
    public float HorizontalDirection { get; set; }
    public float AnimationMovementDelay { get; set; }
    public int ClipIndex { get; set; }
    public float CurrentTorchLifetime { get; set; }
    public float JumpTimer { get; set; }
    public bool Jumped { get; set; }
    public bool XboxInputDownNotOnCooldown { get; set; }
    public bool XboxInputLeftTriggerNotOnCooldown { get; set; }
    public bool CurrentlyInteracting { get; set; }
    public bool PlayingCinematic { get; set; }
    #endregion

    #region Properties returning existing variables
    public LayerMask CollisionMask { get { return collisionMask; } }
    public AudioClip[] CallCowSounds { get { return callCowSounds; } }
    public AudioClip[] StopCowSounds { get { return stopCowSounds; } }
    public AudioClip[] KulningSounds { get { return kulningSounds; } }
    public AudioClip[] JumpSounds { get { return jumpSounds; } }
    public GameObject Bonfire { get { return bonfire; } }
    public Animator Anim { get { return anim; } }
    public AudioSource Source { get { return source; } }
    public float StunRange { get { return stunRange; } set { stunRange = value; } }
    public int NrOfTorches { get { return nrOfTorches; } set { nrOfTorches = value; } }
    public int Health { get { return health; } set { health = value; } }
    public float StunGiantTimer { get { return stunGiantTimer; } set { stunGiantTimer = value; } }
    public float TorchTimer { get { return torchTimer; } set { torchTimer = value; } }
    public Vector3 Velocity { get { return OwnerPhysics.Velocity; } set {OwnerPhysics.Velocity = value; } } // move this to somewhere?? - vibben
    public GameObject Torch { get { return torch; } set { torch = value; } }
    #endregion


    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float stunRange;
    [SerializeField] private int nrOfTorches;
    [SerializeField] private int health;
    [SerializeField] private GameObject bonfire;
    [SerializeField] private float stunGiantTimer;
    [SerializeField] private Animator anim;
    [SerializeField] private float torchTimer;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip[] callCowSounds;
    [SerializeField] private AudioClip[] stopCowSounds;
    [SerializeField] private AudioClip[] kulningSounds;
    [SerializeField] private AudioClip[] jumpSounds;
    [SerializeField] private GameObject torch;


    protected override void Awake()
    {
        OwnerPhysics = GetComponent<PhysicsComponent>();
        //anim = GetComponent<Animator>();
        Inventory = new List<string>();
        Runes = new Rune[3];
        RuneNumber = 0;
        Jumped = false;
        LookDirection = transform.position;
        FaceDirection = transform.position;
        AnimationMovementDelay = 0;
        XboxInputDownNotOnCooldown = true;
        XboxInputLeftTriggerNotOnCooldown = true;
        CurrentlyInteracting = false;
        PlayingCinematic = false;
        base.Awake();
       
    }

    /// <summary>
    /// Adds the received item to the inventory list.
    /// </summary>
    /// <param name="item"> The name of the object</param>
    /// <param name="GO"> A reference to the object</param>
    public void AddItem(string item, GameObject GO)
    {
        Inventory.Add(item);
        Destroy(GO);
    }

    /// <summary>
    /// Removes the item from the inventory list.
    /// </summary>
    /// <param name="item"> Removes the item from the inventory</param>
    public void RemoveItem(string item)
    {
        Inventory.Remove(item);
    }

    /// <summary>
    /// Saves player information
    /// </summary>
    /// <param name="eventInfo"></param>
    public void SavePlayer(EventInfo eventInfo)
    {

        GameDataController.SetPlayerState(this);
    }

    /// <summary>
    /// Loads player information from file
    /// </summary>
    /// <param name="eventInfo"></param>
    public void LoadPlayer(EventInfo eventInfo)
    {

        SaveData2 myInfo = GameDataController.GetPlayerState(this);
        float[] position = myInfo.Position;
        float[] rotation = myInfo.Rotation;
        float[] cameraPos = myInfo.CameraPosition;

        Vector3 loadPosition;
        Vector3 loadRotation;
        Vector3 cameraPosition;

        loadPosition.x = position[0];
        loadPosition.y = position[1];
        loadPosition.z = position[2];

        loadRotation.x = rotation[0];
        loadRotation.y = rotation[1];
        loadRotation.z = rotation[2];

        cameraPosition.x = cameraPos[0];
        cameraPosition.y = cameraPos[1];
        cameraPosition.z = cameraPos[2];

        LoadRunes(myInfo.CurrentRunes);

        nrOfTorches = myInfo.CurrentNrOfTorches;

        transform.position = loadPosition;

        FaceDirection = loadRotation * Time.deltaTime * 2;

        Camera.main.transform.position = cameraPosition;

        Velocity = Vector3.zero;
    }

    /// <summary>
    /// Registers listeners.
    /// </summary>
    public void Start()
    {
        GameComponents.FairGameList.Add(gameObject); // dynamically adds the player to the list once it awakes
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.TorchPickup, PickUpTorch);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.QuestReward, AddReward);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.Save, SavePlayer);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.Load, LoadPlayer);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.PlayerInteracting, FreezeMovement);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.CinematicFreeze, CinematicFreeze);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.CinematicResume, CinematicResume);

        nrOfTorches = 0;
        source = GetComponent<AudioSource>();
        if (SceneLoadController.LoadSaveData == true)
        {
            SceneLoadController.LoadSaveData = false;
            LoadEventInfo lei = new LoadEventInfo { };
            EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.Load, lei);

        }

    }
    
    /// <summary>
    /// Stops player input during cinematics
    /// </summary>
    /// <param name="eventInfo"></param>
    public void CinematicFreeze(EventInfo eventInfo)
    {
        PlayingCinematic = true;
    }

    /// <summary>
    /// Resumes player movement after cinematics end
    /// </summary>
    /// <param name="eventInfo"></param>
    public void CinematicResume(EventInfo eventInfo)
    {
        PlayingCinematic = false;
    }

    /// <summary>
    /// Freezes the player for a time specified by the event
    /// </summary>
    /// <param name="eventInfo"></param>
    public void FreezeMovement(EventInfo eventInfo)
    {
        if(CurrentlyInteracting == false)
        {
            FreezePlayerEventInfo rqei = (FreezePlayerEventInfo)eventInfo;

            if(rqei.Tag.tag == "Gnome")
            {
                anim.SetTrigger("Kick");
            }
            else if(rqei.Tag.tag == "Cow")
            {
                anim.SetTrigger("Cow");
            }
            else
            {
                return;
            }

            StartCoroutine(InteractionMovementLock(rqei.FreezeDuration, rqei.InteractionTarget.position));
        }
    }

    /// <summary>
    /// Increases the value of <see cref="nrOfTorches"/> by one when activated.
    /// </summary>
    /// <param name="eventInfo"> Contains information about the event from <see cref="EventInfo"/></param>
    public void PickUpTorch(EventInfo eventInfo)
    {
        nrOfTorches++;
    }

    /// <summary>
    /// Deals damage to the player.
    /// </summary>
    /// <param name="dmg"> The damage value that will be dealt</param>
    public void GetAttacked(int dmg)
    {
        health -= dmg;
        DamageEventInfo dei = new DamageEventInfo { damage = dmg };
        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.Damage, dei);
    }

    /// <summary>
    /// Adds a rune to the Rune Array.
    /// </summary>
    /// <param name="rune"> Contains information about the runes values</param>
    public void AddReward(EventInfo eventInfo)
    {
        Rune rune = null;
        RewardQuestInfo rqei = (RewardQuestInfo)eventInfo;
        if(rqei.rewardNumber == 10)
        {
            TorchPickup tpei = new TorchPickup { };
            EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.TorchPickup, tpei);
        }
        else { 
            if (rqei.rewardNumber == 1)
            {
                rune = new Rune(1, "Thunder", 50);
            }
            else if (rqei.rewardNumber == 2) { 
                rune = new Rune(2, "Calm", 45);
            }
            else if (rqei.rewardNumber == 3) { 
                rune = new Rune(3, "Locate", 20);
            }
            if (rune == null)
                return;

            rune.Index = RuneNumber;
            if (RuneNumber == 0) {
                CurrentRune = rune;
                ChangeRuneEventInfo crei = new ChangeRuneEventInfo { newRune = rune };
                EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.ChangeRune, crei);
            }
        Runes[RuneNumber] = rune;
        RuneNumber++;
        }
    }

    /// <summary>
    /// Loads runes for the player
    /// </summary>
    /// <param name="runesToLoad"></param>
    public void LoadRunes(int[] runesToLoad)
    {
        RuneNumber = 0;
        Rune rune = null;

        foreach (int i in runesToLoad)
        {
            if (i != 0)
            {
                if (i == 1)
                {
                    rune = new Rune(1, "Thunder", 50);
                }
                else if (i == 2)
                {
                    rune = new Rune(2, "Calm", 45);
                }
                else if (i == 3)
                {
                    rune = new Rune(3, "Locate", 20);
                }
                if (rune == null)
                    return;

                rune.Index = RuneNumber;
                if (RuneNumber == 0)
                {
                    CurrentRune = rune;
                    ChangeRuneEventInfo crei = new ChangeRuneEventInfo { newRune = rune };
                    EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.ChangeRune, crei);
                }
                Runes[RuneNumber] = rune;
                RuneNumber++;
            }
        }
    }

    /// <summary>
    /// Creates a cooldown on the axis inputs of the xbox controller
    /// </summary>
    /// <returns></returns>
    public IEnumerator XboxCooldown()
    {
        while (Input.GetAxisRaw("Change rune axis") != 0 || Input.GetAxisRaw("Rune activation axis") != 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);

        }
        XboxInputDownNotOnCooldown = true;
        XboxInputLeftTriggerNotOnCooldown = true;

    }

    /// <summary>
    /// Faces the player towards the interaction target and makes the player come to a stop.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    public IEnumerator InteractionMovementLock(float duration, Vector3 targetPosition)
    {
        CurrentlyInteracting = true;
        //Vector3.ClampMagnitude(Velocity, 0.7f);
        Velocity = Vector3.zero;
        FaceDirection = new Vector3(targetPosition.x, 0, targetPosition.z) * Time.deltaTime * 2;
        yield return new WaitForSeconds(duration);
        CurrentlyInteracting = false;
    }

}

//Main Author: Hjalmar Andersson
//Secondary Author: Marcus Lundqvist
public class Rune
{
    public int Value { get { return value; } }

    private int value;
    public string runeName;
    private float coolDown = 0;
    private float CDTimer = 0;
    private bool used = false;
    private int index = 0;
    public int Index { get { return index; } set { index = value; } }


    /// <summary>
    /// Creates and handles the runes
    /// </summary>
    /// <param name="number"></param>
    /// <param name="name"></param>
    /// <param name="CD"></param>
    public Rune(int number, string name, float CD)
    {
        value = number;
        runeName = name;
        coolDown = CD;
    }

    public bool ReadyToUse()
    {
        if (!used)
            return true;
        return false;
    }

    public void Used()
    {
        used = true;
    }

    public float GetCooldown()
    {
        return coolDown;
    }

    public void CooldownFinish()
    {
        used = false;
    }

    public float GetCurrentCD()
    {
        return CDTimer;
    }

    public int GetRuneValue()
    {
        return value;
    }

    public string GetRuneName()
    {
        return runeName;
    }
}
