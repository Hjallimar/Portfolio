using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

/// <summary>
/// This script is no longer in use
/// </summary>
public class ObjectPool : MonoBehaviour
{

    static public Dictionary<string, List<GameObject>> objectPool;

    /// <summary>
    /// Creates the <see cref="objectPool"/> Dictionary
    /// </summary>
    void Start()
    {
        objectPool = new Dictionary<string, List<GameObject>>();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Adds an object to the Dictionary <see cref="objectPool"/>
    /// If the name of the object already is an existing dictionary it adds it to the list.
    /// If the objects name doesn't yet exist it creates a new key and list.
    /// </summary>
    /// <param name="objectName"></param> is the name of the object that will be added to the dictionary
    /// <param name="go"></param> is a reference to the object that gets added
    public void AddObjectToPool(string objectName, GameObject go)
    {
        if (objectPool.ContainsKey(objectName))
        {
            objectPool[objectName].Add(go);
            Debug.Log("Already contained key, adding to list");
        }
        else
        {
            List<GameObject> newList = new List<GameObject>();
            newList.Add(go);
            objectPool.Add(objectName, newList);
            Debug.Log("No existing key, adding list");
        }

    }

    /// <summary>
    /// Reactivates an inactive object from the dictionary on a new position instead of instantiating a new one.
    /// </summary>
    /// <param name="objectName"></param>
    /// <param name="newPos"></param> Spawning location as <see cref="Vector3"/>
    /// <returns></returns>
    public bool SpawnObjectFromPool(string objectName, Vector3 newPos)
    {
        if (objectPool.ContainsKey(objectName))
        {
            Debug.Log("Key exists");
            foreach(GameObject go in objectPool[objectName])
            {
                Debug.Log("Inside loop");
                if(!go.activeInHierarchy)
                {
                    Debug.Log("Found inactive object in list");
                    go.SetActive(true);
                    go.transform.position = newPos;
                    return true;
                }
            }

            

            Debug.Log("No gameobjects of that instance avalible");
            return false;

        }

        Debug.Log("No gameobjects of that instance");
        return false;
    }
}
