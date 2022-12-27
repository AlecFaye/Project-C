using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjectIndicator : MonoBehaviour
{
    [SerializeField] private float autoDestroyTime = 1.5f;

    void Start()
    {
        Destroy(gameObject, autoDestroyTime);
    }
}
