using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spell : MonoBehaviour {

    [SerializeField]
    protected float globalCD;
    public float GlobalCD { get { return globalCD; } protected set { globalCD = value; } }

    [SerializeField]
    protected float baseCD;
    protected float currentCD;

    [SerializeField]
    protected static Transform holder;

    [SerializeField]
    protected Vector3 castOffset;
    public Vector3 CastPoint { get; protected set; }
    protected static int castDirX = 1;

    protected Movement movement;
    

    protected virtual void OnEnable () {
        holder = GameObject.FindWithTag (MyTags.projectileHolder.ToString ()).transform;
        movement = GetComponentInParent<Movement> ();
        CastPoint += transform.position + castOffset;
    }

    protected virtual bool CanCast () {
        if (isOnCooldown ()) {
            Debug.LogFormat ("Spell on cooldown. {0:0.0}s" , currentCD);
            return false;
        } else {
            currentCD = baseCD;
            return true;
        }
    }

    protected virtual void SetCastPoint () {
        CastPoint = new Vector3 (transform.position.x + ( castOffset.x * castDirX ) , transform.position.y + castOffset.y);
    }

    public virtual void Cast ( float speed , float size , GameObject owner ) { }
    public virtual void Cast ( ) {
        SetCastPoint ();
    }
    
    protected void Update () {
        if (isOnCooldown ()) {
            currentCD -= Time.deltaTime;
        }

        if (Input.GetAxisRaw("Horizontal") != 0) {
            int direction = (int) (Input.GetAxisRaw ("Horizontal"));
            castDirX = ( movement.isWallSliding ) ? -movement.DirX : direction;
        }
    }

    public bool isOnCooldown () {
        return ( currentCD > 0 );
    }

    void OnDrawGizmos () {
        Gizmos.DrawSphere (CastPoint , 0.25f);
    }


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