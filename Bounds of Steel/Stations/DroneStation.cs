using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneStation : StationBase
{
    [SerializeField] private GameObject dronePrefab = null;
    [SerializeField] private float constructNewDroneTimer = 60f;
    [SerializeField] private List<GameObject> stations = new List<GameObject>();

    [SerializeField] private List<Transform> stablePos = new List<Transform>();

    private Dictionary<Vector3, GameObject> drones = new Dictionary<Vector3, GameObject>();
    private GameObject targetToRepair = null;

    private bool active = false;
    private bool reparing = false;
    private float construcionTimer = 0;
    new void Start()
    {
        base.Start();
        EventCoordinator.RegisterEventListener<AssignTargetToRepairEventInfo>(SendDronesToRepair);

        stationActivated += ActivateRepairDrone;
        stationDeactivated += DeactivateRepairDrone;
        targetToRepair = stations[0];
        foreach(Transform trans in stablePos)
        {
            GameObject drone = Instantiate(dronePrefab);
            drone.transform.position = trans.position;
            drone.GetComponent<RepairDrone>().Stable = trans.position;
            drones.Add(trans.position, drone);
        }
    }

    new void Update()
    {
        base.Update();

        if (active)
        {
            if (drones.Count < stablePos.Count)
            {
                construcionTimer += Time.deltaTime;
                if (construcionTimer >= constructNewDroneTimer)
                {
                    ConstructNewDrone();
                }
            }
        }



    }

    private void ActivateRepairDrone()
    {
        active = true;
        //Send event that updates the mech artillery so the player can choose what to repair
    }
    private void DeactivateRepairDrone()
    {
        active = false;
        //Send event that updates the mech artillery so the player can't choose what to repair
    }

    public void PrintDeathTestText()
    {
        Debug.Log("main station deded");
    }

    private void DeadDrone(EventInfo ei)
    {
        if (drones.ContainsValue(ei.GO))
        {
            drones.Remove(ei.GO.GetComponent<RepairDrone>().Stable);
        }
    }

    private void ConstructNewDrone()
    {
        bool constructed = false;
        foreach (Transform trans in stablePos)
        {
            if (!drones.ContainsKey(trans.position) && !constructed)
            {
                GameObject drone = Instantiate(dronePrefab);
                drone.transform.position = trans.position;
                drone.GetComponent<RepairDrone>().Stable = trans.position;
                drones.Add(trans.position, drone);
                construcionTimer = 0;
                constructed = true;
            }
        }
    }

    private void SendDronesToRepair(EventInfo ei)
    {
        AssignTargetToRepairEventInfo attrei = (AssignTargetToRepairEventInfo)ei;

        if (!reparing)
        {
            targetToRepair = attrei.Target;
            foreach (Vector3 key in drones.Keys)
            {
                drones[key].GetComponent<RepairDrone>().StartRepairTarget(targetToRepair);
            }
            reparing = true;
        }
    }

    private void dronesGoingToReturn()
    {

    }
}
