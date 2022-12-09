using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ArrowFunction : MonoBehaviour
{
    private Rigidbody arrowRigidbody;

    private void Awake()
    {
        arrowRigidbody = GetComponent<Rigidbody>();
    }

    public void Create(float speed, float damage)
    {
        //damage += Arrow Damage
        Debug.Log("Arrow damage: " + damage);
        arrowRigidbody.velocity = transform.forward * speed;
        
    }

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") {
            arrowRigidbody.velocity = Vector3.zero;
            transform.SetParent(other.transform);
            yield return new WaitForSeconds(5);
            Destroy(gameObject);
        }
    }
}
