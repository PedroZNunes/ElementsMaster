using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour {

    [SerializeField]
    protected float globalCD;
    public float GlobalCD { get { return globalCD; } protected set { globalCD = value; } }

    [SerializeField]
    protected float baseCD;
    protected float currentCD;

    [SerializeField]
    protected static Transform holder;

    [SerializeField]
    protected Vector3 castOffset;
    public Vector3 CastPoint { get; protected set; }
    protected static int castDirX = 1;

    protected Movement movement;
    

    protected virtual void OnEnable () {
        holder = GameObject.FindWithTag (MyTags.projectileHolder.ToString ()).transform;
        movement = GetComponentInParent<Movement> ();
        CastPoint += transform.position + castOffset;
    }

    protected virtual bool CanCast () {
        if (isOnCooldown ()) {
            Debug.LogFormat ("Spell on cooldown. {0:0.0}s" , currentCD);
            return false;
        } else {
            currentCD = baseCD;
            return true;
        }
    }

    protected virtual void SetCastPoint () {
        CastPoint = new Vector3 (transform.position.x + ( castOffset.x * castDirX ) , transform.position.y + castOffset.y);
    }

    public virtual void Cast ( float speed , float size , GameObject owner ) { }
    public virtual void Cast ( ) {
        SetCastPoint ();
    }
    
    protected void Update () {
        if (isOnCooldown ()) {
            currentCD -= Time.deltaTime;
        }

        if (Input.GetAxisRaw("Horizontal") != 0) {
            int direction = (int) (Input.GetAxisRaw ("Horizontal"));
            castDirX = ( movement.isWallSliding ) ? -movement.DirX : direction;
        }
    }

    public bool isOnCooldown () {
        return ( currentCD > 0 );
    }

    void OnDrawGizmos () {
        Gizmos.DrawSphere (CastPoint , 0.25f);
    }

}