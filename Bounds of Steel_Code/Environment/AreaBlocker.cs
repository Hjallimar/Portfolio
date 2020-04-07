using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaBlocker : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float health = 200f;
    [SerializeField] private float activeHealth = 200f;

    [Header("Blockers")]
    [SerializeField] private GameObject blockade1 = null;
    [SerializeField] private GameObject blockade2 = null;

    [Header("Hitboxes")]
    [SerializeField] private GameObject hitBox = null;

    [SerializeField] private GameObject activeHitbox1 = null;
    [SerializeField] private GameObject activeHitbox2 = null;

    [Header("Rocks, must have rb and collider")]
    [SerializeField] private List<GameObject> Rocks = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        EventCoordinator.RegisterEventListener<DamageEventInfo>(TakeDamage);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ActivateBlockade();
        }
        
    }

    void TakeDamage(EventInfo ei)
    {
        DamageEventInfo dei = (DamageEventInfo)ei;
        if (dei.ObjectHit == hitBox)
        {
            if (health > 0)
            {
                health -= dei.Damage;
            }
            if(health <= 0)
            {
                ActivateBlockade();
            }
        }
        else if(dei.ObjectHit == activeHitbox1 || dei.ObjectHit == activeHitbox2)
        {
            if (activeHealth > 0)
            {
                activeHealth -= dei.Damage;
            }

            if(activeHealth <= 0)
            {
                DeactivateBlockade();
            }
        }
    }


    void ActivateBlockade()
    {
        hitBox.SetActive(false);

        blockade1.SetActive(true);
        blockade2.SetActive(true);

        foreach(GameObject gO in Rocks)
        {
            gO.GetComponent<Rigidbody>().isKinematic = false;
            gO.GetComponent<Collider>().enabled = true;
        }
    }

    void DeactivateBlockade()
    {
        blockade1.SetActive(false);
        blockade2.SetActive(false);
    }
}
