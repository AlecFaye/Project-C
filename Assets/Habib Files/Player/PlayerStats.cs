using QFSW.QC;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class PlayerStats : MonoBehaviour, IDamageable
{
    #region Variables

    [SerializeField] ThirdPersonController player;

    #region Weapon Variables
    
    public SliderBar weaponChargeBar;

    #endregion

    #region Health Variables

    [SerializeField] private SliderBar healthBar;

    private float maxHealth = 100;
    private float minHealth = 0;

    private float currentHealth;

    #endregion

    #region Score Variables

    [SerializeField] private TextMeshProUGUI scoreText;

    private int currentScore = 0;

    #endregion

    #endregion

    #region Start/Update Functions

    private void Start() {
        if (!player.IsOwner) Destroy(this.gameObject);
        SetBaseStats();
    }


    // Maybe Convert Stats into a dictionary
    private void SetBaseStats() {
        SetMaxHealth(maxHealth, true);
        UpdateScoreText();
    }

    #endregion

    #region IDamagable Functions

    public void TakeDamage(IDamageable damager, float damageTaken, Weapon.WeaponType damageType = Weapon.WeaponType.None) {
        UpdateCurrentHealth(currentHealth - damageTaken); 
    }
    public Transform GetTransform() { return this.transform; }

    #endregion

    #region Health Functions

    private void SetMaxHealth(float health, bool fullHeal = false) {
        healthBar.SetMaxValue(health);
        if (fullHeal)
            UpdateCurrentHealth(health);
    }
    private void UpdateCurrentHealth(float health) {
        currentHealth = health;
        Debug.Log("Current Health: " + currentHealth);

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

    #region Score Functions

    public void AddScore(int score) {
        currentScore += score;
        UpdateScoreText();
    }
    public void SubtractScore(int score) {
        currentScore -= score;
        UpdateScoreText();
    }
    private void UpdateScoreText() {
        string newText = "Score: " + currentScore;
        scoreText.text = newText;
    }

    #endregion

    #region Quantum Commands

    [Command("Stats.Damage_Player")]
    private void DamagePlayer(float damageValue) {
        if (damageValue > 0)
            Debug.Log("Ooof You Took " + damageValue + " Damage!");
        else
            Debug.Log("Ay You Gained " + damageValue + " Health!");

        UpdateCurrentHealth(currentHealth - damageValue);
    }

    [Command("Stats.Player_Score")]
    private void UpdatePlayerScore(int playerScore) {
        AddScore(playerScore);
    }

    #endregion
}
