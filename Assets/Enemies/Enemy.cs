using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    enum WeaknessType { None, PickAxe, Axe, Bow, Tome }
    enum ArmourType { VeryLow, Low, Medium, High, VeryHigh }

    readonly Dictionary<ArmourType, float> armourDamageReduction = new()
    {
        { ArmourType.VeryLow,  0.10f },
        { ArmourType.Low,      0.25f },
        { ArmourType.Medium,   0.50f },
        { ArmourType.High,     0.75f },
        { ArmourType.VeryHigh, 0.90f }
    };

    [SerializeField] [Range(1, 200)] private float maxHealth = 1;
    [SerializeField] [Range(1, 200)] private float currentHealth = 1;

    [SerializeField] private ArmourType armourType = ArmourType.Medium;
    [SerializeField] private Weapon.WeaponType weaknessType = Weapon.WeaponType.Pickaxe;

    [SerializeField] [Range(25, 150)] private float movementSpeed = 50.0f;
    [SerializeField] [Range(1, 5)] private float weaknessDamageMultiplier = 1.5f;

    [SerializeField] private int attackDamage;
    [SerializeField] private float attackSpeed;

    public void TakeDamage(float damageTaken, Weapon.WeaponType damageType) 
    {
        damageTaken *= damageType == weaknessType ? weaknessDamageMultiplier : 1.0f;
        damageTaken *= (1 - armourDamageReduction[armourType]);
        
        Mathf.Clamp(currentHealth - damageTaken, 0, maxHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Aggro onto Player!");
            // Aggro onto the Player
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("De-Aggro off Player!");
            // De-Aggro off the Player
        }
    }
}
