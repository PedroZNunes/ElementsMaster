using UnityEngine;

public class TheFireWall : MonoBehaviour {

    float maxDuration;

    public void Initialize(float maxDuration ) {
        this.maxDuration = maxDuration;
        Destroy (gameObject , maxDuration);
    }

    void OnTriggerEnter2D ( Collider2D col ) {
        if (col.gameObject.CompareTag (MyTags.enemy.ToString ())) {
            Debug.Log ("Firewall dealt xxx dmg to the enemy: " + col.name);
        }
        else if (col.GetComponent<FireballProjectile> () != null) {
            Debug.Log ("Firewall buffed fireball projectile: " + col.name);
        }
    }
    
}
