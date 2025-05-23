using UnityEngine;

public class EnemyHealth : Health
{
    protected override void Die()
    {
        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.RegisterEnemyKilled();
        }
        base.Die();
    }
}