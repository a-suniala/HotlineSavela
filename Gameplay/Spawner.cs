using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public bool devMode;

    public Enemy enemyPrefab;
    public Color flashColor = Color.red;
    public Wave[] waves;
    public Renderer tilePrefab;

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesAlive;
    float nextSpawnTime;

    MapGenerator map;

    float timeBetweenCampingChecks = 2;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    bool isDisabled;

    Color initialColor;

    public event System.Action<int> OnNewWave;

    public Wave CurrentWave {
        get {
            return currentWave;
        }
    }

    public int EnemiesLeftInWave () {
        return enemiesAlive;
    }

    void Start () {
        playerEntity = FindObjectOfType<Player> ();
        playerT = playerEntity.transform;

        initialColor = tilePrefab.sharedMaterial.color;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator> ();
        NextWave ();
    }

    void Update () {
        if (!isDisabled) {
            if (Time.time > nextCampCheckTime) {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                isCamping = (Vector3.Distance (playerT.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerT.position;
            }

            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime) {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine (SpawnEnemy ());
            }
        }

        if (devMode) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                StopCoroutine (SpawnEnemy ());
                foreach (Enemy enemy in FindObjectsOfType<Enemy> ()) {
                    Destroy (enemy.gameObject);
                }
                NextWave ();
            }
        }
    }

    IEnumerator SpawnEnemy () {
        float spawnDelay = 1f;
        float tileFlashSpeed = 4f;

        Transform spawnTile = map.GetRandomOpenTile ();
        if (isCamping) {
            spawnTile = map.GetTileFromPosition (playerT.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer> ().material;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay) {
            tileMat.color = Color.Lerp (initialColor, flashColor, Mathf.PingPong (spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }
        tileMat.color = initialColor;
        Enemy spawnedEnemy = Instantiate (enemyPrefab, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics (currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
    }

    void OnPlayerDeath () {
        isDisabled = true;
    }

    void OnEnemyDeath () {
        enemiesAlive--;

        if (enemiesAlive == 0) {
            NextWave ();
        }
    }

    void ResetPlayerPosition () {
        playerT.position = map.GetTileFromPosition (Vector3.zero).position + Vector3.up * 3;
    }

    void NextWave () {
        if (currentWaveNumber > 0) AudioManager.instance.PlaySound2D ("LevelComplete");
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length) {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesAlive = enemiesRemainingToSpawn;

            if (OnNewWave != null) {
                OnNewWave (currentWaveNumber);
            }
            ResetPlayerPosition ();
        }
    }

    [System.Serializable]
    public class Wave {
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;
    }

}
