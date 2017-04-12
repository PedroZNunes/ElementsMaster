using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField]
    LayerMask obstacleCollisionMask;
    [SerializeField]
    List<GameObject> enemyPrefabs = new List<GameObject> ();
    [SerializeField]
    float spawnIntervalBase;
    [SerializeField]
    string holderName = "Enemies";
    Transform enemyHolder;

    Coroutine spawningCoroutine;

    List<Vector2> coordinatesUsed = new List<Vector2> ();

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
        //loop the enemies at random
        //if (currentDifficulty + enemyDifficulty <= maxDifficulty)
        //return the prefab
        //else if (currentDifficulty + enemyDifficulty < minDifficulty)
        //pick something stronger or decrease the delay
        return new GameObject();
    }

    void Spawn () {
        Spawn (1);
    }

    void Spawn ( int n ) {
        if (n == 0) { return; }
        //estimate which enemy should spawn
        GameObject toInstantiate = SelectEnemyToSpawn ();
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
