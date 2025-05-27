using UnityEngine;
using GameJolt.API;
using UnityEngine.UI;
using GameJolt.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Button trophiesListButton;
    [SerializeField] Button startGameButton;
    [SerializeField] string gameSceneName;

    private void Awake()
    {
        trophiesListButton.onClick.AddListener(() =>
        {
            GameJoltUI.Instance.ShowTrophies();
        });

        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGame);
        }
    }
    void Start()
    {
        if (TrophyManager.Instance != null)
        {
            TrophyManager.Instance.UnlockLoginTrophy();
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }
}
