using UnityEngine;

/// <summary>
/// Deals damage.
/// </summary>
public class Damage : MonoBehaviour {

    Health targetHP;

    public bool DealDamage ( int damage, GameObject target ) {
        targetHP = target.GetComponent<Health> ();
        if (targetHP == null)
            return false;

        //damage type calculations go here
        Debug.LogFormat ("{0} took {1} damage." , target.name , damage);
        targetHP.LoseHP (damage);
        return true;
    }
}
