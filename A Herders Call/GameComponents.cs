using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Main Author: Hjalmar Andersson    
//Secondary Author: Marcus Lundqvist

public class GameComponents : MonoBehaviour
{

    //Lists


    public static List<GameObject> CowList;
    public static List<GameObject> FollowingCowsList; // behövs inte
    public static List<GameObject> CapturedCows; // behövs inte
    public static List<GameObject> WolfList; // behövs inte
    public static List<GameObject> GiantList; //behövs inte?
    public static List<GameObject> GnomeList; // behövs inte
    public static List<GameObject> AnimalPens;
    public static List<GameObject> FairGameList;
    public static List<GameObject> NightObjects; // behövs inte
    public static List<string> CowNames; // make private, public string getRandomCowName()
    private bool day = true; // behövs denna? är inte detta ett event?


    //public Vector3 PlayerPosition;
    //Prefabs
    [SerializeField] private GameObject cowPrefab;
    [SerializeField] private GameObject wolfPrefab;
    [SerializeField] private GameObject giantPrefab;
    [SerializeField] private GameObject gnomePrefab;
    [SerializeField] private GameObject animalPenPrefab;
    [SerializeField] private GameObject rainComponent;
    [SerializeField] private Material daySky;

    [SerializeField] private GameObject clouds;
    [SerializeField] private ParticleSystem stars;

    private void Awake()
    {
        CowList = new List<GameObject>();
        FollowingCowsList = new List<GameObject>();
        WolfList = new List<GameObject>();
        GiantList = new List<GameObject>();
        GnomeList = new List<GameObject>();
        AnimalPens = new List<GameObject>();
        CapturedCows = new List<GameObject>();
        FairGameList = new List<GameObject>();
        NightObjects = new List<GameObject>();
        CowNames = new List<string>() {"Rosa", "Darla", "Daisy", "Bella", "Cow", "Majros", "Stina", "Maja", "Sara", "Lindra", "Majbritt", "Bettan", "Lillemor"};
    }

    private void Start()
    {
        StartCoroutine(SpawnAGnome());
        StartCoroutine(StartTheRain());
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.SwapToNight ,DayToNightAndViceVersa);
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.SwapToDay ,DayToNightAndViceVersa);
        Terrain.activeTerrain.collectDetailPatches = false;
    }


    /// <summary>
    /// Has a 30% chance of spawning a <see cref="gnomePrefab"/> every set amount of seconds for every cow that is following the player.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnAGnome() 
    {
        float rand = 0f;
        while (true)
        {
            rand = Random.Range(0f, 100f);
            if (FollowingCowsList.Count > 0 && day && rand > 60f) 
            {
                foreach (GameObject cow in FollowingCowsList)
                {
                    Vector3 newSpawnPoint = Vector3.zero;
                    int count = 0;
                    while (newSpawnPoint.magnitude < 10f && count < 15)
                    {
                        float NewX = Random.Range(-20, 20);
                        float NewZ = Random.Range(-20, 20);
                        newSpawnPoint = new Vector3(NewX, 0, NewZ);
                        count++;
                    }
                    newSpawnPoint.x += cow.transform.position.x;
                    newSpawnPoint.z += cow.transform.position.z;
                    RaycastHit groundCheck;
                    if (Physics.SphereCast(newSpawnPoint + new Vector3(0, 100, 0), 1f, Vector3.down, out groundCheck, 150, LayerMask.GetMask("Terrain")))
                    {
                        newSpawnPoint.y += groundCheck.point.y;
                        GameObject newGnome = Instantiate(gnomePrefab, newSpawnPoint, Quaternion.identity);
                        newGnome.GetComponent<GnomeStateMachine>().Target = cow;
                        GnomeList.Add(newGnome);
                    }
                }
            }
            yield return new WaitForSeconds(20.0f);
        }
    }

    /// <summary>
    /// Waits 3-10 minutes and then triggers a raining effect lasting 2-4 minutes.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartTheRain()
    {
        while (true)
        {
            float rand = Random.Range(3, 10);

            yield return new WaitForSeconds(rand * 60);
            rainComponent.GetComponent<RainFollowScript>().StartRain();
            rand = Random.Range(2, 4);
            Debug.Log("Raining for " + rand);
            yield return new WaitForSeconds(rand * 60);
            rainComponent.GetComponent<RainFollowScript>().StopRain();
        }
    }

    /// <summary>
    /// reacts to a swap to night or a swap to day event. Sets the value if it day or night.
    /// </summary>
    /// <param name="eventInfo"></param>
    public void DayToNightAndViceVersa(EventInfo eventInfo)
    {
        day = !day;
        if (day)
        {
            clouds.SetActive(true);
            Debug.Log("Clouds are back");
            stars.emissionRate = 0f;
        }
        else
        {
            clouds.SetActive(false);
            stars.emissionRate = 1f;
            Debug.Log("Clouds are gone");
        }
    }
}
