using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTelporterScript : MonoBehaviour
{
    //Fixa så att spelaren kan gå in, låsas i pos, inte kunna styra, dörren stängs och spelaren förflyttas med meshen till ett ställe där den ska, sedan öppnas dörren.

    [SerializeField] private Transform destination = null;
    [SerializeField] private GameObject mech = null;
    [SerializeField] private float doorTimer = 3;

    [Header("The Pod")]
    [SerializeField] private GameObject doors = null;
    private Vector3 openPos = Vector3.zero;
    private Vector3 closedPos = Vector3.zero;

    [Header("The Base Pod")]
    [SerializeField] private GameObject baseDoors = null;
    private Vector3 baseOpenPos = Vector3.zero;
    private Vector3 baseClosePos = Vector3.zero;

    private bool teleporting = false;
    private bool closed = false;
    private float timer = 0;
    private MechMovement mechMovement = null;

    private Vector3 posOffset = Vector3.zero;
    void Start()
    {
        openPos = doors.transform.position;
        closedPos = openPos;
        closedPos.y += 30;

        baseClosePos = baseDoors.transform.position;
        baseOpenPos = baseClosePos;
        baseOpenPos.y -= 30;

        mechMovement = mech.GetComponent<MechMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T) && !teleporting)
        {
            teleporting = true;
            StartCoroutine(TeleportPlayer());
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == mech)
        {
            teleporting = true;
            StartCoroutine(TeleportPlayer());
            baseDoors.transform.position = baseClosePos;
        }
    }

    void openTheDoor()
    {
        doors.transform.position = Vector3.Lerp(closedPos, openPos, timer / doorTimer);
        baseDoors.transform.position = Vector3.Lerp(baseClosePos, baseOpenPos, timer / doorTimer);

        if (timer / doorTimer >= 1)
        {
            Debug.Log("openpos: " + openPos + ", Timer " + (timer/3));
            closed = false;
        }
    }

    void closeTheDoor()
    {
        doors.transform.position = Vector3.Lerp(openPos, closedPos, timer / doorTimer);

        if (timer / doorTimer >= 1)
        {
            Debug.Log("openpos: " + openPos + ", Timer " + (timer / 3));
            closed = true;
        }
    }

    private IEnumerator TeleportPlayer()
    {
        TeleportationDoneEventInfo tdei = new TeleportationDoneEventInfo(gameObject);
        EventCoordinator.ActivateEvent(tdei);
        mechMovement.Teleporting = true;
        timer = 0;
        posOffset = transform.position - mech.transform.position;
        while (teleporting)
        {

            while (!closed)
            {
                closeTheDoor();
                timer += Time.deltaTime;
                yield return null;
            }
            timer = 0;
            mech.transform.position = destination.position - posOffset;
            while (closed)
            {
                openTheDoor();
                timer += Time.deltaTime;
                yield return null;
            }
            teleporting = false;
        }
        mechMovement.Teleporting = false;
        timer = 0;
    }
}
