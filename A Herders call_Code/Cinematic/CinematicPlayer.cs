using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

//Author: Hjalmar Andersson

public class CinematicPlayer : MonoBehaviour
{
    [SerializeField] private bool pause = false;
    [SerializeField] private VideoPlayer vhs;
    [SerializeField] private GameObject cinematicPlane;
    [SerializeField] private AudioSource menuAudio;

    void Start()
    {
        cinematicPlane.SetActive(false);
    }
    
    /// <summary>
    /// Enables the player to press escape to close the cinematic
    /// </summary>
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && vhs.isPlaying)
        {
            menuAudio.volume = 1;
            cinematicPlane.SetActive(false);
            vhs.Stop();
        }
    }

    /// <summary>
    /// Starts a video that the player can see
    /// </summary>
    public void PlayVideo()
    {
        menuAudio.volume = 0;
        cinematicPlane.SetActive(true);
        vhs.Play();
    }
}
