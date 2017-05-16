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
    protected Transform holder;

    void Awake () {
        holder = GameObject.FindWithTag (MyTags.projectileHolder.ToString ()).transform;
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

    public virtual void Cast (int dirX, float speed, float size, Vector2 castPoint, GameObject owner) { }
    public virtual void Cast ( int dirX ) { }

    protected void Update () {
        if (isOnCooldown ()) {
            currentCD -= Time.deltaTime;
        }
    }

    public bool isOnCooldown () {
        return ( currentCD > 0 );
    }

}