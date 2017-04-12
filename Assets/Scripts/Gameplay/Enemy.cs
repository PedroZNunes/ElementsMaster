using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D) , typeof (Movement), typeof(EnemyAI))]
public class Enemy : Actor {

    [HideInInspector]
    public Movement movement;

    Controller2D controller;
    EnemyAI ai;

    void Awake () {
        ai = GetComponent<EnemyAI> ();
        controller = GetComponent<Controller2D> ();
        movement = GetComponent<Movement> ();
    }

}
