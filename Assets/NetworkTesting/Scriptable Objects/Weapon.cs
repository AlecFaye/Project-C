using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using StarterAssets;
using Unity.Services.Lobbies.Models;

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

    public ThirdPersonController _player;

    
}
