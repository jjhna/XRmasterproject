using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StickyScript : MonoBehaviour
{
    private GameObject assetholder;
    [Tooltip("This is the sphere that attaches to the object and shoots the laser at the child sphere: StickySphere must be 0.1f size")]
    public GameObject leftstickysphere, rightstickysphere;
    [Tooltip("The parent of this gameobject")]
    public GameObject parentObj;
    [Tooltip("Boolean to see if this gameobject has a parent")]
    public bool hasparent;
    [Tooltip("This is the laser beam that connects the child to the parent")]
    public GameObject laserbeam;
    [Tooltip("This is the grouped item text that will hover above the gameobjects: GroupedItemText")]
    public GameObject spawngroupeditem;
    [Tooltip("This is the laser that has been spawned to connect the browsers")]
    public GameObject laserspawned;
    private GameObject groupeditem, myhead;
    public List<NewChildClass> newchildclasslist = new List<NewChildClass>();
    public bool isnotempty, destroyit, inventorychecked;
    private StickyScript[] componentstuff;
    private float heightbrowserspawn;

    // Start is called before the first frame update
    void Start()
    {
        assetholder = GameObject.Find("AssetHolder");
        myhead = GameObject.Find("VRCamera"); 
        heightbrowserspawn = MenuScript.theOne.Mheightbrowserspawn;
    }

    // The fixed update checks every to see if this gameobject has a parent and child
    void FixedUpdate()
    {
        CheckHasParentChild();
        CheckUnder();
        CheckStickySides();
        CheckDisconnectButton();
    }

    // [class] A class to keep track of the children that are connect to this parent, the gameobject, lasers and parent spheres
    public class NewChildClass
    {
        public GameObject child;
        public GameObject laser;

        // [feature] Function for the NewChildClass to assign the child gameobject, laser and parent sphere
        public NewChildClass(GameObject whatchild, GameObject whatlaser)
        {
            child = whatchild;
            laser = whatlaser;
        }
    }

    // [feature] A feature to check which side the child (this)browser is against the parent browser. 
    // If the child is to the left and moves to the right then the lasers will switch sides and directions.
    private void CheckStickySides()
    {
        if (hasparent)
        {
            // https://docs.unity3d.com/ScriptReference/Transform.InverseTransformPoint.html
            Vector3 parentRelative = parentObj.transform.InverseTransformPoint(transform.position);
            Vector3 leftsphere = parentObj.GetComponent<StickyScript>().leftstickysphere.transform.InverseTransformPoint(transform.position);
            Vector3 rightsphere = parentObj.GetComponent<StickyScript>().rightstickysphere.transform.InverseTransformPoint(transform.position);
            //if parentObj is on the right of this gameobject then the laser lines will switch spheres
            if (parentRelative.x > 0 && rightsphere.x > 0)
            {
                laserspawned.GetComponent<LaserScript>().childObj = leftstickysphere;
                laserspawned.GetComponent<LaserScript>().parentObj = parentObj.GetComponent<StickyScript>().rightstickysphere;
            }
            //if parentObj is on the left of this gameobject then the laser lines will switch spheres
            else if (parentRelative.x < 0 && leftsphere.x < 0)
            {
                laserspawned.GetComponent<LaserScript>().childObj = rightstickysphere;
                laserspawned.GetComponent<LaserScript>().parentObj = parentObj.GetComponent<StickyScript>().leftstickysphere;
            }
            //if parentObj is on the right side and parents right sphere is further to the right then the laser will stick to the right spheres
            else if (parentRelative.x > 0 && rightsphere.x < 0)
            {
                laserspawned.GetComponent<LaserScript>().childObj = rightstickysphere;
                laserspawned.GetComponent<LaserScript>().parentObj = parentObj.GetComponent<StickyScript>().rightstickysphere;
            }
            //if parentObj is on the left side and parents left sphere is further to the left then the laser will stick to the left spheres
            else if (parentRelative.x < 0 && leftsphere.x > 0)
            {
                laserspawned.GetComponent<LaserScript>().childObj = leftstickysphere;
                laserspawned.GetComponent<LaserScript>().parentObj = parentObj.GetComponent<StickyScript>().leftstickysphere;
            }
        }
    }

    //[feature] Checks if the child browser (this) has a parent if so then the disconnect button is enabled otherwise its disabled
    private void CheckDisconnectButton()
    {
        if (hasparent)
        {
            this.GetComponent<BrowsControlPanelAdditions>().runCodeInBrowser("document.getElementById('disconnect').disabled = false");
        }
        else
        {
            this.GetComponent<BrowsControlPanelAdditions>().runCodeInBrowser("document.getElementById('disconnect').disabled = true");
        }
    }

    // [feature] make the gameobject "theParent" the parent of this gameobject, initialize who the parent gameobject is going to be
    public void MakeParent(GameObject theChild)
    {
        bool isachild = false;
        componentstuff = theChild.GetComponentsInChildren<StickyScript>();
        // Iterate through the componentstuff array (as mentioned above)
        // The purpose of the search is to prevent the object child from connecting to any of it's own children (think incest)
        // So if the parent tags one of its children then the 1st selection (child object) is removed
        foreach (StickyScript item in componentstuff)
        {
            // if the item matches the first object in the array then the iteration or search continues
            // This is because Unity's GetComponentsInChildren adds in the parent object as the 1st iteration for some reason
            if (item == componentstuff[0])
            {
                continue;
            }
            // If one of the item gameobject matches the object parent (2nd choice) then isachild is true breaking the foreach loop
            // thus doing nothing since the browser in question is already a child/grandchild of this browser
            if (item.gameObject == this.gameObject)
            {
                isachild = true;
                break;
            }
        }

        if (!isachild)
        {
            if (!theChild.GetComponent<StickyScript>().hasparent)
            {
                _MakeParent(theChild);
            }
            else
            {
                theChild.GetComponent<StickyScript>().destroyit = false;
                theChild.GetComponent<StickyScript>().RemoveParent();
                _MakeParent(theChild);
            }
        }
    }

    // [feature] The actual function to make the parent to reduce typing in the code twice
    private void _MakeParent(GameObject theChild)
    {
        theChild.GetComponent<StickyScript>().hasparent = true;
        theChild.GetComponent<StickyScript>().laserspawned = Instantiate(laserbeam, theChild.GetComponent<StickyScript>().leftstickysphere.transform.position, Quaternion.identity, this.transform);
        theChild.GetComponent<StickyScript>().laserspawned.GetComponent<LaserScript>().SetupLaser(theChild.GetComponent<StickyScript>().leftstickysphere, leftstickysphere, true);
        NewChildClass mystuff = new NewChildClass(theChild, theChild.GetComponent<StickyScript>().laserspawned);
        newchildclasslist.Add(mystuff);

        theChild.GetComponent<StickyScript>().parentObj = this.gameObject;
        // make Parentobj the parent of this gameobject, first got to put the object back into the scene
        //transform.SetParent(assetholder.transform); // this helped fix an issue last time but now its causing more issues
        theChild.transform.SetParent(this.gameObject.transform);
    }

    // [feature] Function to remove both child and parent spheres and laser
    // Destroys the child sphere and calls the function RemoveParentSphere from the parent script to remove the parent sphere and laser
    public void RemoveParent()
    {
        if (hasparent)
        {
            // Create temporary variables
            bool foundit = false;
            GameObject foundlaser = null;
            NewChildClass foundfound = null;

            // Iterate through the entire NewChildClass list and find which child gameobject matches the childObj 
            foreach(NewChildClass found in parentObj.GetComponent<StickyScript>().newchildclasslist)
            {
                if (found.child == this.gameObject)
                {
                    // if found then the next if statement is ran with the following variables being assigned
                    foundit = true;
                    foundlaser = found.laser;
                    foundfound = found;
                }
            }
            // If the correct child gameobject is found then destroy the parent sphere, laser and remove it from the list
            if (foundit)
            {
                Destroy(foundlaser);
                parentObj.GetComponent<StickyScript>().newchildclasslist.Remove(foundfound);
            }

            // Remove parent gameobject and the related class
            if (!destroyit)
            {
                transform.SetParent(assetholder.transform);
            }
            parentObj = null;
            hasparent = false;
        }
    }

    // [feature] Function to remove all the children from this gameobject and its respective list
    public void RemoveAllChildren()
    {
        if (isnotempty)
        {
            // Remove all the children in the newchildclasslist
            foreach(NewChildClass found in newchildclasslist.ToList())
            {
                found.child.GetComponent<StickyScript>().destroyit = false;
                found.child.GetComponent<StickyScript>().RemoveParent();
            }   
        }
    }

    // [feature] Function to continue to check if this gameobject is a gameobject with children but no parent
    // If so then the SpawnGroupedCanvas() is called otherwise the RemoveGroupedCanvas() is called
    private void CheckHasParentChild()
    {
        // Check if this gameobject has any children, if empty then false, otherwise true
        if (newchildclasslist.Count.Equals(0))
        {
            isnotempty = false;
        }
        else
        {
            isnotempty = true;
        }
        // Spawn the grouped item above the browser otherwise destroy if the item already exists in the scene
        if (!hasparent && isnotempty)
        {
            if (groupeditem == null)
            {
                groupeditem = GameObject.Instantiate(spawngroupeditem, this.gameObject.transform.position + new Vector3(0, 2, 0), Quaternion.identity, this.transform);
            }
            else
            {
                groupeditem.transform.LookAt(myhead.transform);
                groupeditem.transform.rotation *= Quaternion.Euler(0, 180, 0);
            }
        }
        else
        {
            if (groupeditem != null)
            {
                Destroy(groupeditem);
                groupeditem = null;
            }       
        }
    }

    // [feature] Function to check if the gameobject falls under the scene
    private void CheckUnder()
    {
        // If the y position of this gameobject is less than 0 then the gameobject will teleport above the ground floor
        if (transform.position.y < 0 && !inventorychecked)
        {
            float currentheight = MenuScript.theOne.PlayerCurrentHeight();
            Vector3 pos = transform.position;
            // calculate the players camera height + the public variable
            transform.position = new Vector3(pos.x, currentheight + heightbrowserspawn, pos.z); 
            // make the browser look at the players camera if going under the scene
            transform.rotation = myhead.transform.rotation;
        }
    }
}
