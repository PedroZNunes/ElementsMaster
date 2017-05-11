using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour {
    
    private static float globalCooldown;

    void LateUpdate () {
        if (inGlobalCD ()) {
            globalCooldown -= Time.deltaTime;
        }
    }

    public bool inGlobalCD () {
        return ( globalCooldown > 0 );
    }

    public virtual void Cast (short dirX, float speed, float size, Vector2 castPoint, GameObject owner) { }

}