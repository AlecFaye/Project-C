using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class Weapon : using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "New Weapon")]
public class Weapon : ScriptableObject
{
    public enum WeaponType
    {
        None,
        Axe,
        Bow,
        Pickaxe,
        Tome
    }

    [Header("Texts/View")]
    public string weaponName;
    public string description;
    public Sprite inventorySprite;
    public Transform weaponModel;

    [Header("Damage Stats")]
    public float damageValue;

    public WeaponType weaponType;

    [Header("Animation Stats")]
    [Tooltip("If the character can Attack or not.")]
    public bool CanAttack = true;

    [Tooltip("If the character is Attacking or not.")]
    public bool IsAttacking = false;

    [Tooltip("How long the Attack goes for (float).")]
    public float AttackingTime = 0.8f;

    [Tooltip("Attack Cooldown value (float).")]
    public float AttackingCooldown = 1f;



    public void Attack()
    {
        switch (weaponType)
        {
            case WeaponType.None:
               Debug.Log("Wait stop should be NONE");
               break;
            case WeaponType.Axe:
               Debug.Log("Attack with an AXE");
               break;
            case WeaponType.Bow:
               Debug.Log("Attack with a BOW");
               break;
            case WeaponType.Pickaxe:
               Debug.Log("Attack with a PICKAXE");
               break;
            case WeaponType.Tome:
               Debug.Log("Attack with a TOME");
               break;
        }
    }

}
