using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFire : Buffs {

    [SerializeField]
    private float slowPercent;
    private float interval = 0.3f;

    private Enemy target;
    [SerializeField]
    private Damage damage;
    private Coroutine dealDamageCoroutine;

    [SerializeField]
    private AudioClip consumeSound;
    [SerializeField]
    private ParticleSystem explosionParticles;
    [SerializeField]
    private ParticleSystem fireParticles;

    [SerializeField]
    private float pitchOffsetRange;
    private float pitchBase = 1;

    private AudioSource audioSource;

    private void Awake () {
        audioSource = GetComponent<AudioSource> ();

        target = GetComponentInParent<Enemy> ();
        target.OnDeath += OnTargetDeath;
    }

    private void OnEnable () {
        Refresh ();
    }

    // Update is called once per frame
    private void Update () {
        if (currentDuration <= 0f) {
            target.OnDeath -= OnTargetDeath;
            Destroy (gameObject);
        }
        currentDuration -= Time.deltaTime;
    }

    private void OnTargetDeath () {
        target.OnDeath -= OnTargetDeath;
        Destroy (gameObject);
    }

    public override void MovementUpdate ( ref Vector2 velocityMod ) {
        velocityMod.x = velocityMod.x - velocityMod.x * slowPercent / 100;
    }

    public void Refresh () {
        currentDuration = this.duration;

        if (dealDamageCoroutine == null) {
            dealDamageCoroutine = StartCoroutine (DealDamage (target));
        }
    }

    public IEnumerator DealDamage (Enemy target) {
        WaitForSeconds wfs = new WaitForSeconds (interval);
        while (gameObject.activeInHierarchy) {
            damage.DealDamage (target);
            yield return wfs;
        }
    }

    public void Consume () {
        transform.SetParent (transform.parent.parent);

        int dmg = Mathf.FloorToInt (damage.Amount * ( currentDuration / interval ));
        damage.DealDamage (dmg , target);
        Debug.LogFormat ("Enemy conflagrated for {0} dmg.", dmg);

        currentDuration = explosionParticles.main.duration;

        if (dealDamageCoroutine != null) {
            StopCoroutine (dealDamageCoroutine);
        }

        audioSource.clip = consumeSound;
        audioSource.loop = false;
        audioSource.Play ();

        fireParticles.Stop ();

        explosionParticles.gameObject.SetActive (true);
    }

}