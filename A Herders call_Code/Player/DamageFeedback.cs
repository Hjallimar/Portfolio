using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Author: Hjalmar Andersson

public class DamageFeedback : MonoBehaviour
{

    [SerializeField] private Image damageIndicator;
    [SerializeField] private Image healthFrame;
    [SerializeField] private float healOverTime;
    [SerializeField] private Image deathPanel;
    [SerializeField] private Text deathText;
    private Color imageColor;
    private float Health = 100;
    private float timer = 10;
    private bool activeHoT;

    void Start()
    {
        activeHoT = false;
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.Damage, FlashScreen);
        imageColor = damageIndicator.color;
        imageColor.a = 0f;
        damageIndicator.color = imageColor;
        timer = healOverTime;
    }

 
    /// <summary>
    /// Determins what ´to do when a damageEvent has been recived
    /// </summary>
    /// <param name="eventInfo"></param>
    private void FlashScreen(EventInfo eventInfo)
    {
        DamageEventInfo dei = (DamageEventInfo)eventInfo;
        if (dei.damage < 0)
            return;
        Health -= dei.damage * 10;
        if(Health > 0) { 
            imageColor.a = (100 - Health)/200;
            healthFrame.color = imageColor;
            StartCoroutine(Flash());
        }
        else
        {
            StartCoroutine(FadeToDeathAndBack());
            imageColor.a = 0;
            healthFrame.color = imageColor;
            Health = 100;
        }
        Debug.Log("I'm attacked");
        if(activeHoT == false)
        {
            StartCoroutine(TimeToHeal());
        }
        else
        {
            timer += healOverTime;
        }
    }

    /// <summary>
    /// Flashes the players screen with a Image to indicate that the player as taken damage
    /// </summary>
    /// <returns></returns>
    private IEnumerator Flash()
    {
        Debug.Log("Running");
        float dmgTick = 0;
        while(dmgTick <= 0.1)
        {
            dmgTick += Time.deltaTime;
            imageColor.a = dmgTick * 8;
            damageIndicator.color = imageColor;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        dmgTick *= 3;
        while (dmgTick >= 0)
        {
            dmgTick -= Time.deltaTime;
            imageColor.a = dmgTick * 8;
            damageIndicator.color = imageColor;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        imageColor.a = 0f;
        damageIndicator.color = imageColor;
    }

    /// <summary>
    /// If the player has not taken damage for timer amount of Seconds, then the player will restore 10 hp and the red indicator will tone out slightly
    /// </summary>
    /// <returns></returns>
    private IEnumerator TimeToHeal()
    {
        Debug.Log("Heal started");
        activeHoT = true;
        float check = 0;
        while(Health < 100) { 
            while(check < timer)
            {
                check += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            check = 0;
            DamageEventInfo dei = new DamageEventInfo { damage = -10 };
            Health += 10;
            imageColor.a = (100 - Health) / 200;
            healthFrame.color = imageColor;
            Debug.Log("Health Gained, current health:" + Health);
        }
        timer = healOverTime;
        activeHoT = false;
    }

    /// <summary>
    /// Fades in a black screen and tells the player it died. Then fades out and restores health to the player and changes position to safe zone
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeToDeathAndBack()
    {
        float timer = 0;
        float check = 0;
        Color blackFade = new Color(0, 0, 0, 0);
        Color textFade = new Color(255, 0, 0, 0);
        CinematicEventInfo ceiStop = new CinematicEventInfo {  };
        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.CinematicFreeze, ceiStop);
        while (timer < 3f)
        {
            timer += Time.deltaTime;
            check = timer / 3f;
            blackFade.a= Mathf.Lerp(0, 1, check);
            deathPanel.color = blackFade;
            textFade.a = Mathf.Lerp(0, 1, check);
            deathText.color = textFade;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        blackFade.a = 255;
        deathPanel.color = blackFade;
        textFade.a = 255;
        deathText.color = textFade;
        yield return new WaitForSeconds(1);
        timer = 0;
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            check = timer / 2f;
            blackFade.a = Mathf.Lerp(1, 0, check);
            deathPanel.color = blackFade;
            textFade.a = Mathf.Lerp(1, 0, check);
            deathText.color = textFade;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        CinematicEventInfo ceiStart = new CinematicEventInfo { };
        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.CinematicResume, ceiStart);
    }
    
}
