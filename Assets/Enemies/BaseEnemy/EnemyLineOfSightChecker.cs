using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyLineOfSightChecker : MonoBehaviour
{
    public SphereCollider sphereCollider;
    public float fieldOfView = 90.0f;
    public LayerMask lineOfSightLayers;

    public delegate void GainSightEvent(Player player);
    public GainSightEvent OnGainSight;

    public delegate void LoseSightEvent(Player player);
    public LoseSightEvent OnLoseSight;

    private Coroutine checkForLineOfSightCoroutine;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (!CheckLineOfSight(player))
            {
                checkForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(player));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            OnLoseSight?.Invoke(player);
            if (checkForLineOfSightCoroutine != null)
            {
                StopCoroutine(checkForLineOfSightCoroutine);
            }
        }
    }

    private bool CheckLineOfSight(Player player)
    {
        Vector3 playerOffset = new(0, player.GetComponent<CharacterController>().height / 2, 0);
        Vector3 direction = (player.transform.position + playerOffset - transform.position).normalized;

        if (Vector3.Dot(transform.forward, direction) >= Mathf.Cos(fieldOfView))
        {
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, sphereCollider.radius, lineOfSightLayers))
            {
                if (hit.transform.GetComponent<Player>() != null)
                {
                    OnGainSight?.Invoke(player);
                    return true;
                }
            }
        }

        return false;
    }

    private IEnumerator CheckForLineOfSight(Player player)
    {
        WaitForSeconds wait = new(0.1f);

        while (!CheckLineOfSight(player))
        {
            yield return wait;
        }
    }
}
