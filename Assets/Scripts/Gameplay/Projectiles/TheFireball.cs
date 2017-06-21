using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

/// <summary>
/// component to be attached to the fireball instance
/// </summary>
[RequireComponent(typeof (Controller2D))]
public class TheFireball : Projectile {

    [SerializeField]
    private BuffedFireball buffedFireball;

    [SerializeField]
    private GameObject OnFirePrefab;

    private Knockback knockback;
    
    private int dirX;
    private float speed;

    private Damage damage;

    private bool isBuffed;

    [SerializeField]
    private ParticleSystem particles;

    [SerializeField]
    private ParticleSystem particlesExplosion;
    
    [SerializeField]
    private AudioClip explosionSound;
    [SerializeField]
    private AudioClip awakeSound;
    [SerializeField]
    private float pitchOffsetRange;
    private float pitchBase = 1;
    [SerializeField]
    private AudioSource awakeAudioSource;
    [SerializeField]
    private AudioSource explosionAudioSource;

    private Controller2D controller;
    private BoxCollider2D collider;

    private ParticleSystem.MinMaxGradient defaultGradient;

    /// <summary>
    /// initialize the component, ensuring that the reuse of it (from the pool) won't be affected by the previous lifetime
    /// </summary>
    /// <param name="dirX">horizontal direction</param>
    /// <param name="spawnPosition"></param>
    /// <param name="speed"></param>
    /// <param name="size"></param>
    /// <param name="maxDuration"></param>

    private void Awake () {
        defaultGradient = particles.colorOverLifetime.color;
    }

    public void Initialize ( int dirX , Vector2 spawnPosition , ref float speed , ref Vector2 size , ref Damage damage , ref Knockback knockback , ref float maxDuration ) {

        ResetBuff ();
        
        gameObject.SetActive (true);

        this.dirX = dirX;

        transform.position = spawnPosition;

        this.speed = speed * dirX;

        transform.localScale = new Vector3 (size.x , size.y , 1);

        controller = GetComponent<Controller2D> ();
        collider = controller.collider;

        this.damage = damage;
        this.knockback = knockback;

        ParticleSystem.ColorOverLifetimeModule colorModule = particles.colorOverLifetime;
        colorModule.color = defaultGradient;

        awakeAudioSource.clip = awakeSound;
        awakeAudioSource.pitch = Random.Range (pitchBase - pitchOffsetRange , pitchBase + pitchOffsetRange);
        awakeAudioSource.Play ();

        StartCoroutine (Die (maxDuration)); //TODO: projectiles object pool for memory fragmentation
    }

    private void Update () {
        controller.Move (Vector2.right * ( speed * Time.deltaTime ));
    }

    private void OnController2DTrigger ( Collider2D col ) {
        if (collider.enabled) {
            if (col.GetComponent<TheFireWall> () != null) {
                TheFireWall theFireWall = col.GetComponent<TheFireWall> ();
                Buff ();
            }
            Enemy e = col.GetComponent<Enemy> ();
            if (e != null) {
                damage.DealDamage (e);
                knockback.Push (col , dirX);
                if (isBuffed) {
                    SetOnFire (col.gameObject);
                }

                Explode ();
            } else if (col.GetComponent<Block> () != null) {
                ColliderDistance2D colDist = col.Distance (collider);
                Vector3 dist = ( colDist.pointB - colDist.pointA );
                transform.position = transform.position - dist;
                Explode ();
            }
        }
    }

    private void SetOnFire ( GameObject target) {
        OnFire onFire = target.GetComponentInChildren<OnFire> ();
        if (onFire == null) {
            Instantiate (OnFirePrefab , target.transform.position, Quaternion.identity , target.transform);
        }
        else {
            onFire.Refresh ();
        }
    }

    private void ResetBuff () {
        isBuffed = false;
        particles.transform.localScale = Vector3.one;
        
    }

    private void Buff () {
        if (!isBuffed) {
            isBuffed = true;

            particles.transform.localScale = Vector3.one * buffedFireball.size;

            ParticleSystem.ColorOverLifetimeModule colorModule = particles.colorOverLifetime;
            ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient (buffedFireball.gradientMin , buffedFireball.gradientMax);
            colorModule.color = gradient;
        }
    }

    private void Explode () {
        collider.enabled = false; 
        particles.Stop ();

        explosionAudioSource.pitch = Random.Range (pitchBase - pitchOffsetRange , pitchBase + pitchOffsetRange);

        particlesExplosion.gameObject.SetActive (true);

        //audioSource.clip = explosionSound;
        //audioSource.Play ();

        StartCoroutine (Die (particlesExplosion.main.startLifetime.constantMax));
    }

    /// <summary>
    /// works like the unityengine.destroy method, but deactivating it for the pool
    /// </summary>
    /// <param name="time">time after which the object will die</param>
    /// <returns></returns>
    private IEnumerator Die (float time) {
        while (time > 0) {
            time -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate ();
        }
        collider.enabled = true;
        particlesExplosion.gameObject.SetActive (false);
        gameObject.SetActive (false);
    }

    [Serializable]
    private struct BuffedFireball {
        public float size;
        public Color gradientMin;
        public Color gradientMax;
        private Knockback knockback;
    }
}
