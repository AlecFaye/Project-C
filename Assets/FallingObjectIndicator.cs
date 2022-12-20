using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjectIndicator : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 2.5f);
    }
}
