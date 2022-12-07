using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class Weapon : using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "New Weapon")]
public class Weapon : ScriptableObject
{
    public enum WeaponType
    {
        NONE,
        AXE,
        BOW,
        PICKAXE,
        TOME
    }

    public string weaponName;
    public string description;
    

    
    public Transform weaponModel;

    public float damageValue;

    public WeaponType weaponType;

    public void Attack(WeaponType weaponChosen)
    {
        switch (weaponChosen)
        {
            case WeaponType.NONE:
               Debug.Log("Wait stop should be NONE");
               break;
            case WeaponType.AXE:
               Debug.Log("Attack with an AXE");
               break;
            case WeaponType.BOW:
               Debug.Log("Attack with a BOW");
               break;
            case WeaponType.PICKAXE:
               Debug.Log("Attack with a PICKAXE");
               break;
            case WeaponType.TOME:
               Debug.Log("Attack with a TOME");
               break;
        }
    }

}
