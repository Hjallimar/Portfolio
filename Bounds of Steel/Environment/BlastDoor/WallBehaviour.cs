using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class WallBehaviour : MonoBehaviour
{
    [SerializeField] private float alertRange = 100.0f;
    [SerializeField] private Animator anim = null;
    [SerializeField] private NavMeshObstacle obstacle = null;
    [SerializeField] private AudioSource sound = null;
    private GameObject player = null;

    bool open = false;
    float timer = 0;
    void Start()
    {
        player = GameController.Player;
    }

    void OnDestroy()
    {
    }

    
    public void DestroyedGate()
    {
        GateDestroyedEventInfo gdei = new GateDestroyedEventInfo(gameObject, "Destroyed");
        EventCoordinator.ActivateEvent(gdei);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        {
            OpenTheGate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == player)
        {
            CloseTheGate();
        }
    }

    void OpenTheGate()
    {
        Debug.Log("I shall open");
        anim.SetTrigger("OpenGate");
        obstacle.enabled = false;
        sound.Play();
    }

    void CloseTheGate()
    {
        Debug.Log("I shall close");
        anim.SetTrigger("CloseGate");
        obstacle.enabled = true;
        sound.Play();
    }
}


