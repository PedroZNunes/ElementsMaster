using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour {
    
    protected float globalCooldown;

    void Update () {
        if (globalCooldown > 0) {
            globalCooldown -= Time.deltaTime;
        }
    }

    public virtual void CastSpell (int dirX) { }



}