using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

// Created by: Jonathan Na from the University of Hawaii at Manoa, Laboratory for Advanced Visualization & Application (LAVA)
// The only purpose of this script is to interact with game ojects from a far distance. Allowing the player to rotate, 
// pull, push the gameobject in while following the direction of the controller that's holding it.
public class DistanceGrabScript : MonoBehaviour
{
    public GameObject assetholder;
    private GameObject parentObj;
    private StickyScript SCS;

    void Awake()
    {
        assetholder = GameObject.Find("AssetHolder");
        SCS = this.gameObject.GetComponent<StickyScript>();
    }
    
    // [feature] When this function is used then the game object that is being held with the distance control script will become the child 
    // of this game object (hand), 
    // ex: if I selected/held a cube then that cube will be the child of the hand that is being used
    public void ObjGrabbed(Hand hand)
    {
        getGrabbedBy(hand.gameObject);
    }

    public void getGrabbedBy(GameObject go) {
        // Checks to see if the held gameobject contains a parent if so then that gameobject is temporarily stored for the release function
        if (SCS.hasparent) { parentObj = SCS.parentObj; }
        // transform.SetParent(go.gameObject.transform);
        transform.parent = go.transform;
    }

    // [feature] When this function is used then the game object that was being held is released and no longer becomes the child of the hand
    // However it's important that the game object becomes the parent of "AssetHolder", otherwise the game object 
    // will continue to be the parent of the hand under the "Dontdestroyonload" and will carry onto other scenes. 
    public void ObjReleased(Hand hand)
    {
        getReleased();
    }

    public void getReleased() {
        transform.SetParent(assetholder.transform);
        // If the object had a parent then it recalls the parentObj to store the release gameobject pack to the parent 
        // if (SCS.hasparent) { transform.SetParent(parentObj.transform); }
        if (SCS.hasparent) { transform.parent = parentObj.transform; }
        parentObj = null;
    }
}
