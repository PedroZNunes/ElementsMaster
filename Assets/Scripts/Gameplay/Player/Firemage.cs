using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Fireball))]
public class Firemage : Mastery {

    private Spells spells;

    [SerializeField]
    float projectileSpeed = 1f;
    [SerializeField]
    float projectileSize = 1f;

    private Controller2D controller;

    private int dirX;

    void Awake () {
        controller = GetComponentInParent<Controller2D> ();
        spells.fireball = gameObject.GetComponent <Fireball> ();
    }

    void Update () {
        dirX = controller.collisions.faceDirection;
    }

    public override void Spell1 () {
        if (inGlobalCD ()) { return; }
        globalCD += spells.fireball.GlobalCD;
        Vector2 castPoint = (Vector2) transform.position + Vector2.right * dirX * 0.5f;
        spells.fireball.Cast (dirX , projectileSpeed , projectileSize , castPoint , gameObject);
    }

    public override void Spell2 () {
        if (inGlobalCD ()) { return; }
        //cast firedash (dirX)
    }

    public override void Spell3 () {
        if (inGlobalCD ()) { return; }
        //cast firewall (dirX, castPoint)
    }

    public override void Spell4 () {
        if (inGlobalCD ()) { return; }
        //cast fire elemental (castPoint)
    }

    [System.Serializable]
    struct Spells {
        public Fireball fireball;
    }
}
