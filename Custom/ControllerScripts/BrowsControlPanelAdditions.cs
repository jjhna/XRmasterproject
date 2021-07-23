using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZenFulcrum.EmbeddedBrowser;
using Valve.VR;
using Valve.VR.InteractionSystem;

/**
 * Purpose: add additional effects to the control panel without fussing with ZFB scripts
 * 
 * Additions:
 *  Sticky Mode
 *  (want) remove parent from sticky
 *  Put in inventory
 */


public class BrowsControlPanelAdditions : MonoBehaviour
{
    StickyScript stickyScript;
    private GameObject CCC;
    public static BrowsControlPanelAdditions theOne;
    public bool stickyOn;

    private void Awake()
    {
        CCC = GameObject.Find("CentralControlPanel");
        theOne = this;
    }

    void Start()
    {
        // After awake, which is when VRBrowserPanel adds
        ZenFulcrum.EmbeddedBrowser.Browser b = GetComponent<VRBrowserPanel>().controlBrowser;


        b.RegisterFunction("ControllerModeSticky", args => EnableModeSticky());

        b.RegisterFunction("ParentModeSticky", args => EnableParent());

        //b.RegisterFunction("setButtonParentStatus", args => setButtonParentStatus(args[0]));

        b.RegisterFunction("UnparentSticky", args => EnableUnparentSticky());

        b.RegisterFunction("AddThisBrowserToInventory", args => AddThisBrowserToInventory());

        // This function works and CAN be used to add browser monitoring.
        //b.onConsoleMessage += (message, source) =>
        //{
        //    Debug.Log("BrowsControlPanelAdditions detected console message" + message);
        //    if (message.IndexOf("stick") > -1)
        //    {
        //        Debug.Log("Probably want to switch ");
        //    }
        //};
    }
    
    public void EnableModeSticky()
    {
        // Show sticky tooltips. This is difference since both controllers could activate.
        MenuScript.theOne.RightHints(4);
        MenuScript.theOne.LeftHints(4);

        //stickyScript = this.GetComponent<StickyScript>();
        // Access all the Browsers in the scene to let them know that this gameobject is looking for a parent
        // Use VRMainControlPanel.cs allBrowsers list to iterate through the browsers in the scene

        //Get this gameobject and convert it to the objChild of the stickychildscript of this gameobject or something else
        CCC.GetComponent<VRMainControlPanel>().MakeParentButtonOn(this.gameObject);
    }

    public void EnableParent()
    {
        if (stickyOn)
        {
            GameObject temp = CCC.GetComponent<VRMainControlPanel>().childSticky;
            this.GetComponent<StickyScript>().MakeParent(temp);
            CCC.GetComponent<VRMainControlPanel>().ParentButtonOff();
        }
    }

    public void EnableUnparentSticky()
    {
        this.GetComponent<StickyScript>().destroyit = false;
        this.GetComponent<StickyScript>().RemoveParent();
    }

    public void setButtonParentStatus(bool enabled)
    {
        Debug.Log("setButtonParentStatus " + enabled);
        if (enabled)
        {
            runCodeInBrowser("document.getElementById('btn_unparent').disabled = null");
        } else
        {
            runCodeInBrowser("document.getElementById('btn_unparent').disabled = true");
        }
    }

    public void runCodeInBrowser(string code)
    {
        ZenFulcrum.EmbeddedBrowser.Browser b = GetComponent<VRBrowserPanel>().controlBrowser;
        b.EvalJS(code).Done();
    }

    public void AddThisBrowserToInventory()
    {
        // IDK why this is needed. Keeping as is in case something else needs the vars.
        MenuScript ms = MenuScript.theOne;
        ms.emptyinventory = 0;
        for (int i = 9; i > -1; i--)
        {
            if (ms.inventoryArray[i] != null) 
            {
                ms.emptyinventory++; 
            }
            else  
            { 
                ms.inventoryholder = i; 
            }
        }
        if (ms.emptyinventory == 10)
        {
            // Show reposition tooltips
            MenuScript.theOne.LeftHints(5);
            MenuScript.theOne.RightHints(5);
        }
        else
        {
            ms.InventoryIt(gameObject);
            // turn off the stickies if stored into the inventory
            CCC.GetComponent<VRMainControlPanel>().ParentButtonOff();
        }
        // if (ms.inventoryholder > -1)
        // {
        //     ms.InventoryIt(gameObject);
        // }
    }



}
