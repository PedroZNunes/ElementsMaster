using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour {

    [SerializeField]
    private GameObject[] prefabs;
    private Transform holder;

    [SerializeField]
    private int interval;
    [SerializeField]
    private int intervalOffset;
    private WaitForSeconds intervalInSeconds;

    private List<GameObject> pool; //talvez uma array. talvez uma lista. talvez uma lista ordenada. talvez um dictionary com key difficulty.
    //a quantidade de cada inimigo na pool vai indicar qual a chance de tal inimigo ser spawnado. a pool controla a quantidade.
    //diferentes pools vao ser usadas para cada inimigo.
    //na hora de escolher de qual pool pegar o proximo inimigo, ele olha no count da pool quando faz o random. assim a maior pool vai sempre ter maior chance de spawnar mobs.

    private int poolActiveCount;

    [SerializeField]
    private int difficultyTarget; //difficulty target sera estipulada por um sistema de balanceamento. futuramente.
    
    private Coroutine spawningCoroutine;


    void Awake () {
        holder = this.transform;
        /* instanciar um numero poolCount de vezes o inimigo referente aquela pool.
         * aqui todos os inimigos devem ser instanciados e desativados para futuro uso durante o jogo.
         */
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

    private IEnumerator Spawn () {
        int difficultyCurrent;
        int difficultyDifference;
        /* A cada currentInterval segundos esse spawner vai olhar pra pool 
         * pegar cada difficulty de cada inimigo ativo e somar. comparar esse resultado com a dificuldade target.
         * a sobra vai ser a difficulty difference. ela vai servir pra decidir qual inimigo spawnar na proxima wave.
         * ele entao vai chamar um metodo para spawnar a wave e passar como parametro a dificuldade de diferença.
         */
        yield return intervalInSeconds;
    }

    private IEnumerator SpawnWave ( int difficultyRemaining ) {
        /* Ele vai receber a dificuldade que falta para chegar ao alvo.
         * Ele manda 
         * Faz um loop na POOL de inimigos consultando a dificuldade de cada um. compara então essa dificuldade com a restante. 
         * a ordem do loop vai ser aleatória. se o mob puder ser spawnado, ele será. como a pool possui mais inimigos faceis do que dificeis,
         * ele vai spawnar esses com mais frequencia. e quanto mais inimigos dificeis, menor a chance de spawnar outro. isso evita que tenha apenas inimigos dificeis de uma só vez.
         * após escolher o inimigo, ele chama um metodo que acha uma plataforma viável para spawnar o inimigo.
         * tendo o inimigo e a plataforma, eh soh inicializar o inimigo desejado usando a referencia da pool
         * a inicialização vai ser feita igual as spells sao castadas, porem será bem completa para evitar lixo de memória.
         * inicializa, move, ativa.
         */

        //estimate which enemy should spawn
        GameObject toInstantiate = SelectEnemyToSpawn ();
        if (toInstantiate == null) {
            Debug.Log ("Not enough difficulty to spawn an enemy. Spawn aborted.");
            return;
        }
        //choose a platform. this should be closer than the end of the screen + offset
        Vector2 pos = PickPlatform ();
        //spawn enemy under the holder;
        GameObject enemy = Instantiate (toInstantiate , pos , Quaternion.identity , holder);
        //recursive to allow multiple spawns
        if (n > 0) {
            Spawn (n - 1);
        }


        yield return null;
    }

    private GameObject SelectEnemyToSpawn () {
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

    private void SpawnEnemy () {

    }



    private Vector2 PickPlatform () {
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
