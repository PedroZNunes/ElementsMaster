using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Movement), typeof (EnemyAI), typeof (Health))]
public class Enemy : Actor {

    [HideInInspector]
    public Movement movement;
    private Health health;
    private EnemyAI ai;

    [SerializeField]
    private int difficulty;
    public int Difficulty { get { return difficulty; } }

    /// <summary>
    /// initializes all variables.
    /// this is very important for the object pool system. this avoids that the newly spawned enemy comes with information from its past life
    /// </summary>
    /// <param name="position"> the block position in which it will be spawned. must add the offset it has from the ground in y </param>
    public void Initialize ( Vector2 position ) {
        controller = GetComponent<Controller2D> ();
        movement = GetComponent<Movement> ();

        health = GetComponent<Health> ();
        health.FillHP ();

        transform.position = position + Vector2.up * (controller.collider.size.y + controller.SkinWidth);

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
