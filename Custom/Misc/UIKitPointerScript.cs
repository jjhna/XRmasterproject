using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by: Jonathan Na from the University of Hawaii at Manoa, Laboratory for Advanced Visualization & Application (LAVA)
// This is an additional script for the UIKitPointer Game Object, it will check for any duplicate game objects 
// and destroy the original version when a new scene is loaded (or reloaded)
public class UIKitPointerScript : MonoBehaviour
{
    // Finds the UIKitPointer gameobject (or this.gameObject) and the duplicate game object
    // if the duplicate game object exist then the original game object is destroyed to avoid confusion
    void Awake()
    {
        if (GameObject.Find(gameObject.name) && GameObject.Find(gameObject.name) != this.gameObject)
        {
            Destroy(this.gameObject);
        }
    }
}
