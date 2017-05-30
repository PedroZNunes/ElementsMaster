using UnityEngine;

public sealed class Fireball : Spell {

    [SerializeField]
    private Object prefab;

    [SerializeField]
    private float velocity = 15f;

    private float size = 1f;
    
    public override void Cast ( float speedMod , float sizeMod , GameObject owner ) {
        if (CanCast ()) {
            base.Cast ();
            GameObject fireballGO = Instantiate (prefab , CastPoint , Quaternion.identity , holder) as GameObject;
            TheFireball projectile = fireballGO.GetComponent<TheFireball> ();
            float finalVelocity = velocity * speedMod;
            float finalSize = size * sizeMod;
            projectile.Initialize (castDirX , ref finalVelocity , ref finalSize , ref owner);
        }
    }

}
