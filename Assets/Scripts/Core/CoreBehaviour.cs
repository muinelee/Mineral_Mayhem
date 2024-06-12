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
    [SerializeField] GameObject explosionVFX;

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

    [SerializeField] protected AudioClip spawnedSFX;
    [SerializeField] protected AudioClip explosionSFX;
    public int OnDeathCollectibleAmount = 3;

    public override void Spawned()
    {
        if (!Runner.IsServer)
            return;

        HP = maxHealth;
        team = NetworkPlayer.Team.Neutral;

        RPC_PlaySpawnedSFX();

        if (RoundManager.Instance != null) RoundManager.Instance.ResetRound += Die;
    }

    public void OnTakeDamage(float damageAmount, bool isReact)
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
            HandleDeath();
            Runner.Spawn(explosionVFX, transform.position, Quaternion.identity);
            AudioManager.Instance.PlayAudioSFX(explosionSFX, transform.position);
            Die();
        }
        else if (healthPercentage <= 0.75f && healthPercentage > 0.50f && !spawn75flag)
        {
            SpawnCollectible(spawnPoint.position, spawnPoint.rotation);
            spawn75flag = true;
        }
        else if (healthPercentage <= 0.50f && healthPercentage > 0.25f && !spawn50flag)
        {
            SpawnCollectible(spawnPoint.position, spawnPoint.rotation);
            spawn50flag = true;
        }
        else if (healthPercentage <= 0.25f && !spawn25flag)
        {
            SpawnCollectible(spawnPoint.position, spawnPoint.rotation);
            spawn25flag = true;
        }
    }

    private void Die()
    {
        if (!Object.HasStateAuthority) return;
        if (RoundManager.Instance != null) RoundManager.Instance.ResetRound -= Die;
        Runner.Despawn(Object);
    }

    private void SpawnCollectible(Vector3 spawnPos, Quaternion spawnRot)
    {
        if (!Object.HasStateAuthority) return;

        if (collectiblePrefabs != null && collectiblePrefabs.Length > 0 && spawnPoint != null)
        {
            GameObject randomCollectible = collectiblePrefabs[random.Next(0, collectiblePrefabs.Length)];
            NetworkObject spawnedItem = Runner.Spawn(randomCollectible, spawnPos, spawnRot);
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
    public void HandleDeath()
    {
        float radius = spawnPoint.GetComponent<CoreCollecibleSpawnMovement>().radius;
        float angleStep = 360f / OnDeathCollectibleAmount;
        for (int i = 0; i < OnDeathCollectibleAmount; i++)
        {
            float angle = (i * angleStep) * Mathf.Deg2Rad;
            float x = transform.position.x + Mathf.Cos(angle) * radius;
            float z = transform.position.z + Mathf.Sin(angle) * radius;
            Vector3 spawnPos = new Vector3(x, spawnPoint.position.y, z);
            Vector3 lookDir = spawnPos - transform.position;
            lookDir.y = 0;
            SpawnCollectible(spawnPos, Quaternion.LookRotation(lookDir));
        }
    }

    // Function for if object respawns
    public void HandleRespawn() { }

    // Function for if the object can get knockedback
    public void OnKnockBack(float force, Vector3 source) { }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlaySpawnedSFX()
    {
        AudioManager.Instance.PlayAudioSFX(spawnedSFX, transform.position);
    }
}
