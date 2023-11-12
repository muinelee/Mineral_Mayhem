using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class AttackManager : NetworkBehaviour
{
    public List<Player_InputController> players;

    public static AttackManager instance;

    private void Awake()
    {
        if (!instance) instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPlayer(Player_InputController player)
    {
        Debug.Log($"Added {player} to list");
        players.Add(player);

        foreach (Player_InputController p in players)
        {
            foreach (GameObject a in p.attackController.attacks)
            {
                Debug.Log(a.name);
                GameObject attack = Instantiate(a, p.attackController.attackPoint.position, p.gameObject.transform.rotation);
            }
        }
    }

    public void RemovePlayer(Player_InputController player)
    {
        players.Remove(player);
    }
}