using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[RequireComponent (typeof (PolygonCollider2D))]
public sealed class EnemySpawner : MonoBehaviour {

    private HashSet<Collider2D> availableColliders = new HashSet<Collider2D> ();
    public HashSet<Collider2D> AvailableColliders { get { return availableColliders; } }

    [SerializeField]
    private Range waveInterval; //interval between waves
    [SerializeField]
    private Range waveDuration; //base duration for spawning the wave's enemies
    [SerializeField]
    private int waveDifficultyMin; //minimum difficulty to spawn a wave.
    private int difficultyMin; //minimum difficulty possible to spawn

    [SerializeField]
    private Pool[] pools; //one pool for each type of enemy. each pool provides the difficulty also.
    [SerializeField]
    private Vector2 spawningAreaSize;

    [SerializeField]
    private int difficultyTarget; //difficulty target will be stipulated by a balance system in the near future.

    [SerializeField]
    private LayerMask obstacleLayerMask;

    [SerializeField]
    private string holderName;
    private Transform holder;

    private PolygonCollider2D col;
    private Coroutine spawningCoroutine;
    private Player player;
    
    private void Awake () {
        GameObject holderGO = (GameObject) Instantiate (new GameObject () , transform.parent);
        holderGO.name = holderName;
        holder = holderGO.transform;

        col = GetComponent<PolygonCollider2D> ();
        player = FindObjectOfType<Player> ();
        // Filling the pools
        /* instanciar um numero poolCount de vezes o inimigo referente aquela pool.
         * aqui todos os inimigos devem ser instanciados e desativados para futuro uso durante o jogo.
         */
        for (int i = 0 ; i < pools.Length ; i++) {
            pools[i].Initialize (holder);
            difficultyMin = ( pools[i].Difficulty < difficultyMin ) ? pools[i].Difficulty : difficultyMin;
        }
        float before = Time.realtimeSinceStartup;
        MapPlatforms ();
        Debug.LogFormat ("Map platforms consumed {0} seconds.", Time.realtimeSinceStartup - before);
    }

    private void OnEnable () { GameManager.stateChangedEvent += OnStateChanged; }
    private void OnDisable () { GameManager.stateChangedEvent -= OnStateChanged; }

    /// <summary>
    /// sets which platforms can be used as spawning point.
    /// </summary>
    private void MapPlatforms () {
        float distance = 0.5f;
        ContactFilter2D filter = new ContactFilter2D ();
        filter.SetLayerMask (obstacleLayerMask);

        List<Collider2D> colList = new List<Collider2D> ();
        Collider2D[] allBlocks = new Collider2D[4000];
        RaycastHit2D[] hit = new RaycastHit2D[1];

        col.OverlapCollider (filter , allBlocks);

        foreach (Collider2D c in allBlocks) {
            if (c == null) {
                continue;
            }

            int n = c.Raycast (Vector2.up , filter , hit , distance);
            if (n == 0) {
                availableColliders.Add (c);
                continue;
            }
        }

        Debug.LogFormat ("Mapping finished. {0} available colliders for spawning" , AvailableColliders.Count);
    }

    public void StartSpawning () {
        if (spawningCoroutine == null) {
            spawningCoroutine = StartCoroutine (Spawn ());
        }
    }

    public void StopSpawning () {
        if (spawningCoroutine != null) {
            StopCoroutine (spawningCoroutine);
        }
    }

    /// <summary>
    /// calculates difficulty difference and calls a method to spawn the wave passing the difference as parameter.
    /// </summary>
    private IEnumerator Spawn () {
        while (true) {
            WaitForSeconds intervalInSeconds = new WaitForSeconds (Random.Range (waveInterval.Min , waveInterval.Max));

            int difficultyCurrent = 0;
            foreach (Pool pool in pools) {
                difficultyCurrent += pool.ActiveDifficulty ();
            }

            int difficultyDifference = difficultyTarget - difficultyCurrent;
            Debug.Log ("Difficulty Difference: " + difficultyDifference);
            if (difficultyDifference >= waveDifficultyMin) {
                StartCoroutine (SpawnWave (difficultyDifference));
            }

            yield return intervalInSeconds;
        }
    }

    /// <SpawnWaveSummary>
    /// receives the difficulty remaining, calls a method to pick random enemies to be spawned according to the difficulty remaining in the wave.
    /// then calls a method to pick a platform for the enemy to be spawned on
    /// loops thrugh the list of enemies to be spawned calling another method to initialize and activate each one
    /// </SpawnWaveSummary>
    private IEnumerator SpawnWave ( int difficultyRemaining ) {
        Queue<GameObject> toSpawn = ChooseEnemiesToSpawn (ref difficultyRemaining);
        float waveTotalDuration = Random.Range (waveDuration.Min , waveDuration.Max);
        float subWaveInterval = waveTotalDuration / toSpawn.Count;
        float subWaveIntervalOffset = 0.3f;

        while (toSpawn.Count > 0) {
            WaitForSeconds wfs = new WaitForSeconds (Random.Range (subWaveInterval - subWaveIntervalOffset, subWaveInterval + subWaveIntervalOffset));
            //spawn enemy
            Vector2 spawnPos = PickPlatform ();
            //Debug.LogFormat ("{0} spawned. t: {1}." , toSpawn.Peek ().name, Time.time);
            GameObject spawned = toSpawn.Dequeue ();
            Enemy e = spawned.GetComponent<Enemy> ();
            if (e!= null) {
                e.Initialize (spawnPos);
            }
            else {
                Debug.LogError ("Enemy component not found. spawned: " + spawned.GetInstanceID());
            }
            
            yield return wfs;
        }
    }

    /// <ChooseEnemiesToSpawn>
    /// randomly picks an enemy with difficulty below the remain
    /// adds the enemy to the list if the pool has available enemies
    /// </ChooseEnemiesToSpawn>
    /// <param name="difficultyRemaining"> difference between current and target difficulty </param>
    /// <returns>
    /// a list of objects to spawn in the wave in arbitrary order
    /// </returns>
    private Queue<GameObject> ChooseEnemiesToSpawn ( ref int difficultyRemaining ) {
        
        Queue<GameObject> toSpawn = new Queue<GameObject> ();
        while (difficultyRemaining > difficultyMin) {
            List<Pool> poolsAvailable = new List<Pool> ();
            int enemiesTotal = 0;

            foreach (Pool pool in pools) {
                if (pool.Difficulty <= difficultyRemaining) {
                    poolsAvailable.Add (pool);
                    enemiesTotal += pool.Size;
                }
            }

            if (enemiesTotal == 0) {
                return new Queue<GameObject> ();
            }

            //pick enemies
            int index = Random.Range (0 , enemiesTotal - 1);
            foreach (Pool pool in poolsAvailable) {
                if (pool.Size <= index) {
                    index -= pool.Size;
                }
                else {
                    GameObject go = (GameObject) pool.instances.GetValue (index);
                    if (go != null) {
                        if (!go.activeInHierarchy && !toSpawn.Contains(go)) {
                            difficultyRemaining -= pool.Difficulty;
                            toSpawn.Enqueue (go);
                            break;
                        }
                    }
                }
            }
        }
        return toSpawn;
    }

    /// <summary>
    /// overlaps a capsule of spawning area size
    /// compare a random index from that list to the available colliders (defined by the mapping)
    /// </summary>
    /// <returns> a vector2 representing the top-most centered position of the block. if nothing is found, returns a vector.zero</returns>
    private Vector2 PickPlatform () {
        Vector2 origin = player.transform.position;
        List<Collider2D> cols = new List<Collider2D> (Physics2D.OverlapCapsuleAll (origin , spawningAreaSize , CapsuleDirection2D.Horizontal , 0));
        for (int i = 0 ; i < cols.Count ; i++) {
            int index = Random.Range (0 , cols.Count - 1);
            if (AvailableColliders.Contains (cols[index])) {
                return new Vector2 (cols[index].bounds.center.x , cols[index].bounds.max.y);
            }
        }
        return Vector2.zero;
    }

    /// <summary>
    /// function linked to an event to track when states change. this takes care of starting and stopping the spawn system.
    /// </summary>
    /// <param name="oldState"></param>
    /// <param name="newState"></param>
    private void OnStateChanged ( GameManager.States oldState , GameManager.States newState ) {
        if (newState == GameManager.States.Play && oldState != GameManager.States.Pause) {
            StartSpawning ();
        }
        if (oldState == GameManager.States.Play && newState != GameManager.States.Pause) {
            StopSpawning ();
        }
    }

    /// <summary>
    /// object pool to avoid fragmentation.
    /// </summary>
    [Serializable]
    public class Pool {
        [SerializeField]
        private int difficulty;
        [SerializeField]
        private int size;
        [SerializeField]
        private GameObject prefab;

        public int Difficulty { get { return difficulty; } }
        public int Size { get { return size; } }
        public GameObject Prefab { get { return prefab; } }

        [HideInInspector]
        public GameObject[] instances;

        private Transform holder;

        /// <summary>
        /// spawn all objects and deactivate them.
        /// </summary>
        /// <param name="holder"> transform responsible for storing the instances </param>
        public void Initialize (Transform holder) {
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

        public int ActiveDifficulty () {
            int n = 0;
            for (int i = 0 ; i < instances.Length ; i++) {
                if (instances[i].activeInHierarchy) {
                    n+= difficulty;
                }
            }
            return n;
        }
    }

    [Serializable]
    public struct Range {
        [SerializeField]
        private int min;
        public int Min { get { return min; } }
        [SerializeField]
        private int max;
        public int Max { get { return max; } }

        public Range ( int min , int max ) {
            this.min = min;
            this.max = max;
        }
    }
}
