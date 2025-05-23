using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;              
    public Transform firePoint;                  
    public float bulletSpeed = 15f;              
    public float fireRate = 0.2f;                

    private Camera mainCamera;
    private float nextFireTime = 0f;

    void Start()
    {
        mainCamera = Camera.main;

        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);        
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        float rayDistance;
        Vector3 targetPoint = Vector3.zero;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            targetPoint = ray.GetPoint(rayDistance);

            Vector3 direction = targetPoint - firePoint.position;
            direction.y = 0;
            direction.Normalize();

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
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
    }
}