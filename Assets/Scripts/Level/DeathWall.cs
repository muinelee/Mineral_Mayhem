using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWall : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other) 
    {
        if (other.transform.tag == "NPC") other.GetComponent<NPC_Core>().TakeDamage(100, 0, transform.position, STATUS_EFFECT.NONE, 0,0);
        else if (other.transform.tag == "Participant") other.GetComponent<Player_Core>().TakeDamage(100, 0, transform.position, STATUS_EFFECT.NONE, 0,0);
    }
}
