using UnityEngine;
using System.Collections;

public abstract class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public float flashDuration = 0.1f;
    protected bool isFlashing = false;
    public bool destroyOnDeath = true;
    public float destroyDelay = 0.2f;

    public Material flashMaterial;
    protected Renderer[] renderers;
    protected Material[] originalMaterials;

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        renderers = GetComponentsInChildren<Renderer>();

        originalMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            // CRÍTICO: Usar sharedMaterial para no crear instancias
            originalMaterials[i] = renderers[i].sharedMaterial;
        }

        if (flashMaterial == null)
        {
            Debug.Log("Falta asignar material para el efecto flash de daño");
        }
    }

    public virtual void TakeDamage(float damageAmount)
    {
        IInvincible invincibleObject = GetComponent<IInvincible>();
        if (invincibleObject != null && invincibleObject.IsInvincible())
        {
            return;
        }

        currentHealth -= damageAmount;

        if (!isFlashing)
        {
            StartCoroutine(FlashEffect());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual IEnumerator FlashEffect()
    {
        isFlashing = true;

        foreach (Renderer renderer in renderers)
        {
            renderer.material = flashMaterial;
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = originalMaterials[i];
        }

        isFlashing = false;
    }

    protected virtual void Die()
    {
        SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);

        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }

    // Método virtual para restaurar completamente el estado de salud
    // Las clases derivadas pueden sobrescribirlo para restauración específica
    public virtual void RestoreToFullHealth()
    {
        currentHealth = maxHealth;

        // Detener cualquier efecto de flash
        StopAllCoroutines();
        isFlashing = false;

        // Restaurar materiales originales
        if (originalMaterials != null && renderers != null)
        {
            for (int i = 0; i < renderers.Length && i < originalMaterials.Length; i++)
            {
                if (renderers[i] != null && originalMaterials[i] != null)
                {
                    renderers[i].material = originalMaterials[i];
                    renderers[i].enabled = true;
                }
            }
        }
    }
}