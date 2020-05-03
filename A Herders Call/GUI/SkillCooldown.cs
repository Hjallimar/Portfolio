using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Main Author: Hjalmar Andersson

public class SkillCooldown : MonoBehaviour
{
    bool used = false;
    public Image picture;
    public float coolDown;
    
    void Start()
    {
        coolDown = 5;
        used = false;
        coolDown = 1 / coolDown;
        picture.fillAmount = 1f;

    }
    
    /// <summary>
    /// Controlls the cooldown on runes
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            used = true;
            picture.fillAmount = 0f;
        }
        if (used == true)
        {
            picture.fillAmount += coolDown * Time.deltaTime;
            if (picture.fillAmount == 1)
            {
                Debug.Log("Done!");
                used = false;
            }
        }
    }  
    
}
