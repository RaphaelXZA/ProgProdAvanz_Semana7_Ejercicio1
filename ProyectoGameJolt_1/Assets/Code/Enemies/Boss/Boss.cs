using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class Boss : MonoBehaviour
{
    [Header("Configuración Base")]
    private Transform player;
    private NavMeshAgent agent;
    private Health playerHealth;
    private EnemyHealth bossHealth;

    public float moveSpeed = 2f;
    public float updateTargetInterval = 0.5f;
    public float stoppingDistance = 2f;

    [Header("Ataque Cuerpo a Cuerpo")]
    public float attackDamage = 20f;
    public float attackInterval = 2f;
    public string targetTag = "Player";
    private float nextAttackTime = 0f;
    private bool isPlayerInRange = false;

    [Header("Comportamientos Activos")]
    public bool enableSingleShot = true;
    public bool enableDamageAbsorption = true;
    public bool enableShotgunAttack = true;

    [Header("Referencias")]
    public BossSingleShot singleShotBehavior;
    public BossDamageAbsorption absorptionBehavior;
    public BossShotgunAttack shotgunBehavior;

    [Header("Referencias para Disparos")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    private Coroutine updateTargetCoroutine;
    private bool isPerformingSpecialAttack = false;

    protected virtual void Start()
    {
        InitializeBaseBehavior();
        InitializeBehaviors();
    }

    private void InitializeBaseBehavior()
    {
        agent = GetComponent<NavMeshAgent>();
        bossHealth = GetComponent<EnemyHealth>();

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

    private void InitializeBehaviors()
    {
        if (enableSingleShot && singleShotBehavior != null)
        {
            singleShotBehavior.Initialize(this, bulletPrefab, firePoint);
        }

        if (enableDamageAbsorption && absorptionBehavior != null)
        {
            absorptionBehavior.Initialize(this);
        }

        if (enableShotgunAttack && shotgunBehavior != null)
        {
            shotgunBehavior.Initialize(this, bulletPrefab, firePoint);
        }
    }

    void Update()
    {
        if (bossHealth != null && !bossHealth.IsAlive())
        {
            if (agent != null)
                agent.isStopped = true;
            return;
        }

        if (isPlayerInRange && Time.time >= nextAttackTime && !isPerformingSpecialAttack)
        {
            AttackPlayer();
        }
    }

    IEnumerator UpdateTargetPosition()
    {
        while (true)
        {
            if (player != null && agent != null && agent.isActiveAndEnabled &&
                bossHealth != null && bossHealth.IsAlive() && !isPerformingSpecialAttack)
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

    //COMPORTAMIENTOS
    public Transform GetPlayer() => player;
    public NavMeshAgent GetAgent() => agent;
    public bool IsAlive() => bossHealth != null && bossHealth.IsAlive();

    public BossDamageAbsorption AbsorptionBehavior => absorptionBehavior;

    public void SetSpecialAttackState(bool isAttacking)
    {
        isPerformingSpecialAttack = isAttacking;
        if (agent != null)
        {
            agent.isStopped = isAttacking;
        }
    }
    public bool IsPerformingSpecialAttack() => isPerformingSpecialAttack;

    protected virtual void OnDisable()
    {
        if (updateTargetCoroutine != null)
        {
            StopCoroutine(updateTargetCoroutine);
        }
    }
}