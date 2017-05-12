using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Spell {

    [SerializeField]
    protected Object prefab;

    Transform holder;

	void Awake () {
        holder = GameObject.FindWithTag (MyTags.projectileHolder.ToString ()).transform;
    }

    public override void Cast ( int dirX , float speed , float size , Vector2 castPoint , GameObject owner ) {
        if (CanCast ()) {
            GameObject fireball = Instantiate (prefab , castPoint , Quaternion.identity , holder) as GameObject;
            FireballProjectile projectile = fireball.GetComponent<FireballProjectile> ();
            projectile.Initialize (ref dirX , ref speed , ref size , ref owner);
        }
    }

}
