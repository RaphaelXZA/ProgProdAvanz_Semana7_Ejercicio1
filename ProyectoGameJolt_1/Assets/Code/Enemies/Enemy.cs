using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;
    private Health playerHealth;
    private EnemyHealth enemyHealth;

    public float moveSpeed = 3.5f;
    public float updateTargetInterval = 0.3f;
    public float stoppingDistance = 1.2f;

    public float attackDamage = 10f;
    public float attackInterval = 1.0f;
    public string targetTag = "Player";
    private float nextAttackTime = 0f;
    private bool isPlayerInRange = false;
    private Coroutine updateTargetCoroutine;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = true;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag(targetTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponent<Health>();
        }

        updateTargetCoroutine = StartCoroutine(UpdateTargetPosition());
    }

    void Update()
    {
        if (enemyHealth != null && !enemyHealth.IsAlive())
        {
            if (agent != null)
                agent.isStopped = true;
            return;
        }

        if (isPlayerInRange && Time.time >= nextAttackTime)
        {
            AttackPlayer();
        }
    }

    IEnumerator UpdateTargetPosition()
    {
        while (true)
        {
            if (player != null && agent != null && agent.isActiveAndEnabled && enemyHealth.IsAlive())
            {
                agent.SetDestination(player.position);
            }

            yield return new WaitForSeconds(updateTargetInterval);
        }
    }

    void AttackPlayer()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            nextAttackTime = Time.time + attackInterval;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            isPlayerInRange = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            isPlayerInRange = false;
        }
    }

    protected virtual void OnDisable()
    {
        if (updateTargetCoroutine != null)
        {
            StopCoroutine(updateTargetCoroutine);
        }
    }
}