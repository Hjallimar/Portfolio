using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;
using UnityEngine.Events;

public class MechMovement : MonoBehaviour
{
    #region SerializeField Varaibles
    [Header("Movement related variables")]
    [SerializeField] private float joystickRotationSpeed = 50.0f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float rotateOffset = 5f;
    [SerializeField] private float turnSpeedModifier = 10f;
    [SerializeField, Range(0, 90)] private float movementAngle = 50;
    [SerializeField] private Transform verticalRotationPoint = null;
    [SerializeField] private AnimationCurve powerEffectOnAcceleration = null;
    [SerializeField] private AnimationCurve powerEffectOnMaxSpeed = null;
    [Space]
    [Header("Physics related variables")]
    [SerializeField] private float gravity = 1f;
    [SerializeField, Min(0.0f)] private float skinWidth = 0.01f;
    [SerializeField] private LayerMask collisionLayers = 0;
    [SerializeField, Range(0f, 1f)] private float airResistanceCoefficient = 0.95f;
    [Space]
    [SerializeField, Tooltip("Set to true to display current movement stats on screen")] private bool showDebugText = false;
    [SerializeField] private bool activateMech = false;
    [SerializeField] private Transform mech;
    [SerializeField] private Transform player;  
    [SerializeField] private SteamVR_Action_Vector2 thumbstickAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("default", "Thumbstick");
    [SerializeField] private SteamVR_Action_Vector2 joysticksteering = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("JoystickSteering", "JoystickSteering");
    [SerializeField] private SteamVR_Action_Boolean gripAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "GrabGrip");
    [SerializeField] private SteamVR_Input_Sources leftHand;
    [SerializeField] private SteamVR_Input_Sources rightHand;
    [SerializeField] private Valve.VR.InteractionSystem.LinearMapping powerController; // has a single variable with a value between 0.0f and 1.0f
    [Space]
    [Header("Animation for the mech")]
    [SerializeField] private bool activateHeadrotation = false;
    [SerializeField] private bool activeAnimation = true;
    [SerializeField] private Animator animator = null;
    [SerializeField] private Animator blinders = null;
    [SerializeField] private bool rotating = false;
    [SerializeField, Range(0f, 1f)] private float joystickRotationThreshold = 0.5f;
    #endregion

    #region Properties
    public float MaxSpeed { get { return maxSpeed; } set { maxSpeed = value; } }
    public float RotateOffset { get { return rotateOffset; } set { rotateOffset = value; } }
    public Vector3 MechFacingDirection { get { return mech.forward; } }
    private float CurrentPowerPercentage { get { return powerController.value; } }
    private float CurrentAcceleration { get { return acceleration * powerEffectOnAcceleration.Evaluate(CurrentPowerPercentage); } }
    private float CurrentMaxSpeed { get { return maxSpeed * powerEffectOnMaxSpeed.Evaluate(CurrentPowerPercentage); } }
    private float FloatEpsilon { get { return MathHelper.FloatEpsilon; } }
    private Quaternion mechForward { get { return mech.transform.rotation; } }
    public bool Teleporting { get; set; } = false;
    #endregion

    #region Private Varaibles
    private Vector3 direction = Vector3.zero;
    private Transform cameraTransform;
    private RaycastHit groundCheckHit;
    private CapsuleCollider thisCollider;
    private float groundCheckDistance = 0.01f;
    private Vector3 velocity = Vector3.zero;
    private bool grounded = false;
    private bool rotationStarted = false;
    private float rotationStopDelay = 0f;
    private float rotationStopDelayMax = 0.3f;
    private int verticalRotationOrientation = 1;

    //private Vector3 velocity = Vector3.zero;

    private bool leftActive = false;
    private bool rightActive = false;

    //Bools for the animation, can be changed to triggers later
    private float playSpeed = 0;

    #endregion

    void Awake()
    {
        thisCollider = GetComponent<CapsuleCollider>();

        SetVariables();
    }

    public void SetVariables()
    {
        verticalRotationOrientation = PlayerPrefs.GetInt("InvertedYAxis");
    }

    void Start()
    {
        EventCoordinator.RegisterEventListener<PickUpEventInfo>(MechActivation);
        EventCoordinator.RegisterEventListener<DropItemEventInfo>(MechDeactivation);
        cameraTransform = Camera.main.transform;
        mech.rotation = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0);
        joysticksteering.actionSet.Activate();
    }

    void Update()
    {
        #region bypassing hold-handle requirements
        leftActive = true;
        rightActive = true;
        #endregion

        grounded = GroundCheck(out groundCheckHit);

        PlayerTriggerdRotate();
        GetDirectionToMove();

        if (rotationStopDelay > 0 && rotationStarted == false)
        {
            rotationStopDelay -= Time.deltaTime;
        }

        if (grounded == false)
        {
            velocity += Vector3.down * gravity * Time.deltaTime;
        }

        if (Teleporting == false)
        {
            CheckCollision(velocity * Time.deltaTime);
            velocity *= Mathf.Pow(airResistanceCoefficient, Time.deltaTime);
        }
        
        if (showDebugText)
        {
            SendMovementStatDebugEvent();
        }

        if (rightActive && joysticksteering[rightHand].axis.x != 0)
            rotating = true;
        else
            rotating = false;

        blinders.SetBool("Activate", rotating);
    }

    private void PlayerTriggerdRotate()
    {
        if (!rightActive)
            return;

        if (activateHeadrotation)
        {
            //Quaternion diffQuat = Quaternion.FromToRotation(new Vector3(mech.forward.x, 0.0f, mech.forward.z).normalized, new Vector3(cameraTransform.forward.x, 0.0f, cameraTransform.forward.z).normalized);
            //float angle;
            //Vector3 axis;
            //diffQuat.ToAngleAxis(out angle, out axis);
            //if (angle > rotateOffset)
            //{
            //    mech.rotation *= Quaternion.AngleAxis(angle - rotateOffset, axis);
            //}
            //if (activateBlinders)
            //    rotationStartedEvent.Invoke(this);
        }
        else if (joysticksteering[rightHand].axis.x != 0 && rotationStarted == false)
        {
            rotationStarted = true;
        }
        else if (joysticksteering[rightHand].axis.x == 0 && rotationStarted == true)
        {
            rotationStopDelay = rotationStopDelayMax;
            rotationStarted = false;
        }



        mech.transform.Rotate(0, joystickRotationSpeed * joysticksteering[rightHand].axis.x * Time.deltaTime, 0, Space.Self);
        //player.Rotate(0, joystickRotationSpeed * joysticksteering[rightHand].axis.x * Time.deltaTime, 0, Space.Self);

        if (joysticksteering[rightHand].axis.y > joystickRotationThreshold | joysticksteering[rightHand].axis.y < -joystickRotationThreshold)
        {
            float rotationAmount = (joystickRotationSpeed * joysticksteering[rightHand].axis.y * Time.deltaTime) * verticalRotationOrientation;

            if (verticalRotationPoint.localEulerAngles.x + rotationAmount < 345 && verticalRotationPoint.localEulerAngles.x + rotationAmount > 50)
            {
                rotationAmount = 345 - verticalRotationPoint.localEulerAngles.x;
            }
            
            if ((verticalRotationPoint.localEulerAngles.x + rotationAmount > 15 || verticalRotationPoint.localEulerAngles.x + rotationAmount > 375) && (verticalRotationPoint.localEulerAngles.x + rotationAmount < 50 || verticalRotationPoint.localEulerAngles.x + rotationAmount > 375))
            {
                rotationAmount = 15 - verticalRotationPoint.localEulerAngles.x;
            }

            verticalRotationPoint.Rotate(rotationAmount, 0, 0, Space.Self);
        }
    }

    private void GetDirectionToMove()
    {
        if (grounded && leftActive)
        {
            direction.x = thumbstickAction[leftHand].axis.x;
            direction.z = thumbstickAction[leftHand].axis.y;
            direction = mechForward * direction;
            direction = Vector3.ProjectOnPlane(direction, groundCheckHit.normal).normalized * direction.magnitude;

        }
        else
        {
            if (grounded)
            {
                direction = Vector3.zero;
            }
            else
            {
                direction = Vector3.Slerp(direction, velocity, 0.25f);
            }
        }

        if (direction.magnitude < FloatEpsilon)
        {
            Decelerate();
            if (activeAnimation)
                animator.SetBool("Moving", false);
        }
        else
        {
            Accelerate();
            if (activeAnimation)
                animator.SetBool("Moving", grounded);
        }

        if (activeAnimation)
        {
            playSpeed = Mathf.Min((velocity.magnitude / CurrentMaxSpeed) + 0.5f, 1f);
            animator.SetFloat("Speed", playSpeed);
        }
    }

    private void PhysicalIRLMovement()
    {
        if(!activateMech)
        {
            mech.transform.position = new Vector3(player.position.x, mech.transform.position.y, player.position.z);
        }
    }

    private void Accelerate()
    {
        //if(direction.magnitude > 0)
        //{
        //    float currentAcceleration = acceleration;
        //    if(currentPowerPercentage < 1.0f)
        //    {
        //        currentAcceleration *= currentPowerPercentage;
        //    }
        //    else
        //    {
        //        currentAcceleration *= 1.0f + (positivePowerEffectOnAcceleration * (currentPowerPercentage - 1.0f));
        //    }
        //    rb.AddForce(mechForward * direction * currentAcceleration * Time.deltaTime, ForceMode.Impulse);
        //    //rb.velocity += direction * currentAcceleration * Time.deltaTime;
        //}
        //else
        //{
        //    rb.AddForce(-rb.velocity * deceleration * Time.deltaTime, ForceMode.Impulse);
        //    //rb.velocity -= rb.velocity * deceleration * Time.deltaTime;
        //    if(Mathf.Approximately(rb.velocity.magnitude, 0.1f))
        //    {
        //        rb.velocity = Vector3.zero;
        //    }
        //}

        Vector3 velocityOnGround = Vector3.ProjectOnPlane(velocity, groundCheckHit.normal);

        velocity += direction.normalized * CurrentAcceleration * Time.deltaTime;
        velocity = Vector3.Lerp(velocity, direction.normalized * velocity.magnitude, turnSpeedModifier * Time.deltaTime);

        // TODO: this might need to be changed due to the effect that the power system has on the current max speed.
        if (velocity.sqrMagnitude > CurrentMaxSpeed * CurrentMaxSpeed)
        {
            velocity = velocity.normalized * CurrentMaxSpeed;
        }

        //mech.transform.position += mechForward * velocity * Time.deltaTime;
        //player.position += mechForward * velocity * Time.deltaTime;
    }

    private void Decelerate()
    {
        Vector3 velocityOnGround = Vector3.ProjectOnPlane(velocity, groundCheckHit.normal);

        Vector3 decelerationVector = velocityOnGround.normalized * deceleration * Time.deltaTime;

        if (decelerationVector.sqrMagnitude > velocityOnGround.sqrMagnitude)
            velocity = Vector3.zero;
        else
            velocity -= decelerationVector;
    }

    private void SendMovementStatDebugEvent()
    {
        SendTextToUI("Current mech acceleration: " + CurrentAcceleration +
            "\nCurrent mech max speed: " + CurrentMaxSpeed +
            "\nCurrent mech acceleration modifier: " + powerEffectOnAcceleration.Evaluate(CurrentPowerPercentage) +
            "\nCurrent mech max speed modifier: " + powerEffectOnMaxSpeed.Evaluate(CurrentPowerPercentage));
    }

    private void SendTextToUI(string name)
    {
        DebugEventInfo dei = new DebugEventInfo(gameObject, name, 0);
        EventCoordinator.ActivateEvent(dei);
    }

    private void MechActivation(EventInfo ei)
    {
        PickUpEventInfo puei = (PickUpEventInfo)ei;
        if (puei.Hand == leftHand)
        {
            leftActive = true;
        }
        else if(puei.Hand == rightHand)
        {
            rightActive = true;
        }
    }

    private void MechDeactivation(EventInfo ei)
    {
        DropItemEventInfo diei = (DropItemEventInfo)ei;
        if (diei.Hand == leftHand)
        {
            leftActive = false;
        }
        else if (diei.Hand == rightHand)
        {
            rightActive = false;
        }
    }

    private bool FindCollision(Vector3 direction, float maxDistance)
    {
        return FindCollision(direction, out RaycastHit raycastHit, maxDistance);
    }

    private bool FindCollision(Vector3 direction, out RaycastHit raycastHit, float maxDistance)
    {
        Vector3 topPoint = transform.position + thisCollider.center + transform.up * (thisCollider.height / 2 - thisCollider.radius);
        Vector3 bottomPoint = transform.position + thisCollider.center - transform.up * (thisCollider.height / 2 - thisCollider.radius);

        return Physics.CapsuleCast(topPoint, bottomPoint, thisCollider.radius, direction.normalized, out raycastHit, maxDistance, collisionLayers, QueryTriggerInteraction.Ignore);
    }

    private bool GroundCheck()
    {
        return GroundCheck(out RaycastHit raycastHit);
    }

    private bool GroundCheck(out RaycastHit raycastHit)
    {
        bool grounded = FindCollision(Vector3.down, out raycastHit, groundCheckDistance + skinWidth);
        return grounded;
    }

    private void CheckCollision(Vector3 movement)
    {
        bool collisionFound = FindCollision(movement.normalized, out RaycastHit collisionHit, Mathf.Infinity);

        if (collisionFound)
        {
            Vector3 hitNormal = collisionHit.normal;

            //Räkna ut vinkeln mellan ytan som träffas och hur mechen rör sig, sedan med sinus räkna ut hur långt ifrån ytan man måste stanna för att vara minst skinwidth ifrån.
            float angle = (Vector3.Angle(hitNormal, movement.normalized) - 90) * Mathf.Deg2Rad;
            float snapDistanceFromHit = skinWidth / Mathf.Sin(angle);

            //Räknar ut hur långt man kan nå med snapmovment så den stannar precis innan en collision och begränsar längden till movement
            Vector3 snapMovement = movement.normalized * (collisionHit.distance - snapDistanceFromHit);
            snapMovement = Vector3.ClampMagnitude(snapMovement, movement.magnitude);

            //Nu blir movement det som är kvar efter den åkt fram till en kant eller så långt den når
            movement -= snapMovement;

            //räkna ut normalkraften och lägg till på movement så man får en korrekt återstående kraft efter att ha träffat ytan.
            Vector3 hitNormalMovement = MathHelper.NormalForce(movement, hitNormal);
            movement += hitNormalMovement;

            Vector3 topPoint = transform.position + snapMovement + thisCollider.center + transform.up * (thisCollider.height / 2 - thisCollider.radius);
            Vector3 bottomPoint = transform.position + snapMovement + thisCollider.center - transform.up * (thisCollider.height / 2 - thisCollider.radius);
            if (Physics.CapsuleCast(topPoint, bottomPoint, thisCollider.radius, Vector3.down, Mathf.Infinity, collisionLayers, QueryTriggerInteraction.Ignore))
            {
                //Placera spelaren så långt fram som möjligt utan att krocka.
                transform.position += snapMovement;
            }


            if (movement.y > 0.0f && Vector3.Angle(hitNormal, Vector3.up) > movementAngle)
            {
                movement.y = 0.0f;
            }

            if (hitNormalMovement.sqrMagnitude > FloatEpsilon * FloatEpsilon)
            {
                velocity += MathHelper.NormalForce(velocity, hitNormal);
            }
            if(movement.sqrMagnitude > FloatEpsilon * FloatEpsilon)
            {
                CheckCollision(movement);
            }






            //if(Vector3.Angle(hitNormal, Vector3.up) < movementAngle)
            //{
            //    if(hitNormalMovement.sqrMagnitude > floatEpsilon * floatEpsilon)
            //    {
            //        velocity += MathHelper.NormalForce(velocity, hitNormal);
            //    }

            //    if(movement.sqrMagnitude > floatEpsilon * floatEpsilon)
            //    {
            //        CheckCollision(movement);
            //    }
            //}
            //else if(hitNormalMovement.sqrMagnitude > floatEpsilon * floatEpsilon)
            //{
            //    velocity = Vector3.zero;
            //}




            //if (angle * Mathf.Rad2Deg < movementAngle)
            //{
            //    if(hitNormalMovement.magnitude > floatEpsilon)
            //    {
            //        //handle collision
            //        velocity += MathHelper.NormalForce(velocity, hitNormal);
            //    }

            //    if (movement.magnitude > floatEpsilon)
            //    {
            //        //testa kollision på kraften som finns kvar efter collision
            //        CheckCollision(movement);
            //    }
            //}

            
        }
        else if (movement.magnitude > FloatEpsilon)
        {
            transform.position += movement;
        }
    }

    [System.Serializable]
    public class RotationEvent : UnityEvent<MechMovement> { }
}
