using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;
using ZenFulcrum.EmbeddedBrowser;
using VRUiKits.Utils;
using UnityEditor;
using Evereal.VRVideoPlayer;

// Created by: Jonathan Na from the University of Hawaii at Manoa, Laboratory for Advanced Visualization & Application (LAVA)
// This is the menu script or in a sense the main control panel that connects all the menu options and its children into one script
// So if you're looking for changing controller settings, creating quick links, files and the game settings then this is the obj to look at
public class MenuScript : MonoBehaviour
{
    #region ImportantVariables
    //These are important ints that can be adjusted by the user before starting the scene
    [Tooltip("How far you want to move backwards")]
    public float Mbackstep;
    [Tooltip("How fast you want to pull or push a browser")]
    public float Mmovebrowserspeed;
    [Tooltip("How fast you want the browser to spin around")]
    public float Mturnbrowserspeed;
    [Tooltip("How high you want the browser to spawn")]
    public float Mheightbrowserspawn;
    #endregion

    #region Boolean
    // Boolean used by the Resize, Inventory, menu button, playerrespawn, etc scripts
    public bool menuclicked;
    // Boolean only used by the MenuScript.cs
    private bool leftinventon, rightinventon;
    #endregion

    #region Gameobjects
    // Gameobjects  used by the Resize, Inventory, menu button, playerrespawn, etc scripts
    [Tooltip("The gameobject: FloorPlan must be added")]
    public GameObject floorobj;
    [Tooltip("The gameobject: CurrentFloorOption must be added")]
    public GameObject floorcurrent;
    [Tooltip("Please add in the audioclip: Clicksounds38 or any clicking sfx")]
    public AudioClip audioclickclip;
    [Tooltip("Please add in the gameobject asset: CustomBrowser")]
    public GameObject browsertemplate;
    public GameObject videobrowsertemplate;
    public GameObject rightController;
    public GameObject leftController;
    public Text leftControllerTrigger, leftControllerGrip, leftControllerTrack;
    public Text rightControllerTrigger, rightControllerGrip, rightControllerTrack;
    public GameObject canvasObj, lefthand, righthand, fileMenu, 
    audioclick, floor1, floor2, floor3, floor4, floor5, theplayer, 
    assetholder, inventoryScreen, inventoryOther, teleporty, snapTurn, teleportfloor, leftHandy, rightHandy, videoleft, videoright;
    // Gameobjects only used by the MenuScript.cs
    private GameObject inventorySpot, inventorytemp, inventoryitemText, inventoryLeft, inventoryRight, 
    resizeX, resizeY, resizeZ, laserMenu, headspawn, myhead, objpointspawn, groupedinventory, inventoryholderquad, inventorylocation;
    #endregion

    #region Misc variables
    // Both audioclickclip and audioclipvol are used for the audio when clicking a button
    [Tooltip("Controls the volume of the button clicking sound")]
    public float audioclipvol;
    private Vector3 Mplayerloc;
    private ButtonScript btnscript;
    public int inventorySize = 10, emptyinventory, inventoryholder, inventoryselected;
    public GameObject[] inventoryArray = new GameObject[10];
    private StickyScript[] groupedinventorystuff;
    #endregion

    int lastSelectedTab = 1; // Start on Settings
    public static MenuScript theOne;

    // When the system is awake then the following game objects will be assigned and turned off
    void Awake()
    {
        theOne = this; // DKMOD: Making access easier for me.
        btnscript = GetComponent<ButtonScript>();
        canvasObj = GameObject.Find("MenuCanvas");
        canvasObj.SetActive(false);
        resizeX = GameObject.Find("ResizeXScale");
        resizeY = GameObject.Find("ResizeYScale");
        resizeZ = GameObject.Find("ResizeZScale");
        inventorySpot = GameObject.Find("InventoryItemSpot");
        inventoryOther = GameObject.Find("InventoryOtherItems");
        inventoryitemText = GameObject.Find("InventoryItemNumText");
        inventoryScreen = GameObject.Find("InventoryCamScreen");
        inventoryScreen.SetActive(false);
        inventoryLeft = GameObject.Find("InventoryCamLeft");
        inventoryRight = GameObject.Find("InventoryCamRight");
        objpointspawn = GameObject.Find("ObjPointerSpawner");
        leftHandy = GameObject.Find("LeftHand");
        rightHandy = GameObject.Find("RightHand");
        leftController = GameObject.Find("LeftController");
        rightController = GameObject.Find("RightController");
        teleporty = GameObject.Find("Teleporting");
        snapTurn = GameObject.Find("Snap Turn");
        laserMenu = GameObject.Find("Laser");
        fileMenu = GameObject.Find("FileCanvas");
        fileMenu.SetActive(false);
        righthand = GameObject.Find("RightHandMenu");
        lefthand = GameObject.Find("LeftHandMenu");
        audioclick = GameObject.Find("ButtonClickAudio");
        floor1 = GameObject.Find("Floor1");
        floor2 = GameObject.Find("Floor2");
        floor3 = GameObject.Find("Floor3");
        floor4 = GameObject.Find("Floor4");
        floor5 = GameObject.Find("Floor5");
        teleportfloor = GameObject.Find("Teleporting Area");
        theplayer = GameObject.Find("Player3");
        assetholder = GameObject.Find("AssetHolder");
        headspawn = GameObject.Find("HeadSpawner");
        myhead = GameObject.Find("VRCamera"); 
        groupedinventory = GameObject.Find("GroupedQuadInventory");
        inventoryholderquad = GameObject.Find("InventoryHolder");
        inventorylocation = GameObject.Find("GroupedLocation");
        videoleft = GameObject.Find("VideoLaserLeft");
        videoright = GameObject.Find("VideoLaserRight");
    }

    void Start()
    {

    }

    // The fixed update checks every so interval for the menu, and resize canvas
    void FixedUpdate()
    {
        CanvasOnOff();
    }

    void Update()
    {
        Mplayerloc = myhead.transform.position;
    }

    #region Menu

    // Close the menu, called from hand's MenuButtonScript and potentially from menu items.
    public void closeMenu()
    {
        menuclicked = false;
        // Also to make sure to close the File browser canvas and inventory camera screen when the menu is closed
        fileMenu.SetActive(false);
        inventoryScreen.SetActive(false);
    }

    // [feature] To switch the menu canvas between either the left  hands. This will make the left 
    // the parent of the canvas. 
    public void LeftMenu()
    {
        audioclick.GetComponent<AudioSource>().PlayOneShot(audioclickclip, audioclipvol);
        canvasObj.transform.position = lefthand.transform.position;
        canvasObj.transform.rotation = lefthand.transform.rotation;
        fileMenu.transform.position = lefthand.transform.position - new Vector3(0, 0, -0.001f);
        fileMenu.transform.rotation = lefthand.transform.rotation;
        inventoryScreen.transform.position = inventoryLeft.transform.position;
        inventoryScreen.transform.rotation = lefthand.transform.rotation;
        objpointspawn.transform.position = lefthand.transform.position;
        objpointspawn.transform.rotation = lefthand.transform.rotation;
        objpointspawn.transform.SetParent(lefthand.transform);
        canvasObj.transform.SetParent(lefthand.transform);
        fileMenu.transform.SetParent(lefthand.transform);
        inventoryScreen.transform.SetParent(lefthand.transform);
    }

    // [feature] To switch the menu canvas between either the right hands. This will make the right hands
    // the parent of the canvas. 
    public void RightMenu()
    {
        audioclick.GetComponent<AudioSource>().PlayOneShot(audioclickclip, audioclipvol);
        canvasObj.transform.position = righthand.transform.position;
        canvasObj.transform.rotation = righthand.transform.rotation;
        fileMenu.transform.position = righthand.transform.position - new Vector3(0, 0, -0.001f);
        fileMenu.transform.rotation = righthand.transform.rotation;
        inventoryScreen.transform.position = inventoryRight.transform.position;
        inventoryScreen.transform.rotation = righthand.transform.rotation;
        objpointspawn.transform.position = righthand.transform.position;
        objpointspawn.transform.rotation = righthand.transform.rotation;
        objpointspawn.transform.SetParent(righthand.transform);
        canvasObj.transform.SetParent(righthand.transform);
        fileMenu.transform.SetParent(righthand.transform);
        inventoryScreen.transform.SetParent(righthand.transform);
    }

    public void menuGenericOpenAt(GameObject go) {
        audioclick.GetComponent<AudioSource>().PlayOneShot(audioclickclip, audioclipvol);
        canvasObj.transform.position = go.transform.position;
        canvasObj.transform.rotation = go.transform.rotation;
        fileMenu.transform.position = go.transform.position - new Vector3(0, 0, -0.001f);
        fileMenu.transform.rotation = go.transform.rotation;
        inventoryScreen.transform.position = inventoryRight.transform.position;
        inventoryScreen.transform.rotation = go.transform.rotation;
        objpointspawn.transform.position = go.transform.position;
        objpointspawn.transform.rotation = go.transform.rotation;
        objpointspawn.transform.SetParent(go.transform);
        canvasObj.transform.SetParent(go.transform);
        fileMenu.transform.SetParent(go.transform);
        inventoryScreen.transform.SetParent(go.transform);
    }
    public void menuChangeParentTo(GameObject go) {
        objpointspawn.transform.SetParent(go.transform);
        canvasObj.transform.SetParent(go.transform);
        fileMenu.transform.SetParent(go.transform);
        inventoryScreen.transform.SetParent(go.transform);
    }


    // [feature] Turns the menu canvas on or off (enable or disabled)
    // It also checks to make sure any other canvas's aren't active atm and disables the distance control script to avoid clipping
    public void CanvasOnOff()
    {
        if (menuclicked)
        {
            laserMenu.GetComponent<LineRenderer>().enabled = true;
            canvasObj.SetActive(true);
        }
        else
        {
            laserMenu.GetComponent<LineRenderer>().enabled = false;
            canvasObj.SetActive(false);
        }
    }

    #endregion

    #region Hints

    // [feature] Function that creates/utilizes the SteamVR controller hints for the right hand. 
    // Each controller mode will have its own hints
    public void RightHints(int i)
    {
        switch (i)
        {
            case 1:
            rightControllerTrigger.text = "Left Click";
            rightControllerGrip.text = " ";
            rightControllerTrack.text = "Browser Mode: Scroll or Right Click";
            break;

            case 2:
            rightControllerTrack.text = "Resize mode";
            rightControllerGrip.text = " ";
            rightControllerTrigger.text = "Select Object";
            break;

            case 3:
            rightControllerTrigger.text = "Select Object";
            rightControllerGrip.text = " ";
            rightControllerTrack.text = "Distance Grabber mode: Spin, Pull or Push Object";
            break;

            case 4:
            rightControllerTrack.text = "Sticky mode: Select a parent";
            rightControllerGrip.text = " ";
            rightControllerTrigger.text = "Sticky Object";
            break;

            case 5:
            rightControllerTrack.text = "Inventory mode";
            rightControllerGrip.text = " ";
            rightControllerTrigger.text ="Inventory is full";
            break;
            
            default:
            break;
        }
    }

    // [feature] Function that creates/utilizes the SteamVR controller hints for the left hand. 
    // Each controller mode will have its own hints
    public void LeftHints(int i)
    {
        switch (i)
        {
            case 1:
            leftControllerTrigger.text = "Left Click";
            leftControllerGrip.text = " ";
            leftControllerTrack.text = "Browser Mode: Scroll or Right Click";
            break;

            case 2:
            leftControllerTrack.text = "Resize mode";
            leftControllerGrip.text = " ";
            leftControllerTrigger.text = "Select Object";
            break;

            case 3:
            leftControllerTrigger.text = "Select Object";
            leftControllerGrip.text = " ";
            leftControllerTrack.text = "Distance Grabber mode: Spin, Pull or Push Object";
            break;

            case 4:
            leftControllerTrack.text = "Sticky mode: Select a parent";
            leftControllerGrip.text = " ";
            leftControllerTrigger.text = "Sticky Object";
            break;

            case 5:
            leftControllerTrack.text = "Inventory mode";
            leftControllerGrip.text = " ";
            leftControllerTrigger.text ="Inventory is full";
            break;
            
            default:
            break;
        }
    }
    #endregion

    #region CreateQuickLinkBrowsers

    // [feature] Function to spawn a new browser into the scene by getting the url address
    // Also uses the HeadSpawnChange() function from the HeadSpawnScript.cs 
    // It will then spawn the gameobject in front of the player, force it to look at the players VR camera head
    // Then rotate the object 180 degrees so the player can see the browser
    // Then it will add the browser into the allBrowsers static list 
    public void CreateQuickLink(string i){
        CreateQuickLink(i, Vector3.zero);
    }
    public void CreateQuickLink(string i, Vector3 positionOffset)
    {
        audioclick.GetComponent<AudioSource>().PlayOneShot(audioclickclip, audioclipvol);
        Vector3 gethead = headspawn.GetComponent<HeadSpawnScript>().HeadSpawnChange();
        headspawn.transform.LookAt(myhead.transform);
        headspawn.transform.rotation *= Quaternion.Euler(0, 180, 0);
        float rotY = headspawn.transform.eulerAngles.y;
        float rotZ = headspawn.transform.eulerAngles.z;
        Quaternion rot =  Quaternion.Euler(0, rotY, rotZ);
        string vidurl = System.IO.Path.GetExtension(i);
        bool isvideo = false;
        isvideo = VideoExtensionCheck(vidurl);
        GameObject temp, temptemp;
        if (isvideo)
        {
            temp = Instantiate(videobrowsertemplate, gethead, rot);
            GameObject temptemptemp = temp.gameObject.transform.GetChild(0).GetChild(0).gameObject;
            // This might be redundant since we're already loading the video with the url but it should help prevent future
            // errors such as pulling up files from the state saver
            temptemptemp.GetComponent<VRVideoPlayer>().videoUrl = i;
            temptemptemp.GetComponent<VRVideoPlayer>().Load(i, true);
            temptemp = temp.gameObject.transform.GetChild(0).gameObject;
            temptemp.GetComponent<Browser>()._url = i;
            temp.GetComponent<VRBrowserPanel>().AssignURLVideo(i);
        }
        else
        {
            temp = Instantiate(browsertemplate, gethead, rot);
            temptemp = temp.gameObject.transform.GetChild(0).gameObject;
            temptemp.GetComponent<Browser>()._url = i;
        }
        temp.transform.Translate(positionOffset);
        VRBrowserPanel tab = temp.GetComponent<VRBrowserPanel>();
        VRMainControlPanel.allBrowsers.Add(tab);
    }
    
    // For Sage2 stuff
    public void CreateQuickLinkAtLocRotScale(string i, Vector3 pos, Quaternion rot, Vector3 scale)
    {

        GameObject wholeBrowser = Instantiate(browsertemplate, pos, rot);
        VRBrowserPanel tab = wholeBrowser.GetComponent<VRBrowserPanel>();
        VRMainControlPanel.allBrowsers.Add(tab);
        GameObject contentObject = wholeBrowser.gameObject.transform.GetChild(0).gameObject; // Depends on structure order
        contentObject.GetComponent<Browser>()._url = i;
        contentObject.transform.localScale = scale;
    }
    public void CreatePele()
    {
        CreateQuickLink("http://pele.manoa.hawaii.edu/");
        CreateQuickLink("http://pele.manoa.hawaii.edu/display.html?clientID=-1");
    }

    public void CreateMakani()
    {
        CreateQuickLink("http://makani.manoa.hawaii.edu/");
        CreateQuickLink("http://makani.manoa.hawaii.edu/display.html?clientID=-1");
    }

    public void CreateCyber()
    {
        CreateQuickLink("http://cyber.manoa.hawaii.edu/");
        CreateQuickLink("http://cyber.manoa.hawaii.edu/display.html?clientID=-1");
    }

    public void CreateMokulua()
    {
        CreateQuickLink("http://mokulua.manoa.hawaii.edu/");
        CreateQuickLink("http://mokulua.manoa.hawaii.edu/display.html?clientID=-1", new Vector3(1, 0, 0));
    }

    public void CreateDrive()
    {
        //CreateQuickLink("https://www.google.com/drive/");
        CreateQuickLink("game.local/omniviewer/index.html?filepath=new.note");
    }

        public void CreateGoogle()
    {
        CreateQuickLink("https://www.google.com/");
    }

    public void CreateYouTube()
    {
        CreateQuickLink("https://www.youtube.com/");
    }

    public void CreateDropbox()
    {
        CreateQuickLink("https://www.dropbox.com/?_hp=c");
    }

    public void CreateLavalava()
    {
        CreateQuickLink("https://whereby.com/lavalava");
    }

    public void CreateHotlava()
    {
        CreateQuickLink("https://whereby.com/hotlava");
    }

        public void CreateGE()
    {
        CreateQuickLink("https://www.google.com/earth/");
    }

    public void CreateGM()
    {
        CreateQuickLink("https://www.google.com/maps");
    }

    public void CreateGraph()
    {
        // CreateQuickLink("https://www.monroecc.edu/faculty/paulseeburger/calcnsf/CalcPlot3D/");
        CreateQuickLink("https://sketch.io/sketchpad/");
    }

    public void CreateData()
    {
        // CreateQuickLink("https://www.mathworks.com/solutions/image-video-processing/3d-image-processing.html");
        CreateQuickLink("https://chowderman.github.io/xp-paint.html");
    }

    public void CreatePDF()
    {
        CreateQuickLink("https://isotropic.org/papers/chicken.pdf");
    }

    public void StateSave()
    {
        StateSaverLoader.theOne.saveInstanceToFile();
        closeMenu();
    }

    public void StateLoad()
    {
        // Load should now open to file browser
        TabSelection(3);
        // Just in case tell it to navigate to the state folder
        FileBrowser fb = fileMenu.GetComponent<FileBrowser>();
        fb.SetBrowserWindow(null, fb.iniPathHelper + "StateSaves");
    }
    #endregion

    #region MenuTabs

    // Got really annoyed that the menu didn't open where I last was.
    // Was getting in the way of testing. Also normal usage of new items and inventory.
    public void OpenLastMenuTab()
    {
        TabSelection(lastSelectedTab);
    }
    
    // [feature] This function controls the tabs (left side) of the menu canvas. It mostly focuses on the file browser
    // canvas and will only show the file canvas when the correct tab is selected otherwise it's deactivated 
    public void TabSelection(int i)
    {
        lastSelectedTab = i;
        switch (i)
        {
            case 1: TabReset(); break;

            case 2: TabReset(); break;

            case 3:
            audioclick.GetComponent<AudioSource>().PlayOneShot(audioclickclip, audioclipvol);
            fileMenu.SetActive(true);
            inventoryScreen.SetActive(false);
            break;

            case 4:
            audioclick.GetComponent<AudioSource>().PlayOneShot(audioclickclip, audioclipvol);
            fileMenu.SetActive(false);
            inventoryScreen.SetActive(true);
            break; 
            
            case 5: TabReset(); break; 

            default: lastSelectedTab = 1; break;
        }
    }

    // [feature] Function that resets the tab by setting the filemenu and inventoryscreen object to false/inactive
    private void TabReset()
    {
        audioclick.GetComponent<AudioSource>().PlayOneShot(audioclickclip, audioclipvol);
        fileMenu.SetActive(false);
        inventoryScreen.SetActive(false);
    }

    #endregion

    #region MenuFiles

    // [feature] This function is for the button select in the file canvas, it connects with the file canvas to get
    // the return file path string to be used for the url. The function then creates a new browser template
    // and then enters in the path file string into the url. First the function will check if the following
    // boolean is true (if it's file that can be selected) then it'll open the file in a browser.
    public void FilePathOpen()
    {
        if (fileMenu.GetComponent<FileBrowser>().boolselected == true)
        {
            string stringtemp = fileMenu.GetComponent<FileBrowser>().finalreturnpath;

            if (stringtemp.LastIndexOf(".") > 0)
            { // If there is an extension, see if it might be a statesavefile.
                string extension = stringtemp.Substring(stringtemp.LastIndexOf("."));
                if (extension == StateSaverLoader.fileDefaultSuffix)
                { // If it is a state save
                    Debug.Log("Attempting to state load from " + stringtemp);
                    StateSaverLoader.theOne.readFromFileAndLoad(stringtemp);
                    // Close menu to prevent multiple clicks?
                    closeMenu();
                    // return now, don't open a browser
                    return;
                }
            }
            Vector3 gethead = headspawn.GetComponent<HeadSpawnScript>().HeadSpawnChange();
            headspawn.transform.LookAt(myhead.transform);
            headspawn.transform.rotation *= Quaternion.Euler(0, 180, 0);
            float rotY = headspawn.transform.eulerAngles.y;
            float rotZ = headspawn.transform.eulerAngles.z;
            Quaternion rot =  Quaternion.Euler(0, rotY, rotZ);
            string vidurl = System.IO.Path.GetExtension(stringtemp);
            GameObject temp, temptemp;
            bool isvideo = false;
            isvideo = VideoExtensionCheck(vidurl);
            // Debug.Log("isvideo: " + isvideo);

            // Modification of url will need relative references
            // Debug.Log("erase me, application.datapath:" + Application.dataPath);
            // Debug.Log("erase me, adjusted:" + determinePathMod(stringtemp));
            
            // Checks to see if the file contains a video extension if so the video browser is pulled, otherwise the omniviewer is pulled.
            if (isvideo)
            {
                temp = Instantiate(videobrowsertemplate, gethead, rot);
                VRBrowserPanel tab = temp.GetComponent<VRBrowserPanel>();
                GameObject temptemptemp = temp.gameObject.transform.GetChild(0).GetChild(0).gameObject;
                temptemp = temp.gameObject.transform.GetChild(0).gameObject;
                // This might be redundant since we're already loading the video with the url but it should help prevent future
                // errors such as pulling up files from the state saver
                temptemptemp.GetComponent<VRVideoPlayer>().videoUrl = stringtemp;
                temptemptemp.GetComponent<VRVideoPlayer>().Load(stringtemp, true);
                temptemp.GetComponent<Browser>()._url = stringtemp;
                temp.GetComponent<VRBrowserPanel>().AssignURLVideo(stringtemp);
                VRMainControlPanel.allBrowsers.Add(tab);
            }
            else
            {
                temp = Instantiate(browsertemplate, gethead, rot);
                VRBrowserPanel tab = temp.GetComponent<VRBrowserPanel>();
                temptemp = temp.gameObject.transform.GetChild(0).gameObject;
                string filepath = determinePathMod(stringtemp);
                if (filepath != null)
                {
                    filepath = "game.local/omniviewer/index.html?filepath=" + filepath;
                }
                else
                {
                    filepath = stringtemp;
                }
                temptemp.GetComponent<Browser>()._url = filepath;
                VRMainControlPanel.allBrowsers.Add(tab);
            }
        }
    }

    // [feature] This function is a path helper for the omniviewer.
    // The goal is get the appropriate relative path so the omniviewer can get the correct location.
    public string determinePathMod(string pathOfFileToView)
    {
        // Copy file into a temp location, need to know name of file
        string[] filePathParts = pathOfFileToView.Split('/');
        string fileName = filePathParts[filePathParts.Length - 1];

        // Do this by getting asset folder location, then modify into assets
        string assetFolder = Application.dataPath;
        assetFolder = assetFolder.Substring(0, assetFolder.LastIndexOf("/"));
        assetFolder += "/BrowserAssets/tmp/" + fileName;


        // File.Copy(pathOfFileToView, assetFolder);


        // Fails? No clear indication why.
        // EDIT: fails if ANY part of the path doesn't exist. Meaning the destination folder MUST exist.

        try
        {
            FileUtil.CopyFileOrDirectory(pathOfFileToView, assetFolder);
        } catch(System.Exception e)
        {
            // Delete first then copy
            FileUtil.DeleteFileOrDirectory(assetFolder);
            FileUtil.CopyFileOrDirectory(pathOfFileToView, assetFolder);
        }


        return "../tmp/" + fileName;



        // The following is an old version that proves the server sanitizes the path
        // Meaning: no traveling up the folder structure beyond initial host folder





        //string[] filePathParts = pathOfFileToView.Split('/');

        //// Get path parts of the assets folder
        //string assetFolder = Application.dataPath;
        //// But that needs adjustment to point to the omniviewer
        //assetFolder = assetFolder.Substring(0, assetFolder.LastIndexOf("/"));
        //assetFolder += "/BrowserAssets/omniviewer";
        //Debug.Log("erase me, sanity check on modified path to omniviewer");
        //Debug.Log(pathOfFileToView);
        //Debug.Log(assetFolder);
        //string[] assetPathParts = assetFolder.Split('/');

        //int partMatch = 0;
        //// For each of the file's part paths, see how much asset parts match
        //for (partMatch = 0; partMatch < filePathParts.Length;)
        //{
        //    // Check if there is a part match
        //    if (filePathParts[partMatch] == assetPathParts[partMatch])
        //    {
        //        partMatch++;
        //        // If partMach is greater or equal, then there is a full path match upto the assetFolder.
        //        if (partMatch >= assetPathParts.Length)
        //        {
        //            break;
        //        }
        //    } else
        //    {
        //        break;
        //    }
        //}

        //// Example: paths:
        //// C:\Users\LAVA\Downloads\a.png
        //// C:\Users\LAVA\Documents\Dylan\unityProjects\SAGEXR\BrowserAssets\omniviewer
        //// This should put partMatch at, 3

        //Debug.Log("erase me, what is partMatch?" + partMatch);
        //string modifiedPath = "";
        //if (partMatch == 0)
        //{
        //    // It is on a different drive, we can't navigate that yet
        //    return null;
        //}
        //// If less, then need to traverse up an amount equal to the difference
        //else if (partMatch < assetPathParts.Length)
        //{
        //    modifiedPath = "..";
        //    int i;
        //    for (i = 1; partMatch + i < assetPathParts.Length; i++)
        //    {
        //        modifiedPath += "/..";
        //    }
        //    // Finally use the rest of the original file path
        //    for (i = partMatch; i < filePathParts.Length; i++)
        //    {
        //        modifiedPath += "/" + filePathParts[i];
        //    }

        //} else
        //{
        //    // If partMatch is equal, then need to get rid of those prefixes and use the reference from end of asset path
        //    modifiedPath = "."; // will add / during loop
        //    for (int i = partMatch; i < filePathParts.Length; i++)
        //    {
        //        modifiedPath += "/" + filePathParts[i];
        //    }
        //}
        //return modifiedPath;
    }
    
    // [feature] Function to reopen the file browser canvas for the "Clicke Here" button. This is so that the 
    // file browser canvas closes properly but the user can reopen the file canvas without pressing on a new
    // tab every time they reopen the menu canvas. 
    public void FileMenuReopen()
    {
        audioclick.GetComponent<AudioSource>().PlayOneShot(audioclickclip, audioclipvol);
        fileMenu.SetActive(true);
    }

    #endregion

    #region ElevatorFloor

    // [feature] Function to change floors in the Elevator tab
    // This is so that the player knows what floor they're on and what floor they want to get to
    // It gets the string variable from the gameobject floorobj ("FloorPlan") and parses into an int
    // which is then placed into a switch statement to teleport the player to the selected floor, also calls the FadeOut() and FadeIn()
    public void ChangeFloors()
    {
        audioclick.GetComponent<AudioSource>().PlayOneShot(audioclickclip, audioclipvol);
        string i = floorobj.GetComponent<OptionsManager>().selectedValue;
        //Parse the string into an int to get the current floor number
        int ii = int.Parse(i);
        if (floorcurrent.GetComponent<Text>())
        {
            floorcurrent.GetComponent<Text>().text = i;
        }
        // Switch statement between floors 1-5
        switch (ii)
        {
            case 1: FloorFadeIn(floor1.transform.position); break;

            case 2: FloorFadeIn(floor2.transform.position); break;

            case 3: FloorFadeIn(floor3.transform.position); break;

            case 4: FloorFadeIn(floor4.transform.position); break;

            case 5: FloorFadeIn(floor5.transform.position); break;

            default: break;
        }
    }

    // [feature] Function that Fades in the player screen and moves the player to the starting position
    private void FloorFadeIn(Vector3 i)
    {
        FadeOut();
        Invoke("FadeIn", 1.0f);
        teleportfloor.transform.position = i;
        theplayer.transform.position = i;
    }

    // [feature] Function to call the SteamVR Fade script to fade out the players camera, camera goes from clear to dark
    private void FadeOut()
    {
        SteamVR_Fade.Start(Color.clear, 0.0f);
        SteamVR_Fade.Start(Color.black, 0.0f);
    }

    // [feature] Function to call the SteamVR Fade script to fade in the players camera, camera goes from dark to clear
    private void FadeIn()
    {
        SteamVR_Fade.Start(Color.black, 0.0f);
        SteamVR_Fade.Start(Color.clear, 1.0f);
    }

    // These two features are for the BackStepControlScript.cs
    private void FadeInBackStep()
    {
        SteamVR_Fade.Start(Color.black, 0.0f);
        SteamVR_Fade.Start(Color.clear, 0.1f);
    }

    public void OtherFade()
    {
        FadeOut();
        Invoke("FadeInBackStep", 0.1f);
    }

    #endregion 

    #region Inventory

    // [feature] Function that will either close the inventory canvas or save the inventory into an empty inventory slot
    // By doing so, a physical object turns kinematic to prvent any collisions and browsers are reloaded 
    // to prevent any videos that are still currently playing. Inventory objects then reset their position
    // and rotation and scale and set as the child of the inventory slot gameobject and turns the corresponding
    // button as interactive. 
    public void InventoryIt(GameObject inventoryObj)
    {
        // Need to check to see if inventoryObj contains a parent object, if so then remove the link between the child and parent
        if (inventoryObj.GetComponent<StickyScript>().hasparent)
        {
            inventoryObj.GetComponent<StickyScript>().destroyit = false;
            inventoryObj.GetComponent<StickyScript>().RemoveParent();
        }
        // Find if it has children if so then its a grouped item so replace object with canvas in the inventory screen
        if (inventoryObj.GetComponent<StickyScript>().isnotempty)
        {
            // If the inventory has children then perform grouped item protocol
            groupedinventorystuff = inventoryObj.GetComponentsInChildren<StickyScript>();
            foreach (StickyScript item in groupedinventorystuff)
            {
                item.inventorychecked = true;
                // if the item matches the first object in the array then the iteration or search continues
                // This is because Unity's GetComponentsInChildren adds in the parent object as the 1st iteration for some reason
                if (item == groupedinventorystuff[0])
                {
                    continue;
                }
                // If one of the item gameobject matches the object parent (2nd choice) then isachild is true breaking the foreach loop
                if (item.gameObject.GetComponentInChildren<Browser>())
                {
                    item.gameObject.GetComponentInChildren<Browser>().Reload(false);
                }
                if (item.gameObject.GetComponentInChildren<VideoPlayerCtrl>())
                {
                    item.gameObject.GetComponentInChildren<VideoPlayerCtrl>().PauseVideo();
                }
            }
        }
        else
        {
            // Check to see if the inventory object contains the Browser script
            if (inventoryObj.GetComponentInChildren<Browser>())
            {
                inventoryObj.GetComponentInChildren<Browser>().Reload(false);
            }
            if (inventoryObj.gameObject.GetComponentInChildren<VideoPlayerCtrl>())
            {
                inventoryObj.gameObject.GetComponentInChildren<VideoPlayerCtrl>().PauseVideo();
            }
            inventoryObj.GetComponent<StickyScript>().inventorychecked = true;
        }
        inventoryObj.transform.localScale = inventoryOther.transform.localScale;
        inventoryObj.transform.position = inventoryOther.transform.position;
        inventoryObj.transform.rotation = inventoryOther.transform.rotation;
        inventoryObj.transform.SetParent(inventoryOther.transform);
        inventoryArray[inventoryholder] =  inventoryObj;
        btnscript.inventoryButArray[inventoryholder].interactable = true;
        inventoryObj = null;
    }

    // [feature] Function that links the inventory gameobject array to the button array, also double checks to reset it's position
    // and rotation. The function will then place the corresponding inventory item to the preview screen canvas
    // and change the text string to the corresponding inventory number 
    public void InventoryLink(int i)
    {
        if (inventoryArray[i] != null)
        {
            // Reset the grouped inventory canvas to the other spot location when selecting another inventory item
            groupedinventory.transform.position = inventoryOther.transform.position;
            groupedinventory.transform.rotation = inventoryOther.transform.rotation;
            // If the inventory being held is full then swap it out,  move it to saved location
            // In other words remove the previous selected item
            if (inventorytemp != null)
            {
                // Move the inventory to the other spot location and save its rotation
                inventorytemp.transform.position = inventoryOther.transform.position;
                inventorytemp.transform.rotation = inventoryOther.transform.rotation;
            }
            // The new inventory item will now take the selected inventory spot
            inventoryselected = i;
            inventorytemp = inventoryArray[i];
            var x = i + 1;
            inventoryitemText.GetComponent<Text>().text = x.ToString();
            // Check to see if the inventory object has children if so then place the grouped item canvas at the inventory location
            if (inventoryArray[i].GetComponent<StickyScript>().isnotempty)
            {
                groupedinventory.transform.position = inventorylocation.transform.position;
                groupedinventory.transform.rotation = inventorylocation.transform.rotation;
            }
            else
            {
                // If the inventory object doesn't have any children then: 
                // If true then the object gets placed into the screen/canvas
                inventoryArray[i].transform.position = inventorySpot.transform.position;
            }
        }
    }

    // [feature] Function that loads the selected inventory slot to be spawned (teleported) in front of the player
    // This is done by changing the parent of the inventory to another gameobject in the scene. 
    // Then removing the kinematic features of any physical object to make it able to collide/interact again.
    // The corresponding inventory button is grayed out and the number removed from the preview canvas screen. 
    public void LoadInventory()
    {
        audioclick.GetComponent<AudioSource>().PlayOneShot(audioclickclip, audioclipvol);
        Vector3 gethead = headspawn.GetComponent<HeadSpawnScript>().HeadSpawnChange();
        headspawn.transform.LookAt(myhead.transform);
        headspawn.transform.rotation *= Quaternion.Euler(0, 180, 0); 
        float rotY = headspawn.transform.eulerAngles.y;
        float rotZ = headspawn.transform.eulerAngles.z;
        inventoryArray[inventoryselected].transform.rotation = Quaternion.Euler(0, rotY, rotZ);
        inventoryArray[inventoryselected].transform.position = gethead;
        inventoryArray[inventoryselected].transform.SetParent(assetholder.transform);
        btnscript.inventoryButArray[inventoryselected].interactable = false;
        // Find if it has children if so then its a grouped item so replace object with canvas in the inventory screen
        if (inventoryArray[inventoryselected].GetComponent<StickyScript>().isnotempty)
        {
            // If the inventory has children then perform grouped item protocol
            groupedinventorystuff = inventoryArray[inventoryselected].GetComponentsInChildren<StickyScript>();
            foreach (StickyScript item in groupedinventorystuff)
            {
                item.inventorychecked = false;
            }
        }
        else
        {
            inventoryArray[inventoryselected].GetComponent<StickyScript>().inventorychecked = false;
        }
        inventoryitemText.GetComponent<Text>().text = "";
        inventoryArray[inventoryselected] = null;
        inventorytemp = null;
        groupedinventory.transform.position = inventoryOther.transform.position;
        groupedinventory.transform.rotation = inventoryOther.transform.rotation;

    }

    #endregion

    //[feature] to keep track of the currents players height (camera) so that the browsers can spawn and the correct height
    public float PlayerCurrentHeight()
    {
        return Mplayerloc.y;
    }

    //[feature] to check the url string extension for any related video file extension for the file browser
    // https://docs.unity3d.com/Manual/VideoSources-FileCompatibility.html
    private bool VideoExtensionCheck(string vidstring)
    {
        bool result = false;
        //Some of the files are in uppercase such as: .MP4 which might cause some errors
        vidstring = vidstring.ToLower();
        // Debug.Log("vidstring: " + vidstring);

        switch (vidstring)
        {
            case ".mp4": result = true; break;
            case ".mpeg": result = true; break;
            case ".avi": result = true; break;
            case ".m4v": result = true; break;
            case ".mov": result = true; break;
            case ".mpg": result = true; break;
            case ".wmv": result = true; break;
            case ".asf": result = true; break;
            case ".dv": result = true; break;
            case ".webm": result = true; break;
            case ".ogv": result = true; break;
            case ".vp8": result = true; break;
            case ".flv": result = true; break;
            default: break;
        }
        return result;
    }
}
