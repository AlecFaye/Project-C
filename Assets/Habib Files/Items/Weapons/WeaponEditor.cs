#if UNITY_EDITOR // This stops the game from building this
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;


[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{
    #region SerializedProperties
    SerializedProperty weaponName;
    SerializedProperty description;
    SerializedProperty inventorySprite;
    SerializedProperty weaponModel;

    SerializedProperty weaponType;

    SerializedProperty attackingTime;

    SerializedProperty attackingCooldown;

    SerializedProperty damageValue;

    SerializedProperty attackSpeed;

    #region Weapon Specific Variables
    SerializedProperty cutLevel;
    
    SerializedProperty _arrowType;

    SerializedProperty mineLevel;

    SerializedProperty chargeDrainedRate;

    #endregion

    #region Charge Variables
    
    SerializedProperty maxCharge;
    SerializedProperty startingCharge;
    SerializedProperty chargeGainedRate;

    #endregion

    bool WeaponVisualInfoGroup, WeaponBaseStatsInfoGroup = true;
    #endregion

    #region Ignore this unless needed
    private void OnEnable()
    {
        weaponName = serializedObject.FindProperty("weaponName");
        description = serializedObject.FindProperty("description");
        inventorySprite = serializedObject.FindProperty("inventorySprite");
        weaponModel = serializedObject.FindProperty("weaponModel");

        weaponType = serializedObject.FindProperty("weaponType");

        attackingTime = serializedObject.FindProperty("attackingTime");

        attackingCooldown = serializedObject.FindProperty("attackingCooldown");

        damageValue = serializedObject.FindProperty("damageValue");
        attackSpeed = serializedObject.FindProperty("attackSpeed");

        cutLevel = serializedObject.FindProperty("cutLevel");


        _arrowType = serializedObject.FindProperty("_arrowType");
        mineLevel = serializedObject.FindProperty("mineLevel");
        chargeDrainedRate = serializedObject.FindProperty("chargeDrainedRate");

        maxCharge = serializedObject.FindProperty("maxCharge");
        startingCharge = serializedObject.FindProperty("startingCharge");
        chargeGainedRate = serializedObject.FindProperty("chargeGainedRate");
    }
    #endregion

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
                        EditorGUILayout.PropertyField(maxCharge);
                        EditorGUILayout.PropertyField(startingCharge);
                        EditorGUILayout.PropertyField(chargeGainedRate);
                        EditorGUILayout.PropertyField(_arrowType);
                        break;
                    case Weapon.WeaponType.Pickaxe:
                        EditorGUILayout.PropertyField(mineLevel);
                        break;
                    case Weapon.WeaponType.Tome:
                        EditorGUILayout.PropertyField(maxCharge);
                        EditorGUILayout.PropertyField(startingCharge);
                        EditorGUILayout.PropertyField(chargeGainedRate);
                        EditorGUILayout.PropertyField(chargeDrainedRate);
                        break;
                }
            }
            

        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif