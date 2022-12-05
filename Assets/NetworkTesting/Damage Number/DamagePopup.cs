using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    // Should be called when an enemy gets hit -> should recieve position of where to put it and amount of damage to display
    public static DamagePopup Create(Vector3 position, int damageValue) {
        Transform damagePopupTransform = Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity); // Will Create the popup object (just the floating text)
        
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageValue);

        return damagePopup;
    }


    private TextMeshPro TextMesh;
    private float DisapearTimer;
    private Color TextColor;
    
    // Grabs Text box component so we can change text to damge value
    private void Awake() {
        TextMesh = transform.GetComponent<TextMeshPro>();
    }

    // Sets the value of the text in the damage popup prefab
    public void Setup(int DamageValue){
        TextMesh.SetText(DamageValue.ToString());
        TextColor = TextMesh.color;
        DisapearTimer = 1f;
    }

    private void Update() {
        float moveYSpeed = 1f;
        transform.position += new Vector3(0f, moveYSpeed, 0f) * Time.deltaTime;
        
        DisapearTimer -= Time.deltaTime;
        if (DisapearTimer < 0) {
            float disapearSpeed = 3f;
            TextColor.a -= disapearSpeed * Time.deltaTime;
            TextMesh.color = TextColor;
            if (TextColor.a <= 0) Destroy(gameObject);
        }
    }

}
