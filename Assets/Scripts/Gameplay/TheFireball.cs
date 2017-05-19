using UnityEngine;

[RequireComponent(typeof(Damage), typeof (CircleCollider2D))]
public class TheFireball : Projectile {

    public GameObject owner { get; private set; }

    private Vector2 dir;
    private float speed;
    private float size;
    private float maxDuration = 4f;

    private Damage damage;
    [SerializeField]
    private int baseDamage;

    private ParticleSystem particles;

    void OnEnable () {
        damage = GetComponent<Damage> ();
        particles = GetComponentInChildren<ParticleSystem> ();
        transform.localScale *= size;
        Destroy (gameObject , maxDuration); //TODO: projectiles object pool for memory fragmentation
    }

    void Update () {
        float moveDistance = speed * Time.deltaTime;
        transform.Translate (dir * moveDistance);
    }

    void OnTriggerEnter2D ( Collider2D col ) {
        if (col.tag == MyTags.block.ToString ()) {
            Destroy (gameObject);
        } else if (col.tag == MyTags.enemy.ToString()) {
            damage.DealDamage (baseDamage , col.gameObject);
            Destroy (gameObject);
        }
    }

    void OnDestroy () {
        particles.transform.SetParent (transform.parent , true);
        ParticleSystem.Burst[] burst = { new ParticleSystem.Burst (0 , 10 , 20) };
        particles.emission.SetBursts (burst);

        Destroy (particles, )
        Debug.Log("FireballProjectile destroyed.");
        //remove this projectile from lists and whatever event or i dont know yet
    }


    public void Initialize ( int dirX , ref float speed , ref float size , ref GameObject owner ) {
        dir = Vector2.right * dirX;
        this.speed = speed;
        this.size = size;
        this.owner = owner;
    }
}
