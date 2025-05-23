using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;            
    public float smoothSpeed = 10f;     
    public Vector3 offset = new Vector3(0, 15, 0); 

    private Vector3 lastValidPosition;  
    private bool targetLost = false;   

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                lastValidPosition = target.position;
            }
        }
        else
        {
            lastValidPosition = target.position;
        }
    }

    void LateUpdate()
    {
        
        if (target != null && target.gameObject.activeInHierarchy)
        {
            lastValidPosition = target.position;
            targetLost = false;

            Vector3 desiredPosition = lastValidPosition + offset;

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            transform.position = smoothedPosition;

        }
        else if (!targetLost)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                lastValidPosition = target.position;
            }
            else
            {
                targetLost = true;
                Debug.Log("Objetivo de cámara perdido. Manteniendo última posición.");
            }
        }
    }
}