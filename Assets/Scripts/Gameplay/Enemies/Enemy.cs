using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Movement))]
public class Enemy : Actor {

    [HideInInspector]
    public Movement movement;

  
    public int difficulty;

    void Awake () {
        controller = GetComponent<Controller2D> ();
        movement = GetComponent<Movement> ();
    }

}
