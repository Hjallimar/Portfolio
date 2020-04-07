using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WallComponent : MonoBehaviour
{
    [Header("Wallbase is the prefab game obj")]
    [SerializeField] private GameObject wallBase = null;
    [Header("The Parent is the one it should move towards")]
    [SerializeField] private GameObject parent;
    [Header("The Navmesh Obstacle")]
    [SerializeField] private NavMeshObstacle obstacle = null;
    [Header("Time in seconds for the door to close")]
    [SerializeField] private float timeToClose;
    private Vector3 startPos;
    private float timelaps;
    private float timer = 0;
    bool open = false;
    bool close = true;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        timelaps = 1 / timeToClose;
        EventCoordinator.RegisterEventListener<GateDestroyedEventInfo>(Destroyed);
        EventCoordinator.RegisterEventListener<AdjustDoorEventInfo>(AdjustDoor);
    }

    private void OnDestroy()
    {
        EventCoordinator.UnregisterEventListener<GateDestroyedEventInfo>(Destroyed);
        EventCoordinator.UnregisterEventListener<AdjustDoorEventInfo>(AdjustDoor);
    }

    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            if (timer < 1)
            {
                timer += timelaps * Time.deltaTime;
                AdjustGate(timer);
            }
        }
        else if (close)
        {
            if (timer > 0)
            {
                timer -= timelaps * Time.deltaTime;
                AdjustGate(timer);
            }
        }
    }

    void AdjustGate(float t)
    {
        transform.position = Vector3.Lerp(startPos, parent.transform.position, t);
    }


    void AdjustDoor(EventInfo ei)
    {
        if (ei.GO.name == wallBase.name)
        {
            open = !open;
            close = !close;
        }
    }
    void Destroyed(EventInfo ei)
    {
        if (ei.GO.name == wallBase.name)
        {
            if (obstacle != null)
                obstacle.enabled = false;
        }
    }
}
