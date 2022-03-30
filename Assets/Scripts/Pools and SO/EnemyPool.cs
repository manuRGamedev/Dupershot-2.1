using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    #region  VARIABLES
    public static EnemyPool instance;
    [Header("Pool de Enemigos")]
    [SerializeField] EnemyData[] enemies;
    [SerializeField] EnemyData[] asteroids;
    [SerializeField] Transform poolContainer;

    private List<GameObject>[] objectPools;
    [HideInInspector] public List<GameObject>[] particlePools;
    Enemy asteroidController;
    ParticleSystem particlesController;

    [Header("Variables de Spawn")]
    [SerializeField] float spawnDistanceLimits = 15f;
    [SerializeField] float spawnScreenLimits = 6f;
    //[SerializeField] float maxAsteroidScale = 3f;
    [Tooltip("Probabilidad de que los asteroides aparezcan en la trayectoria del jugador")]
    [SerializeField] [Range(1f, 100f)] float obstructionProb = 50f;
    float startSpawnRate;
    [Header("Spawn Rate")]
    [SerializeField]float currentSpawnRate;
    [SerializeField] float spawnsPerSecond = 0.6f;
    [SerializeField] float maxSpawnRate = 2.5f;

    
    [Space]
    [SerializeField] int maxEnemiesOnScreenStartValue = 10;
    [SerializeField] int maxEnemiesOnScreen = 25;
    [SerializeField] int currentMaxEnemiesOnScreen;
    int OnScreenAmountOfEnemies;

    bool isSpawning;

    [Header("Variables de Levelup")]
    [SerializeField] float spawnRateIncreaseValue = 0.125f;
    [SerializeField] int maxEnemiesIncreaseValue = 5;

    [Header("Variables de Destruccion")]
    [SerializeField] float particlesLifetime;
    [SerializeField] [Range(3, 50)] int particlesAmount = 15;
    [SerializeField] [Range(1, 15)] int amountErrorMargin = 3;
    [SerializeField] float emiterLifetime = 2f;

    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CreateThreatPools();
    }

    public void Initialize()
    {
        //Inicializa spawn rate
        startSpawnRate = spawnsPerSecond;
        currentSpawnRate = spawnsPerSecond;
        OnScreenAmountOfEnemies = 0;
        currentMaxEnemiesOnScreen = maxEnemiesOnScreenStartValue;

        foreach(EnemyData data in enemies)
        {
            data.Initialize();
        }

        StopAllCoroutines();
        //Comienza a instanciar asteroides
        Invoke("StartWave", 2f);
    }
    public void StartWave()
    {
        StartCoroutine(SpawnLoopCorroutine());
    }

    public void CreateThreatPools()
    {
        //Crea una matriz auxiliar para asignar todos los enemigos
        EnemyData[] allEnemies = new EnemyData[enemies.Length + asteroids.Length];

        //Se asignan enemigos y asteroides a dicha matriz
        for (int i = 0; i < enemies.Length; i++)
        {
            allEnemies[i] = enemies[i];
        }
        for (int i = 0; i < asteroids.Length; i++)
        {
            allEnemies[enemies.Length + i] = asteroids[i];
        }

        //Inicializa la pool de enemigos
        objectPools = new List<GameObject>[allEnemies.Length];
        //Inicializa la pool de particulas
        particlePools = new List<GameObject>[allEnemies.Length];

        for (int enemyDataIndex = 0; enemyDataIndex < allEnemies.Length; enemyDataIndex++)
        {
            EnemyData data = allEnemies[enemyDataIndex];

            //Initialize the data and assigns a new pool to it
            data.Initialize();
            GameObject newPool = new GameObject(data.name + "Pool");
            data.AssignContainer(Instantiate(newPool, poolContainer).transform);

            //Se asigna una lista para controlar las instancias
            List<GameObject> poolObjectList = new List<GameObject>();
            objectPools[enemyDataIndex] = poolObjectList;

            //Se asigna su pool de particulas
            List<GameObject> poolParticleList = new List<GameObject>();
            particlePools[enemyDataIndex] = poolParticleList;

            //Crea las instancias para ser manejadas en la pool
            for (int i = 0; i < data.numberOfInstances; i++)
            {
                //Instancia copias del Enemigo, se almacenan en el parent de la pool y se registran en una lista.
                GameObject newEnemy = Instantiate(data.GetEnemyPrefab(), data.enemyContainer);
                newEnemy.GetComponent<Enemy>().SetPool(objectPools[enemyDataIndex]);
                newEnemy.GetComponent<Enemy>().ID = enemyDataIndex;
                newEnemy.GetComponent<Enemy>().sizeInPool = data.spaceInPool;
                objectPools[enemyDataIndex].Add(newEnemy);
                newEnemy.SetActive(false);

                //Instancia copias de la particula, se almacenan en el parent de la pool y se registran en una lista.
                GameObject newParticle = Instantiate(data.particlesPrefab, data.enemyContainer);
                newParticle.GetComponent<Enemy>().SetPool(objectPools[enemyDataIndex]);
                newParticle.GetComponent<Enemy>().ID = enemyDataIndex;
                particlePools[enemyDataIndex].Add(newParticle);
                newParticle.SetActive(false);
            }
        }
    }

    int GetRandomPoolIndex()
    {
        //Se declaran las variables que determinan el resultado del proceso de selección.
        List<GameObject> choosenPool = null;
        int indexCount = -1;

        //Intenta escoger un enemigo de la pool de enemigos.
        if (enemies.Length > 0)
        {
            float maxProbability = 0;

            for (int i = 0; i < enemies.Length; i++)
            { 
                EnemyData data = enemies[i];

                Debug.Log(data.name + " => " + data.probabilityValue);
                if (Probability(data.probabilityValue))
                {
                    if (data.probabilityValue > maxProbability)
                    {
                        Debug.Log(data.name + " was choosen");
                        maxProbability = data.probabilityValue;
                        choosenPool = objectPools[i];
                        indexCount = i;
                    }
                }
            }
        }


        //Si no se escoge ningun enemigo, se instancia un asteroide
        if (choosenPool == null)
        {
            indexCount = enemies.Length;
            int totalAsteroidProbablity = 0;
            int currentProbability = 0;

            foreach (EnemyData data in asteroids)
            {
                totalAsteroidProbablity += (int)data.probabilityValue;
            }

            int r = Random.Range(0, totalAsteroidProbablity);

            for (int i = 0; i < asteroids.Length; i++)
            {
                if (r < (asteroids[i].probabilityValue + currentProbability))
                {
                    indexCount = enemies.Length + i;
                    choosenPool = objectPools[enemies.Length + i];
                    break;
                }
                else
                {
                    currentProbability += (int)asteroids[i].probabilityValue;
                }
            }

            //Se actualiza la probabilidad de aparición de cada enemigo
            foreach (EnemyData data in enemies)
            {
                data.UpdateProbability();
            }
        }
        else
        {
            //Se actualiza la probabilidad de aparición de cada enemigo
            for (int i = 0; i < enemies.Length; i++)
            {
                if (objectPools[i] == choosenPool)
                {
                    enemies[i].ReduceProbability();
                }
                else
                {
                    enemies[i].UpdateProbability();
                }
            }
        }

        //Antes de devolver un valor, se evalua si ha salido escogido algun enemigo

        if (choosenPool == null)
        {
            //Si no ha sido escogido ningún enemigo, devuelve un valor no válido
            return -1;
        }
        else if (choosenPool.Count <= 0)
        {
            //Si la pool no contiene ningún enemigo, devuelve un valor no válido
            return -1;
        }
        else
        {
            //Debug.Log("Selected pool index = " + indexCount.ToString());
            //Si ha salido escogida alguna pool de enemigos, devuelve su índice
            return Mathf.Clamp(indexCount, 0, objectPools.Length - 1);
        }        
    }

    public void LevelUpEnemies(int value)
    {
        switch (value % 3)
        {
            case 0:
                //Aumenta la frecuencia de spawn en general (reduce el tiempo de espera entre spawns)
                currentSpawnRate = Mathf.Clamp(currentSpawnRate + spawnRateIncreaseValue, spawnsPerSecond, maxSpawnRate);
                break;

            case 1:
                //Aumenta la cantidad de enemigos que pueden estar presentes en escena
                currentMaxEnemiesOnScreen = Mathf.Clamp(currentMaxEnemiesOnScreen + maxEnemiesIncreaseValue, maxEnemiesOnScreenStartValue, maxEnemiesOnScreen);
                break;

            case 2:  
                foreach (EnemyData enemy in enemies)
                {
                    enemy.LevelUpEnemy();
                }
                break;
        }
    }    

    public void SetAsteroidActive()
    {
        if (OnScreenAmountOfEnemies >= currentMaxEnemiesOnScreen) { return; }        

        //Se identifican las variables
        int poolIndex = GetRandomPoolIndex();
        var currentPool = objectPools[0];

        //Si la seleccion de enemigos devuelve un valor válido, SE CONTINUA con el proceso de spawn
        if (poolIndex >= 0 && poolIndex <= objectPools.Length)
        {
            currentPool = objectPools[poolIndex];
        }
        //Si la seleccion de enemigos NO devuelve un valor válido, SE DETIENE el proceso
        else
        {
            return;
        }

        var newThreat = currentPool[0];

        try
        {
            OnScreenAmountOfEnemies += newThreat.GetComponent<Enemy>().sizeInPool;
        }
        catch
        {
            OnScreenAmountOfEnemies++;
        }

        Enemy controller;

        //Se randomiza una ubicacion de spawn
        int randomSpawnCase;
        if (Probability(obstructionProb)) randomSpawnCase = PlayerController.instance.movementState;
        else randomSpawnCase = Random.Range(1, 5);

        float x = 0, y = 0;
        switch (randomSpawnCase)
        {
            case 1:
                x = Random.Range(-spawnScreenLimits, spawnScreenLimits);
                y = spawnDistanceLimits + Random.Range(-0.1f, 0.1f);
                break;

            case 2:
                x = spawnDistanceLimits + Random.Range(-0.1f, 0.1f);
                y = Random.Range(-spawnScreenLimits, spawnScreenLimits);
                break;

            case 3:
                x = Random.Range(-spawnScreenLimits, spawnScreenLimits);
                y = -spawnDistanceLimits + Random.Range(-0.1f, 0.1f);
                break;

            case 4:
                x = -spawnDistanceLimits + Random.Range(-0.1f, 0.1f);
                y = Random.Range(-spawnScreenLimits, spawnScreenLimits);
                break;
        }

        //Se instancia un ENEMIGO en la posición elegida
        newThreat.transform.position = new Vector3(x, y, 0) + PlayerController.instance.transform.position;               

        //Se activa
        newThreat.SetActive(true);

        //Se retira de la pool
        currentPool.Remove(newThreat);
        //Debug.Log(currentPool.ToString() + currentPool.Count);
    }

    public void SetAshesActive(Vector3 position, int id)
    {
        //Se selecciona un set de particulas
        var newParticles = particlePools[id][0];

        //Se asigna una posición y se reconoce el particle system
        newParticles.transform.position = position;
        ParticleSystem controller = newParticles.GetComponent<ParticleSystem>();

        //Se retira de la lista
        particlePools[id].Remove(newParticles);

        //Se activa, y se emiten particulas
        newParticles.SetActive(true);
    }

    public void ReturnToPool(GameObject poolObject, List<GameObject> pool, int size)
    {
        if (!pool.Contains(poolObject))
        {
            OnScreenAmountOfEnemies -= size;
            pool.Add(poolObject);
            poolObject.SetActive(false);
        }
    }

    public void ReturnParticlesToPool(GameObject poolObject, List<GameObject> pool)
    {
        if (!pool.Contains(poolObject))
        {
            pool.Add(poolObject);
            poolObject.SetActive(false);
        }
    }

    bool Probability(float condition)
    {
        condition = Mathf.Clamp(condition, 0, 100);

        int r = Random.Range(1, 100);

        if (r <= condition) return true;
        else return false;
    }

    public void SetSpawning(bool value)
    {
        isSpawning = value;
    }
    
    IEnumerator SpawnLoopCorroutine()
    {
        do
        {
            if (GameManager.instance.state == GameState.Playing && isSpawning) 
            {
                SetAsteroidActive();
            }

            yield return new WaitForSeconds(1 / currentSpawnRate);

        } while (GameManager.instance.state != GameState.GameOver);
    }

}

[System.Serializable]
public class AsteroidVariation
{
    public Sprite asteroidSprite;
    public Vector2 spriteScale = new Vector2(2,2);    
}
