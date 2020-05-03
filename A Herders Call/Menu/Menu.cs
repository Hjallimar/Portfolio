using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;

//Main Author: Hjalmar Andersson

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject menuButton;
    [SerializeField] private GameObject h2PPanel;
    [SerializeField] private GameObject quitPanel;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audio;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject previousButton;
    [SerializeField] private Text h2PMainText;
    [SerializeField] private Text h2PTitleText;
    [SerializeField] private Text progressText;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip[] clickSounds;
    [SerializeField] private GameObject playOrLoadPanel;
    [SerializeField] private Material newSkybox;
    private Material oldSkyBox;

    //Cinematic Stuffs
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject cinematicPlane;

    private Dictionary<string, string> slide = new Dictionary<string, string>();
    private List<string> titles = new List<string>();
    private List<string> textSlides = new List<string>();
    private int currentSlideIndex = 0;
    private int clipIndex;
    private bool loadSaveData;
    

    private void Start()
    {
        loadSaveData = false;
        audioSource.clip = audio;
        audioSource.loop = true;
        audioSource.Play();
        h2PPanel.SetActive(false);
        quitPanel.SetActive(false);
        AddSlides();
        oldSkyBox = RenderSettings.skybox;
        RenderSettings.skybox = newSkybox;
        source = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Enables the panel that shows the player how to play the game.
    /// Also sets the start text and lay out.
    /// </summary>
    public void HowToPlay()
    {
        playSound();
        menuButton.SetActive(false);
        h2PPanel.SetActive(true);
        previousButton.SetActive(false);
        h2PTitleText.text = titles[currentSlideIndex];
        h2PMainText.text = textSlides[currentSlideIndex];
    }
    /// <summary>
    /// Activates the next slide in the the how to play menu
    /// Showing new text and enables the previous button
    /// If the next slide is the last it will also disable the next button
    /// </summary>
    public void NextSlide()
    {
        playSound();
        currentSlideIndex++;
        h2PTitleText.text = titles[currentSlideIndex];
        h2PMainText.text = textSlides[currentSlideIndex];
        if (currentSlideIndex == 1)
        {
            previousButton.gameObject.SetActive(true);
            previousButton.SetActive(true);
        }
        else if (currentSlideIndex == titles.Count - 1)
        {
            nextButton.SetActive(false);
            nextButton.gameObject.SetActive(false);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(previousButton, null);
        }
    }
    /// <summary>
    /// Returns to the previous slide that was sowing
    /// if the previous slide was the first slide it then deactivates the previous button.
    /// </summary>
    public void PreviousSlide()
    {
        playSound();
        currentSlideIndex--;
        h2PTitleText.text = titles[currentSlideIndex];
        h2PMainText.text = textSlides[currentSlideIndex];
        if (currentSlideIndex == 0)
        {
            previousButton.SetActive(false);
            previousButton.gameObject.SetActive(false);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(nextButton, null);
        }
        else if (currentSlideIndex == titles.Count - 2)
        {
            nextButton.gameObject.SetActive(true);
            nextButton.SetActive(true);
        }

    }
    /// <summary>
    /// Calcels the panel that shows the player how to play
    /// returning the player to the main menu
    /// </summary>
    public void CancelThePanel()
    {
        menuButton.SetActive(true);
        currentSlideIndex = 0;
        h2PPanel.SetActive(false);
    }

    /// <summary>
    /// Shows the player a window askin if quiting is what the player wants.
    /// </summary>
    public void QuitTheGame()
    {
        menuButton.SetActive(false);
        quitPanel.SetActive(true);
    }
    /// <summary>
    /// Quits the application
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
    /// <summary>
    /// Returns the player to the main menu after they had chosen no to quit the game.
    /// </summary>
    public void Abort()
    {
        menuButton.SetActive(true);
        quitPanel.SetActive(false);
    }
   /// <summary>
   /// Starts the game and enters a loading screen while prepareing the game
   /// </summary>
    public void PlayGame()
    {
        //wipe save och starta nytt
        SceneLoadController.LoadSaveData = false;
        //loadingScreen.SetActive(true);
        playOrLoadPanel.SetActive(false);
        StartCoroutine(LoadingGameScene());
    }
    /// <summary>
    /// This will load a saved file and continue the game from there. Starts a loading screen while prepering the game.
    /// </summary>
    public void PlayLoadGame()
    {
        SceneLoadController.LoadSaveData = true;
        //loadingScreen.SetActive(true);
        playOrLoadPanel.SetActive(false);
        StartCoroutine(LoadingGameScene());
    }

    /// <summary>
    /// This panel either starts the game or opens up another window. 
    /// The other window opens if there is a saved game file on this game.
    /// </summary>
    public void ChooseHowToPlay()
    {
        menuButton.SetActive(false);
        nextButton.gameObject.SetActive(true);
        nextButton.SetActive(true);
        //ifall ingen scene fans sparad ->PlayGame()
        playOrLoadPanel.SetActive(true);
    }
    /// <summary>
    /// This will play a sound whenever you click on a button with the mouse.
    /// </summary>
    public void playSound()
    {
        clipIndex = Random.Range(1, 5);
        AudioClip clip = clickSounds[clipIndex];
        source.PlayOneShot(clip);
        clickSounds[clipIndex] = clickSounds[0];
        clickSounds[0] = clip;
    }
    /// <summary>
    /// Is beeing run at the start of the scene. Making sure that the how to play idex has all the text it needs.
    /// </summary>
    private void AddSlides()
    {
        //Första Sidan
        titles.Add("Grunder");
        textSlides.Add("Världen är stor och förtjänar utforskande \nAnvänd <b>W, A, S</b> och <b>D</b> för att förflytta dig \nAnvänd <b>musen</b> för att rotera kameran"+
            "\nAnvänd <b>SHIFT</b> för att springa och <b>SPACE</b> för att hoppa" +
            "\n\nVissa föremål och varelser går att interagera med\nAnvänd <b>E</b> för Interaktion" +
            "\n\nDu har möjligheten att plocka upp facklor som kan användas för att lysa upp omgivningen och användas mot vissa fiender\nAnvänd <b>F</b> för att plocka fram en fackla");
        //Andra Sidan
        titles.Add("Kulning");
        textSlides.Add("Som koherde vill du få tillbaka kossorna till sin inhängnad. Detta görs genom kulning, en gammal, nordisk sångteknik för att locka till sig djur\n\nTill en början har du tillgång till <b>tre</b> olika kulningssånger\n Använd <b>1</b> för att kalla på kor\nAnvänd <b>2</b> för att få kon att stanna\nAnvänd <b>3</b> för att lämna in kon till en hage");
        //Tredje Sidan
        titles.Add("Runsånger");
        textSlides.Add("Runtom i världen finns olika <b>Runor</b> som låser upp unika kulningssånger \n\nAnvänd <b>Q</b> för att använda vald runsång \nAnvänd <b>TAB</b> för att byta mellan de olika runsångerna \n" +
            "<b>Gul Runa</b> sammankallar kraften av Tor som kan vara användbart mot en viss fiende... \n <b>Blå Runa</b> lugnar upprörda kor \n<b>Lila Runa</b> får nära kor att råma så de blir lättare att lokalisera");
        //Fjärde Sidan
        titles.Add("Dag och natt");
        textSlides.Add("Världen och dess varelser förändras beroende på om det är dag eller natt \nVar uppmärksam!");
    }

    /// <summary>
    /// Uppdates the progress of the loading screen to the player in form of a % text and a slider loading bar.
    /// </summary>
    /// <param name="value">%Value that has loaded</param>
    private void UpdateProgress(float value)
    {
        //progressSlider.value = value;
        //progressText.text = "Progress: " + (value * 100)+ "%";
    }
    private void LoadingComplete()
    {
        videoPlayer.Stop();
        cinematicPlane.SetActive(false);
        RenderSettings.skybox = oldSkyBox;
        //progressSlider.gameObject.SetActive(false);
    }
    /// <summary>
    /// A corutine that loads a new scene in the background that the player will enter when loading is complete and the player has pressed anykey.
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadingGameScene()
    {
        yield return null;
        
        AsyncOperation ao = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        cinematicPlane.SetActive(true);
        videoPlayer.Play();
        audioSource.Stop();
        ao.allowSceneActivation = false;
        float timer = 0;
        while (!ao.isDone)
        {
            //[0,0.9] > [0, 1]
            float progress = Mathf.Clamp(ao.progress, 0.1f, 0.9f);
            timer = Time.deltaTime;
            progressText.text = "Loading";
            if (ao.progress == 0.9f)
            {
                progressText.text = "Press Space to continue";
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space ) && timer >= 5f) { 
                    LoadingComplete();
                    ao.allowSceneActivation = true;
                }else if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Pause"))
                {
                    LoadingComplete();
                    ao.allowSceneActivation = true;
                }
                else if (timer > 10f && videoPlayer.isPlaying == false)
                {
                    LoadingComplete();
                    ao.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }

}
