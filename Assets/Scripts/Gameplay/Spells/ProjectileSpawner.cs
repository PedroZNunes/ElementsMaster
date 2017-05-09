using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour {

    [SerializeField]
    protected Transform spellsHolder;
    Vector3 spawnPoint;

    void Awake () {
        spawnPoint = 
    }

    public void Spawn<T> (T projectileType) where T : Projectile {
        Instantiate (projectileType.prefab , spawnPoint , Quaternion.identity , spellsHolder);
    }
}
