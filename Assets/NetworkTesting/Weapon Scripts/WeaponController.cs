using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Weapon;


//namespace WeaponController{
public class WeaponController : MonoBehaviour
{
    public ThirdPersonController player; // Refrences the player it's attatched too

    [SerializeField] private Weapon[] Hotbar;

    private int selectedWeapon = 0;

    private Weapon currentWeapon;

    // Makes a list to keep track of which enemies were hit (enemies added by the CollisionDetection Script on weapons)
    public List<Collider> enemiesHitList = new List<Collider>();

    // Stats -> Get overwritten
    private bool CanAttack;
    public bool IsAttacking; // Is public to be called by other functions
    private float AttackingTime;
    private float AttackingCooldown;

    // Channel Variables
    private bool IsChannelingAttack = false; // Used to check if player should be charging their attack
    private float currentBowCharge = 0;
    private float currentTomeCharge = 100;
    private float tomeChargedFor = 0;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform beamHitbox;


    private void Start()
    {
        foreach (Weapon weapon in Hotbar)
        {
            if (weapon != null)
            {
                CreateWeapon(weapon);
            }
        }

        SelectWeapon();
    }

    private void Update() {
        if (IsChannelingAttack && currentWeapon.weaponType == WeaponType.Tome) {
           lineRenderer.SetPositions(new Vector3[] { player._projectileSpawn.position, player.mouseWorldPosition});
        }
    }
    private void CreateWeapon(Weapon weapon)
    {
        Transform tempWeapon = Instantiate(weapon.weaponModel, this.transform.position, Quaternion.identity); // Creates the weapon in the hotbar slot
        tempWeapon.transform.SetParent(this.transform); // Sets this gameobject to the parent of the 
        tempWeapon.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f); // Sets rotation to hand
        weapon._player = player;
    }

    private void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in this.transform)
        {
            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
                currentWeapon = Hotbar[selectedWeapon];
                Transform currentWeaponObject = weapon;

                CanAttack = currentWeapon.CanAttack;
                IsAttacking = currentWeapon.IsAttacking;
                AttackingTime = currentWeapon.attackingTime;
                AttackingCooldown = currentWeapon.attackingCooldown;
            }
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }

    #region On Attack Input
    public void OnAttack()
    {
        switch (Hotbar[selectedWeapon].weaponType)
        {
            case WeaponType.None:
                Debug.Log("Wait stop should be NONE");
                break;

            case WeaponType.Axe:
                if (CanAttack && !IsAttacking && player.Grounded && player.IsOwner)
                    StartCoroutine(AxeAttack());
                break;

            case WeaponType.Bow:
                if (!IsChannelingAttack) {
                    IsChannelingAttack = true;
                    currentBowCharge = 0;
                    InvokeRepeating("BowCharge", 0f, (1f / currentWeapon.chargeGainedRate)); // Invokes the func BowCharge(), instantly once, then once every (1 sec/BowChargeRate)
                }
                else {
                    IsChannelingAttack = false;
                    CancelInvoke("BowCharge");
                    if (CanAttack && !IsAttacking && player.Grounded && player.IsOwner)
                        StartCoroutine(BowAttack(currentBowCharge));
                }
                break;

            case WeaponType.Pickaxe:
                if (CanAttack && !IsAttacking && player.Grounded && player.IsOwner)
                    StartCoroutine(PickaxeAttack());
                break;

            case WeaponType.Tome:
                if (!IsChannelingAttack) {
                    lineRenderer.GetComponent<CollisionDetection>().weapon = currentWeapon;
                    
                    IsChannelingAttack = true;
                    tomeChargedFor = 0;
                    
                    CancelInvoke("TomeCharge");
                    InvokeRepeating("TomeDrain", 0, (1f / currentWeapon.chargeLostRate)); // Invokes the func TomeDrain(), instantly once, then once every (1 sec/TomeChargeRate)
                }
                else {
                    IsChannelingAttack = false;
                    lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
                    CancelInvoke("TomeDrain");
                    InvokeRepeating("TomeCharge", 0f, (1f / currentWeapon.chargeLostRate)); // Invokes the func TomeCharge(), instantly once, then once every (1 sec/TomeChargeRate)
                }
                CanAttack = !IsChannelingAttack;
                IsAttacking = IsChannelingAttack;
                break;
        }
    }
    #endregion

    #region Charge Functions
    private void BowCharge()
    {
        currentBowCharge++;
        if (currentBowCharge > currentWeapon.Max_Charge) currentBowCharge = currentWeapon.Max_Charge;
        //Debug.Log("Current bow charge: " + currentBowCharge);
    }

    private void TomeDrain()
    {
        currentTomeCharge--;
        tomeChargedFor++;
        //Debug.Log("Current tome charge: " + currentTomeCharge);
        //Debug.Log("Current tome charged for: " + tomeChargedFor);
        if (tomeChargedFor % 5 == 0)
        {
            tomeChargedFor = 0;
            if (player.Grounded && player.IsOwner)
                StartCoroutine(TomeAttack());
        }
        if (currentTomeCharge < 0)
        {
            currentTomeCharge = 0;
            //Debug.Log("Out of Energy");
        }
    }

    private void TomeCharge()
    {
        currentTomeCharge++;
        if (currentTomeCharge >= currentWeapon.Max_Held_Charge)
        {
            currentTomeCharge = currentWeapon.Max_Held_Charge;
            CancelInvoke("TomeCharge");
        }
        //Debug.Log("Current tome charge: " + currentTomeCharge);
    }
    #endregion

    #region Weapon Attack Functions
    private IEnumerator AxeAttack()
    {
        Debug.Log("Execute the Axe Attack");
        CanAttack = false;
        IsAttacking = true;
        player._animator.SetTrigger("Axe Attack"); // Will call the currently selected weapon's attack animation
        yield return new WaitForSeconds(AttackingTime);
        IsAttacking = false;
        yield return new WaitForSeconds(AttackingCooldown);
        enemiesHitList = new List<Collider>(); // Resets the list of enemies so that they can be hit again
        CanAttack = true;
    }
    private IEnumerator BowAttack(float chargeValue)
    {
        Debug.Log("Execute the Bow Attack");
        CanAttack = false;
        IsAttacking = true;

        Vector3 aimDir = (player.mouseWorldPosition - player._projectileSpawn.position).normalized;
        Transform tempArrow = Instantiate(currentWeapon._arrowType.arrowModel, player._projectileSpawn.position, Quaternion.LookRotation(aimDir, Vector3.up));
        tempArrow.GetComponent<ArrowFunction>().Create(
            currentWeapon._arrowType.travelSpeed * (currentBowCharge / 100), // Speed of arrow (travelSpeed) * by charge value (0% - 100%)
            (currentWeapon._arrowType.damageValue + (currentWeapon.damageValue * (currentBowCharge / 100))) // Damage of arrow (Arrow Damage + [bow damage * charge value {0% - 100%}] = total damage
            );

        //player._animator.SetTrigger("Axe Attack"); // Will call the currently selected weapon's attack animation

        yield return new WaitForSeconds(AttackingTime);
        IsAttacking = false;
        yield return new WaitForSeconds(AttackingCooldown);
        enemiesHitList = new List<Collider>(); // Resets the list of enemies so that they can be hit again
        CanAttack = true;
    }
    private IEnumerator PickaxeAttack()
    {
        Debug.Log("Execute the Pickaxe Attack");
        CanAttack = false;
        IsAttacking = true;
        player._animator.SetTrigger("Axe Attack"); // Will call the currently selected weapon's attack animation
        yield return new WaitForSeconds(AttackingTime);
        IsAttacking = false;
        yield return new WaitForSeconds(AttackingCooldown);
        enemiesHitList = new List<Collider>(); // Resets the list of enemies so that they can be hit again
        CanAttack = true;
    }
    private IEnumerator TomeAttack() {
        Debug.Log("Execute the Tome Attack");
        Vector3 point0 = player._projectileSpawn.position;
        Vector3 point1 = player.mouseWorldPosition;


        //RaycastHit[] raycastHits = Physics.RaycastAll(player._projectileSpawn.position, (player._projectileSpawn.position - player.mouseWorldPosition).normalized, Vector3.Distance(player._projectileSpawn.position, player.mouseWorldPosition));
        Vector3 aimDir = (point1 - point0).normalized;
        Transform tempBeam = Instantiate(beamHitbox, point0, Quaternion.LookRotation(aimDir, Vector3.up));

        tempBeam.position = Vector3.Lerp(point0, point1, 0.5f);
        tempBeam.localScale = new Vector3(1f, 1f, Vector3.Distance(point0, point1) * 5f);

        //tempArrow.GetComponent<BeamFunction>().Create(
        //    currentWeapon._arrowType.travelSpeed * (currentBowCharge / 100), // Speed of arrow (travelSpeed) * by charge value (0% - 100%)
        //    (currentWeapon._arrowType.damageValue + (currentWeapon.damageValue * (currentBowCharge / 100))) // Damage of arrow (Arrow Damage + [bow damage * charge value {0% - 100%}] = total damage
        //    );

        //player._animator.SetTrigger("Axe Attack"); // Will call the currently selected weapon's attack animation
        yield return new WaitForSeconds(AttackingTime);
        IsAttacking = false;
        yield return new WaitForSeconds(AttackingCooldown);
        enemiesHitList = new List<Collider>(); // Resets the list of enemies so that they can be hit again
        CanAttack = true;
    }
    #endregion

    #region Hotbar Inputs

    private void SwitchHotBar(int hotbarNum) {
        if (player.IsOwner && CanAttack && !IsChannelingAttack) {
            selectedWeapon = hotbarNum - 1; // - 1 for arrary starting at 0
            SelectWeapon();
            Debug.Log(Hotbar[selectedWeapon].weaponName);
        }
    }
    private void OnHotbar1() { SwitchHotBar(1); }
    private void OnHotbar2() { SwitchHotBar(2); }
    private void OnHotbar3() { SwitchHotBar(3); }
    private void OnHotbar4() { SwitchHotBar(4); }
    private void OnHotbar5() { SwitchHotBar(5); }
    #endregion
}