using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main Author: Hjalmar Andersson

public class CameraScript: MonoBehaviour
{
    [SerializeField] private Vector3 cameraDistance = new Vector3(0, 0, -6);
    [SerializeField] private GameObject playerView;
    [SerializeField] private LayerMask cameraMask;
    private Vector3 direction;

    private Vector3 zoomOffset = new Vector3(0.5f, 0.3f, 0f);
    private Vector3 startLocation;
    private Vector3 maxLocation;

    private bool adjustCamera;

    private float mouseSensitivity = 2;

    /// <summary>
    /// Gives <see cref="direction"/>, <see cref="startLocation"/> and <see cref="maxLocation"/> values upon awake.
    /// </summary>
    private void Awake()
    {
        direction = transform.rotation.eulerAngles;
        startLocation = playerView.transform.position;
        maxLocation = startLocation + zoomOffset;
    }

    private void Start()
    {
        adjustCamera = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.CinematicFreeze, DisableAdjust);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.CinematicResume, EnableAdjust);
    }

    /// <summary>
    /// Locks and unlocks the mouse depending on <see cref="Time.timeScale"/> and controlls camera from movement input.
    /// </summary>
    void Update()
    {

        // if down rb scroll value = -1, else if 
        if (adjustCamera == false)
            return;
        else
        {
            ZoomHandler();
            direction.x -= mouseSensitivity * Input.GetAxisRaw("Camera Y");
            direction.y += mouseSensitivity * Input.GetAxisRaw("Camera X");
            direction.x = Mathf.Clamp(direction.x, -50, 50);
            transform.rotation = Quaternion.Euler(direction.x, direction.y, 0f);
            CameraPosition();

            NavigationEventInfo nei = new NavigationEventInfo { navFloat = transform.rotation.eulerAngles.y };
            EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.Navigation, nei);
        }
        
    }

    /// <summary>
    /// Sends raycasts from the player to the cameras position to controll where the player will be placed.
    /// </summary>
    protected void CameraPosition()
    {
        RaycastHit SphereRayCast;
        Vector3 point = transform.rotation * cameraDistance;
        

        if (Physics.SphereCast(playerView.transform.position, 0.3f, point.normalized, out SphereRayCast, Mathf.Abs(cameraDistance.magnitude), cameraMask))
        {
           transform.position = transform.rotation * (cameraDistance.normalized * SphereRayCast.distance) + playerView.transform.position;
        }
        else
        {
            transform.position = transform.rotation * cameraDistance + playerView.transform.position;
        }
    }

    /// <summary>
    /// Receives mouse input to zoom the camera backwards and forwards.
    /// </summary>
    protected void ZoomHandler()
    {
        bool changed = false;
        if(Input.GetAxisRaw("Zoom") < 0 || Input.GetButton("Zoom out"))//if RB do this
        {
            cameraDistance.z -= 8 * Time.deltaTime;

            changed = true;
        }
        else if(Input.GetAxisRaw("Zoom") > 0 || Input.GetButton("Zoom in"))// if LB do this
        {
            cameraDistance.z += 8 * Time.deltaTime;
            changed = true;
        }
        if (changed)
        {
            if (cameraDistance.z > -2)
                cameraDistance.z = -2;
            else if (cameraDistance.z < -8)
                cameraDistance.z = -8;
        }
    }

    private void DistanceReached()
    {
        if (playerView.transform.position.x >= maxLocation.x ||
            playerView.transform.position.y >= maxLocation.y)
            playerView.transform.localPosition = maxLocation;
        else if (playerView.transform.position.x <= startLocation.x ||
            playerView.transform.position.y <= startLocation.y)
            playerView.transform.localPosition = startLocation;
    }

    /// <summary>
    /// Disables the camera fuction to turn around the camera or to zoom
    /// </summary>
    /// <param name="eventInfo"></param>
    private void DisableAdjust(EventInfo eventInfo)
    {
        adjustCamera = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// Enables the camrea fuctions again
    /// </summary>
    /// <param name="eventInfo"></param>
    private void EnableAdjust(EventInfo eventInfo)
    {
        adjustCamera = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}