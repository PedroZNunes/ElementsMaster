using UnityEngine;

[RequireComponent (typeof(Fireball), typeof (FireDash))]
public class Firemage : Mastery {

    private Spells spells;

    [SerializeField]
    private Transform castPoint;
    private int previousCastDirection;
    private int castDirection = 1;

    public float projectileSpeedMod = 1f;
    public float projectileSizeMod = 1f;


    void Awake () {
        spells.fireball = gameObject.GetComponent<Fireball> ();
        spells.fireDash = gameObject.GetComponent<FireDash> ();
        spells.conflagrate = gameObject.GetComponent<Conflagrate> ();
        spells.firewall = gameObject.GetComponent<FireWall> ();
    }

    void Update () {
        if (Input.GetAxisRaw ("Horizontal") != 0 && Input.GetAxisRaw ("Horizontal") != castDirection) 
            castDirection = (int) Input.GetAxisRaw ("Horizontal");
    }

    public override void Spell1 () {
        if (inGlobalCD ()) { return; }
        globalCD += spells.fireball.GlobalCD;
        //TODO: Spell has 3 charges that replenish a little bit slower than the gdc, so that the player cant spam and also gets a new musicality to the skill set. 
        //think about making this a homing fire ball. Ori-like
        spells.fireball.Cast ( projectileSpeedMod , projectileSizeMod , gameObject);
    }

    public override void Spell2 () {
        if (inGlobalCD ()) { return; }
        globalCD += spells.fireDash.GlobalCD;
        //TODO: if you hit and enemy, you gain refill (up to three dashes in a row). but first we need enemies.
        spells.fireDash.Cast ( );
    }

    public override void Spell3 () {
        if (inGlobalCD ()) { return; }
        globalCD += spells.firewall.GlobalCD;

        spells.firewall.Cast ( );
        
    }

    public override void Spell4 () {
        if (inGlobalCD ()) { return; }
        globalCD += spells.conflagrate.GlobalCD;

        spells.conflagrate.Cast ( );
    }

    [System.Serializable]
    struct Spells {
        public Fireball fireball;
        public FireDash fireDash;
        public Conflagrate conflagrate;
        public FireWall firewall;
    }
}
