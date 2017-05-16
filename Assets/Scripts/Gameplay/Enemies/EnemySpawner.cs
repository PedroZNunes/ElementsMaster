using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour {

    [SerializeField]
    private LayerMask obstacleCollisionMask;
    [SerializeField]
    private List<GameObject> enemyPrefabs = new List<GameObject> ();
    [SerializeField]
    private float spawnIntervalBase;    //the interval should be based on the difficulty per second that the game gets.
                                //allowing for faster spawns later. It should not spawn always at the same interval, though.
    [SerializeField]
    private string holderName = "Enemies";
    private Transform enemyHolder;

    private Coroutine spawningCoroutine;

    private List<Vector2> coordinatesUsed = new List<Vector2> ();

    void Awake () {
        enemyHolder = new GameObject (holderName).transform;
    }

    public void StartSpawning () {
        if (spawningCoroutine == null) {
            spawningCoroutine = StartCoroutine (SpawningProcess (spawnIntervalBase));
        }
    }

    public void StopSpawning () {
        if (spawningCoroutine != null) {
            StopCoroutine (spawningCoroutine);
        }
    }

    IEnumerator SpawningProcess (float spawnInterval) {
        while (true) {
            Spawn ();
            yield return new WaitForSeconds (spawnInterval);
        }
    }

    GameObject SelectEnemyToSpawn () {
        //Count how many enemies are on the screen
        Enemy[] enemy = FindObjectsOfType<Enemy> ();
        for (int i = 0 ; i < enemy.Length; i++) {
            if (enemy[i].gameObject.CompareTag (MyTags.enemy.ToString ())){
                //count the amount of medium creeps
                //ps. this could be done in a dictionary<Enemy, EnemyCount>. fill up the dictionary every frame
            }
            //TODO: sum up the difficultys in a variable
        }
        
        
        //loop the enemies at random until there are no enemies to test
        //if (currentDifficulty + enemyDifficulty <= maxDifficulty)
        //return the prefab
        //else if (currentDifficulty + enemyDifficulty < minDifficulty)
        //loop next


        return null;    //if there are no enemies to spawn, return nothing;
    }

    void Spawn () {
        Spawn (1);
    }

    void Spawn ( int n ) {
        if (n == 0) { return; }
        //estimate which enemy should spawn
        GameObject toInstantiate = SelectEnemyToSpawn ();
        if (toInstantiate == null) {
            Debug.Log ("Not enough difficulty to spawn an enemy. Spawn aborted.");
            return;
        }
        //choose a platform. this should be closer than the end of the screen + offset
        Vector2 pos = PickPlatform ();
        //spawn enemy under the holder;
        GameObject enemy = Instantiate(toInstantiate, pos , Quaternion.identity, enemyHolder);
        //recursive to allow multiple spawns
        if (n > 0) {
            Spawn (n - 1);
        }
    }

    Vector2 PickPlatform () {
        //player position and camera size and offset will determine the zone in which the enemy can spawn
        Vector2 playerPos = Player.GetPosition ();
        Vector2 areaOffset = new Vector2 (Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
        Vector2 minBound = playerPos - ( areaOffset );
        Vector2 maxBound = playerPos + ( areaOffset );

        //check for random X and Y and see if that is above a platform? 100% random
        Vector2 randomCoordinates = new Vector2 ();
        randomCoordinates.x = Random.Range (minBound.x , maxBound.x);
        randomCoordinates.y = Random.Range (minBound.y , maxBound.y);
        if (coordinatesUsed.Contains (randomCoordinates)) {
            return PickPlatform ();
        }

        float halfEnemyHeight = 0.5f; //this is not right. FIXME: all enemies should have the same distance from the center to the ground of 0.5f (I THINK) however this cant be hardcoded here
        RaycastHit2D hit = Physics2D.Raycast (randomCoordinates , Vector2.down , halfEnemyHeight , obstacleCollisionMask);
        if (hit) {
            return randomCoordinates;
        }
        else {
            coordinatesUsed.Add (randomCoordinates);
            return PickPlatform ();
        }
        //check the platformlist for the X and Y 
    }
}
