using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Spell {

    [SerializeField]
    protected Object prefab;

    float cooldown;

    Transform holder;

	void Awake () {
        holder = GameObject.FindWithTag (MyTags.ProjectileHolder.ToString ()).transform;
    }

	void Update () {
		if (cooldown > 0) {
            cooldown -= Time.deltaTime;
        }
	}

    public override void Cast ( int dirX , float speed , float size , Vector2 castPoint , GameObject owner ) {
        if (cooldown > 0) {
            Debug.LogFormat ("Spell on cooldown. {0:0.0}s", cooldown);
            return;
        }
        GameObject fireball = Instantiate (prefab , castPoint , Quaternion.identity , holder) as GameObject;
        FireballProjectile projectile = fireball.GetComponent<FireballProjectile> ();
        projectile.Initialize (ref dirX , ref speed , ref size , ref owner);
    }

}
