using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [Header("Referencias del Panel")]
    public TextMeshProUGUI finalTimeText;
    public TextMeshProUGUI finalRoundText;
    public TextMeshProUGUI finalKillsText;
    public Button retryButton;
    public Button menuButton;

    [Header("Configuración de Escenas")]
    public string gameSceneName = "GameScene";
    public string menuSceneName = "MenuScene";

    public static GameOverManager Instance { get; private set; }

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

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RestartGame);
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(GoToMenu);
        }
    }

    private void Start()
    {
        DisplayGameOverStats();

        if (retryButton == null)
        {
            Debug.LogWarning("GameOverManager: falta asignar retryButton");
        }  
    }

    private void DisplayGameOverStats()
    {
        if (GameDataManager.Instance != null)
        {
            if (finalTimeText != null)
            {
                finalTimeText.text = "Tiempo jugado: " + GameDataManager.Instance.GetFormattedGameTime();
            }

            if (finalRoundText != null)
            {
                finalRoundText.text = "Ronda alcanzada: " + GameDataManager.Instance.GetFinalRound().ToString();
            }

            if (finalKillsText != null)
            {
                finalKillsText.text = "Bajas totales: " + GameDataManager.Instance.GetTotalKills().ToString();
            }
        }
        else
        {
            if (finalTimeText != null)
            {
                finalTimeText.text = "Tiempo jugado: 00:00:00";
            }
                
            if (finalRoundText != null)
            {
                finalRoundText.text = "Ronda alcanzada: 1";
            }
                
            if (finalKillsText != null)
            {
                finalKillsText.text = "Bajas totales: 0";
            }
                
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}