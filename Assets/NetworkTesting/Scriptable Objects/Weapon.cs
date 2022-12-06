using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class Weapon : using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "New Weapon")]
public class Weapon : ScriptableObject
{
    public enum DamageType
    {
        None,
        Axe,
        Bow,
        Pickaxe,
        Tome
    }

    public string weaponName;
    public string description;
    

    
    public Transform weaponModel;

    public int damageValue;

    public DamageType damageType;


}
