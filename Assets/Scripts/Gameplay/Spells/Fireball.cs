using UnityEngine;

public sealed class Fireball : Spell {

    [SerializeField]
    private Object prefab;

    [SerializeField]
    private float velocity = 15f;

    private float size = 1f;
    
    public override void Cast ( int dirX , float speedMod , float sizeMod , Vector2 castPoint , GameObject owner ) {
        if (CanCast ()) {
            GameObject fireballGO = Instantiate (prefab , castPoint , Quaternion.identity , holder) as GameObject;
            FireballProjectile projectile = fireballGO.GetComponent<FireballProjectile> ();
            float finalVelocity = velocity * speedMod;
            float finalSize = size * sizeMod;
            projectile.Initialize (ref dirX , ref finalVelocity , ref finalSize , ref owner);
        }
    }
}
