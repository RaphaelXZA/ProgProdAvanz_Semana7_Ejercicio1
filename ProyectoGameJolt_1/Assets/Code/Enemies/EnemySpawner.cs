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

        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnRoutine());

        Debug.Log($"Spawner iniciado. Primera oleada: {currentFibonacciValue} enemigos");
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

        Debug.Log("Spawner detenido");
    }

    IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            SpawnWave(currentFibonacciValue);

            totalWavesSpawned++;
            totalEnemiesSpawned += currentFibonacciValue;

            Debug.Log($"Oleada {totalWavesSpawned}: {currentFibonacciValue} enemigos spawneados");

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
            Debug.LogWarning("No hay enemigos configurados para spawnear");
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
            Debug.LogWarning("La suma total de probabilidades es 0 o negativa");
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

                Debug.LogWarning("Spawneado enemigo por defecto debido a error en probabilidades");
                return;
            }
        }
    }

    //POR SI ACASO: Reiniciar el spawner (volver a Fibonacci 1)
    public void ResetSpawner()
    {
        StopSpawning();

        currentFibonacciIndex = 1;
        currentFibonacciValue = fibonacciSequence[currentFibonacciIndex];
        totalWavesSpawned = 0;
        totalEnemiesSpawned = 0;

        Debug.Log("Spawner reiniciado");
    }

    //POR SI ACASO: Spawnear manualmente
    public void SpawnCurrentWave()
    {
        SpawnWave(currentFibonacciValue);
        totalWavesSpawned++;
        totalEnemiesSpawned += currentFibonacciValue;

        Debug.Log($"Oleada manual: {currentFibonacciValue} enemigos spawneados");

        currentFibonacciValue = GetNextFibonacci();
    }
}