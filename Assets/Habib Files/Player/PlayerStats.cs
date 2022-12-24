using QFSW.QC;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class PlayerStats : MonoBehaviour
{
    #region Variables

    [SerializeField] ThirdPersonController player;

    // Bars
    private HealthBar healthBar;

    // Min/Max Stats
    private float maxHealth = 100;
    private float minHealth = 0;

    // Current Stats
    private float currentHealth;

    #endregion

    #region Start/Update Functions

    private void Start() {
        GrabHealthBar();
        SetBaseStats();
    }


    // Maybe Convert Stats into a dictionary
    private void SetBaseStats() {
        SetMaxHealth(maxHealth, true);
    }

    #endregion

    #region Health Bar Functions
 
    private void GrabHealthBar() {
        if (player.IsOwner && !healthBar)
            healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<HealthBar>();
    }

    private void SetMaxHealth(float health, bool fullHeal = false) {
        healthBar.SetMaxValue(health);
        if (fullHeal)
            UpdateCurrentHealth(health);
    }
    private void UpdateCurrentHealth(float health) {
        currentHealth = health;
        
        if (currentHealth <= minHealth) {
            currentHealth = minHealth;
            Debug.Log("RIP -  Player's Health reached " + currentHealth + " and should be Dead!");
        }
        if (currentHealth >= maxHealth) {
            currentHealth = maxHealth;
            Debug.Log("WHOA -  Player's Health reached " + currentHealth + " and is at Full Health!");
        }
        
        healthBar.SetCurrentValue(currentHealth);
    }

    #endregion

    #region Quantum Commands

    [Command]
    private void DamagePlayer(float damageValue) {
        if (damageValue > 0)
            Debug.Log("Ooof You Took " + damageValue + " Damage!");
        else
            Debug.Log("Ay You Gained " + damageValue + " Health!");

        UpdateCurrentHealth(currentHealth - damageValue);
    }

    #endregion
}
