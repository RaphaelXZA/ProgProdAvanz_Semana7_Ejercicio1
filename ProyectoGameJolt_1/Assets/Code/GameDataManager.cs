using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    [Header("Datos del Juego")]
    [SerializeField] private float totalGameTime;
    [SerializeField] private int finalRound;
    [SerializeField] private int totalKills;

    public static GameDataManager Instance { get; private set; }

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

    public void SaveGameData(float gameTime, int round, int kills)
    {
        totalGameTime = gameTime;
        finalRound = round;
        totalKills = kills;

        Debug.Log($"Datos guardados - Tiempo: {GetFormattedGameTime()}, Ronda: {finalRound}, Kills: {totalKills}");
    }

    public string GetFormattedGameTime()
    {
        int hours = Mathf.FloorToInt(totalGameTime / 3600f);
        int minutes = Mathf.FloorToInt((totalGameTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(totalGameTime % 60f);

        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
    public float GetTotalGameTime() => totalGameTime;
    public int GetFinalRound() => finalRound;
    public int GetTotalKills() => totalKills;

}