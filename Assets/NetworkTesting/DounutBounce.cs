using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DounutBounce : MonoBehaviour
{
    private float DonutPosition;


    void Start()
    {
        Debug.Log("Dumbo");

        //DonutPosition = this.GetComponent<Transform>().LocalPosition.y;
    }


    void Update()
    {
        Bounce();
    }

    private void Bounce()
    {
        if (DonutPosition < 10) DonutPosition += 0.1f;
        if (DonutPosition >= 10) DonutPosition = 8f;
    }
}

