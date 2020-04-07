using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Audio;

//Author: Hjalmar Andersson

public class BookInterface : MonoBehaviour
{
    [SerializeField] private GameObject questLog;
    [SerializeField] private GameObject bookPanel;
    [SerializeField] private Text leftPage;
    [SerializeField] private Text rightPage;

    [SerializeField] private Image bookIcon;
    [SerializeField] private Button videoIcon;
    [SerializeField] private Text pages;
    [SerializeField] private ChapterForBook[] allChapters;
    [SerializeField] private List<ChapterForBook> unlockedChapters;

    [SerializeField] private GameObject cinematicPlane;
    [SerializeField] private VideoPlayer video;
    [SerializeField] private VideoClip[] cinematicVideos;
    [SerializeField] private List<VideoClip> unlockedCinematics;
    private int cinematicIndex = 0;

    private int bookIndex = 0;
    private int unlockedIndex = 0;
    private bool xboxInputUpNotOnCooldown = true;
    private bool xboxInputLeftNotOnCooldown = true;
    private bool xboxInputRightNotOnCooldown = true;

    public AudioMixerSnapshot awake;
    public AudioMixerSnapshot cutscenePlay;
    
    void Start()
    {
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.QuestReward, BookReward);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.CinematicResume, CancelCinematicView);
        pages.text = "PG: " + unlockedIndex.ToString();
        videoIcon.enabled = false;
    }
    
    void Update()
    {
        if (Input.GetButtonDown("Journal key") || (Input.GetAxisRaw("Journal axis") > 0 && xboxInputUpNotOnCooldown == true))
        {
            xboxInputUpNotOnCooldown = false;
            StartCoroutine(XboxCooldown());
            questLog.SetActive(false);
            if (bookPanel.active) { 
                bookPanel.SetActive(false);
                CinematicEventInfo cei = new CinematicEventInfo { };
                EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.CinematicResume, cei);
            }
            else { 
                bookPanel.SetActive(true);
                CinematicEventInfo cei = new CinematicEventInfo { };
                EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.CinematicFreeze, cei);
            }

            if (unlockedIndex == 0)
            {
                bookIndex = 0;
                leftPage.text = "You have nothing stored in the book yet";
                rightPage.text = "";
                return;
            }
            leftPage.text = unlockedChapters[0].Title + "\n" + "\n" + unlockedChapters[0].Story;
            rightPage.text = unlockedChapters[1].Title + "\n" + "\n" + unlockedChapters[1].Story;
            videoIcon.enabled = true;
            bookIndex = 0;
        }
        else if ((Input.GetKeyDown(KeyCode.RightArrow) || (Input.GetAxisRaw("Flip pages axis") > 0) && xboxInputRightNotOnCooldown == true))
        {
            xboxInputRightNotOnCooldown = false;
            StartCoroutine(XboxCooldown());
            if (unlockedIndex == 0)
                return;

            CheckHigherIndex();
            leftPage.text = unlockedChapters[bookIndex].Title + "\n" + "\n" + unlockedChapters[bookIndex].Story;
            rightPage.text = unlockedChapters[bookIndex + 1].Title + "\n" + "\n" + unlockedChapters[bookIndex + 1].Story;
            cinematicIndex = bookIndex / 8;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || (Input.GetAxisRaw("Flip pages axis") < 0 && xboxInputLeftNotOnCooldown == true))
        {
            xboxInputLeftNotOnCooldown = false;
            StartCoroutine(XboxCooldown());
            if (unlockedIndex == 0)
                return;

            CheckLowerIndex();
            rightPage.text = unlockedChapters[bookIndex + 1].Title + "\n" + "\n" + unlockedChapters[bookIndex + 1].Story;
            leftPage.text = unlockedChapters[bookIndex].Title + "\n" + "\n" + unlockedChapters[bookIndex].Story;
            cinematicIndex = bookIndex / 8;
        }

        if(bookPanel.active && cinematicPlane.active == false && Input.GetButtonDown("Interact"))
        {
            PlayCinematic();
        }
    }

    /// <summary>
    /// Rewards the player with 8 pages in the Journal
    /// </summary>
    /// <param name="eventInfo"></param>
    private void BookReward(EventInfo eventInfo)
    {
        RewardQuestInfo rei = (RewardQuestInfo)eventInfo;
        if (rei.rewardNumber == 4)
        {
            for (int i = 0; i < 8; i++)
            {
                unlockedChapters.Add(allChapters[i]);
            }
            unlockedCinematics.Add(cinematicVideos[0]);
        }
        else if (rei.rewardNumber == 5)
        {
            for (int i = 0; i < 8; i++)
            {
                unlockedChapters.Add(allChapters[i + 8]);
            }
            unlockedCinematics.Add(cinematicVideos[1]);
        }
        else if (rei.rewardNumber == 6)
        {
            for (int i = 0; i < 8; i++)
            {
                unlockedChapters.Add(allChapters[i + 16]);
            }
            unlockedCinematics.Add(cinematicVideos[2]);
        }
        else
            return;

        unlockedIndex += 8;
        pages.text = "PG: " + unlockedIndex.ToString();

    }

    /// <summary>
    /// Check if the player can change pages on the book or if its out of bounds
    /// </summary>
    private void CheckHigherIndex()
    {
        if (bookIndex == unlockedIndex - 2)
            return;
        bookIndex += 2;
    }

    /// <summary>
    /// Check if the player can change pages on the book or if its out of bounds
    /// </summary>
    private void CheckLowerIndex()
    {
        if (bookIndex == 0)
            return;
        bookIndex -= 2;
    }

    /// <summary>
    /// Used as a cooldown for the player if she uses an Xbox controller
    /// This becuse Xbox controller has an axis instead of a keydon
    /// </summary>
    private IEnumerator XboxCooldown()
    {
        while (Input.GetAxisRaw("Journal axis") != 0 || Input.GetAxisRaw("Flip pages axis") != 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);

        }
        xboxInputLeftNotOnCooldown = true;
        xboxInputUpNotOnCooldown = true;
        xboxInputRightNotOnCooldown = true;
    }

    /// <summary>
    /// Starts a cinematic that the player can see
    /// </summary>
    public void PlayCinematic()
    {
        Debug.Log("Starting Cinematic");
        cinematicPlane.SetActive(true);
        if (unlockedCinematics.Count == 0)
            return;
        video.clip = unlockedCinematics[cinematicIndex];
        video.Play();
        cutscenePlay.TransitionTo(0f);
    }

    /// <summary>
    /// Cancels the cinematic that the player is currently watching
    /// </summary>
    /// <param name="eventInfo"></param>
    private void CancelCinematicView(EventInfo eventInfo)
    {
        video.Stop();
        cinematicPlane.SetActive(false);
        awake.TransitionTo(1.0f);
    }
}
