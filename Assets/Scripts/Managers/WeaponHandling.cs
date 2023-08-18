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
    public Collider weaponCollider;

    private PlayerStats playerStats;
    private NPCStats npcStats;

    // Start is called before the first frame update
    void Start()
    {
        weaponGrip.transform.parent = rightHandGrip.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Weapon hit " + other.name);
        if (other.CompareTag("Participant"))
        {
            Debug.Log("Weapon hit " + other.name);
            NPCController npc = other.GetComponent<NPCController>();
            PlayerController player = other.GetComponent<PlayerController>();

            if (npc != null)
            {
                npcStats = GetComponent<NPCStats>();
                npc.attackerPosition = transform.position;
                npc.attackerHitStrength = npcStats.hitStrength;
            }
            else if (player != null)
            {
                playerStats = GetComponent<PlayerStats>();
                //player.attackerPosition = transform.position;
                //player.attackerHitStrength = playerStats.hitStrength;
            }
        }
    }

    private void WeaponCollisionToggle()
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
