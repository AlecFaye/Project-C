using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using StarterAssets;
using Unity.Services.Lobbies.Models;


[CreateAssetMenu(fileName = "Weapon", menuName = "New Weapon")]
public class Weapon : ScriptableObject
{
    public ThirdPersonController _player;

    public enum WeaponType
    {
        None,
        Axe,
        Bow,
        Pickaxe,
        Tome
    }

    public WeaponType weaponType;

    public string weaponName;
    public string description;
    public Sprite inventorySprite;
    public Transform weaponModel;

    public bool CanAttack = true;

    public bool IsAttacking = false;

    public float attackingTime = 0.8f;

    public float attackingCooldown = 1f;

    public float damageValue;

    public float attackSpeed;

    #region AxeVariables

    public int cutLevel;

    #endregion

    #region BowVariables

    public float Max_Charge;
    public float currentCharge;
    public float chargeGainedRate;

    public Arrow _arrowType;

    #endregion
    
    #region PickaxeVariables

    public int mineLevel;

    #endregion
    
    #region TomeVariables

    public float Max_Held_Charge;
    public float currentHeldCharge;
    public float chargeLostRate;

    #endregion

}
