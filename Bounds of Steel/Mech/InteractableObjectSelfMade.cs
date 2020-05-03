using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;

public class InteractableObjectSelfMade : MonoBehaviour
{
    [Header("VR Actions and hand")]
    [SerializeField] private SteamVR_Action_Boolean gripAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "GrabGrip");
    [SerializeField] private SteamVR_Input_Sources hand;

    private Transform parent = null;
    private bool grabbed = false;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.localPosition;
    }
    // Update is called once per frame
    void Update()
    {

        if (gripAction[hand].stateUp && grabbed)
        {
            DeactivateMech();
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (gripAction[hand].stateDown && !grabbed)
        {
            ActivateMech(other.gameObject);
        }
    }

    private void ActivateMech(GameObject other)
    {
        grabbed = true;
        parent = transform.parent;
        transform.parent = other.transform;
        PickUpEventInfo amei = new PickUpEventInfo(gameObject, "Sending an activate mech event", hand);
        EventCoordinator.ActivateEvent(amei);
    }

    private void DeactivateMech()
    {
        grabbed = false;
        transform.parent = parent;
        moveTowardsStartPos();
        DropItemEventInfo dmei = new DropItemEventInfo(gameObject, "Sending an activate mech event", hand);
        EventCoordinator.ActivateEvent(dmei);
        
    }

    private void moveTowardsStartPos()
    {
        transform.localPosition = startPos;
    }
}
