using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

// Created by: Jonathan Na from the University of Hawaii at Manoa, Laboratory for Advanced Visualization & Application (LAVA)
// This is the resize script is attached to the controllers and is used to select game objects to be resized 
public class ResizeControlScript : MonoBehaviour
{
    [Tooltip("The selected game object that is slated to be resized")]
    public GameObject objResize;
    [Tooltip("Used check which hand is being used: Left or Right")]
    [SerializeField]
    private SteamVR_Input_Sources whichHand;
    [Tooltip("What controller action will trigger the game action or event")]
    [SerializeField]
    private SteamVR_Action_Boolean steamTrigger;
    [Tooltip("Drag and drop the controller game object: LeftHand or RightHand into this slot")]
    [SerializeField]
    private Transform whichController;
    private GameObject menuObj;
    private MenuScript menuscr;
    public bool objrezbool;
    private bool triggerIsDown = false; // DKMOD allow for the dedicated resize control
    public LayerMask mask;


    SxrLine l1 = new SxrLine();
    SxrLine l2 = new SxrLine(); // for the revised closes point on two lines



    void Awake()
    {
        menuObj = GameObject.Find("AllHandControls");
        menuscr = menuObj.GetComponent<MenuScript>();
    }

    void Start()
    {
        steamTrigger.AddOnStateDownListener(TriggerDown, whichHand);
        steamTrigger.AddOnStateUpListener(TriggerUp, whichHand);
    }

    void FixedUpdate()
    {
        Raybeam();
        // originalResizeWidgetMover();
        closestPointsBetweenLinesResizeWidgetMover();
    }

    // [feature] Uses the SteamVR lazer to show the user what gameobject that they are pointing at
    private void Raybeam()
    {
        // NOTE: Why put this boolean check inside the raycast? Wouldn't it be cheaper to put it outside to make sure the trigger is being pressed?
        // If the trigger is pressed, not currently holding a resize button, menu button isn't clicked and not holding a browser atm
        if (triggerIsDown && !objrezbool && !this.GetComponent<DistanceControlScript>().objgrabbool)
        {
            RaycastHit hit;
            if (Physics.Raycast(whichController.position, whichController.forward, out hit, Mathf.Infinity, mask))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Control_Resize"))
                {
                    Debug.Log("dedicated resize trigger hit on layer 21");
                    objResize = hit.collider.gameObject;

                    // Show resize tooltips
                    if (whichHand == SteamVR_Input_Sources.RightHand)
                    {
                        MenuScript.theOne.RightHints(2);
                        menuscr.snapTurn.GetComponent<SnapTurn>().rightClear = false;
                        menuscr.rightHandy.GetComponent<BackStepControlScript>().isAllowed = false;
                    }
                    else if (whichHand == SteamVR_Input_Sources.LeftHand)
                    {
                        MenuScript.theOne.LeftHints(2);
                        menuscr.snapTurn.GetComponent<SnapTurn>().leftClear = false;
                        menuscr.leftHandy.GetComponent<BackStepControlScript>().isAllowed = false;
                    }
                }
            }
        }
    }

    #region Trigger
    // [feature] Using the VIVE trigger will resize the gameobject 
    private void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        triggerIsDown = true;
    }
    #endregion

    private void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (objResize != null)
        {
            // Activate resize before losing reference
            // Fewer activations should assist with speed
            PointGrabResizeControls pgrc = objResize.transform.parent
                .gameObject.GetComponent<PointGrabResizeControls>();
            pgrc.applyLeftRightResize();
            pgrc.applyUpDownResize();
            pgrc.updateResolution();


            // Now remove references
            objResize = null;
            objrezbool = false;

            // On release show browser tool tips
            if (whichHand == SteamVR_Input_Sources.RightHand)
            {
                //MenuScript.theOne.RightHints(1); 
                menuscr.snapTurn.GetComponent<SnapTurn>().rightClear = true;
                menuscr.rightHandy.GetComponent<BackStepControlScript>().isAllowed = true;
            } else if (whichHand == SteamVR_Input_Sources.LeftHand)
            {
                //MenuScript.theOne.LeftHints(1);
                menuscr.snapTurn.GetComponent<SnapTurn>().leftClear = true;
                menuscr.leftHandy.GetComponent<BackStepControlScript>().isAllowed = true;
            }
        }
        triggerIsDown = false;
    }





    // ---------------------------------------------------------------------------
    // ---------------------------------------------------------------------------
    // ---------------------------------------------------------------------------
    // The following copy paste functions from LineHelper, but modified
    // Reference adjusted to work with vars in this class

    void closestPointsBetweenLinesResizeWidgetMover()
    {
        if (objResize == null) return;

        objrezbool = true;
        if (whichHand == SteamVR_Input_Sources.RightHand)
        {
            menuscr.teleporty.GetComponent<Teleport>().distancegrabbedRight = false;
        } else if (whichHand == SteamVR_Input_Sources.LeftHand)
        {
            menuscr.teleporty.GetComponent<Teleport>().distancegrabbedLeft = false;
        }

        // Work in world space
        l1.point = objResize.transform.position;
        l1.direction = objResize.transform.forward;
        l2.point = whichController.gameObject.transform.position;
        l2.direction = whichController.gameObject.transform.forward;

        SxrTwoVectors points = ResizeControlScript.getClosestPointsOnTwoLinesInSpace(l1, l2);
        objResize.transform.position = points.p1;

        // Should zero out the local values for y and x?


    }


    // ---------------------------------------------------------------------------
    // ---------------------------------------------------------------------------
    // ---------------------------------------------------------------------------
    // The following copy paste functions from LineHelper, but modified
    // Reference adjusted to work with vars in this class

    void originalResizeWidgetMover()
    {
        if (objResize == null) return;

        objrezbool = true;
        if (whichHand == SteamVR_Input_Sources.RightHand)
        {
            menuscr.teleporty.GetComponent<Teleport>().distancegrabbedRight = false;
        } else if (whichHand == SteamVR_Input_Sources.LeftHand)
        {
            menuscr.teleporty.GetComponent<Teleport>().distancegrabbedLeft = false;
        }
        string whichAxis = "";
        string goName = objResize.name;
        if (goName.IndexOf("Left") > 0 || goName.IndexOf("Right") > 0)
        {
            whichAxis = "z";
        }
        else if (goName.IndexOf("Up") > 0 || goName.IndexOf("Down") > 0)
        {
            whichAxis = "y";
        }


        LineDescription sld = getLineDescriptionOf(objResize, whichAxis);
        LineDescription pld = getLineDescriptionOf(whichController.gameObject, whichAxis);

        // Show how they would extend in space
        Debug.DrawRay(sld.goPosition, sld.goForward * 5, Color.red);
        Debug.DrawRay(pld.goPosition, pld.goForward * 5, Color.red);

        // Now where they would actually intersect if at all
        Vector2 intersectPoint = getLineIntersectionIfCan(sld, pld);
        // Intersect point will have x,z values, or NaN if divided by 0
        if (!float.IsNaN(intersectPoint.x))
        {
            // With the point, need to get real y value in 3d space (currently x,z)
            // Can use either game object.
            Vector3 point;
            if (whichAxis == "z")
            {
                point = determineYValueGivenXZ(intersectPoint, objResize);
                objResize.transform.position = point;
            } else if (whichAxis == "y")
            {
                point = determineZValueGivenXY(intersectPoint, objResize);
                objResize.transform.position = point;
            }
        }
        else
        { // no intersection point
            Debug.Log("cannot intersect");
        }
    }

    // ---------------------------------------------------------------------------
    LineDescription getLineDescriptionOf(GameObject go, string whichAxis)
    {
        string goName = go.name;
        LineDescription ld = new LineDescription();
        Vector3 f = go.transform.forward;
        Vector3 pos = go.transform.position;
        ld.goForward = f;
        ld.goPosition = pos;
        ld.x = f.x;
        if (whichAxis == "z")
        {
            ld.y = f.z;
            ld.m = f.z / f.x;
            ld.b = pos.z - (ld.m * pos.x);
        } else if (whichAxis == "y")
        {
            ld.y = f.y;
            ld.m = f.y / f.x;
            ld.b = pos.y - (ld.m * pos.x);
        }
        return ld;
    }

    // ---------------------------------------------------------------------------
    Vector2 getLineIntersectionIfCan(LineDescription ld1, LineDescription ld2)
    {
        float m1 = ld1.m;
        float m2 = ld2.m;
        float b1 = ld1.b;
        float b2 = ld2.b;

        float xIntersect = 0;
        float yIntersect = 0;
        /* Logic solve for two lines x value
            y = mx + b, given two lines
                m1, m2   b1, b2
            m1x + b1 = m2x + b2
            x = (b2 - b1) / (m1 - m2)
            Then recompute for y
        */
        if (m1 != m2)
        {
            xIntersect = (b2 - b1) / (m1 - m2);
        }
        else
        {
            return new Vector2(-9999, -9999); // Cannot divide by 0.
        }
        // Now get y value using either line
        yIntersect = m1 * xIntersect + b1;

        return new Vector2(xIntersect, yIntersect);
    }

    // ---------------------------------------------------------------------------
    Vector3 determineYValueGivenXZ(Vector2 xz, GameObject go)
    {
        float x = xz.x;
        float z = xz.y;
        Vector3 p = go.transform.position;
        Vector3 f = go.transform.forward;
        // Use x position to determine (z work too, just ratios)
        float diff = x - p.x;
        diff = diff / f.x; // gives multiplier
        return new Vector3(x, p.y + (f.y * diff), z);
    }

    // ---------------------------------------------------------------------------
    Vector3 determineZValueGivenXY(Vector2 xy, GameObject go)
    {
        float x = xy.x;
        float y = xy.y;
        Vector3 p = go.transform.position;
        Vector3 f = go.transform.forward;
        // Use x position to determine (z work too, just ratios)
        float diff = x - p.x;
        diff = diff / f.x; // gives multiplier
        return new Vector3(x, y, p.z + (f.z * diff));
    }




    // ---------------------------------------------------------------------------
    // ---------------------------------------------------------------------------
    // ---------------------------------------------------------------------------

    // Basically see this: https://en.wikipedia.org/wiki/Skew_lines#Nearest_Points

    public static SxrTwoVectors getClosestPointsOnTwoLinesInSpace(SxrLine l1, SxrLine l2) {
        Vector3 closestL1, closestL2;
        Vector3 n = Vector3.Cross(l1.direction, l2.direction); // Crossp product of direction vectors
        Vector3 n1 = Vector3.Cross(l1.direction, n);
        Vector3 n2 = Vector3.Cross(l2.direction, n);

        Vector3 tv1, tv2;
        float tf1, tf2;
        // First the line 1 closest point
        tv1 = l2.point - l1.point; // p2 - p1
        tf1 = Vector3.Dot(tv1, n2); // (p2 - p1) . n2
        tf2 = Vector3.Dot(l1.direction, n2); // D1 . n2
        tf1 = tf1 / tf2;
        tv1 = tf1 * l1.direction;
        closestL1 = tv1 + l1.point;

        // Now the second line
        tv1 = l1.point - l2.point;
        tf1 = Vector3.Dot(tv1, n1);
        tf2 = Vector3.Dot(l2.direction, n1);
        tf1 = tf1 / tf2;
        tv1 = tf1 * l2.direction;
        closestL2 = tv1 + l2.point;

        SxrTwoVectors stv = new SxrTwoVectors();
        stv.p1 = closestL1;
        stv.p2 = closestL2;

        return stv;
    }
}


public class SxrLine {
    public Vector3 point;
    public Vector3 direction;
}

public class SxrTwoLines {
    public SxrLine l1;
    public SxrLine l2;
}


public class SxrTwoVectors {
    public Vector3 p1;
    public Vector3 p2;
}





