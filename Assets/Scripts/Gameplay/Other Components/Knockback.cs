using UnityEngine;
using System;

[Serializable]
public class Knockback {

    [SerializeField]
    private Vector2 force;
    public Vector2 Force { get { return force; } }

    public Knockback (Vector2 force) {
        this.force = force;
    }

    public void Push (Collider2D other, int dirX) {
        Movement movement = other.GetComponent<Movement> ();
        if (movement != null) {
            movement.AddForce (new Vector2 (force.x * dirX, force.y));
        }
        else {
            Debug.Log ("No RB found.");
        }
    }

}
