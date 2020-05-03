using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main Author: Debatable

public class PlayerBaseState : State
{
    #region Properties with set methods
    //values that will stay the same regardless of state
    protected Quaternion Rotation { get { return owner.transform.rotation; } set { owner.transform.rotation = value; } }
    protected Vector3 Velocity { get { return owner.Velocity; } set { owner.Velocity = value; } }
    protected Vector3 Direction { get { return owner.Direction; } set { owner.Direction = value; } }
    protected Vector3 LookDirection { get { return owner.LookDirection; } set { owner.LookDirection = value; } }
    protected Vector3 FaceDirection { get { return owner.FaceDirection; } set { owner.FaceDirection = value; } }
    protected float TimerLifetime { get { return owner.TorchTimer; } set { owner.TorchTimer = value; } }
    protected float StunRange { get { return owner.StunRange; } set { owner.StunRange = value; } }
    protected int NrOfTorches { get { return owner.NrOfTorches; } set { owner.NrOfTorches = value; } }
    protected int Health { get { return owner.Health; } set { owner.Health = value; } }
    protected float StunGiantTimer { get { return owner.StunGiantTimer; } set { owner.StunGiantTimer = value; } }
    protected float HorizontalDirection { get { return owner.HorizontalDirection; } set { owner.HorizontalDirection = value; } }
    protected float VerticalDirection { get { return owner.VerticalDirection; } set { owner.VerticalDirection = value; } }
    protected float MovementTimer { get { return owner.AnimationMovementDelay; } set { owner.AnimationMovementDelay = value; } }
    protected float CurrentTorchDuration { get { return owner.CurrentTorchLifetime; } set { owner.CurrentTorchLifetime = value; } }
    protected Rune CurrentRune { get { return owner.CurrentRune; } set { owner.CurrentRune = value; } }
    protected float JumpTimer { get { return owner.JumpTimer; } set { owner.JumpTimer = value; } }
    protected bool Jumped { get { return owner.Jumped; } set { owner.Jumped = value; } }
    protected bool XboxInputDownNotOnCooldown { get { return owner.XboxInputDownNotOnCooldown; } set { owner.XboxInputDownNotOnCooldown = value; } }
    protected bool XboxInputLeftTriggerNotOnCooldown { get { return owner.XboxInputLeftTriggerNotOnCooldown; } set { owner.XboxInputLeftTriggerNotOnCooldown = value; } }
    protected bool CurrentlyInteracting { get {return owner.CurrentlyInteracting; } }
    protected bool PlayingCintematic { get { return owner.PlayingCinematic; } set { owner.PlayingCinematic = value; } }
    protected GameObject Torch { get { return owner.Torch; } set { owner.Torch = value; } }
    #endregion

    #region Properties with only get methods
    private CapsuleCollider CapsuleCollider { get { return owner.GetComponent<CapsuleCollider>(); } }
    private LayerMask CollisionMask { get { return owner.CollisionMask; } }
    protected Transform Position { get { return owner.transform; } }
    protected GameObject Bonfire { get { return owner.Bonfire; } }
    protected Rune[] Runes { get { return owner.Runes; } }
    protected int RuneNumber { get { return owner.RuneNumber; } }
    protected Animator Anim { get { return owner.Anim; } }
    protected AudioSource Source { get { return owner.Source; } }
    protected AudioClip[] CallCowSounds { get { return owner.CallCowSounds; } }
    protected AudioClip[] StopCowSounds { get { return owner.StopCowSounds; } }
    protected AudioClip[] KulningSounds { get { return owner.KulningSounds; } }
    protected AudioClip[] JumpSounds { get { return owner.JumpSounds; } }
    protected int ClipIndex { get { return owner.ClipIndex; } set { owner.ClipIndex = value; } }


    //Physics Componetns
    protected PhysicsComponent OwnerPhysics
    {
        get { return owner.GetComponent<PhysicsComponent>(); }
    }
    #endregion

    //values that can change from state to state

    private Vector3 pointUp;
    private Vector3 pointDown;
    private Vector3 runningDistance;
    private PhysicsComponent physics;
    private RaycastHit capsuleRaycast;
    private bool torchIsActive = false;
    private Vector3 SpawnPoint;
    private float frictionCoefficient = 0.95f;
    private float airFriction = 0.95f;

    protected PlayerStateMashine owner;
    protected float topSpeed = 8f;
    protected float jumpForce = 10f;
    protected float gravity = 6f;
    protected float skinWidth = 0.05f;
    protected float acceleration = 10f;
    protected float maxSpeed = 8f;
    protected bool isDead = false;
    protected float respawTimer = 0;
    
    public override void Initialize(StateMachine owner) {
        this.owner = (PlayerStateMashine)owner;
        OwnerPhysics.SetAirResistance(0.95f);
        CurrentTorchDuration = TimerLifetime;
        SpawnPoint = Position.position;
    }

    public override void Update()
    {
        CollisionCheck(Velocity * Time.deltaTime);
        Velocity = owner.OwnerPhysics.AirFriction(Velocity);
       
        FaceTowardsDirection();
        HealthCheck();

        //1,2,3
        SongInput();
        //F
        ToggleTorch();
        
        TorchDepletionTimer();
        //Q, Tab
        RuneKeys();
    }

    /// <summary>
    /// Faces the player towards the direction they are moving
    /// </summary>
    private void FaceTowardsDirection()
    {
        if (Velocity.magnitude > 0)
        {
            LookDirection = new Vector3(Velocity.x, 0, Velocity.z);
            Position.LookAt(LookDirection);
            FaceDirection += LookDirection * Time.deltaTime * 2;
            if (FaceDirection.magnitude > 1)
                FaceDirection = FaceDirection.normalized;
            Position.LookAt(Position.position + FaceDirection);

            //Position.rotation = Quaternion.RotateTowards(Position.rotation, Quaternion.LookRotation(LookDirection), 180.0f * Time.deltaTime);

        }
        else
        {

            //Position.rotation = Quaternion.RotateTowards(Position.rotation, Quaternion.LookRotation(LookDirection), 180.0f * Time.deltaTime);
            Position.LookAt(Position.position + FaceDirection);
            FaceDirection += LookDirection * Time.deltaTime * 2;
            if (FaceDirection.magnitude > 1)
                FaceDirection = FaceDirection.normalized;
            //Position.LookAt(Position.position + FaceDirection);
        }
        
    }

    /// <summary>
    /// Checks the players current health.
    /// </summary>
    private void HealthCheck()
    {
        if (Health <= 0)
        {
            respawTimer += Time.deltaTime;
            if(isDead == false)
            {
                PlayingCintematic = true;
                isDead = true;
            }
            if(respawTimer >= 3.0f)
                PlayerDeath();
        }
    }

    /// <summary>
    /// Depletes the torch over a set amount of time and triggers events once the timer has reached 0 or less.
    /// </summary>
    private void TorchDepletionTimer()
    {
        if (torchIsActive)
        {

            CurrentTorchDuration -= Time.deltaTime;
            if (CurrentTorchDuration <= 0)
            {
                NrOfTorches--;
                TorchDepleted td = new TorchDepleted { };
                EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.TorchDepleted, td);
                torchIsActive = !torchIsActive;
                Torch.SetActive(torchIsActive);
                CurrentTorchDuration = TimerLifetime;
                Anim.SetBool("TorchUse", false);


                TorchEventInfo tei = new TorchEventInfo { playerPosition = Position.position };
                EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.TorchActivation, tei);

            }
        }
    }

    /// <summary>
    /// Uses different runes depending on the <see cref="CurrentRune"/> value. And enables the selection of rune.
    /// </summary>
    private void RuneKeys()
    {
        if (Input.GetButtonDown("Rune activation key") || (Input.GetAxisRaw("Rune activation axis") > 0 && XboxInputLeftTriggerNotOnCooldown == true))
        {
            if (CurrentRune == null)
                return;
            XboxInputLeftTriggerNotOnCooldown = false;
            owner.StartCoroutine(owner.XboxCooldown());
            AudioClip audioClipSounds = null;
            if (CurrentRune != null && CurrentRune.ReadyToUse() && !Source.isPlaying)
            {
                CurrentRune.Used();
                switch (CurrentRune.GetRuneValue())
                {
                    case 1:
                        audioClipSounds = KulningSounds[1];
                        StunGiantEventInfo sgei = new StunGiantEventInfo { playerPosition = Position.position, stunDistance = StunRange };
                        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.StunGiant, sgei);
                        MadeANoise(audioClipSounds.length, 3);
                        break;

                    case 2:
                        audioClipSounds = KulningSounds[2];
                        CalmCowEvent cce = new CalmCowEvent { playerPosition = Position.position };
                        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.CalmCow, cce);
                        MadeANoise(audioClipSounds.length, 2);
                        break;

                    case 3:
                        audioClipSounds = KulningSounds[0];
                        LocateCowEventInfo lcei = new LocateCowEventInfo { playerPosition = Position.position };
                        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.LocateCow, lcei);
                        MadeANoise(audioClipSounds.length, 1);
                        break;
                    default:
                        break;
                }
                Source.PlayOneShot(audioClipSounds);
                Anim.SetTrigger("Singing");
            }
        }

        if (Input.GetButtonDown("Change rune key") || (Input.GetAxisRaw("Change rune axis") < 0 && XboxInputDownNotOnCooldown == true))
        {
            if (CurrentRune != null)
            {
                XboxInputDownNotOnCooldown = false;
                owner.StartCoroutine(owner.XboxCooldown());
                int temp = CurrentRune.Index;
                temp++;
                if (temp >= RuneNumber)
                    temp = 0;
                CurrentRune = Runes[temp];
                Debug.Log("Sending change rune event with value " + CurrentRune.GetRuneValue());
                ChangeRuneEventInfo crei = new ChangeRuneEventInfo { newRune = CurrentRune };
                EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.ChangeRune, crei);
            }
        }



        
    }

    /// <summary>
    /// Pets the cow and kicks away gnomes.
    /// </summary>
    public void Interact()
    {
        if (Input.GetButtonDown("Interact"))
        {
            PetCowEventInfo pcei = new PetCowEventInfo { playerPosition = Position.position };
            EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.PetCow, pcei);

            GnomeKickEventInfo gkei = new GnomeKickEventInfo { playerPosition = Position.position };
            EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.Gnomekick, gkei);
        }
    }

    /// <summary>
    /// Activates the torch.
    /// </summary>
    private void ToggleTorch()
    {
        if (Input.GetButton("Activate torch") && NrOfTorches > 0 && torchIsActive == false)
        {
            torchIsActive = !torchIsActive;
            Torch.SetActive(torchIsActive);
            Anim.SetBool("TorchUse", true);
            Debug.Log(Anim.GetBool("TorchUse"));
            TorchEventInfo tei = new TorchEventInfo { playerPosition = Position.position };
            EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.TorchActivation, tei);
            

        }
    }

    /// <summary>
    /// Activates various songs depending on key input.
    /// </summary>
    private void SongInput()
    {
        if (Input.GetButtonDown("Call Cow") && !Source.isPlaying)
        {
            Anim.SetTrigger("Singing");
            ClipIndex = Random.Range(1, 4);
            AudioClip clip = CallCowSounds[ClipIndex];
            Source.PlayOneShot(clip);
            CallCowSounds[ClipIndex] = CallCowSounds[0];
            CallCowSounds[0] = clip;
            MadeANoise(clip.length, 0);
            CallCowEventInfo ccei = new CallCowEventInfo
            {
                playerPosition = Position.position,
                player = Position.gameObject
            };

            EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.CallCow, ccei);
        }

        if (Input.GetButtonDown("Stop/Collect Cow") && !Source.isPlaying)
        {
            Anim.SetTrigger("Singing");
            ClipIndex = Random.Range(1, 2);
            AudioClip clip = StopCowSounds[ClipIndex];
            Source.PlayOneShot(clip);
            StopCowSounds[ClipIndex] = StopCowSounds[0];
            StopCowSounds[0] = clip;
            MadeANoise(clip.length, 0);
            StopCowEventInfo scei = new StopCowEventInfo
            {
                playerPosition = Position.position
            };

            EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.StopCow, scei);

            foreach (GameObject wolf in GameComponents.WolfList)
            {

            }
            foreach (GameObject giant in GameComponents.GiantList)
            {

            }
        }

        if (Input.GetButtonDown("Stop/Collect Cow"))
        {
            GameObject penInDistance = null;
            foreach (GameObject animalPen in GameComponents.AnimalPens)
            {
                if (Mathf.Abs(Vector3.Distance(animalPen.transform.position, owner.transform.position)) < animalPen.GetComponent<AnimalPen>().CowCollectionDistance)
                {
                    penInDistance = animalPen;
                }
            }

            if (penInDistance != null)
            {
                CollectCowEventInfo ccei = new CollectCowEventInfo { closestPen = penInDistance };
                EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.CollectCow, ccei);
            }
            MadeANoise(0, 0);
        }
    }

    /// <summary>
    /// A generic event for various shouts that enemies reacts to.
    /// </summary>
    /// <param name="time"></param>
    public void MadeANoise(float time, int songIndex)
    {
        ShoutEventInfo sei = new ShoutEventInfo { playerPosition = Position.position, shoutDuration = time, songId = songIndex};
        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.Song, sei);
    }

    /// <summary>
    /// Checks for collision using <see cref="capsuleRaycast"/> and recursive calls.
    /// </summary>
    /// <param name="frameMovement"></param>
    public void CollisionCheck(Vector3 frameMovement)
    {
        
        pointUp = Position.position + (CapsuleCollider.center + Vector3.up * (CapsuleCollider.height / 2 - CapsuleCollider.radius));
        pointDown = Position.position + (CapsuleCollider.center + Vector3.down * (CapsuleCollider.height / 2 - CapsuleCollider.radius));
        if (Physics.CapsuleCast(pointUp, pointDown, CapsuleCollider.radius, frameMovement.normalized, out capsuleRaycast, Mathf.Infinity, CollisionMask))
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
                ApplyFriction(velocityNormalForce.magnitude);

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
    /// Lowers the players speed by a set amount each update.
    /// </summary>
    public void Decelerate()
    {
        pointUp = Position.position + (CapsuleCollider.center + Vector3.up * (CapsuleCollider.height / 2 - CapsuleCollider.radius));
        pointDown = Position.position + (CapsuleCollider.center + Vector3.down * (CapsuleCollider.height / 2 - CapsuleCollider.radius));
        Physics.CapsuleCast(pointUp, pointDown, CapsuleCollider.radius, Velocity.normalized, out capsuleRaycast, maxSpeed, CollisionMask);

        Vector3 velocityOnGround = Vector3.ProjectOnPlane(Velocity, capsuleRaycast.normal);
        Vector3 decelerationVector = velocityOnGround * frictionCoefficient;

        if(decelerationVector.magnitude > velocityOnGround.magnitude)
        {
            Velocity = Vector3.zero;
        }
        else
        {
            Velocity -= decelerationVector;
        }

     
    }

    /// <summary>
    /// Applies the velocity of a moving object onto the player if the player is standing on it.
    /// </summary>
    /// <param name="collideObject"></param>
    /// <param name="normalForce"></param>
    private void InheritVelocity(Transform collideObject, ref Vector3 normalForce)
    {
        physics = collideObject.GetComponent<PhysicsComponent>();
        if (physics == null)
            return;
        normalForce = normalForce.normalized * (normalForce.magnitude + Vector3.Project(physics.GetVelocity(), normalForce.normalized).magnitude);
        Vector3 forceInDirection = Vector3.ProjectOnPlane(Velocity - physics.GetVelocity(), normalForce.normalized);
        Vector3 friction = -forceInDirection.normalized * normalForce.magnitude * OwnerPhysics.GetStaticFriction();

        if (friction.magnitude > forceInDirection.magnitude)
            friction = friction.normalized * forceInDirection.magnitude;
        Velocity += friction;
    }

    /// <summary>
    /// Applies a constant force of gravity on the player.
    /// </summary>
    public void ApplyGravity()
    {
        //Velocity = Vector3.ProjectOnPlane(Velocity, owner.transform.right); //viktors kod som typ fungerade
        Velocity += Vector3.down * gravity * Time.deltaTime;
       
    }

    /// <summary>
    /// Gradually increases the players velocity.
    /// </summary>
    /// <param name="direction"></param>
    public void Accelerate(Vector3 direction)
    {
        
        pointUp = Position.position + (CapsuleCollider.center + Vector3.up * (CapsuleCollider.height / 2 - CapsuleCollider.radius));
        pointDown = Position.position + (CapsuleCollider.center + Vector3.down * (CapsuleCollider.height / 2 - CapsuleCollider.radius));
        Physics.CapsuleCast(pointUp, pointDown, CapsuleCollider.radius, Velocity.normalized, out capsuleRaycast, maxSpeed, CollisionMask);

        Vector3 velocityOnGround = Vector3.ProjectOnPlane(Velocity, capsuleRaycast.normal);

        float turnDot = Vector3.Dot(direction, velocityOnGround.normalized);

        if (velocityOnGround.magnitude > 0.001f && turnDot < 0.9f)
        {
            Velocity += Vector3.ClampMagnitude(direction, 1.0f) * 10 * acceleration * Time.deltaTime;
        }
        else
        {
            Velocity += Vector3.ClampMagnitude(direction, 1.0f) * acceleration * Time.deltaTime;
        }


        if (Velocity.magnitude > maxSpeed)
        {
            Velocity = Vector3.ClampMagnitude(new Vector3(Velocity.x, 0.0f, Velocity.z), maxSpeed) + Vector3.ClampMagnitude(new Vector3(0.0f, Velocity.y, 0.0f), 5.0f);
        }
        Position.rotation = Quaternion.Euler(direction.x, 0f, 0f);

        
        
    }

    /// <summary>
    /// Applies wind resistance to slow down player speed.
    /// </summary>
    public void AirFriction()
    {
        Velocity *= Mathf.Pow(0.95f, Time.deltaTime);
    }

    /// <summary>
    /// Uses a <see cref="capsuleRaycast"/> to see if the player has made contact with the cround
    /// </summary>
    /// <returns></returns>
    public bool Grounded()
    {
        Vector3 pointUp = Position.position + CapsuleCollider.center + Vector3.up * (CapsuleCollider.height / 6 - CapsuleCollider.radius);
        Vector3 pointDown = Position.position + CapsuleCollider.center + Vector3.down * (CapsuleCollider.height / 6 - CapsuleCollider.radius);
        if (Physics.CapsuleCast(pointUp, pointDown, CapsuleCollider.radius, Vector3.down, out capsuleRaycast, (0.5f + skinWidth), CollisionMask)) // ändrade 0,8 till 0,6
        {
            return true;
        }
        else
            return false;
    }

    public void GroundDistanceCheck()
    {

        if (capsuleRaycast.collider != null)
        {
            if(capsuleRaycast.distance > 0.4f)
            {
                Velocity += new Vector3(0, -capsuleRaycast.distance * 5, 0);
                //Position.position += new Vector3(0, -capsuleRaycast.distance + 0.4f, 0);
            }
        }
       
    }

    /// <summary>
    /// Returns the normal angle of the object below the player.
    /// </summary>
    /// <returns></returns>
    public Vector3 GroundedNormal()
    {
        Vector3 pointUp = Position.position + CapsuleCollider.center + Vector3.up * (CapsuleCollider.height / 6 - CapsuleCollider.radius);
        Vector3 pointDown = Position.position + CapsuleCollider.center + Vector3.down * (CapsuleCollider.height / 6 - CapsuleCollider.radius);
        if (Physics.CapsuleCast(pointUp, pointDown, CapsuleCollider.radius, Vector3.down, out capsuleRaycast, (0.5f + skinWidth), CollisionMask))
        {
            return capsuleRaycast.normal;
        }
        else
        {
            return Vector3.zero;
        }
    }

    /// <summary>
    /// Respawns the player if killed.
    /// </summary>
    public void PlayerDeath(){
        respawTimer = 0;
        Health = 10;
        Position.position = SpawnPoint;
        NrOfTorches = 0;
        isDead = false;
        PlayingCintematic = false;

    }

    /// <summary>
    /// Applies friction to the player in order to slow down movement.
    /// </summary>
    /// <param name="normalForceMagnitude"></param>
    private void ApplyFriction(float normalForceMagnitude)
    {

        RaycastHit collision;
        Vector3 point1 = owner.transform.position + CapsuleCollider.center + Vector3.up * (CapsuleCollider.height / 2 - CapsuleCollider.radius);
        Vector3 point2 = owner.transform.position + CapsuleCollider.center + Vector3.down * (CapsuleCollider.height / 2 - CapsuleCollider.radius);



        if (Physics.CapsuleCast(point1, point2, CapsuleCollider.radius, Vector3.down, out collision, skinWidth * 5, CollisionMask))
        {
            {
              
                if (Velocity.magnitude < normalForceMagnitude * frictionCoefficient)
                {
                    Velocity = Vector3.zero;
                }
                else
                {
                    Vector3 temp = Velocity.normalized;
                    Velocity -= temp * normalForceMagnitude * frictionCoefficient * 0.9f; //tog bort * 0.8f;
                }
            }
        }
    }

    /// <summary>
    /// Also applies air friction.
    /// </summary>
    public void AirResistance()
    {
        Velocity *= Mathf.Pow(airFriction, Time.deltaTime);
    }

   

    /// <summary>
    /// Checks input for jump
    /// </summary>
    public void JumpInput()
    {
        if (Input.GetButton("Jump"))
        {
            Velocity += Vector3.up * jumpForce;
            Jumped = true;
            if (Source.isPlaying == false)
            {
                ClipIndex = Random.Range(1, 3);
                AudioClip clip = JumpSounds[ClipIndex];
                Source.PlayOneShot(clip);
                JumpSounds[ClipIndex] = JumpSounds[0];
                JumpSounds[0] = clip;
            }
            
        }
    }

    /// <summary>
    /// Alters the direction input to match the cameras direction.
    /// </summary>
    public void CameraDirectionChanges()
    {
        Direction = Camera.main.transform.rotation * new Vector3(HorizontalDirection, 0, VerticalDirection).normalized;
        //Direction = Vector3.ProjectOnPlane(Direction, GroundedNormal()).normalized;
    }

    /// <summary>
    /// Gives the movement variables values from input
    /// </summary>
    public void MovementInput()
    {
        VerticalDirection = Input.GetAxisRaw("Vertical");
        HorizontalDirection = Input.GetAxisRaw("Horizontal");

    }

    /// <summary>
    /// Updates the players direction to match the terrains normal.
    /// </summary>
    public void ProjectToPlaneNormal()
    {

        RaycastHit collision;
        Vector3 point1 = owner.transform.position + CapsuleCollider.center + Vector3.up * (CapsuleCollider.height / 2 - CapsuleCollider.radius);
        Vector3 point2 = owner.transform.position + CapsuleCollider.center + Vector3.down * (CapsuleCollider.height / 2 - CapsuleCollider.radius);

        Physics.CapsuleCast(point1, point2, CapsuleCollider.radius, Vector3.down, out collision, topSpeed, CollisionMask);

        Direction = Vector3.ProjectOnPlane(Direction, collision.normal).normalized;
        
    }

    /// <summary>
    /// Stops the player from sliding down hills
    /// </summary>
    public void ControlDirection()
    {
        //Vector3 pointUp = Position.position + CapsuleCollider.center + Vector3.up * (CapsuleCollider.height / 6 - CapsuleCollider.radius);
        //Vector3 pointDown = Position.position + CapsuleCollider.center + Vector3.down * (CapsuleCollider.height / 6 - CapsuleCollider.radius);
        //Physics.CapsuleCast(pointUp, pointDown, CapsuleCollider.radius, Vector3.down, out capsuleRaycast, topSpeed, CollisionMask);

        Vector3 projectedDirection = Vector3.ProjectOnPlane(Direction, capsuleRaycast.normal);
        if(Vector3.Dot(projectedDirection, Velocity) != 1)
        {
        Velocity = projectedDirection.normalized * Velocity.magnitude;
        }
    }

}
