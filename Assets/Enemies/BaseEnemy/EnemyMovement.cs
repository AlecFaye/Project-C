using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public float updateRate = 0.1f;
    public NavMeshTriangulation triangulation;
    public EnemyLineOfSightChecker lineOfSightChecker;

    private NavMeshAgent agent;
    private Animator animator;
    private IDamageable player;

    private EnemyState currentState;
    public EnemyState defaultState;
    public EnemyState State
    {
        get
        {
            return currentState;
        }
        set
        {
            OnStateChange?.Invoke(currentState, value);
            currentState = value;
        }
    }

    public delegate void StateChangeEvent(EnemyState oldState, EnemyState newState);
    public StateChangeEvent OnStateChange;

    public float idleLocationRadius = 4.0f;
    public float idleMoveSpeedMultiplier = 0.5f;
    public Vector3[] waypoints;

    [SerializeField] private int waypointIndex = 0;

    private Dictionary<string, bool> animatorParameters = new();

    private const string IS_WALKING = "isWalking";
    private const string IS_RUNNING = "isRunning";

    private Coroutine followCoroutine = null;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        OnStateChange += HandleStateChange;
    }

    private void Start()
    {
        lineOfSightChecker.OnGainSight += HandleGainSight;
        lineOfSightChecker.OnLoseSight += HandleLoseSight;

        foreach (AnimatorControllerParameter param in animator.parameters)
            animatorParameters.Add(param.name, true);
    }

    private void HandleGainSight(IDamageable player)
    {
        this.player = player;
        
        if (TryGetComponent(out Enemy enemy))
        {
            if (lineOfSightChecker.sphereCollider.radius == lineOfSightChecker.unawareLineOfSightRadius)
                lineOfSightChecker.sphereCollider.radius = lineOfSightChecker.awareLineOfSightRadius;

            if (enemy.enemyScriptableObject.attackConfiguration.attackType == AttackType.Fleeing)
            {
                State = EnemyState.Flee;
            }
            else
            {
                State = EnemyState.Chase;
            }
        }
    }

    private void HandleLoseSight(IDamageable player)
    {
        this.player = null;
        State = defaultState;

        if (lineOfSightChecker.sphereCollider.radius == lineOfSightChecker.awareLineOfSightRadius)
            lineOfSightChecker.sphereCollider.radius = lineOfSightChecker.unawareLineOfSightRadius;
    }
    
    private void Update()
    {
        UpdateMovementAnimator();
    }

    private void OnDisable()
    {
        currentState = defaultState;
    }

    private void UpdateMovementAnimator()
    {
        animatorParameters.TryGetValue(IS_WALKING, out bool hasWalkAnimation);
        animatorParameters.TryGetValue(IS_RUNNING, out bool hasRunAnimation);

        switch (currentState)
        {
            case EnemyState.Chase: case EnemyState.Flee:
                if (hasWalkAnimation && hasRunAnimation)
                {
                    animator.SetBool(IS_WALKING, false);
                    animator.SetBool(IS_RUNNING, agent.velocity.magnitude > 0.01f);
                }
                else if (hasWalkAnimation)
                {
                    animator.SetBool(IS_WALKING, agent.velocity.magnitude > 0.01f);
                }
                else if (hasRunAnimation)
                {
                    animator.SetBool(IS_RUNNING, agent.velocity.magnitude > 0.01f);
                }
                break;
            default:
                if (hasWalkAnimation && hasRunAnimation)
                {
                    animator.SetBool(IS_RUNNING, false);
                    animator.SetBool(IS_WALKING, agent.velocity.magnitude > 0.01f);
                }
                else if (hasWalkAnimation)
                {
                    animator.SetBool(IS_WALKING, agent.velocity.magnitude > 0.01f);
                }
                else if (hasRunAnimation)
                {
                    animator.SetBool(IS_RUNNING, agent.velocity.magnitude > 0.01f);
                }
                break;
        }
    }

    public void Spawn()
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (NavMesh.SamplePosition(triangulation.vertices[Random.Range(0, triangulation.vertices.Length)], out NavMeshHit hit, 2f, agent.areaMask))
            {
                waypoints[i] = hit.position;
            }
            else
            {
                Debug.LogError("Unable to find position for NavMesh near Triangulation!");
            }
        }
        OnStateChange?.Invoke(EnemyState.Spawn, defaultState);
    }

    private void HandleStateChange(EnemyState oldState, EnemyState newState)
    {
        if (oldState != newState)
        {
            if (followCoroutine != null)
                StopCoroutine(followCoroutine);

            if (oldState == EnemyState.Idle)
                agent.speed /= idleMoveSpeedMultiplier;

            switch (newState)
            {
                case EnemyState.Idle:
                    followCoroutine = StartCoroutine(DoIdleMotion());
                    break;
                case EnemyState.Patrol:
                    followCoroutine = StartCoroutine(DoPatrolMotion());
                    break;
                case EnemyState.Chase:
                    followCoroutine = StartCoroutine(FollowTarget());
                    break;
                case EnemyState.Flee:
                    followCoroutine = StartCoroutine(FleeTarget());
                    break;
            }
        }
    }

    private IEnumerator DoIdleMotion()
    {
        WaitForSeconds wait = new(updateRate);

        agent.speed *= idleMoveSpeedMultiplier;

        while(true)
        {
            if (!agent.enabled || !agent.isOnNavMesh)
            {
                yield return wait;
            }
            else if (agent.remainingDistance <= agent.stoppingDistance)
            {
                Vector3 point = Random.insideUnitSphere * idleLocationRadius;

                if (NavMesh.SamplePosition(agent.transform.position + point, out NavMeshHit hit, 2f, agent.areaMask))
                {
                    agent.SetDestination(hit.position);
                }
            }

            yield return wait;
        }
    }

    private IEnumerator DoPatrolMotion()
    {
        WaitForSeconds wait = new(updateRate);

        yield return new WaitUntil(() => agent.enabled && agent.isOnNavMesh);

        agent.SetDestination(waypoints[waypointIndex]);

        while(true)
        {
            if (agent.enabled && agent.isOnNavMesh && agent.remainingDistance <= agent.stoppingDistance)
            {
                waypointIndex++;

                if (waypointIndex >= waypoints.Length)
                {
                    waypointIndex = 0;
                }

                agent.SetDestination(waypoints[waypointIndex]);
            }

            yield return wait;
        }
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds wait = new(updateRate);

        while (true)
        {
            if (agent.enabled)
            {
                agent.SetDestination(player.GetTransform().position);
            }
            yield return wait;
        }
    }

    private IEnumerator FleeTarget()
    {
        WaitForSeconds wait = new(updateRate);

        while (true)
        {
            if (agent.enabled)
            {
                Vector3 direction = (transform.position - player.GetTransform().position).normalized;
                Vector3 destination = transform.position + direction * 10.0f;

                agent.SetDestination(destination);
            }
            yield return wait;
        }
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            Gizmos.DrawWireSphere(waypoints[i], 0.25f);
            if (i + 1 < waypoints.Length)
            {
                Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
            }
            else
            {
                Gizmos.DrawLine(waypoints[i], waypoints[0]);
            }
        }
    }
}
