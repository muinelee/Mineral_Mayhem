using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkHealthHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnHPChanged))]
    public int HP { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    bool isInitialized = false;

    const int startingHP = 100;

    // Start is called before the first frame update
    void Start()
    {
        HP = startingHP;
        isDead = false;
    }

    // Function only called on the server
    public void OnTakeDamage(int damageAmount)
    {
        if (isDead)
        {
            return;
        }

        HP -= damageAmount;

        Debug.Log($"{Time.time} {transform.name} took damage and has {HP} HP left");

        if (HP <= 0)
        {
            Debug.Log($"{Time.time} {transform.name} is dead");

            isDead = true;
        }
    }

    static void OnHPChanged(Changed<NetworkHealthHandler> changed)
    {
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.HP}");
    }

    static void OnStateChanged(Changed<NetworkHealthHandler> changed)
    {
        Debug.Log($"{Time.time} OnStateChanged isDead {changed.Behaviour.isDead}");
    }
}