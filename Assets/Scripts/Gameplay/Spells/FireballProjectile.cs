using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballProjectile : Projectile {

    public GameObject owner { get; private set; }

    Vector2 dir;
    float speed;
    float size;

    public void Initialize (ref int dirX, ref float speed, ref float size, ref GameObject owner) {
        dir = Vector2.right * dirX;
        this.speed = speed;
        this.size = size;
        this.owner = owner;
    }

    void Update () {
        float moveDistance = speed * Time.deltaTime;
        transform.Translate (dir * moveDistance);
    }

    void OnCollisionEnter2D ( Collision2D col ) {
        if (col.otherCollider.GetComponent<Enemy> () != null) {
            Debug.LogFormat ("{0} dealt XXX dmg to enemy {1}" , name , col.otherCollider.name);
        }
    }

}
