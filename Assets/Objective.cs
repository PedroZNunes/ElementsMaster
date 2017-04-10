using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectiveTriggeredEventArgs : EventArgs {
    public GameObject objectiveTriggered;
}

public class Objective : MonoBehaviour {

    public static event EventHandler<ObjectiveTriggeredEventArgs> ObjectiveTriggered;

    public bool isEnabled = false;
    public bool isTriggered = false;

    public void Reset () {
        isEnabled = true;
        isTriggered = false;
    }

    public void OnCollision ( GameObject source ) {
        if (source.CompareTag (MyTags.player.ToString ())) {
            Debug.Log ("Collision with Player");
            
            Trigger ();
        }
    }
    
    void Trigger () {
        if (ObjectiveTriggered != null) {
            ObjectiveTriggeredEventArgs e = new ObjectiveTriggeredEventArgs () { objectiveTriggered = gameObject };
            ObjectiveTriggered (this , e);
        }
        Collider2D col = GetComponent<Collider2D> ();
        col.enabled = isEnabled = false;
        isTriggered = true;

        Debug.Log ("Player triggered the " + name);
    }

}
