using UnityEngine;

/// <summary>
///  Parent for every player class in the game
/// </summary>
public class Mastery : MonoBehaviour {

    protected float globalCD = 0f;

    public virtual void Spell1 () { }
    public virtual void Spell2 () { }
    public virtual void Spell3 () { }
    public virtual void Spell4 () { }

    private void LateUpdate () {
        if (inGlobalCD ()) {
            globalCD -= Time.deltaTime;
        }
    }

    public bool inGlobalCD () {
        return ( globalCD > 0 );
    }
}