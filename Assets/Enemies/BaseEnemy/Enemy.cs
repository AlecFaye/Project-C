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
    public float attackDamage;
    public float attackDelay;

    private const string ATTACK_TRIGGER = "Throw";

    private Coroutine lookCoroutine;

    readonly Dictionary<ArmourType, float> armourDamageReduction = new()
    {
        { ArmourType.VeryLow, 0.10f },
        { ArmourType.Low, 0.25f },
        { ArmourType.Medium, 0.50f },
        { ArmourType.High, 0.75f },
        { ArmourType.VeryHigh, 0.90f }
    };

    [SerializeField] private Transform damagePopupSpawn;

    private void Awake()
    {
        attackRadius.OnAttack += OnAttack;
    }

    private void OnAttack(IDamageable target)
    {
        animator.SetTrigger(ATTACK_TRIGGER);

        if (lookCoroutine != null)
        {
            StopCoroutine(lookCoroutine);
        }

        lookCoroutine = StartCoroutine(LookAt(target.GetTransform()));
    }

    private IEnumerator LookAt(Transform target)
    {
        Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position);
        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);

            time += Time.deltaTime * 2;

            yield return null;
        }

        transform.rotation = lookRotation;
    }

    public virtual void OnEnable()
    {
        SetupAgentFromConfiguration();
    }

    public override void OnDisable()
    {
        base.OnDisable();

        agent.enabled = false;
    }

    public virtual void SetupAgentFromConfiguration()
    {
        currentHealth = enemyScriptableObject.health;
        maxHealth = enemyScriptableObject.health;
        armourType = enemyScriptableObject.armourType;
        weaknessTypes = enemyScriptableObject.weaknessTypes;
        weaknessDamageMultiplier = enemyScriptableObject.weaknessDamageMultiplier;
        attackDamage = enemyScriptableObject.attackDamage;
        attackDelay = enemyScriptableObject.attackDelay;

        agent.acceleration = enemyScriptableObject.acceleration;
        agent.angularSpeed = enemyScriptableObject.angularSpeed;
        agent.areaMask = enemyScriptableObject.areaMask;
        agent.avoidancePriority = enemyScriptableObject.avoidancePriority;
        agent.baseOffset = enemyScriptableObject.baseOffset;
        agent.obstacleAvoidanceType = enemyScriptableObject.obstacleAvoidanceType;
        agent.radius = enemyScriptableObject.radius;
        agent.speed = enemyScriptableObject.speed;
        agent.stoppingDistance = enemyScriptableObject.stoppingDistance;

        movement.updateRate = enemyScriptableObject.aiUpdateInterval;

        attackRadius.sphereCollider.radius = enemyScriptableObject.attackRadius;
        attackRadius.attackDelay = enemyScriptableObject.attackDelay;
        attackRadius.damage = enemyScriptableObject.attackDamage;
    }

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
