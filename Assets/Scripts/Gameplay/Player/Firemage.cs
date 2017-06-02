using UnityEngine;

[RequireComponent (typeof(Fireball), typeof (FireDash), typeof(FireWall))]
[RequireComponent (typeof (Conflagrate))]
public class Firemage : Mastery {

    private Spells spells;

    public float projectileSpeedMod = 1f; //public mod to be used by buffs and debuffs to alter the speed of the projectile
    public float projectileSizeMod = 1f; //public mod to be used by buffs and debuffs to alter the size of the projectile


    private void Awake () {
        spells.fireball = gameObject.GetComponent<Fireball> ();
        spells.fireDash = gameObject.GetComponent<FireDash> ();
        spells.conflagrate = gameObject.GetComponent<Conflagrate> ();
        spells.firewall = gameObject.GetComponent<FireWall> ();
    }

    public override void Spell1 () {
        if (inGlobalCD ()) { return; }
        globalCD += spells.fireball.GlobalCD;
        //TODO: Spell has 3 charges that replenish a little bit slower than the gdc, so that the player cant spam and also gets a new musicality to the skill set. 
        //think about making this a homing fire ball.
        spells.fireball.Cast ( projectileSpeedMod , projectileSizeMod );
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

        spells.firewall.Cast ();
    }

    public override void Spell4 () {
        if (inGlobalCD ()) { return; }
        globalCD += spells.conflagrate.GlobalCD;

        spells.conflagrate.Cast ( );
    }

    [System.Serializable]
    private struct Spells {
        public Fireball fireball;
        public FireDash fireDash;
        public Conflagrate conflagrate;
        public FireWall firewall;
    }
}
