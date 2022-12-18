using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

[CreateAssetMenu(fileName = "Arrow", menuName = "Create Item/New Arrow")]
public class Arrow : ScriptableObject
{
    public string arrowName;
    public string description;
    public Sprite inventorySprite;
    public Transform arrowModel;

    public float damageValue;

    public float travelSpeed;

}
