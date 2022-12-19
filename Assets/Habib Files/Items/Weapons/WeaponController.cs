using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public ThirdPersonController owner;

    public virtual void AttackStart() {
        Debug.Log("Reeeee No Attack Start Set");
    }
    public virtual void AttackEnd() {
        Debug.Log("No Attack End Set");
    }
}
