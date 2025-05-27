using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealth : EnemyHealth
{
    private BossDamageAbsorption absorptionBehavior;

    public static BossHealth Instance { get; private set; }

    protected override void Start()
    {
        base.Start();

        Instance = this;

        Boss boss = GetComponent<Boss>();
        if (boss != null)
        {
            absorptionBehavior = boss.AbsorptionBehavior;
        }

        if (absorptionBehavior == null)
        {
            absorptionBehavior = GetComponent<BossDamageAbsorption>();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowBossHealthBar();
        }
    }

    public override void TakeDamage(float damageAmount)
    {
        if (absorptionBehavior != null)
        {
            bool isAbsorbing = absorptionBehavior.IsAbsorbing();

            if (isAbsorbing)
            {
                absorptionBehavior.TryAbsorbDamage(damageAmount);
                return;
            }
        }
        base.TakeDamage(damageAmount);
    }

    protected override void Die()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideBossHealthBar();
        }

        if (Instance == this)
        {
            Instance = null;
        }

        base.Die();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideBossHealthBar();
            }
        }
    }
}