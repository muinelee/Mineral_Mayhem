using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer_Health : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnHPChanged))]
    public float HP { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    private bool isInitialized = false;

    [SerializeField] private float startingHP = 100;

    // Start is called before the first frame update
    public override void Spawned()
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

        HP -= (float) damageAmount;

        Debug.Log($"{Time.time} {transform.name} took damage and has {HP} HP left");

        if (HP <= 0)
        {
            Debug.Log($"{Time.time} {transform.name} is dead");

            isDead = true;
        }
    }

    static void OnHPChanged(Changed<NetworkPlayer_Health> changed)
    {
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.HP}");
    }

    static void OnStateChanged(Changed<NetworkPlayer_Health> changed)
    {
        Debug.Log($"{Time.time} OnStateChanged isDead {changed.Behaviour.isDead}");
    }

    public float GetStartingHP()
    {
        return startingHP;
    }
}