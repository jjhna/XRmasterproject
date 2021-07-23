using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

// Created by: Jonathan Na from the University of Hawaii at Manoa, Laboratory for Advanced Visualization & Application (LAVA)
// This script is created for the distance grab controller so that a user can grab an object from a far distance
// Once the user has grabbed an object then the user can rotate and pull/push the object from the user using the touchpad
public class DistanceControlScript : MonoBehaviour
{
    public GameObject objGrabbed;
    private DistanceGrabScript DGS;
    [Tooltip("Used check which hand is being used: Left or Right")]
    [SerializeField]
    private SteamVR_Input_Sources whichHand;
    [Tooltip("What controller action will trigger the game action or event")]
    [SerializeField]
    private SteamVR_Action_Boolean steamTrigger;
    [Tooltip("What controller action will trigger the game action or event")]
    [SerializeField]
    private SteamVR_Action_Boolean steamPadTrigger;
    [Tooltip("What controller action will trigger the game action or event")]
    [SerializeField]
    private SteamVR_Action_Boolean steamGripTrigger;
    [Tooltip("Drag and drop the controller game object: LeftHand or RightHand into this slot")]
    [SerializeField]
    private Transform whichController;
    [Tooltip("What controller action will trigger the game action or event")]
    [SerializeField]
    private SteamVR_Action_Vector2 steamScroll;
    private Hand hand;
    private Vector2 scrollInfo;
    private GameObject menuObj;
    private MenuScript menuscr;
    public bool objgrabbool = false;
    private bool objheld;
    private bool triggerIsDown = false; // DKMOD to allow dedicated reposition target
    private RaycastHit hit;
    public LayerMask mask;
    private float movebrowserspeed, turnbrowserspeed;

    void Start()
    {
        steamTrigger.AddOnStateDownListener(TriggerDown, whichHand);
        steamTrigger.AddOnStateUpListener(TriggerUp, whichHand);
        hand = GetComponent<Hand>();
        menuObj = GameObject.Find("AllHandControls");
        menuscr = menuObj.GetComponent<MenuScript>();
        movebrowserspeed = MenuScript.theOne.Mmovebrowserspeed;
        turnbrowserspeed = MenuScript.theOne.Mturnbrowserspeed;
    }
    
    // Update is called once per frame
    // Scroll info needs to be collected continously from the Update() because scroll info is spontaneous and constantly changing
    void Update()
    {
        scrollInfo = steamScroll.GetAxis(whichHand);
        MoveRotateObj();
    }

    void FixedUpdate()
    {
        Raybeam();
        TriggerCheck();
    }

    // [feature] Uses the SteamVR lazer to show the user what gameobject that they are pointing at
    private void Raybeam()
    {
        // NOTE: Why put this boolean check inside the raycast? Wouldn't it be cheaper to put it outside to make sure the trigger is being pressed?
        // If the trigger is pressed, not currently holding a browser, menu button isn't clicked and not holding a resize button atm
        if (triggerIsDown && !objgrabbool && !this.GetComponent<ResizeControlScript>().objrezbool)
        {
            // beamoff has NOTHING to do with visual status
            if (Physics.Raycast(whichController.position - (whichController.forward / 6), whichController.forward, out hit, Mathf.Infinity, mask)) // DKMOD, used for dedicated distance grab
            {
                // Show reposition tooltips, note this only happens when you "click" on the content of the browser or menu
                if (whichHand == SteamVR_Input_Sources.RightHand)
                {
                    MenuScript.theOne.RightHints(1);
                }
                else if (whichHand == SteamVR_Input_Sources.LeftHand)
                {
                    MenuScript.theOne.LeftHints(1);
                }

                // NOTE: this gameobject is returning the browser prefab. HOW it is autotraversing up. Double checked, the layer is only applied to the child dedicated control
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Control_DistanceGrab"))
                {
                    objGrabbed = hit.transform.gameObject;
                    objheld = true;
                }
            }
        }
    }

    // [feature] Using the VIVE trigger will create the move the selected gameobject
    private void TriggerCheck()
    {
        // Mod, if objGrabbed has special tag
        // NOTE, this will cause problems with existing code when both distance control is enabled and the special object is grabbed.
        // NOTE2, very confused, objGrabbed is the browser prefab, meaning raycast autotraversed
        if (objheld)
        {
            if (triggerIsDown)
            {
                // Copying over code, but does this really fire every update?
                // It will need to crawl structure then get grab
                if (objGrabbed.GetComponent<DistanceGrabScript>() != null)
                {
                    DGS = objGrabbed.GetComponent<DistanceGrabScript>();
                    DGS.ObjGrabbed(hand);
                    objgrabbool = true;

                    // Show reposition tooltips
                    if (whichHand == SteamVR_Input_Sources.RightHand)
                    {
                        MenuScript.theOne.RightHints(3);
                        menuscr.snapTurn.GetComponent<SnapTurn>().rightClear = false;
                        menuscr.teleporty.GetComponent<Teleport>().distancegrabbedRight = false;
                        menuscr.rightHandy.GetComponent<BackStepControlScript>().isAllowed = false;
                    }
                    else if (whichHand == SteamVR_Input_Sources.LeftHand)
                    {
                        MenuScript.theOne.LeftHints(3);
                        menuscr.snapTurn.GetComponent<SnapTurn>().leftClear = false;
                        menuscr.teleporty.GetComponent<Teleport>().distancegrabbedLeft = false;
                        menuscr.leftHandy.GetComponent<BackStepControlScript>().isAllowed = false;
                    }
                }
            }
            else
            {
                // Copypaste from just below
                if (objgrabbool) DGS.ObjReleased(hand);
                DGS = null;
                objGrabbed = null;
                objheld = false;
                objgrabbool = false;

                // On release show browser tool tips
                if (whichHand == SteamVR_Input_Sources.RightHand)
                {
                    //MenuScript.theOne.RightHints(1);
                    menuscr.snapTurn.GetComponent<SnapTurn>().rightClear = true;
                    menuscr.rightHandy.GetComponent<BackStepControlScript>().isAllowed = true;
                }
                else if (whichHand == SteamVR_Input_Sources.LeftHand)
                {
                    //MenuScript.theOne.LeftHints(1);
                    menuscr.snapTurn.GetComponent<SnapTurn>().leftClear = true;
                    menuscr.leftHandy.GetComponent<BackStepControlScript>().isAllowed = true;
                }
            }

        }
        else
        {
            if (whichHand == SteamVR_Input_Sources.RightHand)
            {
                menuscr.teleporty.GetComponent<Teleport>().distancegrabbedRight = true;
            }
            else if (whichHand == SteamVR_Input_Sources.LeftHand)
            {
                menuscr.teleporty.GetComponent<Teleport>().distancegrabbedLeft = true;
            }
        }
    }

    // [feature] While holding down the trigger the child obj will either rotate or move based off touchpad controls
    private void MoveRotateObj()
    {
        if (objgrabbool)
        {
            // Move browser towards or away from player
            if (scrollInfo.y > 0.8f)
            {
                //Scroll up, moves object away from player
                DGS.transform.position += whichController.transform.forward * 3 * Time.deltaTime * movebrowserspeed;
            }
            else if (scrollInfo.y > 0.6f)
            {
                DGS.transform.position += whichController.transform.forward * 2 * Time.deltaTime * movebrowserspeed;
            }
            else if (scrollInfo.y > 0.3f)
            {
                DGS.transform.position += whichController.transform.forward * 1 * Time.deltaTime * movebrowserspeed;
            }
            else if (scrollInfo.y < -0.8f)
            {
                //Scroll down, moves object towards player
                DGS.transform.position += -whichController.transform.forward * 3 * Time.deltaTime * movebrowserspeed;
            }
            else if (scrollInfo.y < -0.6f)
            {
                DGS.transform.position += -whichController.transform.forward * 2 * Time.deltaTime * movebrowserspeed;
            }
            else if (scrollInfo.y < -0.3f)
            {
                DGS.transform.position += -whichController.transform.forward * 1 * Time.deltaTime * movebrowserspeed;
            }

            // turns browser left or right
            if (scrollInfo.x > 0.8f)
            {
                //Scroll right, rotates object to the right
                DGS.transform.RotateAround(DGS.transform.position, Vector3.up, 90 *Time.deltaTime * turnbrowserspeed);
            }
            else if (scrollInfo.x > 0.6f)
            {
                DGS.transform.RotateAround(DGS.transform.position, Vector3.up, 60 *Time.deltaTime * turnbrowserspeed);
            }
            else if (scrollInfo.x > 0.3f)
            {
                DGS.transform.RotateAround(DGS.transform.position, Vector3.up, 30 *Time.deltaTime * turnbrowserspeed);
            }
            else if (scrollInfo.x < -0.8f)
            {
                //Scroll left, rotates object to the left
                DGS.transform.RotateAround(DGS.transform.position, Vector3.down, 90 *Time.deltaTime * turnbrowserspeed);
            }
            else if (scrollInfo.x < -0.6f)
            {
                DGS.transform.RotateAround(DGS.transform.position, Vector3.down, 60 *Time.deltaTime * turnbrowserspeed);
            }
            else if (scrollInfo.x < -0.3f)
            {
                DGS.transform.RotateAround(DGS.transform.position, Vector3.down, 30 *Time.deltaTime * turnbrowserspeed);
            }
        }
    }

    #region Trigger
    // [feature] Using the VIVE trigger will create the move the selected gameobject
    private void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        triggerIsDown = true;
    }

    private void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        triggerIsDown = false;
    }
    #endregion
}