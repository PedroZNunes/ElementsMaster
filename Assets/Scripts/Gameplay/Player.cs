using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller2D), typeof (Movement))]
public class Player : Actor {

    [SerializeField]
    static GameObject playerPrefab;

    [SerializeField]
    static Transform playerHolder;

    [HideInInspector]
    public Movement movement;

    Controller2D controller;

    static Vector3 spawnPosition;
    static Vector3 currentPosition;

    //Singleton Instance
    public static Player Instance {
        get {
            if (instance == null) {
                Debug.LogError ("No Player instance found.");
            }
            return instance;
        }
    }
    private static Player instance = null;

    void Awake () {
        controller = GetComponent<Controller2D> ();
        movement = GetComponent<Movement> ();
    }

    public static void Spawn () {
        spawnPosition = GameObject.FindGameObjectWithTag (MyTags.playerRespawn.ToString ()).transform.position;
        Debug.Assert (spawnPosition != null , "No player spawn location set");

        if (instance == null && FindObjectOfType<Player> () == null) {
            Debug.Log ("Spawning Player");
           GameObject playerGO =  Instantiate (playerPrefab , spawnPosition , Quaternion.identity , playerHolder);
            if (instance == null) {
                instance = playerGO.GetComponent<Player> ();
            }
        }
        else {
            Debug.Log ("Spawning Player failed. The player already exists");
        }
    }

    

    static public Vector3 GetPosition () {
        if (instance != null)
            return currentPosition;
        Debug.Log ("Player instance not set");
        return Vector3.zero;
    }

}
