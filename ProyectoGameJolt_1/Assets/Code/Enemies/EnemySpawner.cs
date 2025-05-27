using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class EnemyPrefabData
{
    public GameObject prefab;
    [Range(0f, 100f)]
    public float probability = 50f;
}

public class EnemySpawner : MonoBehaviour
{
    public List<EnemyPrefabData> enemyPrefabs = new List<EnemyPrefabData>();
    public float spawnInterval = 3f;
    public bool startSpawningOnStart = false;

    [Header("Configuración de Oleadas por Ronda")]
    [SerializeField] private int wavesPerRoundIncrease = 2;
    [SerializeField] private int currentRoundWavesLimit = 0;
    [SerializeField] private int wavesSpawnedThisRound = 0;

    [SerializeField] private int currentFibonacciIndex = 0;
    [SerializeField] private int currentFibonacciValue = 1;

    [SerializeField] private bool isSpawning = false;
    [SerializeField] private int totalWavesSpawned = 0;
    [SerializeField] private int totalEnemiesSpawned = 0;

    [SerializeField] private List<int> fibonacciSequence = new List<int>();

    private Coroutine spawnCoroutine;

    void Start()
    {
        InitializeFibonacci();
        CalculateCurrentRoundWavesLimit();

        if (startSpawningOnStart)
        {
            StartSpawning();
        }
    }

    void InitializeFibonacci()
    {
        fibonacciSequence.Clear();

        int a = 0, b = 1;
        fibonacciSequence.Add(a);
        fibonacciSequence.Add(b);

        for (int i = 2; i < 20; i++)
        {
            int next = a + b;
            fibonacciSequence.Add(next);
            a = b;
            b = next;
        }

        //Empieza con 1
        currentFibonacciIndex = 1;
        currentFibonacciValue = fibonacciSequence[currentFibonacciIndex];
    }

    void CalculateCurrentRoundWavesLimit()
    {
        int currentRound = 1;
        if (RoundManager.Instance != null)
        {
            currentRound = RoundManager.Instance.GetCurrentRound();
        }

        currentRoundWavesLimit = wavesPerRoundIncrease + currentRound;
        wavesSpawnedThisRound = 0;
    }

    int GetNextFibonacci()
    {
        currentFibonacciIndex++;

        if (currentFibonacciIndex >= fibonacciSequence.Count)
        {
            int next = fibonacciSequence[fibonacciSequence.Count - 1] + fibonacciSequence[fibonacciSequence.Count - 2];
            fibonacciSequence.Add(next);
        }

        return fibonacciSequence[currentFibonacciIndex];
    }

    public void StartSpawning()
    {
        if (isSpawning || enemyPrefabs.Count == 0)
            return;

        CalculateCurrentRoundWavesLimit();

        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        if (!isSpawning)
            return;

        isSpawning = false;

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            if (wavesSpawnedThisRound >= currentRoundWavesLimit)
            {
                StopSpawning();
                yield break;
            }

            SpawnWave(currentFibonacciValue);

            totalWavesSpawned++;
            totalEnemiesSpawned += currentFibonacciValue;
            wavesSpawnedThisRound++;

            currentFibonacciValue = GetNextFibonacci();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnWave(int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnSingleEnemy();
        }
    }

    void SpawnSingleEnemy()
    {
        if (enemyPrefabs.Count == 0)
        {
            return;
        }

        float totalProbability = 0f;
        foreach (var enemyData in enemyPrefabs)
        {
            if (enemyData.prefab != null)
            {
                totalProbability += enemyData.probability;
            }
        }

        if (totalProbability <= 0f)
        {
            return;
        }

        float randomValue = Random.Range(0f, totalProbability);

        float currentProbability = 0f;
        foreach (var enemyData in enemyPrefabs)
        {
            if (enemyData.prefab != null)
            {
                currentProbability += enemyData.probability;

                if (randomValue <= currentProbability)
                {
                    GameObject spawnedEnemy = Instantiate(enemyData.prefab, transform.position, transform.rotation);

                    if (RoundManager.Instance != null)
                    {
                        RoundManager.Instance.RegisterEnemySpawned();
                    }

                    return;
                }
            }
        }

        foreach (var enemyData in enemyPrefabs)
        {
            if (enemyData.prefab != null)
            {
                GameObject spawnedEnemy = Instantiate(enemyData.prefab, transform.position, transform.rotation);

                if (RoundManager.Instance != null)
                {
                    RoundManager.Instance.RegisterEnemySpawned();
                }
                return;
            }
        }
    }
    // Reiniciar el spawner completamente (volver a Fibonacci 1 y resetear contadores)
    public void ResetSpawner()
    {
        StopSpawning();
        InitializeFibonacci();

        totalWavesSpawned = 0;
        totalEnemiesSpawned = 0;
        wavesSpawnedThisRound = 0;

        CalculateCurrentRoundWavesLimit();
    }

}