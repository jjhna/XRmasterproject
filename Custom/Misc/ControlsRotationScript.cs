using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by: Jonathan Na from the University of Hawaii at Manoa, Laboratory for Advanced Visualization & Application (LAVA)
// This is the controls rotation script to force the controls gameobject to follow the players head camera movments on its x-axis.

public class ControlsRotationScript : MonoBehaviour
{
    public GameObject theHead;

    // Start is called before the first frame update
    void Start()
    {
        theHead = GameObject.Find("VRCamera");
    }

    // Update is called once per frame
    void Update()
    {
        // Need to lay it on the y-axis so it can swivel around
        //transform.LookAt(new Vector3(theHead.transform.position.x, transform.position.y, theHead.transform.position.z));
        // Need to multiply by 180 degrees to flip the asset around
        //transform.rotation *= Quaternion.Euler(0, 180, 0);

        // Only be used if you want to swivel and look up or down
        // transform.LookAt(theHead.transform);
        // transform.rotation *= Quaternion.Euler(0, 180, 0);

        // Only to be used if rotation on x-axis
        // transform.rotation = Quaternion.Euler(theHead.transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);

        
        transform.LookAt(theHead.transform);
        Vector3 tmp = transform.localRotation.eulerAngles;
        tmp.y = 0;
        tmp.z = 0;
        tmp.x *= -1;
        transform.localRotation = Quaternion.Euler(tmp);
    }
}
