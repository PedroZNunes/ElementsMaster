using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballProjectile : Projectile {

    float projectileSpeed;
    Vector2 dir;

    public void Initialize (float speed, int dirX) {
        projectileSpeed = speed;
        dir = new Vector2 (0 , dirX);
    }

    void Update () {
        float moveAmount = projectileSpeed * Time.deltaTime;
        transform.Translate (dir * moveAmount);
    }

}
