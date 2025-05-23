using UnityEngine;
using System.Collections;

public class Enemy_Ghost : Enemy, IInvincible
{
    [Header("Modo Invencible")]
    public float invincibilityDuration = 3f;
    public float invincibilityInterval = 10f;
    public float alphaValueWhenInvincible = 0.5f;

    private bool isInvincible = false;
    private Renderer[] renderers;
    private float[] originalAlphaValues;
    private Coroutine invincibilityCoroutine;

    protected override void Start()
    {
        base.Start();

        renderers = GetComponentsInChildren<Renderer>();

        originalAlphaValues = new float[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalAlphaValues[i] = renderers[i].material.color.a;
        }

        invincibilityCoroutine = StartCoroutine(InvincibilityLoop());
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
        }
    }

    private IEnumerator InvincibilityLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(invincibilityInterval);

            SetInvincibilityState(true);

            yield return new WaitForSeconds(invincibilityDuration);

            SetInvincibilityState(false);
        }
    }

    private void SetInvincibilityState(bool invincible)
    {
        isInvincible = invincible;

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            Color currentColor = renderer.material.color;

            if (invincible)
            {
                currentColor.a = alphaValueWhenInvincible;
            }
            else
            {
                currentColor.a = originalAlphaValues[i];
            }

            renderer.material.color = currentColor;
        }
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }
}