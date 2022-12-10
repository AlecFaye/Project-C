using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public float updateRate = 0.1f;

    private NavMeshAgent _agent;
    private Animator _animator;

    private const string IS_WALKING = "isWalking";
    private const string IS_RUNNING = "isRunning";

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _animator.SetBool(IS_WALKING, _agent.velocity.magnitude > 0.0f);
    }
}
