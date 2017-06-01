using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Damage), typeof (CircleCollider2D))]
public class TheFireball : Projectile {

    public GameObject owner { get; private set; }

    private Vector2 dir;
    private float speed;
    private float moveAmount;

    private Damage damage;
    [SerializeField]
    private int baseDamage;

    [SerializeField]
    private ParticleSystem particles;
    [SerializeField]
    private ParticleSystem particlesExplosion;

    private CircleCollider2D col;


    public void Initialize ( int dirX , Vector2 spawnPosition , ref float speed , ref Vector2 size , ref float maxDuration , ref GameObject owner ) {

        gameObject.SetActive (true);

        damage = GetComponent<Damage> ();

        dir = Vector2.right * dirX;
        transform.position = spawnPosition;

        this.speed = speed;
        moveAmount = speed * Time.deltaTime;

        transform.localScale = new Vector3 (size.x , size.y , 1);

        col = GetComponent<CircleCollider2D> ();
        col.offset = new Vector2 (moveAmount * dirX , 0f);

        this.owner = owner;

        StartCoroutine (Die (maxDuration)); //TODO: projectiles object pool for memory fragmentation
    }

    private void Update () {
        float moveDistance = speed * Time.deltaTime;
        transform.Translate (dir * moveDistance);
    }

    private void OnTriggerEnter2D ( Collider2D otherCollider ) {
        if (col.enabled) {
            if (otherCollider.tag == MyTags.block.ToString ()) {
                ColliderDistance2D colDist = otherCollider.Distance (col);
                Vector3 dist = ( colDist.pointB - colDist.pointA );
                transform.position = transform.position - dist;
                Explode ();
            }
            else if (otherCollider.tag == MyTags.enemy.ToString ()) {
                damage.DealDamage (baseDamage , col.gameObject);
                Explode ();
            }
        }
    }

    private void Explode () {
        col.enabled = false; 
        particles.Stop ();
        particlesExplosion.gameObject.SetActive (true);
        StartCoroutine (Die (particlesExplosion.main.startLifetime.constantMax));
    }

    private IEnumerator Die (float time) {
        while (time > 0) {
            time -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate ();
        }
        col.enabled = true;
        particlesExplosion.gameObject.SetActive (false);
        gameObject.SetActive (false);
    }

    

}
