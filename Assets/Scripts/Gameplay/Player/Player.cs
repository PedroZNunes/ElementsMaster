using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Movement))]
public class Player : Actor {
    
    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    static Transform playerHolder;

    [HideInInspector]
    public Movement movement;
    
    static Vector3 currentPosition;

    //Singleton Instance
    private static Player instance = null;
    public static Player Instance {
        get {
            if (instance == null) {
                Debug.LogError ("No Player instance found.");
            }
            return instance;
        }
    }
    //the player must receive the mastery before the opening animations and should addcomponent the desired mastery. the desired mastery must add all component it uses
    //and the playerinput should stay mastery-agnostic

    void Awake () {
        controller = GetComponent<Controller2D> ();
        movement = GetComponent<Movement> ();
    }

    public void Spawn () {
        //play animation
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
    }

    static public Vector3 GetPosition () {
        if (instance != null)
            return currentPosition;
        Debug.Log ("Player instance not set");
        return Vector3.zero;
    }

}
