using UnityEngine;
using GameJolt.API;
using System.Collections.Generic;

public class TrophyManager : MonoBehaviour
{
    [Header("Configuración de Trofeos")]
    [SerializeField] private bool debugMode = true;

    [Header("IDs de Trofeos de Game Jolt")]
    [SerializeField] private int loginTrophyId = 267754;      
    [SerializeField] private int kill30EnemiesTrophyId = 268889;     
    [SerializeField] private int killBossTrophyId = 268890;          
    [SerializeField] private int round5FullHealthTrophyId = 268888;  
    [SerializeField] private int allTrophiesTrophyId = 268887;       

    public static TrophyManager Instance { get; private set; }

    private HashSet<int> unlockedTrophies = new HashSet<int>();
    private bool hasKilledBoss = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeTrophySystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeTrophySystem()
    {
        if (debugMode)
        {
            Debug.Log("TrophyManager: Sistema de trofeos inicializado");
        }
    }


    public void UnlockLoginTrophy()
    {
        UnlockTrophy(loginTrophyId, "¡Bienvenido al juego!");
    }

    public void CheckKill30Trophy(int totalKills)
    {
        if (totalKills >= 30 && kill30EnemiesTrophyId != 0)
        {
            UnlockTrophy(kill30EnemiesTrophyId, "¡30 enemigos eliminados!");
        }
    }

    public void UnlockBossKillTrophy()
    {
        if (!hasKilledBoss && killBossTrophyId != 0)
        {
            hasKilledBoss = true;
            UnlockTrophy(killBossTrophyId, "¡Jefe derrotado!");
        }
    }

    public void CheckRound5FullHealthTrophy(int currentRound, bool hasFullHealth)
    {
        if (currentRound >= 5 && hasFullHealth && round5FullHealthTrophyId != 0)
        {
            UnlockTrophy(round5FullHealthTrophyId, "¡Llegaste a la ronda 5 con vida completa!");
        }
    }

    public void CheckAllTrophiesUnlocked()
    {
        if (allTrophiesTrophyId == 0) return;

        bool hasWelcome = unlockedTrophies.Contains(loginTrophyId);
        bool hasKill30 = unlockedTrophies.Contains(kill30EnemiesTrophyId);
        bool hasBossKill = unlockedTrophies.Contains(killBossTrophyId);
        bool hasRound5 = unlockedTrophies.Contains(round5FullHealthTrophyId);

        if (hasWelcome && hasKill30 && hasBossKill && hasRound5)
        {
            UnlockTrophy(allTrophiesTrophyId, "¡Todos los trofeos desbloqueados! ¡Maestro del juego!");
        }
    }


    private void UnlockTrophy(int trophyId, string message)
    {
        if (unlockedTrophies.Contains(trophyId))
        {
            if (debugMode)
            {
                Debug.Log($"TrophyManager: Trofeo {trophyId} ya estaba desbloqueado");
            }
            return;
        }

        Trophies.Unlock(trophyId, (bool success) =>
        {
            if (success)
            {
                unlockedTrophies.Add(trophyId);

                if (debugMode)
                {
                    Debug.Log($"TrophyManager: ¡Trofeo desbloqueado! {message} (ID: {trophyId})");
                }

                CheckAllTrophiesUnlocked();
            }
            else
            {
                if (debugMode)
                {
                    Debug.LogWarning($"TrophyManager: Error al desbloquear trofeo {trophyId}: {message}");
                }
            }
        });
    }
    public bool IsTrophyUnlocked(int trophyId)
    {
        return unlockedTrophies.Contains(trophyId);
    }

    public int GetUnlockedTrophiesCount()
    {
        return unlockedTrophies.Count;
    }

}