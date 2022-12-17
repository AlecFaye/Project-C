using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : PoolableObject, IDamageable
{
    public EnemyMovement movement;
    public Animator animator;
    public NavMeshAgent agent;
    public EnemyScriptableObject enemyScriptableObject;
    public AttackRadius attackRadius;

    public float currentHealth = 1;
    public float maxHealth = 1;
    public ArmourType armourType = ArmourType.Medium;
    public List<Weapon.WeaponType> weaknessTypes = new() { Weapon.WeaponType.Pickaxe };
    public float weaknessDamageMultiplier = 1.5f;
    public float damage;
    public float attackDelay;

    private const string IS_ATTACKING = "isAttacking";

    private Coroutine lookCoroutine;

    private readonly Dictionary<ArmourType, float> armourDamageReduction = new()
    {
        { ArmourType.None,     0.0f  },
        { ArmourType.VeryLow,  0.10f },
        { ArmourType.Low,      0.25f },
        { ArmourType.Medium,   0.50f },
        { ArmourType.High,     0.75f },
        { ArmourType.VeryHigh, 0.90f }
    };

    [SerializeField] private Transform damagePopupSpawn;

    private void Awake()
    {
        attackRadius.OnAttack += OnAttack;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        agent.enabled = false;
    }

    #region Animation Triggers
    public void StartMeleeDamage()
    {
        Debug.Log("START dealing Melee Damage");
    }

    public void StopMeleeDamage()
    {
        Debug.Log("STOP dealing Melee Damage");
    }

    public void ReleaseProjectile()
    {
        GetComponentInChildren<RangedAttackRadius>().ReleaseProjectile();
    }

    public void FinishedAttacking()
    {
        animator.SetBool(IS_ATTACKING, false);

        if (!enemyScriptableObject.attackConfiguration.isRanged)
            agent.enabled = true;


    }
    #endregion

    #region Attack Functions
    private void OnAttack(IDamageable target)
    {
        animator.SetBool(IS_ATTACKING, true);

        if (lookCoroutine != null)
            StopCoroutine(lookCoroutine);
        lookCoroutine = StartCoroutine(LookAt(target.GetTransform()));
    }

    private IEnumerator LookAt(Transform target)
    {
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(target.position.x, 0, target.position.z) - new Vector3(transform.position.x, 0, transform.position.z));
        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);

            time += Time.deltaTime * 2;

            yield return null;
        }

        transform.rotation = lookRotation;
    }
    #endregion

    public void TakeDamage(float damageTaken, Weapon.WeaponType damageType)
    {
        foreach (Weapon.WeaponType weakness in weaknessTypes)
        {
            if (damageType == weakness)
                damageTaken *= weaknessDamageMultiplier;
        }
        damageTaken *= (1 - armourDamageReduction[armourType]);

        SpawnDamagePopup(damageTaken);

        currentHealth -= damageTaken;

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

    private void SpawnDamagePopup(float damage)
    {
        DamagePopup.Create(damagePopupSpawn.position, damage);
    }
}