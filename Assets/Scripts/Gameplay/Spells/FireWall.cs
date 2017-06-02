using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a wall of fire that does damage overtime.
/// </summary>
public class FireWall : Spell {

    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private float duration;
    [SerializeField]
    private float tickTime;
    [SerializeField]
    private LayerMask layerMask;

    private float halfWidth;

    protected override void OnEnable () {
        base.OnEnable ();
        halfWidth = prefab.GetComponent<BoxCollider2D> ().size.x / 2;
    }

    public override void Cast ( ) {
        if (CanCast ()) {
            base.Cast ();
            //if there's an obstacle ahead, cast just before it.
            RaycastHit2D hit = Physics2D.Raycast (transform.position , new Vector2 (castDirX , 0f), castOffset.x, layerMask);
            if (hit) {
                CastPoint = new Vector3 (transform.position.x + ( ( hit.distance - halfWidth ) * castDirX ) , CastPoint.y);
            }
            //TODO: check if jumping and cast a variation of the spell in case it is.
            GameObject firewallGO = Instantiate (prefab , CastPoint , Quaternion.identity , holder);
            TheFireWall theFireWall = firewallGO.GetComponent<TheFireWall> ();
            theFireWall.Initialize (duration);
        }

    }

}
