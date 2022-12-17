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
    private Player player;

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
    private Coroutine changeBaseOffsetCoroutine = null;

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

    private void HandleGainSight(Player player)
    {
        this.player = player;
        State = EnemyState.Chase;
    }

    private void HandleLoseSight(Player player)
    {
        this.player = null;
        State = defaultState;
    }

    private void Update()
    {
        if (currentState == EnemyState.Chase)
        {
            if (animatorParameters.TryGetValue(IS_WALKING, out _))
                animator.SetBool(IS_WALKING, false);

            if (animatorParameters.TryGetValue(IS_RUNNING, out _))
                animator.SetBool(IS_RUNNING, agent.velocity.magnitude > 0.01f);
            else
                animator.SetBool(IS_WALKING, agent.velocity.magnitude > 0.01f);
        }
        else
        {
            if (animatorParameters.TryGetValue(IS_RUNNING, out _))
                animator.SetBool(IS_RUNNING, false);

            if (animatorParameters.TryGetValue(IS_WALKING, out _))
                animator.SetBool(IS_WALKING, agent.velocity.magnitude > 0.01f);
        }
    }

    private void OnDisable()
    {
        currentState = defaultState;
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

            if (oldState == EnemyState.Chase)
            {
                if (IsFlyingEnemy())
                {
                    if (changeBaseOffsetCoroutine != null)
                        StopCoroutine(changeBaseOffsetCoroutine);
                    changeBaseOffsetCoroutine = StartCoroutine(ChangeBaseOffset(4.0f));
                }
            }

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

        if (changeBaseOffsetCoroutine != null)
        {
            StopCoroutine(changeBaseOffsetCoroutine);
            changeBaseOffsetCoroutine = null;
        }

        while (true)
        {
            if (agent.enabled)
            {
                agent.SetDestination(player.transform.position);

                if (IsFlyingEnemy())
                {
                    Vector3 playerPosition = new(player.transform.position.x, 0, player.transform.position.z);
                    Vector3 enemyPosition = new(transform.position.x, 0, transform.position.z);

                    if (Vector3.Distance(playerPosition, enemyPosition) < 5.0f)
                    {
                        if (changeBaseOffsetCoroutine == null)
                            changeBaseOffsetCoroutine = StartCoroutine(ChangeBaseOffset(1.2f));
                    }
                }
            }
            yield return wait;
        }
    }

    private IEnumerator ChangeBaseOffset(float targetHeight)
    {
        WaitForSeconds wait = new(updateRate);

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("FlyingAttack"))
            yield return wait;

        float timeElapsed = 0;
        float duration = 2.0f;

        while (timeElapsed < duration)
        {
            while (animator.GetCurrentAnimatorStateInfo(0).IsName("FlyingAttack"))
                yield return wait;

            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);

            agent.baseOffset = Mathf.Lerp(agent.baseOffset, targetHeight, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        agent.baseOffset = targetHeight;
    }

    private bool IsFlyingEnemy()
    {
        Enemy enemy = agent.GetComponentInParent<Enemy>();
        if (enemy)
            return enemy.enemyScriptableObject.attackConfiguration.isFlying;
        return false;
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