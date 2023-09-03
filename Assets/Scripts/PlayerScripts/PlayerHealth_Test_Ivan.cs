using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth_Test_Ivan : MonoBehaviour
{
    public FloatVariable currentHealth;
    public FloatVariable maxHealth;

    public FloatVariable lerpTimer;
    public FloatVariable chipSpeed;

    public Image frontHealthBar;
    public Image backHealthBar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth.value = maxHealth.value;
    }

    void Update()
    {
        currentHealth.value = Mathf.Clamp(currentHealth.value, 0, maxHealth.value);
        UpdateHealthUI();

        // Testing Damage
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(1);
        }
    }

    public void UpdateHealthUI()
    {
        Debug.Log(currentHealth.value);
        float fillFront = frontHealthBar.fillAmount;
        float fillBack = backHealthBar.fillAmount;
        float hFraction = currentHealth.value / maxHealth.value;

    }

    // Damaging the player
    public void TakeDamage(float damage)
    {
        currentHealth.value -= damage;
        lerpTimer.value = 0;

        // Debug displaying how much health the player lost
        Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth.value);

        if (currentHealth.value <= 0)
        {
            // Play the Death Animation from the player controller
            Debug.Log("Player is dead!");
        }
    }
}
