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

    public void DealDamage (int damage, Enemy target ) {
        target.LoseHealth (damage);
        //damage type calculations go here
        Debug.LogFormat ("{0} took {1} damage." , target.name , damage);
    }

    public void DealDamage ( Enemy target ) {
        DealDamage (baseAmount , target);
    }
}
