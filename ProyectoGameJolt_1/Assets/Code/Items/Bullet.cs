using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 3f;         
    public float damage = 10f;          
    public string targetTag;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector3 direction, float speed)
    {
        rb.linearVelocity = direction * speed;
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(targetTag))
        {
            Health healthscript = other.gameObject.GetComponent<Health>();

            if (healthscript != null)
            {
                healthscript.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }

}