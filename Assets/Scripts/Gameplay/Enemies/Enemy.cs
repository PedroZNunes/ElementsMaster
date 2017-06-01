using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Movement), typeof (EnemyAI))]
public class Enemy : Actor {

    [HideInInspector]
    public Movement movement;
    private Health health;
    private EnemyAI ai;

    [SerializeField]
    private int difficulty;
    public int Difficulty { get { return difficulty; } }


    public void Initialize ( Vector2 position ) {
        controller = GetComponent<Controller2D> ();
        movement = GetComponent<Movement> ();

        health = GetComponent<Health> ();
        health.FillHP ();

        transform.position = position + Vector2.up * controller.collider.size.y / 2 ;

        ai = GetComponent<EnemyAI> ();
        gameObject.SetActive (true);
        ai.StartCheckingAggro ();
    }

    public override void Die () {
        //in the future this might be an animation.
        gameObject.SetActive (false);
        ai.StopCheckingAggro ();
    }
}
