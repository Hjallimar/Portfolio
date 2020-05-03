using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfPack : MonoBehaviour
{
    public List<GameObject> wolves;
    public GameObject Alpha;
    public GameObject prey;
    public bool prayFound;
    //public GameObject wolfPrefab;
    public Vector3 fleePosition;

    public float packSize;
    public Vector3 packCenter;
    public Vector3 PackVelocity;

    public float radius = 2f;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject w in wolves)
        {
            Vector3 distance = w.transform.position - transform.position;
            //if ( distance.magnitude > radius)
            //change position of where to go;
        }
        packSize = wolves.Count;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TimeToRun()
    {
        fleePosition = new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
    }

    public void SetPrey(GameObject newPrey)
    {
        prey = newPrey;
    }


}
