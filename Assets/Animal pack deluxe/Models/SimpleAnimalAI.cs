using UnityEngine;
using UnityEngine.AI;

public class SimpleAnimalAI : MonoBehaviour
{
    [Header("Settings")]
    public Transform player;
    public float detectionRange = 5f;
    public float wanderRadius = 10f;
    [Range(0f, 1f)]
    public float eatChance = 0.2f;
    public bool isAggressive = true; // если false — убегает от игрока

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null)
            Debug.LogError("NavMeshAgent не найден на " + gameObject.name);
        if (animator == null)
            Debug.LogError("Animator не найден на " + gameObject.name);

        PickRandomState();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance < detectionRange)
        {
            ReactToPlayer();
        }
        else
        {
            WanderBehaviour();
        }

        // плавная анимация движения
        if (animator != null)
            animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void ReactToPlayer()
    {
        if (animator != null)
            animator.SetBool("IsEating", false);

        if (isAggressive)
        {
            agent.SetDestination(player.position);
            if (animator != null)
                animator.SetBool("IsAttacking", true);
        }
        else
        {
            Vector3 dir = (transform.position - player.position).normalized;
            Vector3 runTo = transform.position + dir * 5f;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(runTo, out hit, 5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            if (animator != null)
                animator.SetBool("IsAttacking", false);
        }
    }

    void WanderBehaviour()
    {
        if (animator != null)
            animator.SetBool("IsAttacking", false);

        if (!agent.hasPath || agent.remainingDistance < 1f)
        {
            PickRandomState();
        }
    }

    void PickRandomState()
    {
        if (Random.value < eatChance)
        {
            agent.ResetPath();
            if (animator != null)
                animator.SetBool("IsEating", true);
        }
        else
        {
            if (animator != null)
                animator.SetBool("IsEating", false);

            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }
}