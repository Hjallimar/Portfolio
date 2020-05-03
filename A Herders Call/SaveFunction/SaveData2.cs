using System;
using System.Collections.Generic;

[System.Serializable]
public struct SaveData2
{
    //Player stats
    public float CurrentHealth;
    public int CurrentNrOfTorches;
    public float[] Position;
    public float[] Rotation;
    public int[] CurrentRunes;

    //Quest variables
    public List<int> QuestsCompleted;
    public int CowsCollected;
    public Dictionary<int, int> ActiveQuests;

    //Day and night variables
    public int CelestialStateIndex;
    public float[] SunPosition;
    public float[] MoonPosition;
    public float SunIntensity;
    public float MoonIntensity;
    public float CheckTimer;
    public float Timer;
    public bool TutorialIsFinished;

    //Camera Variables
    public float[] CameraPosition;

    //AI lists
    public List<CowsData> CowInfoList;
    public List<WolvesData> WolfInfoList;
    public List<GiantsData> GiantInfoList;
}

[System.Serializable]
public struct CowsData
{
    public string Id;
    public float[] Position;
    public float[] Rotation;
    public float[] SpawnLocation;
    public int StateIndex;
}

[System.Serializable]
public struct WolvesData
{
    public string Id;
    public float[] Position;
    public float[] Rotation;
    public float[] SpawnLocation;
    public int StateIndex;
}

[System.Serializable]
public struct GiantsData
{
    public string Id;
    public float[] Position;
    public float[] Rotation;
    public float[] SpawnLocation;
    public int StateIndex;
}