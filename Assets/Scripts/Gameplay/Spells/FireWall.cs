using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWall : Spell {

    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private float offsetX;
    [SerializeField]
    private float duration;
    [SerializeField]
    private float tickTime;


    public override void Cast (int dirX, Vector2 castPoint) {
        if (CanCast ()) {
            //TODO: check if jumping and cast a variation of the spell in case it is.
            GameObject firewallGO = Instantiate (prefab , castPoint + Vector2.right * offsetX , Quaternion.identity , holder);
            TheFireWall theFireWall = firewallGO.GetComponent<TheFireWall> ();
            theFireWall.Initialize (duration);
        }

    }

}
