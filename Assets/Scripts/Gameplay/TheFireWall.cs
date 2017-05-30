using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Damage), typeof (BoxCollider2D))]
public class TheFireWall : MonoBehaviour {

    [SerializeField]
    private Tick tick;

    private float currentTime;

    [SerializeField]
    private LayerMask layerMask;

    private BoxCollider2D col;

    private Damage damage;


    public void Initialize(float maxDuration ) {
        col = GetComponent<BoxCollider2D> ();
        damage = GetComponent<Damage> ();
        //calculate damage. for now using just the base
        Destroy (gameObject , maxDuration);
        StartCoroutine (DamageTicking ());
    }

    void OnTriggerEnter2D ( Collider2D col ) {
        if (col.GetComponent<TheFireball> () != null) {
            TheFireball theFireball = col.GetComponent<TheFireball> ();
            //theFireball.Buff();
            Debug.Log ("Firewall buffed fireball projectile: " + col.name);
        }
    }

    private IEnumerator DamageTicking () {
        while (gameObject != null) {
            //checkar col
            Vector2 origin = col.bounds.center;

            RaycastHit2D[] hits = Physics2D.BoxCastAll (origin , col.size , 0f , Vector2.up, 0f, layerMask);
            if (hits.Length > 0) {
                for (int i = 0 ; i < hits.Length ; i++) {
                    damage.DealDamage (tick.tickBaseDamage, hits[i].collider.gameObject);
                }
            }
            yield return new WaitForSeconds(tick.tickInterval);
        }
    }

    [System.Serializable]
    public struct Tick {
        public float tickInterval;
        public int tickBaseDamage;
    }
}
