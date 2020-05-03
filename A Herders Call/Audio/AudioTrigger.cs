using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

public class AudioTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip pizziClip;
    [SerializeField] private AudioClip arcoClip;
    [SerializeField] private int audioIndex;
    
    void Start()
    {
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.CompleteQuest, ChangeMusic);
        source.clip = pizziClip;
    }

    /// <summary>
    /// When an object with the tag player enters a collision with this objects trigger
    /// then a sound will start playing from the object and the players music audio will tone down
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            AudioSoundEventInfo asei = new AudioSoundEventInfo { Ambient = true, soundIndex = -1 };
            EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.AudioSound, asei);
            source.mute = false;

            if (!source.isPlaying)
            {
                source.loop = true;
                source.Play();
            }

        }
    }

    /// <summary>
    /// When an object with tag player exits the objects trigger
    /// The sound will tone down and the players music will come back
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            // triggar du ett event
            AudioSoundEventInfo asei = new AudioSoundEventInfo { Ambient = true, soundIndex = -1 };
            EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.AudioSound, asei);

           // AudioSoundEventInfo asei2 = new AudioSoundEventInfo { Ambient = true, soundIndex = audioIndex};
            //EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.AudioSound, asei2);

            source.mute = true;

        }
    }

    /// <summary>
    /// This Changes the music to another index if a quest with reward 5 is completed
    /// </summary>
    /// <param name="eventInfo"></param>
    private void ChangeMusic(EventInfo eventInfo)
    {
        CompleteQuestEventInfo cqei = (CompleteQuestEventInfo)eventInfo;
        if (cqei.eventQuestID == 5)
        {
            StartCoroutine(ChangingTheMusic());
        }
    }

    /// <summary>
    /// This makes the change in music more of a fade over than just instant switch
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangingTheMusic()
    {
        float timer = 0;
        while(timer < 4)
        {
            source.volume = 1 - (timer / 4);
            timer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        source.Stop();
        source.clip = arcoClip;
        source.Play();
        timer = 0;
        while (timer < 4)
        {
            source.volume = (timer / 4);
            timer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }


}
