using UnityEngine;

public class FireballProjectile : Projectile {

    public GameObject owner { get; private set; }

    private Vector2 dir;
    private float speed;
    private float size;
    private float maxDuration = 4f;

    void OnEnable () {
        Destroy (gameObject , maxDuration); //TODO: projectiles object pool for memory fragmentation
    }

    void Start () {
        transform.localScale = Vector2.one * size;
    }

    void Update () {
        float moveDistance = speed * Time.deltaTime;
        transform.Translate (dir * moveDistance);
    }

    void OnTriggerEnter2D ( Collider2D col ) {
        if (col.tag == MyTags.block.ToString ()) {
            Destroy (gameObject);
        } else if (col.tag == MyTags.enemy.ToString()) {
            Debug.LogFormat ("{0} dealt XXX dmg to enemy {1}" , name , col.name);
        }
    }

    void OnDestroy () {
        Debug.Log("FireballProjectile destroyed.");
        //remove this projectile from lists and whatever event or i dont know yet
    }


    public void Initialize ( ref int dirX , ref float speed , ref float size , ref GameObject owner ) {
        dir = Vector2.right * dirX;
        this.speed = speed;
        this.size = size;
        this.owner = owner;
    }
}
