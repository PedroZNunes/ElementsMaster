using UnityEngine;

[RequireComponent (typeof (Movement))]
public class Player : Actor {
    
    [SerializeField]
    private Transform spawnPoint;

    [HideInInspector]
    public Movement movement;

    private static Vector3 currentPosition;

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

    private void Awake () {
        controller = GetComponent<Controller2D> ();
        movement = GetComponent<Movement> ();
    }

    /// <summary>
    /// resets the player position to the spawn point.
    /// </summary>
    public void Spawn () {
        //play animation
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
    }

    public static Vector3 GetPosition () {
        if (instance != null)
            return currentPosition;
        Debug.Log ("Player instance not set");
        return Vector3.zero;
    }

    /// <summary>
    /// NOT IMPLEMENTED YET.
    /// </summary>
    public override void Die () {
        //in the future this might be an animation.
    }
}
