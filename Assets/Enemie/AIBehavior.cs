using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    private NavMeshAgent agent;
    private Animator animator;

    [Header("Detection")]
    public float detectionRadius = 6f;
    public float attackDistance = 2f;
    public float losePlayerDistance = 25f;

    [Header("Patrol")]
    public Transform patrolCenter;
    public float patrolRadius = 10f;
    public float idleTime = 2f;

    [Header("Combat")]
    public float attackCooldown = 1.5f;
    public float screamDuration = 2f;

    [Header("Performance")]
    public float aiUpdateRate = 0.2f;
    public float pathUpdateRate = 0.3f;

    private float idleTimer;
    private float attackTimer;
    private float screamTimer;
    private float aiTimer;
    private float pathTimer;

    private bool hasScreamed = false;
    private bool isDead = false;

    private State currentState;

    private enum State
    {
        Patrol,
        Idle,
        Scream,
        Chase,
        Attack
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        ChangeState(State.Patrol);
    }

    void Update()
    {
        if (player == null || isDead) return;

        aiTimer -= Time.deltaTime;

        if (aiTimer > 0f) return;

        aiTimer = aiUpdateRate;

        float distance = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                PatrolUpdate(distance);
                break;

            case State.Idle:
                IdleUpdate(distance);
                break;

            case State.Scream:
                ScreamUpdate();
                break;

            case State.Chase:
                ChaseUpdate(distance);
                break;

            case State.Attack:
                AttackUpdate(distance);
                break;
        }

        float speed = agent.desiredVelocity.magnitude;
animator.SetFloat("Speed", speed, 0.15f, Time.deltaTime);
    }

    void ChangeState(State newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (newState)
        {
            case State.Patrol:

                agent.isStopped = false;
                SetRandomDestination();

                break;

            case State.Idle:

                agent.ResetPath();
                agent.isStopped = true;
                idleTimer = idleTime;

                break;

            case State.Scream:

                agent.ResetPath();
                agent.isStopped = true;

                screamTimer = screamDuration;

                animator.SetTrigger("Scream");

                break;

            case State.Chase:

                agent.isStopped = false;

                break;

            case State.Attack:

                agent.isStopped = true;
                attackTimer = 0f;

                break;
        }
    }

    void PatrolUpdate(float distance)
    {
        if (distance < detectionRadius)
        {
            if (!hasScreamed)
            {
                hasScreamed = true;
                ChangeState(State.Scream);
            }
            else
            {
                ChangeState(State.Chase);
            }
            return;
        }

        if (!agent.hasPath || agent.remainingDistance < 1f)
        {
            ChangeState(State.Idle);
        }
    }

    void IdleUpdate(float distance)
    {
        if (distance < detectionRadius)
        {
            if (!hasScreamed)
            {
                hasScreamed = true;
                ChangeState(State.Scream);
            }
            else
            {
                ChangeState(State.Chase);
            }
            return;
        }

        idleTimer -= aiUpdateRate;

        if (idleTimer <= 0)
        {
            ChangeState(State.Patrol);
        }
    }

    void ScreamUpdate()
    {
        screamTimer -= aiUpdateRate;

        if (screamTimer <= 0)
        {
            ChangeState(State.Chase);
        }
    }

    void ChaseUpdate(float distance)
    {
        if (distance > losePlayerDistance)
        {
            hasScreamed = false;
            ChangeState(State.Patrol);
            return;
        }

        if (distance <= attackDistance)
        {
            ChangeState(State.Attack);
            return;
        }

        pathTimer -= aiUpdateRate;

        if (pathTimer <= 0f)
        {
            pathTimer = pathUpdateRate;
            agent.stoppingDistance = attackDistance;
            agent.SetDestination(player.position);
        }
    }

    void AttackUpdate(float distance)
    {
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;

        if (lookPos != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookPos);

        attackTimer -= aiUpdateRate;

        if (distance > attackDistance)
        {
            ChangeState(State.Chase);
            return;
        }

        if (attackTimer <= 0f)
        {
            animator.SetTrigger("Attack");
            attackTimer = attackCooldown;
        }
    }

    void SetRandomDestination()
    {
        Vector3 randomPoint = Random.insideUnitSphere * patrolRadius;

        if (patrolCenter != null)
            randomPoint += patrolCenter.position;
        else
            randomPoint += transform.position;

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        agent.isStopped = true;
        agent.enabled = false;

        animator.SetBool("Dead", true);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, losePlayerDistance);

        if (patrolCenter != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(patrolCenter.position, patrolRadius);
        }
    }
}