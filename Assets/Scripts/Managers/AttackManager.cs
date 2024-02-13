using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[RequireComponent(typeof(NetworkObject))]
public class AttackManager : NetworkBehaviour
{
    public List<Player_InputController> players;

    public static AttackManager instance;

    private void Awake()
    {
        if (!instance) instance = this;
    }

    public void AddPlayer(Player_InputController player)
    {
        players.Add(player);
    }

    public void RemovePlayer(Player_InputController player)
    {
        players.Remove(player);
    }
}