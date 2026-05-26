using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Base zombie controller with pathfinding and basic AI behaviors.
/// </summary>
public class ZombieController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 30f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Detection")]
    [SerializeField] private float visionRange = 20f;
    [SerializeField] private float visionAngle = 90f;
    [SerializeField] private float hearingRange = 30f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Night Boost")]
    [SerializeField] private float nightDamageMultiplier = 1.5f;
    [SerializeField] private float nightSpeedMultiplier = 1.3f;

    // State
    private enum ZombieState { Idle, Patrolling, Chasing, Attacking, Dead }
    private ZombieState currentState = ZombieState.Idle;
    private Transform playerTransform;
    private float lastAttackTime = 0f;
    private float baseSpeed;

    private void Start()
    {
        currentHealth = maxHealth;
        playerTransform = FindObjectOfType<PlayerController>()?.transform;
        
        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();

        baseSpeed = navMeshAgent.speed;
    }

    private void Update()
    {
        if (currentHealth <= 0)
            return;

        // Update based on game time
        UpdateNightModifiers();

        // Check for player
        if (CanSeePlayer())
        {
            ChasePlayer();
        }
        else if (CanHearPlayer())
        {
            MoveTowards(playerTransform.position);
        }
        else
        {
            Patrol();
        }
    }

    private void UpdateNightModifiers()
    {
        if (GameManager.Instance.IsNight)
        {
            navMeshAgent.speed = baseSpeed * nightSpeedMultiplier;
        }
        else
        {
            navMeshAgent.speed = baseSpeed;
        }
    }

    private bool CanSeePlayer()
    {
        if (playerTransform == null) return false;

        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distToPlayer > visionRange) return false;

        // Check if in front
        Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dirToPlayer) > visionAngle) return false;

        // Raycast to check line of sight
        if (Physics.Raycast(transform.position, dirToPlayer, distToPlayer, ~playerLayer))
            return false;

        return true;
    }

    private bool CanHearPlayer()
    {
        if (playerTransform == null) return false;
        return Vector3.Distance(transform.position, playerTransform.position) < hearingRange;
    }

    private void ChasePlayer()
    {
        currentState = ZombieState.Chasing;
        MoveTowards(playerTransform.position);

        if (Vector3.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            AttackPlayer();
        }
    }

    private void MoveTowards(Vector3 target)
    {
        navMeshAgent.SetDestination(target);
        
        if (animator != null)
        {
            float speed = navMeshAgent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
        }
    }

    private void Patrol()
    {
        currentState = ZombieState.Patrolling;

        if (!navMeshAgent.hasPath || navMeshAgent.remainingDistance < 0.5f)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * 20f;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 20f, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
            }
        }
    }

    private void AttackPlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;
        currentState = ZombieState.Attacking;

        float appliedDamage = damage;
        if (GameManager.Instance.IsNight)
            appliedDamage *= nightDamageMultiplier;

        PlayerStats playerStats = playerTransform.GetComponent<PlayerStats>();
        if (playerStats != null)
            playerStats.TakeDamage(appliedDamage, "Zombie Attack");

        if (animator != null)
            animator.SetTrigger("Attack");

        // Sound effect
        if (audioSource != null)
            audioSource.PlayOneShot(audioSource.clip);
    }

    /// <summary>
    /// Take damage
    /// </summary>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (animator != null)
        {
            animator.SetTrigger("Damage");
        }
    }

    private void Die()
    {
        currentState = ZombieState.Dead;
        navMeshAgent.enabled = false;
        
        if (animator != null)
            animator.SetTrigger("Death");

        // Disable after animation
        StartCoroutine(DisableAfterDelay(3f));
    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    public float GetHealthPercent() => currentHealth / maxHealth;
}