using UnityEngine;
using System.Collections;

public class BossSingleShot : MonoBehaviour
{
    [Header("Configuración de Disparo")]
    private GameObject bulletPrefab;
    private Transform firePoint;
    public float bulletSpeed = 10f;
    public float shootInterval = 3f;
    public float shootRange = 15f;

    private Boss boss;
    private Coroutine shootCoroutine;

    public void Initialize(Boss bossReference, GameObject bulletPrefabRef, Transform firePointRef)
    {
        boss = bossReference;
        bulletPrefab = bulletPrefabRef;
        firePoint = firePointRef;

        if (firePoint == null)
        {
            firePoint = transform;
        }

        shootCoroutine = StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootInterval);

            if (boss != null && boss.IsAlive() && !boss.IsPerformingSpecialAttack())
            {
                Transform player = boss.GetPlayer();
                if (player != null && IsPlayerInRange(player))
                {
                    ShootAtPlayer(player);
                }
            }
        }
    }

    private bool IsPlayerInRange(Transform player)
    {
        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= shootRange;
    }

    private void ShootAtPlayer(Transform player)
    {
        if (bulletPrefab == null)
        {
            return;
        }

        Vector3 direction = (player.position - firePoint.position).normalized;
        direction.y = 0;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        if (bulletComponent != null)
        {
            bulletComponent.targetTag = "Player";
            bulletComponent.Initialize(direction, bulletSpeed);
        }
        else
        {
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = direction * bulletSpeed;
            }
        }
    }

    private void OnDisable()
    {
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
        }
    }
}