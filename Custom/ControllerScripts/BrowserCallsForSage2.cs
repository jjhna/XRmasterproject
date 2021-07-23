using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using ZenFulcrum.EmbeddedBrowser;

public class BrowserCallsForSage2 : MonoBehaviour
{


    // ---------------------------------------------------------------------------
    // ---------------------------------------------------------------------------
    // ---------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        // // After awake can access the browser
        Browser browser = GetComponent<VRBrowserPanel>().contentBrowser;


        // WTF? Can't register functions in the browser?
        // This didn't work when put in the VRBrowserPanel either
        // Is there something preventing it that isn't shown? Like only allowed on game.local files?


        // // SAGE2 currently attempts 3 function calls

        // // First call, create app: 7 params: id, x,y,w,h, wall w, wall h.
        // browser.RegisterFunction("sxrCreateApp",
        //     args => sxrCreateApp(args[0], (float) args[1], (float) args[2],
        //     (float) args[3], (float) args[4], (float) args[5], (float) args[6]));


        // browser.RegisterFunction("sxrCloseApp", args => sxrCloseApp(args[0]));


        // browser.RegisterFunction("sxrSetPositionAndSize",
        //     args => sxrSetPositionAndSize(args[0], (float) args[1], (float) args[2],
        //     (float) args[3], (float) args[4]));
        // Debug.Log("Created sxr functions for browser");
        // Debug.Log("content browser url?" + browser._url);







        // EDIT: Bigger wtf. once this activates, it causes problems
        // EDIT2: errors in functions break all further console messages
        // This function works and CAN be used to add browser monitoring.
        browser.onConsoleMessage += (message, source) =>
        {
            try {
            if (message.IndexOf("sxrCreateApp") != -1) {
                Debug.Log(message);
                string[] parts = message.Split('|');
                sxrCreateApp(parts[1], float.Parse(parts[2]), float.Parse(parts[3]),
                    float.Parse(parts[4]), float.Parse(parts[5]), float.Parse(parts[6]), float.Parse(parts[7]));
            }
            if (message.IndexOf("sxrCloseApp") != -1) {
                Debug.Log(message);
                sxrCloseApp(message.Substring(message.IndexOf("|") + 1));
            }
            } catch (System.Exception e) {}
        };


    }

    // Update is called once per frame
    void Update()
    {
    }


    // ---------------------------------------------------------------------------
    // ---------------------------------------------------------------------------
    // ---------------------------------------------------------------------------

    public static void sxrCreateApp(string id, float x, float y, float w, float h,
        float wallw, float wallh) {
        // Debug.Log("sxrCreateApp");

        // MenuScript.theOne.CreateQuickLink(
        //     "https://mokulua.manoa.hawaii.edu/appView.html?appId=" + id
        //     + "&pointerName=SXR&pointerColor=%27#E57200'", new Vector3 (0,0, -0.1f));


        
        // Altering a bit.
        // Find the first likely display it came from. (backwards array search just in case)
        // Then get that display's position, rotation, scale.
        // Start by matching app to display but slightly "forward" so it doesn't cover the display.

        // Assumptions
        string displayUrlPart = "mokulua.manoa.hawaii.edu/display.html"; // Only works with mokulua for now


        // Find likely source display
        List<VRBrowserPanel> allVrbp = VRBrowserPanel.allVrbPanels;
        VRBrowserPanel displayPanel = null;
        Vector3 displayPos = Vector3.zero;
        Quaternion displayRot = new Quaternion(0,0,0,0);
        Vector3 displayScale = Vector3.zero;
        bool found = false;
        for (int i = allVrbp.Count - 1; i > -1; i--) { // Works backwards
            if (allVrbp[i].contentBrowser._url.IndexOf(displayUrlPart) != -1) {
                try {
                    displayPanel = allVrbp[i];
                    displayRot = allVrbp[i].gameObject.transform.rotation;
                    displayScale = allVrbp[i].contentBrowser.gameObject.transform.localScale;
                    found = true;
                    break;
                } catch (System.Exception e) {}
            }
        }
        if (found) {
            // Pass app center
            displayPos = determinePositionOfAppGivenDisplay(displayPanel, displayScale,
                x + w/2, y + h/2, wallw, wallh);
                // x + w/2, y + h/2, wallw, wallh);
            displayScale = determineScaleOfAppGivenDisplay(displayPanel, displayScale,
                w, h, wallw, wallh);
            // displayScale = displayScale / 2;

            MenuScript.theOne.CreateQuickLinkAtLocRotScale(
            "https://mokulua.manoa.hawaii.edu/appView.html?appId=" + id
            + "&pointerName=SXR&pointerColor=%27#E57200'",
            displayPos, displayRot, displayScale);
        }
    }

    // ---------------------------------------------------------------------------
    
    public static Vector3 determinePositionOfAppGivenDisplay(VRBrowserPanel displayPanel, Vector3 displayScale,
                                    float appCenterx, float appCentery, float wallw, float wallh) {
        // Centers based on the display position, maybe base on content?
        Vector3 appPosToMakeAt = displayPanel.gameObject.transform.position;
        // Apply small amount of backwards to the position so they do not overlap
        // Backwards because the browser face is on its back
        appPosToMakeAt += displayPanel.gameObject.transform.forward * -0.03f; // This should translate into cm

        // BUT, centering app over display doesn't center it over its representation.
        // Need to offset from center

        // Position is now centered on browser.
        Browser contentBrowser = displayPanel.contentBrowser;
        Vector2 displayRes = contentBrowser.Size;
        float displayWtohRatio = displayRes.x / displayRes.y;
        Debug.Log("erase me, wall reports resolution as " + wallw + "," + wallh);
        float wallWtohRatio = wallw / wallh;

        Vector2 emulatedDisplayRes = Vector2.zero;
        Vector2 wallSizeOnDisplay = Vector2.zero;
        if (wallWtohRatio < displayWtohRatio) { // wall content is limited by display height
            wallSizeOnDisplay.y = wallh;
            wallSizeOnDisplay.x = wallh * wallWtohRatio; // mult heigt by WALL ratio to get wall max width that fits
            emulatedDisplayRes.y = wallh;
            emulatedDisplayRes.x = wallh * displayWtohRatio;
        } else { // else wall must be limited by display width
            wallSizeOnDisplay.x = wallw;
            wallSizeOnDisplay.y = displayRes.x / wallWtohRatio;
            emulatedDisplayRes.x = wallw;
            emulatedDisplayRes.y = wallw / displayWtohRatio;
        }
        Vector2 wallCenterFromEmulatedDisplayCenter = new Vector2(
            (wallSizeOnDisplay.x / 2) - (emulatedDisplayRes.x / 2),
            (wallSizeOnDisplay.y / 2) - (emulatedDisplayRes.y / 2));


        // Goal given a resolution determine gameobject scale. res * mod = goScale. mod = goScale / res
        Vector2 resToScale = new Vector2(displayScale.x / emulatedDisplayRes.x, displayScale.y / emulatedDisplayRes.y);
        // Now with app resolution figure out where it should be, or how much to move away from center
        Vector2 appOffset = new Vector2(
            appCenterx - (wallw/2) + wallCenterFromEmulatedDisplayCenter.x,
            appCentery - (wallh/2) + wallCenterFromEmulatedDisplayCenter.y);
        // Vector2 appOffset = new Vector2( appCenterx - (wallw/2), appCentery - (wallh/2) ); // original

        // ** This only works because quad scale 1 is 1 unity unit of distance.
        Vector2 appPosAsScale = new Vector2(appOffset.x * resToScale.x, appOffset.y * resToScale.y);
        // this *should* be how much to offset the new browser by to center over app location
        appPosToMakeAt += displayPanel.gameObject.transform.right * appPosAsScale.x;
        appPosToMakeAt += displayPanel.gameObject.transform.up * -1 * appPosAsScale.y;

        Debug.Log("");
        Debug.Log("");
        Debug.Log("params and res. acx" + appCenterx + ". acy: " + appCentery + ". acy: " + appCentery
         + ". ww: " + wallw + ". wh: " + wallh + ". displayres: " + displayRes);
        Debug.Log("what does make atino think appoffset is" + appOffset);
        Debug.Log("what does make atino think restoscale is" + resToScale);
        Debug.Log("what does make atino think scale is" + appPosAsScale);
        Debug.Log("what does make atino think display right is" + displayPanel.gameObject.transform.right);
        Debug.Log("what does make atino think display up is" + displayPanel.gameObject.transform.up);
        Debug.Log("");
        Debug.Log("");

        return appPosToMakeAt;
    }

    // ---------------------------------------------------------------------------
    
    public static Vector3 determineScaleOfAppGivenDisplay(VRBrowserPanel displayPanel, Vector3 displayScale,
                                    float appWidth, float appHeight, float wallw, float wallh) {
        // OLD
        // // Determine the scale app should be at, start with determing resolution to scale conversion
        // Browser contentBrowser = displayPanel.contentBrowser;
        // Vector2 displayRes = contentBrowser.Size; // resolution of content
        // float displayWtohRatio = displayRes.x / displayRes.y;
        // Debug.Log("erase me, wall reports resolution as " + wallw + "," + wallh);
        // float wallWtohRatio = wallw / wallh;

        // Vector2 wallSizeOnDisplay = Vector2.zero;
        // if (wallWtohRatio < displayWtohRatio) { // wall content is limited by display height
        //     wallSizeOnDisplay.y = displayRes.y;
        //     wallSizeOnDisplay.x = displayRes.y * wallWtohRatio; // mult heigt by WALL ratio to get wall max width that fits
        // } else { // else wall must be limited by display width
        //     wallSizeOnDisplay.x = displayRes.x;
        //     wallSizeOnDisplay.y = displayRes.x / wallWtohRatio;
        // }

        Browser contentBrowser = displayPanel.contentBrowser;
        Vector2 displayRes = contentBrowser.Size; // resolution of content
        float displayWtohRatio = displayRes.x / displayRes.y;
        Debug.Log("erase me, wall reports resolution as " + wallw + "," + wallh);
        float wallWtohRatio = wallw / wallh;

        Vector2 emulatedDisplayRes = Vector2.zero;
        Vector2 wallSizeOnDisplay = Vector2.zero;
        if (wallWtohRatio < displayWtohRatio) { // wall content is limited by display height
            wallSizeOnDisplay.y = wallh;
            wallSizeOnDisplay.x = wallh * wallWtohRatio; // mult heigt by WALL ratio to get wall max width that fits
            emulatedDisplayRes.y = wallh;
            emulatedDisplayRes.x = wallh * displayWtohRatio;
        } else { // else wall must be limited by display width
            wallSizeOnDisplay.x = wallw;
            wallSizeOnDisplay.y = displayRes.x / wallWtohRatio;
            emulatedDisplayRes.x = wallw;
            emulatedDisplayRes.y = wallw / displayWtohRatio;
        }
        Vector2 wallCenterFromEmulatedDisplayCenter = new Vector2(
            (wallSizeOnDisplay.x / 2) - (emulatedDisplayRes.x / 2),
            (wallSizeOnDisplay.y / 2) - (emulatedDisplayRes.y / 2));


        // Goal given a resolution determine gameobject scale. res * mod = goScale. mod = goScale / res
        Vector2 resToScale = new Vector2(displayScale.x / emulatedDisplayRes.x, displayScale.y / emulatedDisplayRes.y);

        // Should be able to mult app values by resToScale
        Vector3 appScale = new Vector3(appWidth * resToScale.x, appHeight * resToScale.y, 1);


        return appScale;
    }

    // ---------------------------------------------------------------------------

    public static void sxrCloseApp(string id) {
            Debug.Log("sxrCreateApp");
        List<VRBrowserPanel> allVrbp = VRBrowserPanel.allVrbPanels;

        for (int i = 0; i < allVrbp.Count; i++) {
            // App Viewers that match id, include the and since app_1 triggers all app_10 11 12 ..
            if (allVrbp[i].contentBrowser._url.IndexOf("appView.html?appId=" + id + "&") != -1) {
                // Close that browser
                try {
                    allVrbp[i].CloseBrowser();
                } catch (System.Exception e) {}
            }
        }
        
    }
    public static void sxrSetPositionAndSize(string id, float x, float y, float w, float h) {
        // Ignore for now
    }


}
