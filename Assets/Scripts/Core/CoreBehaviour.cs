using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;
using Fusion;
using Unity.IO.LowLevel.Unsafe;

public class CoreBehaviour : NetworkBehaviour, IHealthComponent
{   
    //Health
    public int maxHealth = 100;

    //Pickup flags
    private bool spawn75flag = false;
    private bool spawn50flag = false;
    private bool spawn25flag = false;

    public GameObject[] collectiblePrefabs;
    public Transform spawnPoint;
    private System.Random random = new System.Random();

    //Health
    [SerializeField] private Image healthBar;

    public float HP { get; set; }
    public bool isDead { get; set; }
    public NetworkPlayer.Team team { get; set; }

    public override void Spawned()
    {
        if (!Runner.IsServer)
            return;

        HP = maxHealth;
        team = NetworkPlayer.Team.Neutral;
    }

    public void OnTakeDamage(int damageAmount)
    {
        if (!Object.HasStateAuthority) return;

        HP -= damageAmount;
        RPC_CheckHealth(HP);
    }

    private void CheckHealth(float health)
    {
        float healthPercentage = health / maxHealth;

        healthBar.fillAmount = healthPercentage;

        if (healthPercentage <= 0f)
        {
            Die();
        }
        else if (healthPercentage <= 0.75f && healthPercentage > 0.50f && !spawn75flag)
        {
            SpawnCollectible();
            spawn75flag = true;
        }
        else if (healthPercentage <= 0.50f && healthPercentage > 0.25f && !spawn50flag)
        {
            SpawnCollectible();
            spawn50flag = true;
        }
        else if (healthPercentage <= 0.25f && !spawn25flag)
        {
            SpawnCollectible();
            spawn25flag = true;
        }
    }

    private void Die()
    {
        Runner.Despawn(Object);
    }

    private void SpawnCollectible()
    {
        if (!Object.HasStateAuthority) return;

        if (collectiblePrefabs != null && collectiblePrefabs.Length > 0 && spawnPoint != null)
        {
            GameObject randomCollectible = collectiblePrefabs[random.Next(0, collectiblePrefabs.Length)];
            NetworkObject spawnedItem = Runner.Spawn(randomCollectible, spawnPoint.position, spawnPoint.rotation);
            spawnedItem.GetComponent<PickupThrow>().Throw();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_CheckHealth(float health)
    {
        this.CheckHealth(health);
    }

    //INTERFACE FUNCTIONS

    // Function for when object dies
    public void HandleDeath() { }

    // Function for if object respawns
    public void HandleRespawn() { }

    // Function for if the object can get knockedback
    public void OnKnockBack(float force, Vector3 source) { }
}
