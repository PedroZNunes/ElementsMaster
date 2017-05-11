using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firemage : Mastery {
    
    Controller2D controller;
    [SerializeField]
    float projectileSpeed = 1f;
    [SerializeField]
    float projectileSize = 1f;

    private Fireball fireball;
    private short dirX;

    void Awake () {
        controller = GetComponent<Controller2D> ();
        fireball = gameObject.AddComponent<Fireball> ();
    }

    void Update () {
        dirX = (short)controller.collisions.faceDirection;
    }

    public override void Spell1 () {
        //cast fireball (dirX, castPoint)
        Vector2 castPoint = (Vector2) transform.position + Vector2.right * dirX * 0.5f;
        fireball.Cast (dirX , projectileSpeed , projectileSize , castPoint , gameObject);
    }

    public override void Spell2 () {
        //cast firedash (dirX)
    }

    public override void Spell3 () {
        //cast firewall (dirX, castPoint)
    }

    public override void Spell4 () {
        //cast fire elemental (castPoint)
    }

}
