using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamFunction : MonoBehaviour
{
    public float damageValue;

    public List<Collider> enemiesHitList = new List<Collider>(); // Checks if the arrow has hit a target (true == max one target hit, false == max one target not hit yet)

    public IEnumerator Create(float damage)
    {
        damageValue = damage;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (Collider enemy in enemiesHitList) { Debug.Log(enemy.name); }
        
        if (other.CompareTag("Enemy") && !enemiesHitList.Contains(other)) {
            Debug.Log("You did " + damageValue + " damage to " + other.name);
            CollisionDetected collisionDetected = other.GetComponent<CollisionDetected>();

            if (collisionDetected) {
                collisionDetected.Hit(damageValue, Weapon.WeaponType.Bow); // Runs the funtion "Hit" in the other objects CollisionDetected script
                enemiesHitList.Add(other); // Adds current enemy to enemiesHitList to keep track of
            }
        }
    }
}
