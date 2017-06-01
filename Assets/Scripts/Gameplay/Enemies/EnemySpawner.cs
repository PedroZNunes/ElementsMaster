using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public sealed class EnemySpawner : MonoBehaviour {

    private HashSet<Collider2D> availableColliders = new HashSet<Collider2D> ();
    public HashSet<Collider2D> AvailableColliders { get { return availableColliders; } }

    [SerializeField]
    private Range waveInterval;
    [SerializeField]
    private Range waveDuration;
    [SerializeField]
    private int waveDifficultyMin;

    private WaitForSeconds intervalInSeconds;
    
    //a quantidade de cada inimigo na pool vai indicar qual a chance de tal inimigo ser spawnado. a pool controla a quantidade.
    //diferentes pools vao ser usadas para cada inimigo.
    //na hora de escolher de qual pool pegar o proximo inimigo, ele olha no size da pool quando faz o random. assim a maior pool vai sempre ter maior chance de spawnar mobs.
    [SerializeField]
    private Pool[] pools;
    [SerializeField]
    private Vector2 spawningAreaSize;

    [SerializeField]
    private int difficultyTarget; //difficulty target sera estipulada por um sistema de balanceamento futuramente.
    private int difficultyMin;

    [SerializeField]
    private LayerMask obstacleLayerMask;

    private Transform holder;
    private PolygonCollider2D col;
    private Coroutine spawningCoroutine;
    private Player player;
    
    void Awake () {
        GameObject holderGO = (GameObject) Instantiate (new GameObject () , transform.parent);
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

    private void OnStateChanged (GameManager.States oldState, GameManager.States newState) {
        if (newState == GameManager.States.Play) {
            StartSpawning ();
        }
        if (oldState == GameManager.States.Play) {
            StopSpawning ();
        }
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
            if (difficultyDifference > waveDifficultyMin) {
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

        Debug.LogFormat ("Mapping finished. {0} available colliders for spawning", AvailableColliders.Count);
    }

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

        //Can be made into a coroutine for performance purposes.
        public void Initialize (Transform holder) {
            this.holder = holder;

            instances = new GameObject[size];
            for (int i = 0 ; i < size ; i++) {
                instances[i] = Instantiate (prefab , holder);
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
