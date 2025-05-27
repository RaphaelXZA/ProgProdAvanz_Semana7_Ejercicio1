using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Referencias de UI del Juego")]
    public TextMeshProUGUI gameTimeText;
    public TextMeshProUGUI currentRoundText;
    public TextMeshProUGUI totalKillsText;

    [Header("Referencias de UI del Jefe")]
    public GameObject bossHealthPanel;
    public Image bossHealthBar;
    public TextMeshProUGUI bossNameText;

    [Header("Configuración de Actualización")]
    public float updateInterval = 0.1f;

    private float lastUpdateTime;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (gameTimeText == null)
        {
            Debug.LogWarning("UIManager: falta asignar gameTimeText");
        }

        if (currentRoundText == null)
        {
            Debug.LogWarning("UIManager: falta asignar currentRoundText");
        }

        if (totalKillsText == null)
        {
            Debug.LogWarning("UIManager: falta asignar totalKillsText");
        }

        if (bossHealthPanel == null)
        {
            Debug.LogWarning("UIManager: falta asignar bossHealthPanel");
        }

        if (bossHealthBar == null)
        {
            Debug.LogWarning("UIManager: falta asignar bossHealthBar");
        }

        HideBossHealthBar();
    }

    void Update()
    {
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateUI();
            lastUpdateTime = Time.time;
        }
    }

    void UpdateUI()
    {
        UpdateGameUI();

        UpdateBossUI();
    }

    private void UpdateGameUI()
    {
        if (RoundManager.Instance == null)
            return;

        if (gameTimeText != null)
        {
            gameTimeText.text = "Tiempo: " + RoundManager.Instance.GetFormattedGameTime();
        }

        if (currentRoundText != null)
        {
            currentRoundText.text = "Ronda: " + RoundManager.Instance.GetCurrentRound().ToString();
        }

        if (totalKillsText != null)
        {
            totalKillsText.text = "Bajas: " + RoundManager.Instance.GetTotalEnemiesKilled().ToString();
        }
    }

    private void UpdateBossUI()
    {
        if (BossHealth.Instance != null && bossHealthBar != null)
        {
            bossHealthBar.fillAmount = BossHealth.Instance.GetHealthPercent();
        }
    }

    public void ShowBossHealthBar()
    {
        if (bossHealthPanel != null)
        {
            bossHealthPanel.SetActive(true);
        }

        if (bossNameText != null)
        {
            bossNameText.text = "JEFF: The Purple Menace";
        }
    }

    public void HideBossHealthBar()
    {
        if (bossHealthPanel != null)
        {
            bossHealthPanel.SetActive(false);
        }

    }

    public void ForceUpdateUI()
    {
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}