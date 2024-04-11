using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

public class CoreBehaviour : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public GameObject[] collectiblePrefabs;
    public Transform[] spawnPoints;
    private System.Random random = new System.Random();

    //Health
    [SerializeField] private Image healthBar;



    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            TakeDamage(25);
        }


    }


    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        CheckHealth();
    }

    private void CheckHealth()
    {
        float healthPercentage = (float)currentHealth / maxHealth;

        healthBar.fillAmount = healthPercentage;

        if (healthPercentage <= 0f)
        {
            Die();
        }
        else if (healthPercentage <= 0.75f && healthPercentage > 0.50f)
        {
            SpawnCollectible();
        }
        else if (healthPercentage <= 0.50f && healthPercentage > 0.25f)
        {
            SpawnCollectible();
        }
        else if (healthPercentage <= 0.25f)
        {
            SpawnCollectible();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void SpawnCollectible()
    {
        if (collectiblePrefabs != null && collectiblePrefabs.Length > 0 && spawnPoints != null && spawnPoints.Length > 0)
        {
            GameObject randomCollectible = collectiblePrefabs[random.Next(0, collectiblePrefabs.Length)];
            Transform randomSpawnPoint = spawnPoints[random.Next(0, spawnPoints.Length)];
            Instantiate(randomCollectible, randomSpawnPoint.position, Quaternion.identity);
        }
    }

}
