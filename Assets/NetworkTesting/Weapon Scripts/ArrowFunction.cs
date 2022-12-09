using System.Collections;
using System.Collections.Generic;
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
        yield return new WaitForSeconds(10);
        Destroy(this);
    }
}
