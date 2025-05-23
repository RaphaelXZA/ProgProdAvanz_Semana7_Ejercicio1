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

    [Header("Estado Actual")]
    [SerializeField] private int currentRequirement;    
    [SerializeField] private bool isRoundInProgress;    

    [Header("Eventos (opcional)")]
    public UnityEvent onRoundStart;                     
    public UnityEvent onRoundComplete;                  
    public UnityEvent<int> onNewRound; //Numero de nuevas rondas                  

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

        currentRequirement = CalculateRequirement(currentRound);

        isRoundInProgress = true;

        onRoundStart?.Invoke();
        onNewRound?.Invoke(currentRound);

        Debug.Log($"¡Ronda {currentRound} iniciada! Mata {currentRequirement} enemigos para avanzar.");
    }

    public void RegisterEnemyKilled()
    {
        if (!isRoundInProgress)
            return;

        enemiesKilledThisRound++;
        totalEnemiesKilled++;

        Debug.Log($"Enemigo eliminado. {enemiesKilledThisRound}/{currentRequirement}");

        if (enemiesKilledThisRound >= currentRequirement)
        {
            CompleteRound();
        }
    }

    private void CompleteRound()
    {
        isRoundInProgress = false;
        currentRound++;

        Debug.Log($"¡Ronda {currentRound - 1} completada! Avanzando a la ronda {currentRound}...");

        onRoundComplete?.Invoke();

        Invoke("StartRound", 5f);
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

    public void ResetRounds()
    {
        currentRound = 1;
        enemiesKilledThisRound = 0;
        totalEnemiesKilled = 0;
        isRoundInProgress = false;

        CancelInvoke("StartRound");
        Invoke("StartRound", 2f);
    }
}