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

    [Header("Tiempo de Partida")]
    [SerializeField] private float gameStartTime;
    [SerializeField] private float totalGameTime;
    [SerializeField] private bool gameTimeActive = false;

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
        gameStartTime = Time.time;
        gameTimeActive = true;

        Invoke("StartRound", 2f);
    }

    private void Update()
    {
        if (gameTimeActive)
        {
            totalGameTime = Time.time - gameStartTime;
        }
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
    }

    public void RegisterEnemySpawned()
    {
        aliveEnemiesCount++;
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

        
        //VERIFICAR TROFEO
        if (TrophyManager.Instance != null)
        {
            TrophyManager.Instance.CheckKill30Trophy(totalEnemiesKilled);
        }

        if (enemiesKilledThisRound >= currentRequirement)
        {
            CheckRoundCompletion();
        }
    }

    private void CheckRoundCompletion()
    {
        if (enemiesKilledThisRound >= currentRequirement && aliveEnemiesCount <= 0 && IsPlayerAlive())
        {
            if (BossSpawner.Instance != null && !BossSpawner.Instance.CanRoundComplete(currentRound))
            {
                return; 
            }

            CompleteRound();
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

    public void StopGameTime()
    {
        gameTimeActive = false;
    }

    private void CompleteRound()
    {
        isRoundInProgress = false;
        currentRound++;

        //VERIFICAR TROFEO
        if (TrophyManager.Instance != null && currentRound >= 5)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Health playerHealth = player.GetComponent<Health>();
                if (playerHealth != null)
                {
                    bool hasFullHealth = playerHealth.currentHealth >= playerHealth.maxHealth;
                    TrophyManager.Instance.CheckRound5FullHealthTrophy(currentRound, hasFullHealth);
                }
            }
        }

        onRoundComplete?.Invoke();

        Invoke("StartRound", restTimeBetweenRounds);
    }

    public void ForceCheckRoundCompletion()
    {
        CheckRoundCompletion();
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

    public string GetFormattedGameTime()
    {
        int hours = Mathf.FloorToInt(totalGameTime / 3600f);
        int minutes = Mathf.FloorToInt((totalGameTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(totalGameTime % 60f);

        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    public int GetCurrentRound()
    {
        return currentRound;
    }

    public int GetTotalEnemiesKilled()
    {
        return totalEnemiesKilled;
    }

    public float GetTotalGameTime()
    {
        return totalGameTime;
    }

    public bool IsGameTimeActive()
    {
        return gameTimeActive;
    }
}