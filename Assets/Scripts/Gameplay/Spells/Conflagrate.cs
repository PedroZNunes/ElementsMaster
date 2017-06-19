using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NOT IMPLEMENTED YET.
/// </summary>
public class Conflagrate : Spell {

    /* essa spell gera algum tipo de aviso qnd est[a disponivel.
     * ela faz isso atraves de o recebimento de um evento.
     * um evento que passa a posicao do creep em chamas. OnFire event
     */
    [SerializeField]
    private LayerMask enemiesMask;

    private bool isAvailable = false;

    protected override void OnEnable () {
        base.OnEnable ();
        StartCoroutine (CheckOnFireEnemies ());
    }

    public override void Cast () {
        if (CanCast ()) {
            foreach (OnFire onFire in FindObjectsOfType<OnFire>()) {
                if (onFire == null) {
                    continue;
                }
                onFire.Consume ();
            }
        }
    }

    protected override bool CanCast () {
        if (isOnCooldown ()) {
            Debug.LogFormat ("Spell on cooldown. {0:0.000}s" , currentCD);
        }
        else {
            if (isAvailable) {
                currentCD = baseCD;
                return true;
            }
        }
        return false;
    }


    private IEnumerator CheckOnFireEnemies () {
        while (isActiveAndEnabled) {
            OnFire onFire = FindObjectOfType<OnFire> (); ;
            if (onFire != null) {
                isAvailable = true;
            }
            else {
                isAvailable = false;
            }
            yield return new WaitForSeconds (0.2f);
        }
    }

}