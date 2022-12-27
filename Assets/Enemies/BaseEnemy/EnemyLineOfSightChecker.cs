using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyLineOfSightChecker : MonoBehaviour
{
    public SphereCollider sphereCollider;
    public float fieldOfView = 90.0f;
    public LayerMask lineOfSightLayers;

    public float awareLineOfSightRadius;
    public float unawareLineOfSightRadius;

    public delegate void GainSightEvent(IDamageable player);
    public GainSightEvent OnGainSight;

    public delegate void LoseSightEvent(IDamageable player);
    public LoseSightEvent OnLoseSight;

    private Coroutine checkForLineOfSightCoroutine;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable player))
        {
            if (!CheckLineOfSight(player, fieldOfView))
            {
                checkForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(player));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IDamageable player))
        {
            OnLoseSight?.Invoke(player);

            if (sphereCollider.radius == awareLineOfSightRadius)
                sphereCollider.radius = unawareLineOfSightRadius;
            
            if (checkForLineOfSightCoroutine != null)
            {
                StopCoroutine(checkForLineOfSightCoroutine);
            }
        }
    }

    private bool CheckLineOfSight(IDamageable player, float fov)
    {
        Vector3 playerOffset = new(0, player.GetTransform().GetComponent<CharacterController>().height / 2.0f, 0);
        Vector3 direction = (player.GetTransform().position + playerOffset - transform.position).normalized;

        if (Vector3.Dot(transform.forward, direction) >= Mathf.Cos(fov))
        {
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, sphereCollider.radius, lineOfSightLayers))
            {
                if (hit.transform.GetComponent<IDamageable>() != null)
                {
                    OnGainSight?.Invoke(player);
                    return true;
                }
            }
        }

        return false;
    }

    private IEnumerator CheckForLineOfSight(IDamageable player)
    {
        WaitForSeconds wait = new(0.1f);

        while (!CheckLineOfSight(player, 180.0f))
        {
            yield return wait;
        }
    }
}
