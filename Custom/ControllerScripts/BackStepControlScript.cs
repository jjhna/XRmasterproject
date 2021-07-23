using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

// Created by: Jonathan Na from the University of Hawaii at Manoa, Laboratory for Advanced Visualization & Application (LAVA)
// This script is created used by either hands to move the player backwards, it follows the position of the "BackStepSpawn"
// which then teleports to that location when the bottom of the trackpad is clicked on
public class BackStepControlScript : MonoBehaviour
{
    [Tooltip("Used check which hand is being used: Left or Right")]
    [SerializeField]
    private SteamVR_Input_Sources whichHand;
    [Tooltip("What controller action will trigger the game action or event")]
    [SerializeField]
    private SteamVR_Action_Boolean SteamClick;
    private bool southclick = false; 
    public bool isAllowed;
    private GameObject thePlayer, maincamera;
    private float backdistance;

    // Start is called before the first frame update
    void Start()
    {
        SteamClick.AddOnStateDownListener(GripDown, whichHand);
        SteamClick.AddOnStateUpListener(GripUp, whichHand);
        thePlayer = GameObject.Find("Player3");
        maincamera = GameObject.Find("VRCamera");
        backdistance = MenuScript.theOne.Mbackstep;
        isAllowed = true;
    }
    
    void FixedUpdate()
    {
        ClickCheck();
    }

    //[feature] Checks for the bottom of the trackpad being clicked/pressed, if so then the camera will fade
    // and the player will move backwards based off the head cameras position and then disabled until clicked again
    private void ClickCheck()
    {
        if (isAllowed && southclick)
        {
            Vector3 fooward = maincamera.transform.forward;
             MenuScript.theOne.OtherFade();
            fooward.y = 0f;
            thePlayer.transform.position -= fooward * backdistance;
            southclick = false;
        }
    }

    #region Click
    // [feature] Using the VIVE south trackpad click will create the move the player
    private void GripDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        southclick = true;
    }

    private void GripUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        southclick = false;
    }
    #endregion
}
