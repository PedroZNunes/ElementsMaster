using UnityEngine;

/// <summary>
/// Damaging single-target projectile with speed and size.
/// </summary>
public sealed class Fireball : Spell {

    [SerializeField]
    private Pool pool;

    [SerializeField]
    private float velocity = 15f;

    private Vector2 size = Vector2.one;

    [SerializeField]
    private float maxDuration = 4f;

    [SerializeField]
    private Damage damage;
    [SerializeField]
    private Knockback knockback;
    [SerializeField]
    private Knockback buffedKnockback;


    protected override void OnEnable () {
        base.OnEnable ();

        holder = GameObject.FindGameObjectWithTag (MyTags.projectileHolder.ToString ()).transform;
        pool.Initialize (holder);
    }

    /// <summary>
    /// Initializes a free fireball from the pool.
    /// </summary>
    /// <param name="speedMod">speed modificator that multiplies the base speed. to be used by buffs</param>
    /// <param name="sizeMod">size modificator that multiplies the base size. to be used by buffs</param>
    public override void Cast ( float speedMod , float sizeMod ) {
        if (CanCast ()) {
            base.Cast ();
            GameObject go = pool.FindFreeObject ();
            if (go == null) { Debug.LogWarningFormat ("The pool does not have a free {0}" , this.name); }
            else {
                TheFireball theFireball = go.GetComponent<TheFireball> ();

                float velocityFinal = velocity * speedMod;
                Vector2 sizeFinal = size * sizeMod;
                theFireball.Initialize (castDirX , CastPoint , ref velocityFinal , ref sizeFinal , ref damage , ref knockback , ref maxDuration );
            }
        }
    }

}
