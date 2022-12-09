using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ArrowFunction : MonoBehaviour
{
    private Rigidbody arrowRigidbody;

    public float damageValue;

    private void Awake()
    {
        arrowRigidbody = GetComponent<Rigidbody>();
    }

    public void Create(float speed, float damage)
    {
        damageValue = damage;
        arrowRigidbody.velocity = transform.forward * speed;
    }

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player" && other.tag != "Item") {
            Debug.Log("Arrow damage: " + damageValue);
            arrowRigidbody.velocity = Vector3.zero;
            transform.SetParent(other.transform);
            yield return new WaitForSeconds(5);
            Destroy(gameObject);
        }
    }
}
