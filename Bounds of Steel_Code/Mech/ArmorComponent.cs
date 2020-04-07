using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorComponent : MonoBehaviour
{
    [SerializeField] private List<GameObject> armorPlates = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        EventCoordinator.RegisterEventListener<DetatchArmorEventInfo>(DetatchArmorePlate);
    }
    private void OnDestroy()
    {
        EventCoordinator.UnregisterEventListener<DetatchArmorEventInfo>(DetatchArmorePlate);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            DetatchArmorEventInfo daei = new DetatchArmorEventInfo(gameObject, "Detach armor plate");
            EventCoordinator.ActivateEvent(daei);
        }
    }
    private void DetatchArmorePlate(EventInfo ei)
    {
        if(ei.GO.name == gameObject.name && armorPlates.Count > 0)
        {
            foreach(GameObject go in armorPlates)
            {
                if(go != null)
                {
                    go.GetComponent<Rigidbody>().isKinematic = false;
                    go.GetComponent<BoxCollider>().enabled = true;
                    go.transform.parent = null;
                }
            }
            armorPlates.Clear();
        }
    }
}
