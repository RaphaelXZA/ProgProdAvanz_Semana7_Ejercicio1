using UnityEngine;
using System.Collections;

public class BossShotgunAttack : MonoBehaviour
{
    [Header("Configuración de Ataque Escopeta")]
    private GameObject bulletPrefab;
    private Transform firePoint;
    public int bulletsPerShot = 7;
    public float bulletSpeed = 12f;
    public float spreadAngle = 45f;
    public float attackInterval = 8f;
    public float chargeTime = 2f;
    public float attackRange = 12f;

    [Header("Animación de Carga")]
    public float maxScaleMultiplier = 1.5f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 1.5f);

    private Boss boss;
    private Vector3 originalScale;
    private Coroutine attackCoroutine;

    public void Initialize(Boss bossReference, GameObject bulletPrefabRef, Transform firePointRef)
    {
        boss = bossReference;
        bulletPrefab = bulletPrefabRef;
        firePoint = firePointRef;
        originalScale = transform.localScale;

        if (firePoint == null)
        {
            firePoint = transform;
        }

        attackCoroutine = StartCoroutine(ShotgunAttackRoutine());
    }

    private IEnumerator ShotgunAttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);

            if (boss != null && boss.IsAlive() && !boss.IsPerformingSpecialAttack())
            {
                Transform player = boss.GetPlayer();
                if (player != null && IsPlayerInRange(player))
                {
                    yield return StartCoroutine(PerformShotgunAttack(player));
                }
            }
        }
    }

    private bool IsPlayerInRange(Transform player)
    {
        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= attackRange;
    }

    private IEnumerator PerformShotgunAttack(Transform player)
    {
        boss.SetSpecialAttackState(true);

        yield return StartCoroutine(ChargeAnimation());

        ShootShotgunBlast(player);

        yield return StartCoroutine(ReturnToNormalScale());

        boss.SetSpecialAttackState(false);
    }

    private IEnumerator ChargeAnimation()
    {
        float elapsedTime = 0f;

        while (elapsedTime < chargeTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / chargeTime;

            float scaleMultiplier = scaleCurve.Evaluate(progress);
            transform.localScale = originalScale * scaleMultiplier;

            yield return null;
        }
    }

    private void ShootShotgunBlast(Transform player)
    {
        if (bulletPrefab == null)
        {
            return;
        }

        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
        directionToPlayer.y = 0; 

        float baseAngle = Mathf.Atan2(directionToPlayer.z, directionToPlayer.x) * Mathf.Rad2Deg;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            float angleOffset = Mathf.Lerp(-spreadAngle / 2f, spreadAngle / 2f, (float)i / (bulletsPerShot - 1));
            float finalAngle = baseAngle + angleOffset;

            Vector3 shootDirection = new Vector3(
                Mathf.Cos(finalAngle * Mathf.Deg2Rad),
                0,
                Mathf.Sin(finalAngle * Mathf.Deg2Rad)
            ).normalized;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.targetTag = "Player";
                bulletComponent.Initialize(shootDirection, bulletSpeed);
            }
            else
            {
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                if (bulletRb != null)
                {
                    bulletRb.linearVelocity = shootDirection * bulletSpeed;
                }
            }
        }

    }

    private IEnumerator ReturnToNormalScale()
    {
        float duration = 0.3f;
        float elapsedTime = 0f;
        Vector3 currentScale = transform.localScale;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            transform.localScale = Vector3.Lerp(currentScale, originalScale, progress);

            yield return null;
        }

        transform.localScale = originalScale;
    }

    private void OnDisable()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        if (originalScale != Vector3.zero)
        {
            transform.localScale = originalScale;
        }
    }
}