using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// component to be attached to the fireball instance
/// </summary>
[RequireComponent(typeof(Damage), typeof (CircleCollider2D))]
public class TheFireball : Projectile {

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

    /// <summary>
    /// initialize the component, ensuring that the reuse of it (from the pool) won't be affected by the previous lifetime
    /// </summary>
    /// <param name="dirX">horizontal direction</param>
    /// <param name="spawnPosition"></param>
    /// <param name="speed"></param>
    /// <param name="size"></param>
    /// <param name="maxDuration"></param>
    public void Initialize ( int dirX , Vector2 spawnPosition , ref float speed , ref Vector2 size , ref float maxDuration ) {

        gameObject.SetActive (true);

        damage = GetComponent<Damage> ();

        dir = Vector2.right * dirX;
        transform.position = spawnPosition;

        this.speed = speed;
        moveAmount = speed * Time.deltaTime;

        transform.localScale = new Vector3 (size.x , size.y , 1);

        col = GetComponent<CircleCollider2D> ();
        col.offset = new Vector2 (moveAmount * dirX , 0f);

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
                damage.DealDamage (baseDamage , otherCollider.gameObject);
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

    /// <summary>
    /// works like the unityengine.destroy method, but deactivating it for the pool
    /// </summary>
    /// <param name="time">time after which the object will die</param>
    /// <returns></returns>
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
