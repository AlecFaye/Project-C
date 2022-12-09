using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    enum ArmourType { VeryLow, Low, Medium, High, VeryHigh }

    readonly Dictionary<ArmourType, float> armourDamageReduction = new()
    {
        { ArmourType.VeryLow,  0.10f },
        { ArmourType.Low,      0.25f },
        { ArmourType.Medium,   0.50f },
        { ArmourType.High,     0.75f },
        { ArmourType.VeryHigh, 0.90f }
    };
    
    public Animator animator;

    [SerializeField] [Range(1, 200)] private float maxHealth = 1;
    [SerializeField] [Range(1, 200)] private float currentHealth = 1;

    [SerializeField] private ArmourType armourType = ArmourType.Medium;
    [SerializeField] private Weapon.WeaponType weaknessType = Weapon.WeaponType.Pickaxe;

    [SerializeField] [Range(1, 5)] private float weaknessDamageMultiplier = 1.5f;

    [SerializeField] private int attackDamage;
    [SerializeField] private float attackSpeed;

    [SerializeField] private Transform damagePopupSpawn;

    public bool isDead = false;

    public void TakeDamage(float damageTaken, Weapon.WeaponType damageType) 
    {
        if (isDead) return;

        damageTaken *= damageType == weaknessType ? weaknessDamageMultiplier : 1.0f;
        damageTaken *= (1 - armourDamageReduction[armourType]);

        SpawnDamagePopup(damageTaken);

        Mathf.Clamp(currentHealth - damageTaken, 0, maxHealth);

        if (currentHealth <= 0)
        {
            isDead = true;
            animator.SetBool("isDead", isDead);
        }
    }

    private void SpawnDamagePopup(float damage)
    {
        DamagePopup.Create(damagePopupSpawn.position, damage);
    }
}
