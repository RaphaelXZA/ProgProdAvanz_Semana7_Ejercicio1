using UnityEngine;
using UnityEngine.Events;

public class RoundManager : MonoBehaviour
{
    [Header("Configuración de Rondas")]
    public int currentRound = 1;
    public int enemiesKilledThisRound = 0;
    public int totalEnemiesKilled = 0;
    public int enemiesRequiredToAdvance = 5;
    public int enemiesIncreasePerRound = 2;
    public float restTimeBetweenRounds = 5f; 

    [Header("Estado Actual")]
    [SerializeField] private int currentRequirement;
    [SerializeField] private bool isRoundInProgress;
    [SerializeField] private int aliveEnemiesCount = 0; 

    [Header("Eventos")]
    public UnityEvent onRoundStart;
    public UnityEvent onRoundComplete;
    public UnityEvent<int> onNewRound;           

    public static RoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentRequirement = CalculateRequirement(currentRound);
        isRoundInProgress = false;

        Invoke("StartRound", 2f);
    }

    public void StartRound()
    {
        if (isRoundInProgress)
            return;

        enemiesKilledThisRound = 0;
        aliveEnemiesCount = 0;

        currentRequirement = CalculateRequirement(currentRound);

        isRoundInProgress = true;

        onRoundStart?.Invoke();
        onNewRound?.Invoke(currentRound);

        Debug.Log($"¡Ronda {currentRound} iniciada! Mata {currentRequirement} enemigos para avanzar.");
    }

    public void RegisterEnemySpawned()
    {
        aliveEnemiesCount++;
        Debug.Log($"Enemigo spawneado. Enemigos vivos: {aliveEnemiesCount}");
    }

    public void RegisterEnemyKilled()
    {
        if (!isRoundInProgress)
            return;

        enemiesKilledThisRound++;
        totalEnemiesKilled++;

        if (aliveEnemiesCount > 0)
        {
            aliveEnemiesCount--;
        }

        Debug.Log($"Enemigo eliminado. Eliminados: {enemiesKilledThisRound}/{currentRequirement}, Vivos: {aliveEnemiesCount}");

        if (enemiesKilledThisRound >= currentRequirement)
        {
            CheckRoundCompletion();
        }
    }

    private void CheckRoundCompletion()
    {
        if (enemiesKilledThisRound >= currentRequirement && aliveEnemiesCount <= 0 && IsPlayerAlive())
        {
            CompleteRound();
        }
        else if (enemiesKilledThisRound >= currentRequirement && aliveEnemiesCount > 0)
        {
            Debug.Log($"Quedan {aliveEnemiesCount} enemigos vivos. Elimínalos para continuar.");
        }
    }

    private bool IsPlayerAlive()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Health playerHealth = player.GetComponent<Health>();
            return playerHealth != null && playerHealth.IsAlive();
        }
        return false;
    }

    private void CompleteRound()
    {
        isRoundInProgress = false;
        currentRound++;

        Debug.Log($"¡Ronda {currentRound - 1} completada! Avanzando a la ronda {currentRound} en {restTimeBetweenRounds} segundos...");

        onRoundComplete?.Invoke();

        Invoke("StartRound", restTimeBetweenRounds);
    }

    private int CalculateRequirement(int round)
    {
        return enemiesRequiredToAdvance + (enemiesIncreasePerRound * (round - 1));
    }

    public float GetRoundProgress()
    {
        if (currentRequirement <= 0)
            return 0f;

        return (float)enemiesKilledThisRound / currentRequirement;
    }

    public int GetAliveEnemiesCount()
    {
        return aliveEnemiesCount;
    }

    public bool IsRoundComplete()
    {
        return enemiesKilledThisRound >= currentRequirement && aliveEnemiesCount <= 0;
    }

    public void ResetRounds()
    {
        currentRound = 1;
        enemiesKilledThisRound = 0;
        totalEnemiesKilled = 0;
        aliveEnemiesCount = 0;
        isRoundInProgress = false;

        CancelInvoke("StartRound");
        Invoke("StartRound", 2f);
    }
}