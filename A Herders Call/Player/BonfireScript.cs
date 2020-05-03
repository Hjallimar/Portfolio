using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main Author: Hjalmar Andersson

    /// <summary>
    ///Was never used
    /// </summary>
public class BonfireScript : MonoBehaviour
{
    public float burningTime;
    private float kindle = 0f;
  
    void Update()
    {
        kindle += Time.deltaTime;
        if(kindle >= burningTime)
        {
            kindle = 0f;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            Debug.Log("Experiment successful");
        if(other.gameObject.tag == "Wolf")
        {
           // TorchEventInfo tei = new TorchEventInfo { playerPosition = transform.position };
           // EventHandeler.Current.FireEvent(tei);
        }
    }

    /// <summary>
    /// Controlls the objects lifetime through the value of <see cref="burningTime"/>.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Burning()
    {
        yield return new WaitForSeconds(burningTime);
        gameObject.SetActive(false);
    }
}
