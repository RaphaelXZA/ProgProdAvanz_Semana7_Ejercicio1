using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;        
    public float acceleration = 15f;    
    public float deceleration = 20f;    

    private Rigidbody rb;
    private Vector3 moveDirection;
    private Vector3 currentVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Falta un Rigidbody");
        }

        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;

        }
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        moveDirection = new Vector3(moveHorizontal, 0f, moveVertical);

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }
    }

    void FixedUpdate()
    {
        Vector3 targetVelocity = moveDirection * moveSpeed;

        if (moveDirection.magnitude > 0.1f)
        {
            currentVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }

        currentVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = currentVelocity;
    }
}