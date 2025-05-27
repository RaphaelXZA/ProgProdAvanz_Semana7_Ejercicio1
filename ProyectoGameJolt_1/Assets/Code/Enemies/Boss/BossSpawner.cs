using UnityEngine;
using System.Collections;
using System.Reflection;

public class BossSpawner : MonoBehaviour
{
    [Header("Configuración del Boss")]
    public GameObject bossPrefab;
    public Transform spawnPoint;

    [Header("Configuración de Spawn")]
    public int bossRoundInterval = 3;
    public float spawnDelay = 1f;

    [Header("Progresión de Habilidades del Boss")]
    [SerializeField] private int bossAppearanceCount = 0;
    [SerializeField] private bool enableSingleShotOnAppearance = true;
    [SerializeField] private bool enableDamageAbsorptionOnAppearance = true;
    [SerializeField] private bool enableShotgunAttackOnAppearance = true;

    [Header("Estado")]
    [SerializeField] private bool bossSpawned = false;
    [SerializeField] private GameObject currentBoss = null;
    [SerializeField] private int lastBossRound = 0;
    [SerializeField] private bool waitingToSpawnBoss = false;
    [SerializeField] private bool roundRequirementsCompleted = false;

    public static BossSpawner Instance { get; private set; }

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
        if (spawnPoint == null)
        {
            spawnPoint = transform;
        }

        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.onNewRound.AddListener(OnNewRoundStarted);
        }
    }

    private void OnNewRoundStarted(int roundNumber)
    {
        roundRequirementsCompleted = false;

        Debug.Log($"Nueva ronda {roundNumber} iniciada. Es ronda de boss: {ShouldSpawnBossThisRound(roundNumber)}");
    }

    private void Update()
    {
        if (!bossSpawned && !waitingToSpawnBoss)
        {
            CheckForBossSpawn();
        }
    }

    private void CheckForBossSpawn()
    {
        if (RoundManager.Instance == null)
            return;

        int currentRound = RoundManager.Instance.GetCurrentRound();

        if (ShouldSpawnBossThisRound(currentRound) && currentRound != lastBossRound)
        {
            if (!roundRequirementsCompleted && HasCompletedEnemyRequirement())
            {
                roundRequirementsCompleted = true;
                Debug.Log($"Requisitos de ronda {currentRound} completados. Preparando spawn del boss...");

                EnemySpawner spawner = FindAnyObjectByType<EnemySpawner>();
                if (spawner != null)
                {
                    spawner.StopSpawning();
                    Debug.Log("EnemySpawner detenido para spawn del boss");
                }

                StartCoroutine(SpawnBossAfterDelay());
            }
        }
    }

    private bool HasCompletedEnemyRequirement()
    {
        if (RoundManager.Instance == null)
            return false;

        float progress = RoundManager.Instance.GetRoundProgress();
        int aliveEnemies = RoundManager.Instance.GetAliveEnemiesCount();
        bool roundInProgress = !RoundManager.Instance.IsRoundComplete();

        Debug.Log($"Progreso: {progress:F2}, Enemigos vivos: {aliveEnemies}, Ronda en progreso: {roundInProgress}");

        if (progress >= 1f && roundInProgress && !roundRequirementsCompleted)
        {
            Debug.Log("Progreso de ronda alcanzado al 100%! Esperando que termine el spawning...");
            return true;
        }

        return false;
    }

    private IEnumerator SpawnBossAfterDelay()
    {
        waitingToSpawnBoss = true;

        Debug.Log("¡Se acerca el jefe!");

        yield return new WaitForSeconds(spawnDelay);

        Debug.Log("Esperando a que se eliminen los enemigos restantes...");
        while (RoundManager.Instance != null && RoundManager.Instance.GetAliveEnemiesCount() > 0)
        {
            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log("Todos los enemigos eliminados. Spawning boss...");

        SpawnBoss();

        waitingToSpawnBoss = false;
    }

    private void SpawnBoss()
    {
        if (bossSpawned || currentBoss != null || bossPrefab == null)
        {
            Debug.LogWarning("No se puede spawnar boss: ya existe uno o falta prefab");
            return;
        }

        if (RoundManager.Instance == null)
        {
            Debug.LogWarning("BossSpawner: RoundManager no encontrado");
            return;
        }

        bossAppearanceCount++;

        currentBoss = Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);

        if (currentBoss != null)
        {
            ConfigureBossAbilities(currentBoss);

            bossSpawned = true;
            lastBossRound = RoundManager.Instance.GetCurrentRound();

            RoundManager.Instance.RegisterEnemySpawned();

            BossHealth bossHealth = currentBoss.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                Debug.Log($"¡JEFE APARECIDO! Aparición #{bossAppearanceCount} - Ronda {lastBossRound}");
                Debug.Log($"Boss spawneado con salud: {bossHealth.currentHealth}/{bossHealth.maxHealth}");
                LogBossAbilities();

                StartCoroutine(WaitAndCheckBossHealth(bossHealth));
            }
            else
            {
                Debug.LogError("El boss no tiene componente BossHealth!");
                OnBossDied();
            }
        }
        else
        {
            Debug.LogError("No se pudo instanciar el boss");
        }
    }

    private void ConfigureBossAbilities(GameObject boss)
    {
        Boss bossComponent = boss.GetComponent<Boss>();
        if (bossComponent == null)
        {
            Debug.LogError("El boss prefab no tiene componente Boss!");
            return;
        }

        switch (bossAppearanceCount)
        {
            case 1:
                bossComponent.enableSingleShot = false;
                bossComponent.enableDamageAbsorption = false;
                bossComponent.enableShotgunAttack = false;
                break;

            case 2:
                bossComponent.enableSingleShot = enableSingleShotOnAppearance;
                bossComponent.enableDamageAbsorption = false;
                bossComponent.enableShotgunAttack = false;
                break;

            case 3:
                bossComponent.enableSingleShot = enableSingleShotOnAppearance;
                bossComponent.enableDamageAbsorption = enableDamageAbsorptionOnAppearance;
                bossComponent.enableShotgunAttack = false;
                break;

            default:
                bossComponent.enableSingleShot = enableSingleShotOnAppearance;
                bossComponent.enableDamageAbsorption = enableDamageAbsorptionOnAppearance;
                bossComponent.enableShotgunAttack = enableShotgunAttackOnAppearance;
                break;
        }

        Debug.Log($"Boss configurado para aparición #{bossAppearanceCount}");
    }

    private void LogBossAbilities()
    {
        Boss bossComponent = currentBoss.GetComponent<Boss>();
        if (bossComponent != null)
        {
            string abilities = "Habilidades activas: ";
            if (bossComponent.enableSingleShot) abilities += "Disparo Simple ";
            if (bossComponent.enableDamageAbsorption) abilities += "Absorción de Daño ";
            if (bossComponent.enableShotgunAttack) abilities += "Ataque Escopeta ";

            if (!bossComponent.enableSingleShot && !bossComponent.enableDamageAbsorption && !bossComponent.enableShotgunAttack)
            {
                abilities += "Ninguna (Solo ataque cuerpo a cuerpo)";
            }

            Debug.Log(abilities);
        }
    }

    private IEnumerator WaitAndCheckBossHealth(BossHealth bossHealth)
    {
        yield return new WaitForSeconds(0.2f);

        if (bossHealth == null)
        {
            Debug.LogError("BossHealth se volvió null durante la inicialización");
            OnBossDied();
            yield break;
        }

        if (!bossHealth.IsAlive())
        {
            Debug.LogError($"Boss murió durante la inicialización! Salud: {bossHealth.currentHealth}/{bossHealth.maxHealth}");
            OnBossDied();
            yield break;
        }

        Debug.Log($"Boss inicializado correctamente - Salud: {bossHealth.currentHealth}/{bossHealth.maxHealth}");

        StartCoroutine(MonitorBossHealth(bossHealth));
    }

    private IEnumerator MonitorBossHealth(BossHealth bossHealth)
    {
        Debug.Log($"Iniciando monitoreo del boss - Salud: {bossHealth.currentHealth}/{bossHealth.maxHealth}");

        while (bossHealth != null && bossHealth.IsAlive() && currentBoss != null)
        {
            yield return new WaitForSeconds(0.3f);
        }

        if (bossHealth == null || currentBoss == null)
        {
            Debug.Log("Boss destruido/eliminado");
        }
        else if (!bossHealth.IsAlive())
        {
            Debug.Log($"Boss murió por daño - Salud final: {bossHealth.currentHealth}");
        }

        OnBossDied();
    }

    private void OnBossDied()
    {
        Debug.Log($"¡JEFE DERROTADO! (Aparición #{bossAppearanceCount}) La ronda puede continuar");

        
        //VERIFICAR TROFEO
        if (TrophyManager.Instance != null)
        {
            TrophyManager.Instance.UnlockBossKillTrophy();
        }

        bossSpawned = false;
        currentBoss = null;
        roundRequirementsCompleted = false;
        waitingToSpawnBoss = false;

        Debug.Log("Estado del BossSpawner limpiado - La ronda puede completarse ahora");

        if (RoundManager.Instance != null)
        {
            StartCoroutine(ForceRoundCompletionCheck());
        }
    }

    private IEnumerator ForceRoundCompletionCheck()
    {
        yield return new WaitForEndOfFrame();

        Debug.Log("Forzando verificación de completion de ronda...");

        if (RoundManager.Instance != null)
        {
            int currentRound = RoundManager.Instance.GetCurrentRound();
            float progress = RoundManager.Instance.GetRoundProgress();
            int aliveEnemies = RoundManager.Instance.GetAliveEnemiesCount();
            bool canComplete = CanRoundComplete(currentRound);

            Debug.Log($"Estado de ronda: Progreso={progress:F2}, Enemigos vivos={aliveEnemies}, Puede completarse={canComplete}");

            if (progress >= 1f && aliveEnemies <= 0 && canComplete)
            {
                Debug.Log("¡Todas las condiciones cumplidas! Intentando completar ronda...");
                StartCoroutine(TriggerRoundCompletion());
            }
        }
    }

    private IEnumerator TriggerRoundCompletion()
    {
        yield return new WaitForSeconds(0.1f);

        if (RoundManager.Instance != null)
        {
            var methodInfo = typeof(RoundManager).GetMethod("ForceCheckRoundCompletion");
            if (methodInfo != null)
            {
                Debug.Log("Llamando a ForceCheckRoundCompletion...");
                RoundManager.Instance.ForceCheckRoundCompletion();
            }
            else
            {
                var privateMethod = typeof(RoundManager).GetMethod("CheckRoundCompletion",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (privateMethod != null)
                {
                    Debug.Log("Llamando a CheckRoundCompletion manualmente...");
                    privateMethod.Invoke(RoundManager.Instance, null);
                }
                else
                {
                    Debug.LogWarning("No se pudo encontrar ningún método de completion");
                }
            }
        }
    }

    public bool IsBossActive()
    {
        return bossSpawned && currentBoss != null;
    }

    public GameObject GetCurrentBoss()
    {
        return currentBoss;
    }

    public bool ShouldSpawnBossThisRound(int roundNumber)
    {
        return roundNumber % bossRoundInterval == 0;
    }

    public bool IsWaitingToSpawnBoss()
    {
        return waitingToSpawnBoss;
    }

    public bool CanRoundComplete(int roundNumber)
    {
        if (!ShouldSpawnBossThisRound(roundNumber))
        {
            return true;
        }

        if (waitingToSpawnBoss)
        {
            Debug.Log($"Ronda {roundNumber} no puede completarse - Esperando spawn del boss");
            return false;
        }

        if (IsBossActive())
        {
            Debug.Log($"Ronda {roundNumber} no puede completarse - Boss activo");
            return false;
        }

        Debug.Log($"Ronda {roundNumber} puede completarse - Boss derrotado o no spawneado");
        return true;
    }

    public void ForceSpawnBoss()
    {
        if (!bossSpawned && !waitingToSpawnBoss)
        {
            roundRequirementsCompleted = true;
            StartCoroutine(SpawnBossAfterDelay());
        }
    }

    public int GetBossAppearanceCount()
    {
        return bossAppearanceCount;
    }

    public void ResetBossAppearanceCount()
    {
        bossAppearanceCount = 0;
        Debug.Log("Contador de apariciones del boss reseteado");
    }

    public string GetStatusInfo()
    {
        if (RoundManager.Instance == null)
            return "RoundManager no disponible";

        int currentRound = RoundManager.Instance.GetCurrentRound();

        if (bossSpawned)
            return $"Boss activo en ronda {currentRound} (Aparición #{bossAppearanceCount})";

        if (waitingToSpawnBoss)
            return $"Boss apareciendo en ronda {currentRound}... (Será aparición #{bossAppearanceCount + 1})";

        if (ShouldSpawnBossThisRound(currentRound))
        {
            float progress = RoundManager.Instance.GetRoundProgress();
            int aliveEnemies = RoundManager.Instance.GetAliveEnemiesCount();
            return $"Ronda de boss {currentRound} - Progreso: {progress:F1}% - Enemigos: {aliveEnemies} (Será aparición #{bossAppearanceCount + 1})";
        }

        return $"Ronda normal {currentRound} (Próxima aparición del boss será #{bossAppearanceCount + 1})";
    }

    private void OnDestroy()
    {
        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.onNewRound.RemoveListener(OnNewRoundStarted);
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnDisable()
    {
        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.onNewRound.RemoveListener(OnNewRoundStarted);
        }
    }
}