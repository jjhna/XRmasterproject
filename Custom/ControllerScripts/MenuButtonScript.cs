using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR;

// Created by: Jonathan Na from the University of Hawaii at Manoa, Laboratory for Advanced Visualization & Application (LAVA)
// This is the menu button script or in a sense the menu button that opens the menu canvas screen
public class MenuButtonScript : MonoBehaviour
{
    [Tooltip("Used check which hand is being used: Left or Right")]
    [SerializeField]
    private SteamVR_Input_Sources whichHand;
    [Tooltip("What controller action will trigger the game action or event")]
    [SerializeField]
    private SteamVR_Action_Boolean menuClicky;
    private GameObject menuObj;
    private MenuScript menuscr;

    void Awake()
    {
        menuObj = GameObject.Find("AllHandControls");
        menuscr = menuObj.GetComponent<MenuScript>();
    }

    void Start()
    {
        menuClicky.AddOnStateDownListener(ButtonDown, whichHand);
    }

    // [feature] Have it so that if the menu canvas is already active then the user cannot interact with other objects while interacting with the menu
    private void ButtonDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (menuscr.menuclicked)
        {
            menuscr.closeMenu();
        }
        else if (!menuscr.menuclicked)
        {
            // HARDCODED....
            // Make the menu appear over the right hand if the menu button is clicked on the right controller. Otherwise default to left.
            if (fromSource.ToString() == "RightHand")
            {
                menuscr.RightMenu();
            } else
            {
                menuscr.LeftMenu();
            }
            menuscr.menuclicked = true;
            // Open the controller tab when the menu is opened
            menuObj.GetComponent<ButtonScript>().OpenLastMenuTab();
        }
    }
}
