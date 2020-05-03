using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Marcus Lundqvist
//Secondary: Hjalmar Andersson
public class FaceAngleDirection : MonoBehaviour
{
    private RaycastHit hit;
    private Transform parentTransform;
    //private Vector3 surfaceNormal;

    /// <summary>
    /// Sets the variable transform to the parents transform
    /// </summary>
    public void Start()
    {
        parentTransform = GetComponentInParent<Transform>();
    }

    /// <summary>
    /// Sends 2 raycasts downwards, one from of the actors forward position and a one at its backward position.
    /// The Actors <see cref="Transform.forward"/> is then set to the front raycasts hit location subtracted by the back raycasts hit location
    /// </summary>
    public void FixedUpdate()
    {
        if (Physics.Raycast((transform.position + transform.forward) + (Vector3.up * 7), Vector3.down, out hit, 10.0f, LayerMask.GetMask("Terrain")))
        {
            Vector3 forwardHit = hit.point;
            if (Physics.Raycast((transform.position - transform.forward) + (Vector3.up * 7), Vector3.down, out hit, 10.0f, LayerMask.GetMask("Terrain")))
            {
                //surfaceNormal -= hit.normal;
                Vector3 backwardHit = hit.point;
                transform.forward = forwardHit - backwardHit;
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, parentTransform.rotation.eulerAngles.y, parentTransform.rotation.eulerAngles.z);

            }
        }
    }
}
