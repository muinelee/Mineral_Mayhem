using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    GameObject weapon;
    Collider weaponCollider;



    
    // Start is called before the first frame update
    void Start()
    {
        weapon = gameObject;
        weaponCollider = weapon.GetComponent<Collider>();
    }

    void WeaponCollision()
    { 
        if (weaponCollider.enabled == true)
        {
            weaponCollider.enabled = false;
        }
        else
        {
            weaponCollider.enabled = true;
        }
    }
}
