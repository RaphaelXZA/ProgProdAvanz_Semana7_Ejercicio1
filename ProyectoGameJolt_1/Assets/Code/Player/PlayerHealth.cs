using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : Health
{
    public float invincibilityTime = 1.0f;
    public Image healthBar;

    [Header("Configuración de Game Over")]
    public string gameOverSceneName = "GameOverScene";

    private bool isInvincible = false;

    protected override void Start()
    {
        base.Start();
        UpdateUI();
    }

    public override void TakeDamage(float damageAmount)
    {
        if (isInvincible)
            return;

        base.TakeDamage(damageAmount);

        UpdateUI();

        if (IsAlive())
            StartCoroutine(TemporaryInvincibility());
    }

    protected override void Die()
    {
        Debug.Log("Game Over - Jugador muerto");

        // Detener el tiempo del juego en RoundManager
        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.StopGameTime();

            // Guardar los datos del juego antes de cambiar de escena
            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.SaveGameData(
                    RoundManager.Instance.GetTotalGameTime(),
                    RoundManager.Instance.GetCurrentRound(),
                    RoundManager.Instance.GetTotalEnemiesKilled()
                );
            }
        }
        else
        {
            Debug.LogWarning("RoundManager.Instance no encontrado");
        }

        // Cambiar a la escena de Game Over
        SceneManager.LoadScene(gameOverSceneName);
    }

    private System.Collections.IEnumerator TemporaryInvincibility()
    {
        isInvincible = true;

        float endTime = Time.time + invincibilityTime;
        while (Time.time < endTime)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = !renderer.enabled;
            }

            yield return new WaitForSeconds(0.1f);
        }

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }

        isInvincible = false;
    }

    public void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = GetHealthPercent();
        }
    }

    // Este método ya no es necesario porque no reiniciamos en la misma escena
    // Pero lo mantengo por compatibilidad
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateUI();

        // Asegurarse de que los renderers estén visibles
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }

        isInvincible = false;
    }
}