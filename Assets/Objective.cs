using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectiveTriggeredEventArgs : EventArgs {
    public GameObject objectiveTriggered;
}

public class Objective : MonoBehaviour {

    [SerializeField]
    Color notTriggeredColor, triggeredColor;
    
    public static event EventHandler<ObjectiveTriggeredEventArgs> ObjectiveTriggered;

    public bool isEnabled = false;

    bool isTriggered = false;
    public bool IsTriggered {
        get {
            return isTriggered;
        }
        set {
            isTriggered = value;
            ChangeMaterialColor ();
        }
    }

    public void Reset () {
        isEnabled = true;
        IsTriggered = false;
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
        IsTriggered = true;

        Debug.Log ("Player triggered the " + name);
    }

    void ChangeMaterialColor () {
        Renderer rend = GetComponent<Renderer> ();
        if (rend != null) {
            if (isTriggered) {
                rend.material.color = triggeredColor;
            }
            else {
                rend.material.color = notTriggeredColor;
            }
        }
    }
}
