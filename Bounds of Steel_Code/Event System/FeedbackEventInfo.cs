using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEventInfo : EventInfo
{
    public AudioClip Clip { get; private set; }
    public Vector3 Position { get; private set; }

    public SoundEventInfo(GameObject gO, string description, AudioClip clip, Vector3 pos): base(gO, description)
    {
        Clip = clip;
        Position = pos;
    }
}

public class ParticleEventInfo : EventInfo
{
    public GameObject Ps { get; private set; }
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    public ParticleEventInfo(GameObject gO, string description, GameObject ps, Vector3 position, Quaternion rotation) : base(gO, description)
    {
        Ps = ps;
        Position = position;
        Rotation = rotation;
    }
}

public class TextLogEventInfo : EventInfo
{
    public string TextLog { get; private set; }
    public TextLogEventInfo(GameObject gO, string textLog ,string description = "") : base(gO, description)
    {
        TextLog = textLog;
    }
}

public class VoiceLineEventInfo : EventInfo
{
    public AudioClip Clip { get; private set; }
    public VoiceLineEventInfo(GameObject gO, AudioClip clip ,string description = "Playing voiceline"): base (gO, description)
    {
        Clip = clip;
    }
}
