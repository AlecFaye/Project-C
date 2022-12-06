using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    // Should be called when an enemy gets hit -> should recieve position of where to put it and amount of damage to display
    public static DamagePopup Create(Vector3 position, int damageValue) {
        Vector3 updatedPosition = position + new Vector3(Random.Range(-1f,1f), 0f, 0f); // Adds a bit of variance to the spawn location of the Damage Popup
        Transform damagePopupTransform = Instantiate(GameAssets.i.pfDamagePopup, updatedPosition, Quaternion.identity); // Will Create the popup object (just the floating text)
        
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>(); // Gets the component of the Damage Popup to call the Setup Function (line 33 or around there)
        damagePopup.Setup(damageValue); // Runs the Setup function with the damage value parameter

        return damagePopup; // IDK -> I think this checks if it created the damage popup object and if not it breaks
    }


    private const float DISAPEAR_TIMER_MAX = 1f;
    
    private TextMeshPro TextMesh;
    private float DisapearTimer;
    private Color TextColor;
    private Vector3 MoveVector;

    // Grabs Text box component so we can change text to damge value
    private void Awake() {
        TextMesh = transform.GetComponent<TextMeshPro>();
    }

    // Sets the value of the text in the damage popup prefab
    public void Setup(int DamageValue){
        TextMesh.SetText(DamageValue.ToString());
        TextColor = TextMesh.color;
        DisapearTimer = DISAPEAR_TIMER_MAX;

        MoveVector = new Vector3(0.7f, 1f) * 2f;
    }

    private void Update() {
        transform.position += MoveVector * Time.deltaTime;
        MoveVector -= MoveVector * 8f * Time.deltaTime;
        
        if (DisapearTimer > DISAPEAR_TIMER_MAX * 0.5f) {
            // First Half of damage popup lifetime
            float increaseScaleAmount = 0.01f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        } else {
            // Second Half of damage popup lifetime
            float decreaseScaleAmount = 0.01f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        DisapearTimer -= Time.deltaTime;
        if (DisapearTimer < 0) {
            float disapearSpeed = 3f;
            TextColor.a -= disapearSpeed * Time.deltaTime;
            TextMesh.color = TextColor;
            if (TextColor.a <= 0) Destroy(gameObject);
        }
    }

}
