using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

//Main Author: Hjalmar Andersson
//Secondary Author: Alishia Nossborn

public class GameController : MonoBehaviour
{
    public static GameController gameControllerInstance;
    public Slider shoutCD;

    public List<Sprite> runes = new List<Sprite>();
    private Rune[] activeRunes = new Rune[3];

    public GameObject pausePanel;
    public GameObject cinematicPane;

    [SerializeField] private GameObject questPanel;
    [SerializeField] private GameObject book;

    public Text torchAmount;

    public Image thunderRune;
    public Image calmRune;
    public Image locateRune;
    public Image runeSprite;
    public Image runeBackground;
    public Rune currentRune;

    private float cdTimer1;
    private float cdTimer2;
    private float cdTimer3;

    private bool activeTorch = false;
    private int torches;
    float temp = 0;

    public AudioMixerSnapshot awake;
    public AudioMixerSnapshot paused;
    
    void Start()
    {
        pausePanel.SetActive(false);
        shoutCD.gameObject.SetActive(false);
        runeSprite.enabled = false;
        runeBackground.enabled = false;
        gameControllerInstance = this;
        DisEnableAllRunes();
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.ChangeRune, ChangeRune);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.TorchPickup, IncreaseTorch);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.TorchActivation, DecreaseTorch);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.Song, ShoutHasBeenMade);
    }

    void Update()
    {

        if (Input.GetButtonDown("Pause"))
        {
            if (questPanel.active) { 
                questPanel.SetActive(false);
            }
            else if (book.active)
            {
                book.SetActive(false);
                CinematicEventInfo cei = new CinematicEventInfo { };
                EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.CinematicResume, cei);
            }
            else
            { 
                if (pausePanel.active)
                    UnPauseGame();
                else
                {
                    pausePanel.SetActive(true);
                    PauseTheGame();
                }
            }
        }

        if(currentRune != null) {
            if (currentRune.ReadyToUse())
            {
                switch (currentRune.GetRuneValue())
                {
                    case 1:
                        thunderRune.fillAmount = 1;
                        break;
                    case 2:
                        calmRune.fillAmount = 1;
                        break;
                    case 3:
                        locateRune.fillAmount = 1;
                        break;
                }
            }
            else
            {
                switch (currentRune.GetRuneValue())
                {
                    case 1:
                        thunderRune.fillAmount = cdTimer1;
                        break;
                    case 2:
                        calmRune.fillAmount = cdTimer2;
                        break;
                    case 3:
                        locateRune.fillAmount = cdTimer3;
                        break;
                }
            }

        }
    }
    
    private void FixedUpdate()
    {

        if (currentRune != null)
        {
            switch (currentRune.GetRuneValue())
            {
                case 1:
                    thunderRune.fillAmount = cdTimer1;
                    break;
                case 2:
                    calmRune.fillAmount = cdTimer2;
                    break;
                case 3:
                    locateRune.fillAmount = cdTimer3;
                    break;
            }
        }
        CalculateCoolDownForRunes();
        
    }

    /// <summary>
    /// Calculate the cooldowns for each individual rune that the player uses
    /// </summary>
    private void CalculateCoolDownForRunes()
    {
        if (activeRunes[0] != null && !activeRunes[0].ReadyToUse())
        {
            cdTimer1 += 1 / activeRunes[0].GetCooldown() * Time.deltaTime;
            if (cdTimer1 >= 1)
            {
                activeRunes[0].CooldownFinish();
                cdTimer1 = 0f;
            }
        }
        else
            cdTimer1 = 0f;

        if (activeRunes[1] != null && !activeRunes[1].ReadyToUse())
        {
            cdTimer2 += 1 / activeRunes[1].GetCooldown() * Time.deltaTime;
            if (cdTimer2 >= 1)
            {
                activeRunes[1].CooldownFinish();
                cdTimer2 = 0f;
            }
        }
        else
            cdTimer2 = 0f;

        if (activeRunes[2] != null && !activeRunes[2].ReadyToUse())
        {
            cdTimer3 += 3 / activeRunes[2].GetCooldown() * Time.deltaTime;
            if (cdTimer3 >= 1)
            {
                activeRunes[2].CooldownFinish();
                cdTimer3 = 0f;
            }
        }
        else
            cdTimer3 = 0f;
    }

    /// <summary>
    /// Changes the current rune to the next one in line
    /// </summary>
    /// <param name="eventInfo"></param>
    private void ChangeRune(EventInfo eventInfo)
    {
        runeSprite.enabled = true;
        runeBackground.enabled = true;
        ChangeRuneEventInfo changeRuneInfo = (ChangeRuneEventInfo)eventInfo;
        currentRune = changeRuneInfo.newRune;
        DisEnableAllRunes();
        switch (currentRune.GetRuneValue())
        {
            case 1:
                ChangeRuneSprite(changeRuneInfo.newRune);
                thunderRune.enabled = true;
                break;
            case 2:
                ChangeRuneSprite(changeRuneInfo.newRune);
                calmRune.enabled = true;
                break;
            case 3:
                ChangeRuneSprite(changeRuneInfo.newRune);
                locateRune.enabled = true;
                break;
            default:
                runeSprite.sprite = null;
                break;
        }
    }

    /// <summary>
    /// Changes the sprite for the rune so that the rune looks different
    /// </summary>
    /// <param name="newRune"></param>
    private void ChangeRuneSprite(Rune newRune)
    {
        runeSprite.sprite = runes[newRune.GetRuneValue()-1];
        if(activeRunes[newRune.GetRuneValue()-1] == null)
            activeRunes[newRune.GetRuneValue()-1] = newRune;
    }

    /// <summary>
    /// Disables all runes
    /// </summary>
    private void DisEnableAllRunes()
    {
        thunderRune.enabled = false;
        calmRune.enabled = false;
        locateRune.enabled = false;
    }

    /// <summary>
    /// Increases the number of current torches on the GUI
    /// </summary>
    /// <param name="eventInfor"></param>
    private void IncreaseTorch(EventInfo eventInfor)
    {
        torches++;
        torchAmount.text = torches.ToString();
    }
    
    /// <summary>
    /// Removes one of the torches the player had on the gui
    /// </summary>
    /// <param name="eventInfor"></param>
    private void DecreaseTorch(EventInfo eventInfor)
    {
        if (activeTorch == false && torches != 0) {
            activeTorch = true;
            torches--;
            torchAmount.text = torches.ToString();
        }
    }

    /// <summary>
    /// decides what to do when the player makes a shout
    /// </summary>
    /// <param name="eventInfo"></param>
    private void ShoutHasBeenMade(EventInfo eventInfo)
    {
        ShoutEventInfo shoutInfo = (ShoutEventInfo)eventInfo;
        shoutCD.gameObject.SetActive(true);
        shoutCD.maxValue = shoutInfo.shoutDuration;
        shoutCD.value = shoutInfo.shoutDuration;
        StartCoroutine(ShoutCDTimer());
    }

    /// <summary>
    /// Shows a slider bar on the GUI that counts down for aslong as the shouts sound last
    /// </summary>
    private IEnumerator ShoutCDTimer()
    {
        while (shoutCD.value >= 0.5)
        {
            shoutCD.value -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        shoutCD.gameObject.SetActive(false);
    }

    /// <summary>
    /// Pauses the game by setting time.timescale to 0
    /// </summary>
    private void PauseTheGame()
    {
        Time.timeScale = 0;
        CinematicEventInfo cei = new CinematicEventInfo { };
        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.CinematicFreeze, cei);
        paused.TransitionTo(0f);

    }

    /// <summary>
    /// Unpauses the game by setting the Time.timescale to 1
    /// </summary>
    public void UnPauseGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        CinematicEventInfo cei = new CinematicEventInfo { };
        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.CinematicResume, cei);
        awake.TransitionTo(1.0f);

    }

    /// <summary>
    /// Loads the main menu scene
    /// </summary>
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Saves the game by sending a SaveEventInfo
    /// </summary>
    public void SaveTheGame()
    {
        SaveEventInfo sei = new SaveEventInfo { };
        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.Save, sei);
    }
}


