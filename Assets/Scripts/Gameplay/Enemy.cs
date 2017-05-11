using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Movement), typeof(EnemyAI))]
public class Enemy : Actor {

    [HideInInspector]
    public Movement movement;

    EnemyAI ai;
    
    public int difficulty;

    void Awake () {
        ai = GetComponent<EnemyAI> ();
        controller = GetComponent<Controller2D> ();
        movement = GetComponent<Movement> ();
    }

}
