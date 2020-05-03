using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Author: Hjalmar Andersson

public class SwitchSelectedButton : MonoBehaviour
{
    [SerializeField] private GameObject select;

    /// <summary>
    /// Switches the EventSystems selectedGameobject to the scripts select
    /// </summary>
    public void Switch()
    {
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(select, null);
    }
}
