using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandling : MonoBehaviour
{

    public GameObject weapon;
    public GameObject weaponGrip;
    public GameObject weaponGrip2;
    public GameObject rightHandGrip;
    public GameObject leftHandGrip;
    public GameObject leftHand;
    public GameObject rightHand;

    // Start is called before the first frame update
    void Start()
    {
        weaponGrip.transform.parent = rightHandGrip.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
