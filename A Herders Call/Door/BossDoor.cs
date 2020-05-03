using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson
public class BossDoor : MonoBehaviour
{
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;
    [SerializeField] private List<Material> leftDoorMat;
    [SerializeField] private List<Material> rightDoorMat;
    [SerializeField] private Color startColor;
    [SerializeField] private Transform player;
    [SerializeField] private AudioSource audio;
    private bool lerping = false;
    
    int test = 0;

    private Renderer leftRend;
    private Renderer rightRend;
    private int materialIndex = 1;
    private List<int> repeeted = new List<int>();

    void Start()
    {
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.Song, ActivateDoorKey);
        foreach(Material mat in leftDoorMat)
        {
            mat.SetColor(("_EmissionColor"), startColor);
        }
        foreach (Material mat in rightDoorMat)
        {
            mat.SetColor(("_EmissionColor"), startColor);
        }
    }

    /// <summary>
    /// Activates a key/stone when ever a song event is called and the reuirements are good
    /// </summary>
    /// <param name="eventInfo"></param>
    private void ActivateDoorKey(EventInfo eventInfo)
    {
        ShoutEventInfo sei = (ShoutEventInfo)eventInfo;
        Vector3 newPos = (sei.playerPosition - transform.position);
        
        if (sei.songId == 0 || sei.songId == null)
        {
            return;
        }
        else if (newPos.sqrMagnitude > 30 * 30)
        {
            return;
        }
        if (repeeted.Contains(sei.songId))
            return;
        if (sei.songId > 4 || lerping == true)
            return;
        repeeted.Add(sei.songId);
        materialIndex++;
        switch (sei.songId)
        {
            case 1:
                StartCoroutine(LerpMaterial(3));
                   break;
            case 2:
                StartCoroutine(LerpMaterial(1));
                break;
            case 3:
                StartCoroutine(LerpMaterial(2));
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Lerps the material on the door do that the runes get another color
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private IEnumerator LerpMaterial(int index)
    {
        lerping = true;
        float timer = 0;
        if (index == 1)
        {
            while (timer < 4)
            {

                timer += Time.deltaTime;
                float t = timer / 4;
                leftDoorMat[0].SetColor("_EmissionColor", (startColor * timer * 2.5f));
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
        else if (index == 2)
        {
            while (timer < 4)
            {
                timer += Time.deltaTime;
                float t = timer / 4;
                rightDoorMat[0].SetColor("_EmissionColor", (startColor * timer * 2.5f));
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
        else if(index == 3)
        {
            while (timer < 4)
            {
                timer += Time.deltaTime;
                float t = timer / 4;
                rightDoorMat[1].SetColor("_EmissionColor", (startColor * timer * 2.5f));
                leftDoorMat[1].SetColor("_EmissionColor", (startColor * timer * 2.5f));
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

        if(repeeted.Count > 2)
        {
            timer = 0;
            while (timer < 4)
            {
                timer += Time.deltaTime;
                float t = timer / 4;
                leftDoorMat[2].SetColor("_EmissionColor", (startColor * timer * 2.5f));
                rightDoorMat[2].SetColor("_EmissionColor", (startColor * timer * 2.5f));
                yield return new WaitForSeconds(Time.deltaTime);
            }
            StartCoroutine(OpenTheGates());
        }
        lerping = false;
    }

    /// <summary>
    /// Lers a rotation for the door so that it opes up so the player can enter
    /// </summary>
    /// <returns></returns>
    private IEnumerator OpenTheGates()
    {
        audio.Play();
        lerping = true;
        float timer = 0;
        Vector3 positionHolder = Vector3.zero;
        float variable = 0f;
        while(timer <= 8)
        {
            timer += Time.deltaTime;
            variable = Mathf.Lerp(0, 90, timer / 8);
            positionHolder.y = -variable;
            left.transform.localRotation = Quaternion.Euler(positionHolder);

            variable = Mathf.Lerp(0, 90, timer / 8);
            positionHolder.y = variable;
            right.transform.localRotation = Quaternion.Euler(positionHolder);

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
