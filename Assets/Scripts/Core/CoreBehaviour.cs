using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;
using Fusion;

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

    public float HP { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool isDead { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public NetworkPlayer.Team team { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private void Start()
    {
        HP = maxHealth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            TakeDamage(10);
        }
    }


    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        CheckHealth();
    }

    private void CheckHealth()
    {
        float healthPercentage = (float)HP / maxHealth;

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
        Destroy(gameObject);
    }

    private void SpawnCollectible()
    {
        if (collectiblePrefabs != null && collectiblePrefabs.Length > 0 && spawnPoint != null)
        {
            GameObject randomCollectible = collectiblePrefabs[random.Next(0, collectiblePrefabs.Length)];
            Instantiate(randomCollectible, spawnPoint.position, Quaternion.identity);
        }
    }
}
