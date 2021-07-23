using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by: Jonathan Na from the University of Hawaii at Manoa, Laboratory for Advanced Visualization & Application (LAVA)
// This script is created for the laser that is used for the stickysphere, sticky controller
// The laser connects between two spheres on two different gameobjects
public class LaserScript : MonoBehaviour
{
    private LineRenderer linerender;
    [Tooltip("Material for the line renderer: LaserMaterial")]
    public Material linematerial;
    public bool beamoff;
    public GameObject childObj, parentObj;
    private Vector3 childLoc, parentLoc;

    // Start is called before the first frame update
    void Start()
    {
        linerender = GetComponent<LineRenderer>();
        linerender.material = linematerial;
    }

    // Called for every singel frame
    // If the beamoff is true then the child sphere and parent sphere locations are tracked
    // Which calculates the distance and creates a line renderer between both spheres
    void Update()
    {
        if (beamoff)
        {
            childLoc = childObj.transform.position;
            parentLoc = parentObj.transform.position;
            linerender.SetPosition(1, parentLoc);
            linerender.SetPosition(0, childLoc);
        }
    }

    // [feature] Function to set up the laser between the child sphere and parent sphere, initialize both the child and parent spheres
    // Used by the StickyParentScript.cs in the ConnectParentSphere
    public void SetupLaser(GameObject child, GameObject parent, bool beambool)
    {
        childObj = child;
        parentObj = parent;
        beamoff = beambool;
    }
}
