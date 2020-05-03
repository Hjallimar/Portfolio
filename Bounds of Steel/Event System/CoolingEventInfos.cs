using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignCoolingSystemEventInfo : EventInfo
{
    public float Capacity { get; private set; }
    public AssignCoolingSystemEventInfo(GameObject gO, string description, float capacity) : base(gO, description)
    {
        Capacity = capacity;
    }
}

public class StartCoolingSystemEventInfo : EventInfo
{
    public float Capacity { get; private set; }
    public StartCoolingSystemEventInfo(GameObject gO, string description, float capacity) : base(gO, description)
    {
        Capacity = capacity;
    }
}

public class StopCoolingSystemEventInfo : EventInfo
{
    public StopCoolingSystemEventInfo(GameObject gO, string description) : base(gO, description)
    {
    }
}

public class CoolingSystemMalfunctionEventInfo : EventInfo
{
    public CoolingSystemMalfunctionEventInfo(GameObject gO, string description) : base(gO, description)
    {
    }
}

public class OverHeatedEventInfo : EventInfo
{
    public bool OverHeated { get; private set; }
    public OverHeatedEventInfo(GameObject gO, string description, bool overHeat) : base(gO, description)
    {
        OverHeated = overHeat;
    }
}

public class IncreaseHeatEventInfo : EventInfo
{
    public float HeatAmmount  { get; private set; }
    public IncreaseHeatEventInfo(GameObject gO, float heatAmmount, string description) : base(gO, description)
    {
        HeatAmmount = heatAmmount;
    }
}
