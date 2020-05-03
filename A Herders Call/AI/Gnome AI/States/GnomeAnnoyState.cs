using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

[CreateAssetMenu(menuName ="Gnome/GnomeAnnoyState")]
public class GnomeAnnoyState : GnomeBaseState
{
    private float timer = 0;
    private float animationTimer = 0;
    private bool hasAttacked = false;
    [SerializeField] public float cooldown;
    
    /// <summary>
    /// Adds <see cref="Time.deltaTime"/> to timer in order to create a cooldown on how often the game object can attack.
    /// </summary>
    public override void Update()
    {
        timer += Time.deltaTime;
        if (Target == null)
            Debug.Log("There is no target");
        if (CheckRemainingDistance(Target.transform.position, 4.5f))
        {
            if (hasAttacked)
            {
                animationTimer += Time.deltaTime;
            }

            if(timer >= cooldown)
            {

                Audio.PlayOneShot(Sounds[GetSoundIndex(8, 13.9f)]);
                Target.GetComponent<CowSM>().AttackTheCow(0);
                timer = 0f;
                Anim.SetBool("GnomeAttacking", true);
                hasAttacked = true;
            }
            else if (animationTimer >= 1.3f)
            {
                Anim.SetBool("GnomeAttacking", false);
                animationTimer = 0;
            }
            
        }
            AIagent.SetDestination(Target.transform.position);
        
    }

    public override void Exit()
    {
        Anim.SetBool("GnomeAttacking", false);
    }
}
