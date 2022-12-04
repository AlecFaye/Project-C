using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutBounce : MonoBehaviour
{
    // Update is called once per frame
    void update()
    {
        Bounce();
    }

    private void Bounce()
    {
        Debug.Log("Cripples");
        this.GetComponent<Transform>().position += new Vector3(0.01f, 1f, 0.01f);

    }
}
