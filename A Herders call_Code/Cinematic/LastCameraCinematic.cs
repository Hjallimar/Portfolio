using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

public class LastCameraCinematic : MonoBehaviour
{
    [SerializeField] private Camera cinematicView;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject gui;
    private Animator anim;
    private bool played = false;

    void Start()
    {
        cinematicView.enabled = false;
        cinematicView.GetComponent<AudioListener>().enabled = false;
        anim = cinematicView.GetComponent<Animator>();
    }

    /// <summary>
    /// Triggers  a cinematic and activates a new camera
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Starting the final Cinematic");
        if (played)
            return;
        gui.SetActive(false);
        mainCamera.GetComponent<AudioListener>().enabled = false;
        cinematicView.enabled = true;
        played = true;
        StartInGameCinematic();
    }

    /// <summary>
    /// Starts the cinematics by activating an animation on the camera
    /// </summary>
    private void StartInGameCinematic()
    {
        CinematicEventInfo cei = new CinematicEventInfo { };
        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.CinematicFreeze, cei);
        anim.SetTrigger("EndAnim");
        StartCoroutine(PlayingCinematic());
    }

    /// <summary>
    /// Turns of the camera off and stops the animnation after 32 seconds
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayingCinematic()
    {
        yield return new WaitForSeconds(32.0f);
        CinematicEventInfo cei = new CinematicEventInfo { };
        EventHandeler.Current.FireEvent(EventHandeler.EVENT_TYPE.CinematicResume, cei);
        cinematicView.enabled = false;
        mainCamera.GetComponent<AudioListener>().enabled = true;
        gui.SetActive(true);
    }

}
