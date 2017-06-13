using UnityEngine;
using System;

/// <summary>
/// Deals damage.
/// </summary>
[Serializable]
public class Damage {

    private Health targetHP;

    [SerializeField]
    private int baseAmount;

    public void DealDamage (int damage, Collider2D target ) {
        targetHP = target.GetComponent<Health> ();

        //damage type calculations go here
        Debug.LogFormat ("{0} took {1} damage." , target.name , damage);
        targetHP.LoseHP (damage);
    }

    public void DealDamage ( Collider2D target ) {
        DealDamage (baseAmount , target);
    }
}
