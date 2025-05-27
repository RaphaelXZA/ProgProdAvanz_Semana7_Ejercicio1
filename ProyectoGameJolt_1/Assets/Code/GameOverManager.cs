using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using GameJolt.API;

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

    [Header("Game Jolt Score Tables")]
    public int survivalTimeTableId = 1004591; 
    public int totalKillsTableId = 1008629; 

    [Header("Estado de Scores")]
    [SerializeField] private bool scoresSubmitted = false;
    [SerializeField] private bool timeScoreSubmitted = false;
    [SerializeField] private bool killsScoreSubmitted = false;

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
        SubmitScoresToGameJolt();

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

    private void SubmitScoresToGameJolt()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogWarning("GameDataManager no encontrado. No se pueden enviar scores.");
            return;
        }

        SubmitSurvivalTimeScore();
        SubmitTotalKillsScore();
    }

    private void SubmitSurvivalTimeScore()
    {
        if (timeScoreSubmitted) return;

        float totalGameTime = GameDataManager.Instance.GetTotalGameTime();

        int timeInSeconds = Mathf.FloorToInt(totalGameTime);

        string timeFormatted = GameDataManager.Instance.GetFormattedGameTime();
        string extraData = $"Tiempo sobrevivido: {timeFormatted}";

        try
        {
            Scores.Add(timeInSeconds, extraData, survivalTimeTableId);
            timeScoreSubmitted = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al enviar score de tiempo: {e.Message}");
        }

        CheckAllScoresSubmitted();
    }

    private void SubmitTotalKillsScore()
    {
        if (killsScoreSubmitted) return;

        int totalKills = GameDataManager.Instance.GetTotalKills();

        string timeFormatted = GameDataManager.Instance.GetFormattedGameTime();
        string extraData = $"Enemigos eliminados: {totalKills}";

        try
        {
            Scores.Add(totalKills, extraData, totalKillsTableId);
            killsScoreSubmitted = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al enviar score de bajas: {e.Message}");
        }

        CheckAllScoresSubmitted();
    }

    private void CheckAllScoresSubmitted()
    {
        if (timeScoreSubmitted && killsScoreSubmitted && !scoresSubmitted)
        {
            scoresSubmitted = true;

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