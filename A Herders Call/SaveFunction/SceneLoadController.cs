using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadController : MonoBehaviour
{
    public static bool LoadSaveData;
    /// <summary>
    /// Controlls loading of data through a static bool that exists between scenes.
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
