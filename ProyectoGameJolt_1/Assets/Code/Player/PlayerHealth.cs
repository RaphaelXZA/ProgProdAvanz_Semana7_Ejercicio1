using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{                    
    public float invincibilityTime = 1.0f;        
    public Image healthBar;                                              

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
        base.Die();
        Debug.Log("Game Over");
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

    private void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = GetHealthPercent();
        }
    }
}