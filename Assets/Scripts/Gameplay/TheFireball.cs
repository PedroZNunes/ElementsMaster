using UnityEngine;

[RequireComponent(typeof(Damage), typeof (CircleCollider2D))]
public class TheFireball : Projectile {

    public GameObject owner { get; private set; }

    private Vector2 dir;
    private float speed;
    private float moveAmount;

    private float size;
    private float maxDuration = 4f;

    private Damage damage;
    [SerializeField]
    private int baseDamage;

    [SerializeField]
    private ParticleSystem particles;
    [SerializeField]
    private ParticleSystem particlesExplosion;

    private CircleCollider2D col;

    private void OnEnable () {
        damage = GetComponent<Damage> ();
        Destroy (gameObject , maxDuration); //TODO: projectiles object pool for memory fragmentation
    }

    private void Update () {
        float moveDistance = speed * Time.deltaTime;
        transform.Translate (dir * moveDistance);
    }

    private void OnTriggerEnter2D ( Collider2D col ) {
        if (col.tag == MyTags.block.ToString ()) {
            ColliderDistance2D colDist = col.Distance (this.col);
            Vector3 dist = ( colDist.pointB - colDist.pointA );
            transform.position = transform.position - dist;
            Explode ();
        } else if (col.tag == MyTags.enemy.ToString()) {
            damage.DealDamage (baseDamage , col.gameObject);
            Explode ();
        }
    }

    private void Explode () {
        GetComponent<Collider2D> ().enabled = false;
        particles.Stop ();
        particlesExplosion.gameObject.SetActive (true);
        Destroy (gameObject , particlesExplosion.main.startLifetime.constantMax);

        Debug.Log ("FireballProjectile destroyed.");
        //remove this projectile from lists 
    }

    public void Initialize ( int dirX , ref float speed , ref float size , ref GameObject owner ) {
        dir = Vector2.right * dirX;
        
        this.owner = owner;

        this.size = size;
        transform.localScale *= size;

        this.speed = speed;
        moveAmount = speed * Time.deltaTime;
        col = GetComponent<CircleCollider2D> ();
        col.offset = new Vector2 (moveAmount * dirX , 0f);
    }

}
