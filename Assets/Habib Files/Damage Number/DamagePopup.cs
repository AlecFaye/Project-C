using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    // Should be called when an enemy gets hit -> should recieve position of where to put it and amount of damage to display
    public static DamagePopup Create(Vector3 position, float damageValue) {
        Vector3 updatedPosition = position + new Vector3(Random.Range(-1f,1f), 0f, 0f); // Adds a bit of variance to the spawn location of the Damage Popup
        Transform damagePopupTransform = Instantiate(GameAssets.i.pfDamagePopup, updatedPosition, Quaternion.identity); // Will Create the popup object (just the floating text)
        
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>(); // Gets the component of the Damage Popup to call the Setup Function (line 33 or around there)
        damagePopup.Setup(damageValue); // Runs the Setup function with the damage value parameter

        return damagePopup; // IDK -> I think this checks if it created the damage popup object and if not it breaks
    }


    private const float DISAPEAR_TIMER_MAX = 1f; // Max timer for the lifetime of the popup
    
    private TextMeshPro TextMesh;
    private float DisapearTimer; // Timer to be updated
    private Color TextColor;
    private Vector3 MoveVector; // How much the popup should be shifted

    // Grabs Text box component so we can change text to damage value
    private void Awake() {
        TextMesh = transform.GetComponent<TextMeshPro>(); // Gets the component for TextMesh
    }

    // Sets the value of the text in the damage popup prefab
    public void Setup(float DamageValue){
        TextMesh.SetText(DamageValue.ToString()); // Sets the text to the Damage Value
        TextColor = TextMesh.color; // Sets the colour variable to be used later
        DisapearTimer = DISAPEAR_TIMER_MAX;

        MoveVector = new Vector3(0.7f, 1f) * 2f; // Sets how much the thing should move
    }

    private void Update() {
        transform.rotation = Camera.main.transform.rotation; // This Billboards the text to always face the camera

        transform.position += MoveVector * Time.deltaTime; // Moves the popup by the Move Vector
        MoveVector -= MoveVector * 8f * Time.deltaTime; // Updates the Move Vector so that it goes up a bit
        
        if (DisapearTimer > DISAPEAR_TIMER_MAX * 0.5f) {
            // First Half of damage popup lifetime
            float increaseScaleAmount = 0.01f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime; // Increases the size of the text
        } else {
            // Second Half of damage popup lifetime
            float decreaseScaleAmount = 0.01f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime; // Decreases the size of the text
        }

        DisapearTimer -= Time.deltaTime; // Tick timer down
        if (DisapearTimer < 0) { // If timer is less than 0
            float disapearSpeed = 3f;
            TextColor.a -= disapearSpeed * Time.deltaTime; // Set visibility to 0 over the time of disapearSpeed
            TextMesh.color = TextColor; 
            if (TextColor.a <= 0) Destroy(gameObject); // If the visiability is less than 0 destroy the object
        }
    }

}
