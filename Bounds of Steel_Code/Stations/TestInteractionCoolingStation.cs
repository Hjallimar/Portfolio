using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractionCoolingStation : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private float coolantPerSecond = 10;
    [SerializeField] private float timeBeforeActivate = 2;

    [Header("Testing")]
    [SerializeField] private GameObject station = null;
    [SerializeField] private Material refillMat = null;
    private Material standardMat;


    private GameObject player = null;
    private Vector3 playerPos = Vector3.zero;
    private Transform previoursPosition = null;
    private bool moved = false;
    private float timer = 0;
    private bool refilling = false;
    void Start()
    {
        standardMat = station.GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        if(player != null)
        {
            Debug.Log(player.transform.position + " : " + playerPos);
            timer += Time.deltaTime;
            if (player.transform.position != playerPos)
            {
                Debug.Log("player Moved");
                moved = true;
                playerPos = player.transform.position;
            }
            else
            {
                moved = false;
                Debug.Log("player not moving");

            }

            if (moved)
            {
                timer = 0;
                if (refilling)
                {
                    station.GetComponent<MeshRenderer>().material = standardMat;
                    CoolantRefillStatusEventInfo crsei = new CoolantRefillStatusEventInfo(gameObject, false, 0);
                    EventCoordinator.ActivateEvent(crsei);
                }
            }

            if(timer > timeBeforeActivate && !refilling)
            {
                station.GetComponent<MeshRenderer>().material = refillMat;
                CoolantRefillStatusEventInfo crsei = new CoolantRefillStatusEventInfo(gameObject, true, coolantPerSecond);
                EventCoordinator.ActivateEvent(crsei);
                refilling = true;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            player = other.gameObject;

            playerPos = player.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            player = null;

            playerPos = Vector3.zero;
        }
    }
}
