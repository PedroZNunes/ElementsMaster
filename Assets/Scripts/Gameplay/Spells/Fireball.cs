using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Spell {

    [SerializeField]
    GameObject prefab;
    Transform spellsHolder;

    FireballProjectile fireballProjectile;

    float cooldown;
    float projectileSpeed;


    // Use this for initialization
    void Awake () {
        spellsHolder = GameObject.FindGameObjectWithTag (MyTags.SpellHolder.ToString()).transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (cooldown > 0) {
            cooldown -= Time.deltaTime;
        }
	}

    public override void CastSpell (int dirX, Vector2 spawnPoint) {
        if (cooldown > 0) {
            Debug.LogFormat ("Spell on cooldown. {0:0.0}s", cooldown);
            return;
        }
        

        GameObject fireball = Instantiate (prefab , spawnPoint , Quaternion.identity , spellsHolder) as GameObject;
        fireballProjectile = prefab.GetComponent<FireballProjectile> ();
        fireballProjectile.Initialize (projectileSpeed , dirX);
    }

    void OnCollisionEnter2D (Collision2D col ) {
        if (col.otherCollider.GetComponent<Enemy> () != null) {
            //deal damage
        }
    }
}
