using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFire : Buffs {

    [SerializeField]
    private float slowPercent;
    private int damageAmount;
    private float interval = 0.3f;

    private Enemy target;
    [SerializeField]
    private Damage damage;
    private Coroutine dealDamageCoroutine;

    private void OnEnable () {
        Refresh ();
    }

    // Update is called once per frame
    private void Update () {
        if (currentDuration <= 0f) {
            Destroy (gameObject);
        }
        currentDuration -= Time.deltaTime;
    }

    public override void MovementUpdate (ref Vector2 velocityMod) {
        velocityMod.x = velocityMod.x - velocityMod.x * slowPercent / 100;
    }

    private void OnDisable () {
        Destroy (gameObject);
    }

    public void Refresh () {
        target = GetComponentInParent<Enemy> ();
        currentDuration = this.duration;
        
        if (dealDamageCoroutine == null) {
            dealDamageCoroutine = StartCoroutine (DealDamage (target));
        }
    }

    public IEnumerator DealDamage (Enemy target) {
        while (gameObject.activeInHierarchy) {
            damage.DealDamage (target);
            yield return new WaitForSeconds (interval);
        }
    }

}