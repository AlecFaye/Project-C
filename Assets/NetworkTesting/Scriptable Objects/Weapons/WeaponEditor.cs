using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{
    #region SerializedProperties
    SerializedProperty _player;
    SerializedProperty WeaponType;

    SerializedProperty weaponName;
    SerializedProperty description;
    SerializedProperty inventorySprite;
    SerializedProperty weaponModel;

    SerializedProperty weaponType;


    SerializedProperty CanAttack;

    SerializedProperty IsAttacking;

    SerializedProperty attackingTime;

    SerializedProperty attackingCooldown;

    SerializedProperty damageValue;

    SerializedProperty attackSpeed;

    SerializedProperty cutLevel;
    
    SerializedProperty Max_Charge;
    SerializedProperty currentCharge;
    SerializedProperty chargeGainedRate;
    SerializedProperty _arrowType;

    SerializedProperty mineLevel;

    SerializedProperty Max_Held_Charge;
    SerializedProperty currentHeldCharge;
    SerializedProperty chargeLostRate;

    bool WeaponVisualInfoGroup, WeaponBaseStatsInfoGroup = true;
    #endregion


    private void OnEnable()
    {
        _player = serializedObject.FindProperty("_player");
        WeaponType = serializedObject.FindProperty("WeaponType");

        weaponName = serializedObject.FindProperty("weaponName");
        description = serializedObject.FindProperty("description");
        inventorySprite = serializedObject.FindProperty("inventorySprite");
        weaponModel = serializedObject.FindProperty("weaponModel");

        weaponType = serializedObject.FindProperty("weaponType");

        CanAttack = serializedObject.FindProperty("CanAttack");

        IsAttacking = serializedObject.FindProperty("IsAttacking");

        attackingTime = serializedObject.FindProperty("attackingTime");

        attackingCooldown = serializedObject.FindProperty("attackingCooldown");

        damageValue = serializedObject.FindProperty("damageValue");
        attackSpeed = serializedObject.FindProperty("attackSpeed");

        cutLevel = serializedObject.FindProperty("cutLevel");

        Max_Charge = serializedObject.FindProperty("Max_Charge");
        currentCharge = serializedObject.FindProperty("currentCharge");
        chargeGainedRate = serializedObject.FindProperty("chargeGainedRate");
        _arrowType = serializedObject.FindProperty("_arrowType");

        mineLevel = serializedObject.FindProperty("mineLevel");

        Max_Held_Charge = serializedObject.FindProperty("Max_Held_Charge");
        currentHeldCharge = serializedObject.FindProperty("currentHeldCharge");
        chargeLostRate = serializedObject.FindProperty("chargeLostRate");
    }

    public override void OnInspectorGUI()
    {
        Weapon _Weapon = (Weapon)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(weaponType);

        EditorGUILayout.Space(5);

        //WeaponVisualInfoGroup = EditorGUILayout.BeginFoldoutHeaderGroup(WeaponVisualInfoGroup, "Weapon Visual Info");
        //EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.LabelField("Weapon Visual Info", EditorStyles.boldLabel);
        //if (WeaponVisualInfoGroup)
        //{
        EditorGUILayout.PropertyField(weaponName);
        EditorGUILayout.PropertyField(description);
        EditorGUILayout.PropertyField(inventorySprite);
        EditorGUILayout.PropertyField(weaponModel);
        //}

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField("Weapon Stats", EditorStyles.boldLabel);
        if (WeaponBaseStatsInfoGroup)
        {
            if (_Weapon.weaponType != Weapon.WeaponType.None)
            {
                EditorGUILayout.PropertyField(damageValue);
                EditorGUILayout.PropertyField(attackSpeed);
                EditorGUILayout.PropertyField(attackingTime);
                EditorGUILayout.PropertyField(attackingCooldown);

                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("Weapon Unique Stats", EditorStyles.boldLabel);

                switch (_Weapon.weaponType)
                {
                    case Weapon.WeaponType.None:
                        break;
                    case Weapon.WeaponType.Axe:
                        EditorGUILayout.PropertyField(cutLevel);
                        break;
                    case Weapon.WeaponType.Bow:
                        EditorGUILayout.PropertyField(Max_Charge);
                        EditorGUILayout.PropertyField(currentCharge);
                        EditorGUILayout.PropertyField(chargeGainedRate);
                        EditorGUILayout.PropertyField(_arrowType);
                        break;
                    case Weapon.WeaponType.Pickaxe:
                        EditorGUILayout.PropertyField(mineLevel);
                        break;
                    case Weapon.WeaponType.Tome:
                        EditorGUILayout.PropertyField(Max_Held_Charge);
                        EditorGUILayout.PropertyField(currentHeldCharge);
                        EditorGUILayout.PropertyField(chargeLostRate);
                        break;
                }
            }
            

        }



        serializedObject.ApplyModifiedProperties();
    }
}
