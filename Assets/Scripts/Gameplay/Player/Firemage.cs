using UnityEngine;

[RequireComponent (typeof(Fireball), typeof (FireDash))]
public class Firemage : Mastery {

    private Spells spells;

    [SerializeField]
    private Transform castPoint;
    private int previousCastDirection;
    private int castDirection;

    public float projectileSpeedMod = 1f;
    public float projectileSizeMod = 1f;

    private Controller2D controller;


    void Awake () {
        controller = GetComponentInParent<Controller2D> ();
        spells.fireball = gameObject.GetComponent<Fireball> ();
        spells.fireDash = gameObject.GetComponent<FireDash> ();
    }

    void Update () {
        if (Input.GetAxisRaw ("Horizontal") != 0 && Input.GetAxisRaw ("Horizontal") != castDirection) 
            castDirection = (int) Input.GetAxisRaw ("Horizontal");
    }

    public override void Spell1 () {
        if (inGlobalCD ()) { return; }
        globalCD += spells.fireball.GlobalCD;
        
        spells.fireball.Cast (castDirection , projectileSpeedMod , projectileSizeMod , castPoint.position , gameObject);
    }

    public override void Spell2 () {
        if (inGlobalCD ()) { return; }
        globalCD += spells.fireDash.GlobalCD;

        spells.fireDash.Cast (castDirection);
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
        public FireDash fireDash;
    }
}
