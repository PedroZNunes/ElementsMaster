using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// parent of all spells. deals with global cd, cast point and direction
/// </summary>
public class Spell : MonoBehaviour {

    [SerializeField]
    protected float globalCD; //interval of time when the player cannot cast any sort of spells.
    public float GlobalCD { get { return globalCD; } protected set { globalCD = value; } }

    [SerializeField]
    protected float baseCD;
    protected float currentCD;

    [SerializeField]
    protected static Transform holder;

    [SerializeField]
    protected Vector3 castOffset; //distance from the player position where the spell should be cast from
    public Vector3 CastPoint { get; protected set; }
    protected static int castDirX = 1; //spell cast direction

    protected Movement movement;
    

    protected virtual void OnEnable () {
        holder = GameObject.FindWithTag (MyTags.projectileHolder.ToString ()).transform;
        movement = GetComponentInParent<Movement> ();
        CastPoint += transform.position + castOffset;
    }

    protected void Update () {
        if (isOnCooldown ()) {
            currentCD -= Time.deltaTime;
        }

        if (Input.GetAxisRaw ("Horizontal") != 0) {
            int direction = (int) ( Input.GetAxisRaw ("Horizontal") );
            castDirX = ( movement.isWallSliding ) ? -movement.DirX : direction;
        }
    }

    /// <summary>
    /// tests the global cooldown and local cooldown to see if the player is allowed to cast spells
    /// </summary>
    /// <returns>true if the player can cast spells </returns>
    protected virtual bool CanCast () {
        if (!isOnCooldown ()) {
            currentCD = baseCD;
            return true;
        }
        else {
            Debug.LogFormat ("Spell on cooldown. {0:0.000}s" , currentCD);
            return false;
        }
    }

    protected virtual void SetCastPoint () {
        CastPoint = new Vector3 (transform.position.x + ( castOffset.x * castDirX ) , transform.position.y + castOffset.y);
    }

    public virtual void Cast ( float speed , float size ) { }
    public virtual void Cast ( ) {
        SetCastPoint ();
    }

    public bool isOnCooldown () {
        return ( currentCD > 0 );
    }

    /// <summary>
    /// pool of objects from which to take spells that might have many instances.
    /// </summary>
    [Serializable]
    public class Pool {
        [SerializeField]
        private int size;
        [SerializeField]
        private GameObject prefab;

        public int Size { get { return size; } }
        public GameObject Prefab { get { return prefab; } }

        [HideInInspector]
        public GameObject[] instances;

        private Transform holder;

        //Can be made into a coroutine for performance purposes.
        public void Initialize ( Transform holder ) {
            this.holder = holder;

            instances = new GameObject[size];
            for (int i = 0 ; i < size ; i++) {
                instances[i] = Instantiate (prefab , holder);
                instances[i].name = prefab.name;
                instances[i].SetActive (false);
            }
        }

        public int CountActive () {
            int n = 0;
            for (int i = 0 ; i < instances.Length ; i++) {
                if (instances[i].activeInHierarchy) {
                    n++;
                }
            }
            return n;
        }

        public int CountInactive () {
            int n = 0;
            for (int i = 0 ; i < instances.Length ; i++) {
                if (!instances[i].activeInHierarchy) {
                    n++;
                }
            }
            return n;
        }

        public GameObject FindFreeObject () {
            for (int i = 0 ; i < instances.Length ; i++) {
                if (!instances[i].activeInHierarchy) {
                    return instances[i];
                }
            }
            return null;
        }
    }
}