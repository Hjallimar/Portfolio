using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Auhtor: Hjalmar Andersson

public class CompasTest : MonoBehaviour
{
    [SerializeField] private Image N;
    [SerializeField] private Image E;
    [SerializeField] private Image S;
    [SerializeField] private Image W;

    [SerializeField] private float startFill;
    [SerializeField] private float endFill;

    private float compasAngle;
    private Vector3 pointer = new Vector3(0, 300,0);

    void Start()
    {
        N.transform.localPosition = new Vector3(0,300,0);
        W.transform.localPosition = new Vector3(-900, 300, 0);
        E.transform.localPosition = new Vector3(900, 300, 0);
        S.transform.localPosition = new Vector3(900, 300,0);

        //callback till navigation;
        EventHandeler.Current.RegisterListener(EventHandeler.EVENT_TYPE.Navigation, CompassRotation);
    }

    /// <summary>
    /// Reacts to a NavigationEventInfo callback; 
    /// It rotates the Images based on the float that comes with the callback;
    /// Callback is called by the camera and the float is the rotation of the camera;
    /// This secures that the North is always North;
    /// </summary>
    /// <param name="eventInfo">NavigationEventInfo</param>
    private void CompassRotation(EventInfo eventInfo)
    {
        NavigationEventInfo navInfo = (NavigationEventInfo)eventInfo;

        compasAngle = navInfo.navFloat;  

        if (compasAngle >= 270 && compasAngle < 360) // 270 grader
        {
            float p = (compasAngle - 270f) /90;

            LerpPictureBetween(N, 800, 0, p);
            LerpPictureBetween(W, 0, -800, p);
            CheckFill(N);
            CheckFill(W);
        }
        else if (compasAngle >= 180 && compasAngle < 270) // 180 grader
        {
            float p = (compasAngle - 180) / 90;

            LerpPictureBetween(S, 0, -800, p);
            LerpPictureBetween(W, 800, 0, p);
            CheckFill(S);
            CheckFill(W);
        }
        else if (compasAngle >= 90 && compasAngle < 180) // 90 grade
        {
            float p = (compasAngle - 90) / 90;

            LerpPictureBetween(S, 800, 0, p);
            LerpPictureBetween(E, 0, -800, p);
            CheckFill(S);
            CheckFill(E);
        }
        else if (compasAngle >= 0 && compasAngle < 90) // 0 grade
        {
            float p = compasAngle/90.0f;

            LerpPictureBetween(N, 0, -800, p);
            LerpPictureBetween(E, 800, 0, p);
            CheckFill(N);
            CheckFill(E);
        }
    }

    /// <summary>
    /// Checks if the Image should be visable or not;
    /// </summary>
    /// <param name="picture">The Image that should be checked;</param>
    private void CheckFill(Image picture)
    {
        float posX = picture.transform.localPosition.x;
        if( posX < startFill && posX >endFill)
        {
            picture.fillAmount = 1;
        }
        else
        {
            picture.fillAmount = 0;
        }
    }

    /// <summary>
    /// Lerps a Image from one X-value to another X-value, distance is based on the entered procent;
    /// </summary>
    /// <param name="picture">The picture that should move;</param>
    /// <param name="from">From what X-value;</param>
    /// <param name="to">To what X-value;</param>
    /// <param name="time">What procent has it moved;</param>
    private void LerpPictureBetween(Image picture, float from, float to, float procent)
    {
        pointer.x = Mathf.Lerp(from, to, procent);
        picture.transform.localPosition = pointer;
    }
}
