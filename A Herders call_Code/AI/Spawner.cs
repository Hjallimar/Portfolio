using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

public class Spawner : MonoBehaviour
{
    [SerializeField] private string NameOfPrefab;
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject refObjectPool;
    private GameObject spawnedObj = null;
    private float checkForSpawn = 0;
    
    void Start()
    {
        TestSpawn();
    }

    /// <summary>
    /// Checks spawn every minute
    /// </summary>
    private void Update()
    {
        checkForSpawn += Time.deltaTime;
        if(checkForSpawn >= 60)
        {
            TestSpawn();
            checkForSpawn = 0f;
        }
    }

    public void SpawnNewInstance()
    {
        Spawn();
    }

    /// <summary>
    /// Spawns object from the <see cref="ObjectPool"/> in <see cref="refObjectPool"/>
    /// </summary>
    private void Spawn()
    {
        if(refObjectPool.GetComponent<ObjectPool>().SpawnObjectFromPool(NameOfPrefab, transform.position))
        {
            refObjectPool.GetComponent<ObjectPool>().SpawnObjectFromPool(NameOfPrefab, transform.position);
            Debug.Log("Seting active on new Object");
        }
        else
        {
            spawnedObj = Instantiate(prefab);
            ActivateSpawnedScript();
            spawnedObj.transform.position = this.transform.position;
            refObjectPool.GetComponent<ObjectPool>().AddObjectToPool(NameOfPrefab, prefab);
            Debug.Log("No objects avalible, adding new");
            
        }
    }

    /// <summary>
    /// Spawns and object if <see cref="spawnedObj"/> does not exist in the game yet
    /// </summary>
    public void TestSpawn()
    {
        if (spawnedObj == null)
            spawnedObj = Instantiate(prefab, transform.position, Quaternion.identity);
        //refObjectPool.GetComponent<ObjectPool>().AddObjectToPool(NameOfPrefab, prefab);

        
    }

    private void ActivateSpawnedScript()
    {
        switch (name)
        {
            case "Cow":
                spawnedObj.GetComponent<CowSM>().enabled = true;
                break;
            case "Wolf":
                spawnedObj.GetComponent<AlphaStateMachine>().enabled = true;
                break;
            case "Giant":
                spawnedObj.GetComponent<GiantSM>().enabled = true;
                break;
            default:
                break;
        }
    }


}
