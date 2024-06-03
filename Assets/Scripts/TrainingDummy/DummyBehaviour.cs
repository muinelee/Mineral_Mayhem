using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DummyBehaviour : MonoBehaviour, IHealthComponent
{
    // Health
    public int maxHealth = 100;
    [SerializeField] private Image healthBar;

    private Animator anim;
    private Coroutine regenerateHealthCoroutine;
    private bool isTakingDamage = false;

    public float HP { get; set; }
    public bool isDead { get; set; }
    public NetworkPlayer.Team team { get; set; }

    private void Start()
    {
        HP = maxHealth;
        team = NetworkPlayer.Team.Neutral;
        anim = GetComponent<Animator>();
        isDead = false;
    }

    public void OnTakeDamage(float damageAmount)
    {
        if (isDead) return;

        Debug.Log("Taking Damage"); 
        HP -= damageAmount;
        CheckHealth(HP);

        isTakingDamage = true;
        if (regenerateHealthCoroutine != null)
        {
            StopCoroutine(regenerateHealthCoroutine);
        }
        regenerateHealthCoroutine = StartCoroutine(RegenerateHealthAfterDelay());

        SpinAnimation();
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        anim.SetTrigger("Die");
    }

    private IEnumerator RegenerateHealthAfterDelay()
    {
        yield return new WaitForSeconds(3.0f);
        isTakingDamage = false;
        while (!isTakingDamage && HP < maxHealth)
        {
            HP += maxHealth * 0.01f; // Regenerate 1% of max health per frame
            CheckHealth(HP);
            yield return null;
        }
    }

    private void SpinAnimation()
    {
        anim.SetTrigger("Spin");
    }

    private void CheckHealth(float health)
    {
        float healthPercentage = health / maxHealth;
        healthBar.fillAmount = healthPercentage;

        if (healthPercentage <= 0f)
        {
            Die();
        }
    }

    // INTERFACE FUNCTIONS

    // Function for when object dies
    public void HandleDeath()
    {
        Die();
    }

    // Function for if object respawns
    public void HandleRespawn()
    {
        HP = maxHealth;
        isDead = false;
        healthBar.fillAmount = 1.0f;
        anim.SetTrigger("Recover");
    }

    // Function for if the object can get knocked back
    public void OnKnockBack(float force, Vector3 source)
    {
        anim.SetTrigger("KnockBack");
    }
}
