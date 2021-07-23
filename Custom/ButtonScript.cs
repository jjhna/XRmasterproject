using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Created by: Jonathan Na from the University of Hawaii at Manoa, Laboratory for Advanced Visualization & Application (LAVA)
// This is the button script that ties in the OnClick() functions to the buttons when the new player is created
// This is all done to ensure that when loading or creating a new player the OnClick() functions are available
public class ButtonScript : MonoBehaviour
{
    public Button buttonpele, buttonmakani, buttoncyber, buttonmokulua, buttondrive, buttongoogle, buttonfilereopen,
    buttonyoutube, buttondropbox, buttonlavalava, buttonhotlava, buttonGE, buttonGM, buttonselect, 
    buttongraph, buttondata, buttonpdf, buttonStateSave, buttonStateLoad, tabsettings, tabquicklink, tabfiles, 
    tabinventory, tabsave, buttonfloor, buttonloadinventory;
    public GameObject controlcenter;
    public MenuScript menuCS;
    public int inventorySize = 10;
    public Button[] inventoryButArray = new Button[10];

    // When the system is awake then the following game objects will be assigned and turned off
    // First assign the GameObject buttons by drag and drop from the Inspector screen
    // Then add a listener for events so that we assign a function to the OnClick() functions
    void Awake()
    {
        controlcenter = GameObject.Find("AllHandControls");
        menuCS = controlcenter.GetComponent<MenuScript>();
        buttonpele.onClick.AddListener(ButtonPele);
        buttonmakani.onClick.AddListener(ButtonMakani);
        buttoncyber.onClick.AddListener(ButtonCyber);
        buttonmokulua.onClick.AddListener(ButtonMokulua);
        buttondrive.onClick.AddListener(ButtonDrive);
        buttongoogle.onClick.AddListener(ButtonGoogle);
        buttonyoutube.onClick.AddListener(ButtonYouTube);
        buttondropbox.onClick.AddListener(ButtonDropbox);
        buttonlavalava.onClick.AddListener(ButtonLavalava);
        buttonhotlava.onClick.AddListener(ButtonHotlava);
        buttonGE.onClick.AddListener(ButtonGE);
        buttonGM.onClick.AddListener(ButtonGM);
        buttongraph.onClick.AddListener(ButtonGraph);
        buttondata.onClick.AddListener(ButtonData);
        buttonpdf.onClick.AddListener(ButtonPDF);
        buttonStateSave.onClick.AddListener(ButtonStateSave);
        buttonStateLoad.onClick.AddListener(ButtonStateLoad);
        tabsettings.onClick.AddListener(TabSettings);
        tabquicklink.onClick.AddListener(TabQuickLink);
        tabfiles.onClick.AddListener(TabFiles);
        tabinventory.onClick.AddListener(TabInventory);
        tabsave.onClick.AddListener(TabSave);
        buttonselect.onClick.AddListener(ButtonSelect);
        buttonfilereopen.onClick.AddListener(ButtonFileReopen);
        buttonfloor.onClick.AddListener(ButtonFloorChange);
        buttonloadinventory.onClick.AddListener(ButtonLoadInventory);
        for (int i = 0; i < inventorySize; i++)
        {
            inventoryButArray[i].interactable = false;
            InventoryLinkAll(inventoryButArray[i], i);
        }
    }

    // [feature] Add a function to the corresponding MenuScript.cs function so that it can be used by the AddListener() function
    void ButtonPele()
    {
        menuCS.CreatePele();
    }

    void ButtonMakani()
    {
        menuCS.CreateMakani();
    }

    void ButtonCyber()
    {
        menuCS.CreateCyber();
    }

    void ButtonMokulua()
    {
        menuCS.CreateMokulua();
    }

    void ButtonDrive()
    {
        menuCS.CreateDrive();
    }

    void ButtonGoogle()
    {
        menuCS.CreateGoogle();
    }

    void ButtonYouTube()
    {
        menuCS.CreateYouTube();
    }

    void ButtonDropbox()
    {
        menuCS.CreateDropbox();
    }

    void ButtonLavalava()
    {
        menuCS.CreateLavalava();
    }

    void ButtonHotlava()
    {
        menuCS.CreateHotlava();
    }

    void ButtonGE()
    {
        menuCS.CreateGE();
    }

    void ButtonGM()
    {
        menuCS.CreateGM();
    }

    void ButtonGraph()
    {
        menuCS.CreateGraph();
    }

    void ButtonData()
    {
        menuCS.CreateData();
    }

    void ButtonPDF()
    {
        menuCS.CreatePDF();
    }
    void ButtonStateSave()
    {
        menuCS.StateSave();
    }
    void ButtonStateLoad()
    {
        menuCS.StateLoad();
    }

    public void OpenLastMenuTab()
    {
        menuCS.OpenLastMenuTab();
    }
    void TabSettings()
    {
        menuCS.TabSelection(1);
    }

    void TabQuickLink()
    {
        menuCS.TabSelection(2);
    }
    
    void TabFiles()
    {
        menuCS.TabSelection(3);
    }

    void ButtonSelect()
    {
        menuCS.FilePathOpen();
    }

    void ButtonFileReopen()
    {
        menuCS.FileMenuReopen();
    }

    void ButtonFloorChange()
    {
        menuCS.ChangeFloors();
    }

    void TabInventory()
    {
        menuCS.TabSelection(4);
    }
    
    void TabSave()
    {
        menuCS.TabSelection(5);
    }
    void ButtonLoadInventory()
    {
        menuCS.LoadInventory();
    }

    void InventoryLinkAll(Button butt, int i)
    {
        butt.onClick.AddListener( () => 
        {
            menuCS.InventoryLink(i); 
        } );
    }
}
