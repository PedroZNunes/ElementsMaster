using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// the component attached to the firewall instance
/// </summary>
[RequireComponent(typeof (BoxCollider2D))]
public class TheFireWall : MonoBehaviour {

    private List<CollidersCD> trackCollidersCD = new List<CollidersCD> ();

    private float tickTime;

    [SerializeField]
    private LayerMask layerMask;

    private BoxCollider2D collider;

    private Damage damage;
    private Knockback knockback;

    public void Initialize(ref float tickTime, ref Knockback knockback, ref Damage damage, float maxDuration ) {
        collider = GetComponent<BoxCollider2D> ();
        this.tickTime = tickTime;

        this.damage = damage;
        this.knockback = knockback;
        //calculate damage. for now using just the base
        Destroy (gameObject , maxDuration);     
    }

    private void Update () {
        
        if (trackCollidersCD.Count > 0) {
            Predicate<CollidersCD> isOffCD = ( colCD => colCD.cd <= 0 );
            trackCollidersCD.RemoveAll (isOffCD);
        }

        if (trackCollidersCD.Count > 0) { 
            for (int i = 0 ; i < trackCollidersCD.Count ; i++) {
                trackCollidersCD[i].cd -= Time.deltaTime;
            }
        }
    }

    private void OnController2DTrigger ( Collider2D col ) {
        Enemy e = col.GetComponent<Enemy> ();
        if (e != null) {
            if (!isColliderOnCD (col)) {
                int pushDirX = ( col.bounds.center.x - collider.bounds.center.x > 0 ) ? 1 : -1;
                damage.DealDamage (e);
                knockback.Push (col, pushDirX);
                trackCollidersCD.Add (new CollidersCD (col , tickTime));
            }
        }
    }

    private bool isColliderOnCD (Collider2D targetCollider) {
        bool exists = false;
        if (trackCollidersCD.Count > 0) {
            foreach (CollidersCD colCD in trackCollidersCD) {
                if (targetCollider == colCD.Col) {
                    //Enemy already in the list.
                    exists = true;
                    break;
                }
            }
        }
        return exists;
    }

    private class CollidersCD {
        
        private Collider2D col;
        public Collider2D Col {  get { return col; } }
        public float cd;
        

        public CollidersCD (Collider2D col, float cd ) {
            this.col = col;
            this.cd = cd;
        }
    }
}
