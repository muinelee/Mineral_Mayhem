using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DummyBehaviour : NetworkBehaviour, IHealthComponent 
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

    public override void Spawned() 
    {
        anim = GetComponent<Animator>();
        
        if (!Runner.IsServer)
            return; 
        HP = maxHealth;
        team = NetworkPlayer.Team.Neutral;
        isDead = false;
    }

    public void OnTakeDamage(float damageAmount, bool isReact)   
    {
        if (isDead || !Object.HasStateAuthority) return;

        HP -= damageAmount;

        RPC_CheckHealth(HP);

        isTakingDamage = true;
        if (regenerateHealthCoroutine != null)
        {
            StopCoroutine(regenerateHealthCoroutine);
        }
        regenerateHealthCoroutine = StartCoroutine(RegenerateHealthAfterDelay(3f));
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_CheckHealth(float health)
    {
        this.CheckHealth(health);
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        anim.Play("DummyDie");
    }

    private IEnumerator RegenerateHealthAfterDelay(float timeBeforeRegen)
    {
        yield return new WaitForSeconds(timeBeforeRegen);
        isTakingDamage = false;
        while (!isTakingDamage && HP < maxHealth)
        {
            if (HP >= 99) isDead = false; 
            HP += maxHealth * 0.01f; // Regenerate 1% of max health per frame
            CheckHealth(HP);
            yield return null;
        }
    }

    private void SpinAnimation()
    {
        anim.Play("DummySpin");
    }

    private void CheckHealth(float health)
    {
        float healthPercentage = health / maxHealth;
        healthBar.fillAmount = healthPercentage;

        if (health <= 0f)
        {
            Die();
        }

        else if (health > 0) SpinAnimation();
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
        HP = 0; 
        isDead = true;
        healthBar.fillAmount = 0.0f;
        anim.Play("DummyRecover");

        // Start health regeneration
        if (regenerateHealthCoroutine != null)
        {
            StopCoroutine(regenerateHealthCoroutine);
        }
        regenerateHealthCoroutine = StartCoroutine(RegenerateHealthAfterDelay(0.5f));
    }

    // Function for if the object can get knocked back
    public void OnKnockBack(float force, Vector3 source)
    {
        //anim.Play("DummyDie");
    }
}
