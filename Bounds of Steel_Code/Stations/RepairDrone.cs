using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RepairDrone : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private GameObject targetToRepair = null;
    [SerializeField] private GameObject mesh = null;
    [SerializeField] private Transform rotatePoint = null;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float repairSpeed = 10f;
    [SerializeField] private float repairDistance = 10f;

    private Vector3 stable = Vector3.zero;
    public Vector3 Stable { get { return stable; } set { stable = value; } }
    private Vector3 meshStart = Vector3.zero;
    private Quaternion meshRot = Quaternion.identity;
    private bool repairing = false;
    bool active = false;
    // Start is called before the first frame update
    void Start()
    {
        EventCoordinator.RegisterEventListener<RepairingDoneEventInfo>(DoneWithRepair);
    }

    private void Awake()
    {
        stable = transform.position;
        meshStart = mesh.transform.localPosition;
        meshRot = mesh.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            float distance = (agent.destination - transform.position).magnitude;
            if (distance < repairDistance && !repairing)
            {
                StartDroneRepairEventInfo sdrei = new StartDroneRepairEventInfo(gameObject, targetToRepair, repairSpeed);
                EventCoordinator.ActivateEvent(sdrei);
                repairing = true;
            }
            else if (repairing)
            {
               mesh.transform.RotateAround(rotatePoint.position, Vector3.up, 2);
            }
        }
    }

    public void StartRepairTarget(GameObject target)
    {
        targetToRepair = target;
        agent.SetDestination(targetToRepair.transform.position);
        agent.speed = speed;
        active = true;
    }

    public void DoneWithRepair(EventInfo ei)
    {
        RepairingDoneEventInfo rdei = (RepairingDoneEventInfo)ei;
        if(rdei.GO == targetToRepair)
        {
            agent.SetDestination(stable);
            mesh.transform.localPosition = meshStart;
            mesh.transform.localRotation = meshRot;
            active = false;
            repairing = false;
            targetToRepair = null;
        }
    }
}
