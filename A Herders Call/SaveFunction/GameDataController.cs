
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Saves information down to a file formatted to binary.
/// </summary>
public class GameDataController : MonoBehaviour
{
    public static SaveData2 SaveInformation;

    private void Awake()
    {
        LoadData();
    }

    [ContextMenu("Save Data")]
    public void SaveGame()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.info";
        FileStream stream = File.Create(path);

        formatter.Serialize(stream, SaveInformation);
        stream.Close();
    }

    [ContextMenu("Load Data")]
    public void LoadData()
    {
        string path = Application.persistentDataPath + "/save.info";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveInformation = (SaveData2)formatter.Deserialize(stream);
            Debug.Log(SaveInformation.Position);
            stream.Close();

        }
        else
        {
            Debug.LogError("File was missing");
        }

    }

    private void OnDisable()
    {
        SaveGame();
    }

    public static CowsData GetCowState(CowSM cow)
    {
        CowsData cowData = new CowsData() { Id = null };

        if (SaveInformation.CowInfoList == null)
            return cowData;

        if (SaveInformation.CowInfoList.Any(t => t.Id == cow.name))
        {
            return SaveInformation.CowInfoList.FirstOrDefault(t => t.Id == cow.name);
        }



        return cowData;
    }

    public static void SetCowState(CowSM cowInfo)
    {
        if (SaveInformation.CowInfoList == null)
            SaveInformation.CowInfoList = new List<CowsData>();

        CowsData cowData = new CowsData() { Id = cowInfo.name, Position = new float[3], Rotation = new float[3], SpawnLocation = new float[3] ,StateIndex = cowInfo.CurrentState.Index };
        RemakeVectorToArray(cowInfo.transform.position, ref cowData.Position);
        RemakeVectorToArray(cowInfo.transform.rotation.eulerAngles, ref cowData.Rotation);
        RemakeVectorToArray(cowInfo.SpawnPosition, ref cowData.SpawnLocation);

        SaveInformation.CowInfoList.RemoveAll(t => t.Id == cowData.Id);
        SaveInformation.CowInfoList.Add(cowData);
    }

    public static void SetPlayerState(PlayerStateMashine playerInfo)
    {
        SaveInformation.CurrentHealth = playerInfo.Health;
        SaveInformation.CurrentNrOfTorches = playerInfo.NrOfTorches;
        SaveInformation.Position = new float[3];
        SaveInformation.Rotation = new float[3];
        SaveInformation.CurrentRunes = new int[3];
        SaveInformation.CameraPosition = new float[3];
        
        for(int i = 0; i <= 2; i++)
        {
            if(playerInfo.Runes[i] != null)
                SaveInformation.CurrentRunes[i] = playerInfo.Runes[i].Value;
        }

        RemakeVectorToArray(playerInfo.FaceDirection, ref SaveInformation.Rotation);
        RemakeVectorToArray(Camera.main.transform.position, ref SaveInformation.CameraPosition);
        RemakeVectorToArray(playerInfo.transform.position, ref SaveInformation.Position);


    }

    public static SaveData2 GetPlayerState(PlayerStateMashine playerInfo)
    {

        return SaveInformation;

    }

    public static WolvesData GetWolfState(AlphaStateMachine wolf)
    {
        WolvesData wolfData = new WolvesData() { Id = null };

        if (SaveInformation.WolfInfoList == null)
            return wolfData;

        if (SaveInformation.WolfInfoList.Any(t => t.Id == wolf.name))
        {
            return SaveInformation.WolfInfoList.FirstOrDefault(t => t.Id == wolf.name);
        }



        return wolfData;
    }

    public static void SetWolfState(AlphaStateMachine wolfInfo)
    {
        if (SaveInformation.WolfInfoList == null)
            SaveInformation.WolfInfoList = new List<WolvesData>();

        WolvesData wolfData = new WolvesData() { Id = wolfInfo.name, Position = new float[3], Rotation = new float[3], SpawnLocation = new float[3], StateIndex = wolfInfo.CurrentState.Index };
        RemakeVectorToArray(wolfInfo.transform.position, ref wolfData.Position);
        RemakeVectorToArray(wolfInfo.transform.rotation.eulerAngles, ref wolfData.Rotation);
        RemakeVectorToArray(wolfInfo.DenLocation, ref wolfData.SpawnLocation);

        SaveInformation.WolfInfoList.RemoveAll(t => t.Id == wolfData.Id);
        SaveInformation.WolfInfoList.Add(wolfData);
    }

    public static GiantsData GetGiantState(GiantSM giant)
    {
        GiantsData giantData = new GiantsData() { Id = null };

        if (SaveInformation.GiantInfoList == null)
            return giantData;

        if (SaveInformation.GiantInfoList.Any(t => t.Id == giant.name))
        {
            return SaveInformation.GiantInfoList.FirstOrDefault(t => t.Id == giant.name);
        }

        return giantData;
    }

    public static void SetGiantState(GiantSM giantInfo)
    {
        if (SaveInformation.GiantInfoList == null)
            SaveInformation.GiantInfoList = new List<GiantsData>();

        GiantsData giantData = new GiantsData() { Id = giantInfo.name, Position = new float[3], Rotation = new float[3], SpawnLocation = new float[3], StateIndex = giantInfo.CurrentState.Index };
        RemakeVectorToArray(giantInfo.transform.position, ref giantData.Position);
        RemakeVectorToArray(giantInfo.transform.rotation.eulerAngles, ref giantData.Rotation);
        RemakeVectorToArray(giantInfo.StartPosition, ref giantData.SpawnLocation);

        SaveInformation.GiantInfoList.RemoveAll(t => t.Id == giantData.Id);
        SaveInformation.GiantInfoList.Add(giantData);
    }

    public static void RemakeVectorToArray(Vector3 vector, ref float[] array  )
    {
        array[0] = vector.x;
        array[1] = vector.y;
        array[2] = vector.z;

    }

    public static void SetCelestialState(DayAndNightSM celestialInfo)
    {

        SaveInformation.SunPosition = new float[3];

        SaveInformation.MoonPosition = new float[3];
        SaveInformation.CheckTimer = celestialInfo.CheckTimer;
        SaveInformation.Timer = celestialInfo.Timer;
        SaveInformation.TutorialIsFinished = celestialInfo.FinishedTutorial;

        SaveInformation.CelestialStateIndex = celestialInfo.CurrentState.Index;

        SaveInformation.SunIntensity = celestialInfo.Sunlight.intensity;
        SaveInformation.MoonIntensity = celestialInfo.Moonlight.intensity;

        RemakeVectorToArray(celestialInfo.Sun.transform.localPosition, ref SaveInformation.SunPosition);

        RemakeVectorToArray(celestialInfo.Moon.transform.localPosition, ref SaveInformation.MoonPosition);


    }

    public static SaveData2 GetCelestialState(DayAndNightSM celectialInfo)
    {

        return SaveInformation;

    }

    public static void SetQuestState(QuestComponent questInfo)
    {
        SaveInformation.QuestsCompleted = questInfo.CompletedQuests;
        SaveInformation.CowsCollected = questInfo.CollectedCows;

        SaveInformation.ActiveQuests = new Dictionary<int, int>();

        foreach(KeyValuePair<int, Quest> quest in questInfo.QuestLog)
        {
            SaveInformation.ActiveQuests.Add(quest.Key, quest.Value.SaveProgress());
        }
    }

    public static SaveData2 GetQuestState(QuestComponent questInfo)
    {
        return SaveInformation;
    }


}