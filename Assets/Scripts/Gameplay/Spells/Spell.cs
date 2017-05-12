using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour {

    [SerializeField]
    protected float globalCD;
    public float GlobalCD { get { return globalCD; } protected set { globalCD = value; } }

    [SerializeField]
    protected float baseCooldown;

    protected float currentCooldown;

    protected virtual bool CanCast () {
        if (isOnCooldown ()) {
            Debug.LogFormat ("Spell on cooldown. {0:0.0}s" , currentCooldown);
            return false;
        } else {
            currentCooldown = baseCooldown;
            return true;
        }
    }

    public virtual void Cast (int dirX, float speed, float size, Vector2 castPoint, GameObject owner) { }

    protected void Update () {
        if (isOnCooldown ()) {
            currentCooldown -= Time.deltaTime;
        }
    }

    public bool isOnCooldown () {
        return ( currentCooldown > 0 );
    }

}