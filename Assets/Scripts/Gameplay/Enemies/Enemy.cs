using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Movement))]
public class Enemy : Actor {

    [HideInInspector]
    public Movement movement;
    private Health health;

  
    public int difficulty;

    void Awake () {
        controller = GetComponent<Controller2D> ();
        movement = GetComponent<Movement> ();
        health = GetComponent<Health> ();
    }

    public override void Die () {
        //in the future this might be an animation.
        Destroy (gameObject);
    }
}
