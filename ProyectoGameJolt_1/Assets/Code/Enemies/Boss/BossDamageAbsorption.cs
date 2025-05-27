using UnityEngine;
using System.Collections;

public class BossDamageAbsorption : MonoBehaviour
{
    [Header("Configuración de Absorción")]
    public float absorptionDuration = 4f;
    public float absorptionInterval = 12f;
    public float smoothnessIncrease = 0.8f;
    public float healingMultiplier = 0.5f;

    private Boss boss;
    private bool isAbsorbing = false;
    private Renderer[] renderers;
    private float[] originalSmoothness;
    private Coroutine absorptionCoroutine;
    private EnemyHealth bossHealth;

    public void Initialize(Boss bossReference)
    {
        boss = bossReference;
        bossHealth = GetComponent<EnemyHealth>();

        renderers = GetComponentsInChildren<Renderer>();
        originalSmoothness = new float[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Smoothness"))
            {
                originalSmoothness[i] = renderers[i].material.GetFloat("_Smoothness");
            }
            else if (renderers[i].material.HasProperty("_Glossiness"))
            {
                originalSmoothness[i] = renderers[i].material.GetFloat("_Glossiness");
            }
        }

        absorptionCoroutine = StartCoroutine(AbsorptionRoutine());
    }

    private IEnumerator AbsorptionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(absorptionInterval);

            if (boss != null && boss.IsAlive())
            {
                StartAbsorption();
                yield return new WaitForSeconds(absorptionDuration);
                StopAbsorption();
            }
        }
    }

    private void StartAbsorption()
    {
        isAbsorbing = true;

        for (int i = 0; i < renderers.Length; i++)
        {
            Material mat = renderers[i].material;
            if (mat.HasProperty("_Smoothness"))
            {
                mat.SetFloat("_Smoothness", Mathf.Clamp01(originalSmoothness[i] + smoothnessIncrease));
            }
            else if (mat.HasProperty("_Glossiness"))
            {
                mat.SetFloat("_Glossiness", Mathf.Clamp01(originalSmoothness[i] + smoothnessIncrease));
            }
        }
    }

    private void StopAbsorption()
    {
        isAbsorbing = false;

        // Restaurar smoothness original
        for (int i = 0; i < renderers.Length; i++)
        {
            Material mat = renderers[i].material;
            if (mat.HasProperty("_Smoothness"))
            {
                mat.SetFloat("_Smoothness", originalSmoothness[i]);
            }
            else if (mat.HasProperty("_Glossiness"))
            {
                mat.SetFloat("_Glossiness", originalSmoothness[i]);
            }
        }
    }

    public bool TryAbsorbDamage(float damage)
    {
        if (!isAbsorbing || bossHealth == null)
            return false;

        float healAmount = damage * healingMultiplier;
        bossHealth.currentHealth = Mathf.Min(bossHealth.maxHealth, bossHealth.currentHealth + healAmount);

        return true;
    }

    public bool IsAbsorbing() => isAbsorbing;

    private void OnDisable()
    {
        if (absorptionCoroutine != null)
        {
            StopCoroutine(absorptionCoroutine);
        }

        if (renderers != null && originalSmoothness != null)
        {
            StopAbsorption();
        }
    }
}