using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    [SerializeField]
    private int maxHP;
    public int MaxHP { get { return maxHP; } }

    [SerializeField]
    private int currentHP = 0;
    public int CurrentHP {
        get { return currentHP; }
        private set { currentHP = Mathf.Clamp (value , 0 , MaxHP); }
    }

    private void Awake () {
        CurrentHP = MaxHP;
    }

    public void LoseHP (int dmg) {
        CurrentHP -= Mathf.Abs(dmg);
    }

    public void GainHP (int heal) {
        CurrentHP += Mathf.Abs(heal);
    }

    public void CheckDeath () {
        if (currentHP <= 0) {
            Debug.LogFormat ("{0} is dead." , gameObject.name); 
        }
    }

}
