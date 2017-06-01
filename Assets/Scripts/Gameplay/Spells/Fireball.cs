using UnityEngine;


public sealed class Fireball : Spell {
    [SerializeField]
    private Pool pool;

    [SerializeField]
    private float velocity = 15f;

    private Vector2 size = Vector2.one;

    [SerializeField]
    private float maxDuration = 4f;


    protected override void OnEnable () {
        base.OnEnable ();

        holder = GameObject.FindGameObjectWithTag (MyTags.projectileHolder.ToString ()).transform;
        pool.Initialize (holder);
    }

    public override void Cast ( float speedMod , float sizeMod , GameObject owner ) {
        if (CanCast ()) {
            base.Cast ();
            GameObject go = pool.FindFreeObject ();
            if (go == null) { Debug.LogWarningFormat ("The pool does not have a free {0}" , this.name); }
            else {
                TheFireball theFireball = go.GetComponent<TheFireball> ();

                float velocityFinal = velocity * speedMod;
                Vector2 sizeFinal = size * sizeMod;
                theFireball.Initialize (castDirX , CastPoint , ref velocityFinal , ref sizeFinal , ref maxDuration , ref owner);
            }
        }
    }

}
