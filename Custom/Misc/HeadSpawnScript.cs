using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZenFulcrum.EmbeddedBrowser;

// Created by: Jonathan Na from the University of Hawaii at Manoa, Laboratory for Advanced Visualization & Application (LAVA)
// This is the Head Spawn script that ties in when spawning in a new browser (using the quick links)
// or loading an inventory item in front of the player. This is not the script used to create a browser when clicking on
// a new link that opens a tab in the browser (that script can be found in: VRMainControlPanel.cs)
public class HeadSpawnScript : MonoBehaviour
{
    private GameObject myhead, menuhand, theplayer;
    private float heightbrowserspawn;

    // Start is called before the first frame update
    void Start()
    {
        menuhand = GameObject.Find("ObjPointerSpawner");
        myhead = GameObject.Find("VRCamera"); 
        theplayer = GameObject.Find("Player3");
        heightbrowserspawn = MenuScript.theOne.Mheightbrowserspawn;
    }

    void Update()
    {

    }

    // [feature] Function that is called mainly from spawning in a new browser or loading an inventory object (browser)
    // The purpose of this function is so that the object that is being spawned lands in front of the player
    // Note: The object being spawned will be facing away from the player so the object being spawned in needs to be
    // rotated at least 180 degrees and using the LookAt() function again
    public Vector3 HeadSpawnChange()
    {
        // reset head position to this blank gameobject
        transform.position = myhead.transform.position;
        // Rotate this gameobject to look at the menu obj
        transform.LookAt(menuhand.transform);
        // Calculate the distance between the vr camera and menu obj
        float dist = Vector3.Distance(myhead.transform.position, menuhand.transform.position);
        // Move the browser/obj in front of the menu hand of the player
        transform.Translate(0, 0, (dist * 2), menuhand.transform);
        // Get the current player location
        float currentheight = MenuScript.theOne.PlayerCurrentHeight();
        // When spawning make sure the browser is above the ground and not too high
        transform.position = new Vector3(transform.position.x, currentheight + heightbrowserspawn, transform.position.z);
        // Return a the vector 3 position of this empty head gameobject
        return transform.position;
    }
}
