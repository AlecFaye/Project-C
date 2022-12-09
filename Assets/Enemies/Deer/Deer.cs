using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Deer : Enemy
{
    private const float IDLE_TIME = 5.0f;
    private const float EAT_GRASS_TIME = 7.5f;
    private const float MAX_WALK_TIME = 10.0f;

    private enum EnemyState { Idle, Walk, EatGrass, Run }

    private readonly Dictionary<EnemyState, System.Action> stateFunctions = new();

    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private SphereCollider playerDetector;
    
    private readonly float distanceToRunAway = 10.0f;

    private Vector3 unawareDetectorPosition = new(0, 0, 1.5f);
    private Vector3 awareDetectorPosition = new(0, 0, 0);

    private readonly float unawareRadius = 5.0f;
    private readonly float awareRadius = 7.5f;

    private bool isPlayerInRange = false;

    private EnemyState currentState = EnemyState.Idle;
    
    private float idleTimer = 0.0f;

    private void Start()
    {
        SetPlayerDetector(unawareDetectorPosition, unawareRadius);

        stateFunctions.Add(EnemyState.Idle, Idle);
        stateFunctions.Add(EnemyState.Walk, Walk);
        stateFunctions.Add(EnemyState.EatGrass, EatGrass);
        stateFunctions.Add(EnemyState.Run, Run);
    }

    private void Update()
    {
        if (isDead) return;

        stateFunctions[currentState]();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            currentState = EnemyState.Run;
            SetPlayerDetector(awareDetectorPosition, awareRadius);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            SetAllAnimatorParametersToFalse();

            Vector3 direction = (this.transform.position - other.transform.position).normalized;
            Vector3 positionToRunAwayTo = direction * distanceToRunAway + this.transform.position;

            RunAwayFromPlayer(positionToRunAwayTo);
        }
    }

    private void Idle()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            idleTimer = IDLE_TIME;
            SetAllAnimatorParametersToFalse();
        }

        idleTimer -= Time.deltaTime;
        
        if (idleTimer < 0)
        {
            ChooseRandomIdleState();
        }
    }

    private void Walk()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            idleTimer = MAX_WALK_TIME;
            animator.SetBool("isWalking", true);
            navMeshAgent.SetDestination(GetRandomLocation());
        }

        idleTimer -= Time.deltaTime;

        if (HasReachedDestination() || idleTimer < 0.0f)
        {
            animator.SetBool("isWalking", false);
            ChooseRandomIdleState();
        }
    }

    private void EatGrass()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Eating"))
        {
            idleTimer = EAT_GRASS_TIME;
            animator.SetBool("isEating", true);
        }

        idleTimer -= Time.deltaTime;

        if (idleTimer < 0)
        {
            animator.SetBool("isEating", false);
            ChooseRandomIdleState();
        }
    }

    private void Run()
    {
        if (!isPlayerInRange)
        {
            if (HasReachedDestination())
            {
                animator.SetBool("isRunning", false);
                SetPlayerDetector(unawareDetectorPosition, unawareRadius);

                ChooseRandomIdleState();
            }
        }
    }

    private void RunAwayFromPlayer(Vector3 destination)
    {
        currentState = EnemyState.Run;
        animator.SetBool("isRunning", true);
        navMeshAgent.SetDestination(destination);
    }

    private bool HasReachedDestination()
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                return true;
            }
        }
        return false;
    }

    private void SetAllAnimatorParametersToFalse()
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            animator.SetBool(parameter.name, false);
        }
    }

    private void SetPlayerDetector(Vector3 detectionPosition, float detectionRadius)
    {
        playerDetector.center = detectionPosition;
        playerDetector.radius = detectionRadius;
    }

    private void ChooseRandomIdleState()
    {
        currentState = (EnemyState)Random.Range(0, 2 + 1);
    }

    Vector3 GetRandomLocation()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        int t = Random.Range(0, navMeshData.indices.Length - 3);

        Vector3 point = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[t]], navMeshData.vertices[navMeshData.indices[t + 1]], Random.value);
        Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[t + 2]], Random.value);

        return point;
    }
}
