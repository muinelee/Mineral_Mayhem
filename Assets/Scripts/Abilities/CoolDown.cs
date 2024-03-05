using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolDown : MonoBehaviour
{




    public float coolDown = 1;
    public float coolDownTimer; 


    void Update()
    {
        if(coolDownTimer > 0)
        {
            coolDownTimer -= Time.deltaTime; 
        }
        if(coolDownTimer < 0)
        {
            coolDownTimer = 0;
        }
        if(Input.GetButton("Fire2") && coolDownTimer == 0)
        {
            coolDownTimer = coolDown;
        }
    }
} 
