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
    protected int castDirX;

    protected Movement movement;
    

    protected void OnEnable () {
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

    protected virtual void SetCastPointAndDirection () {
        castDirX = ( movement.isWallSliding ) ? -movement.DirX : movement.DirX;
        CastPoint = new Vector3 (transform.position.x + ( castOffset.x * castDirX ) , transform.position.y + castOffset.y);
    }

    public virtual void Cast ( float speed , float size , GameObject owner ) { }
    public virtual void Cast ( ) {
        SetCastPointAndDirection ();
    }
    
    protected void Update () {
        if (isOnCooldown ()) {
            currentCD -= Time.deltaTime;
        }
    }

    public bool isOnCooldown () {
        return ( currentCD > 0 );
    }

    void OnDrawGizmos () {
        Gizmos.DrawSphere (CastPoint , 0.25f);
    }

}