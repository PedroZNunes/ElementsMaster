using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void OnEnable () {
        base.OnEnable ();
        halfWidth = prefab.GetComponent<BoxCollider2D> ().size.x / 2;
    }

    public override void Cast ( ) {
        if (CanCast ()) {
            base.Cast ();
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
