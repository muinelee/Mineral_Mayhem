using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.Entities;

public class NetworkPlayer_Health : CharacterComponent
{
    [Networked(OnChanged = nameof(OnHPChanged))]
    public float HP { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    private bool isInitialized = false;
    private Animator anim;
    private Rigidbody rb;

    [SerializeField] private float startingHP = 100;

    //Base dmg reduction multiplier 1 = normal damage
    [SerializeField] public float dmgReduction = 1.0f;

    public NetworkPlayer.Team team;

    // Start is called before the first frame update
    public override void Spawned()
    {
        if (HP == startingHP) isDead = false;
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        RoundManager.Instance.ResetRound += Respawn;
    }

    public override void OnHit(float x)
    {
        OnTakeDamage((int)x);
    }

    // Function only called on the server
    public void OnTakeDamage(int damageAmount)
    {
        if (isDead)
        {
            return;
        }

        //Applies any damage reduction effects to the damage taken. currDamageAmount created to help with screenshake when being hit instead of adding the equation there
        int currDamageAmount = (int) (damageAmount * dmgReduction);

        HP -= (float) currDamageAmount;

        //Debug.Log($"{Time.time} {transform.name} took damage and has {HP} HP left");

        if (HP <= 0)
        {
            Debug.Log($"{Time.time} {transform.name} is dead");

            isDead = true;
        }
        else NetworkCameraEffectsManager.instance.CameraHitEffect(currDamageAmount);
    }

    private void HandleDeath()
    {
        // Disable input
        //GetComponent<NetworkPlayer_InputController>().enabled = false;
        Character.Controller.enabled = false;
        // Disable movement
        //GetComponent<NetworkPlayer_Movement>().enabled = false;
        Character.Movement.enabled = false;
        // Disable attack
        //GetComponent<NetworkPlayer_Attack>().enabled = false;
        Character.Attack.enabled = false;
        // Disable sphere collider
        //GetComponent<SphereCollider>().enabled = false;
        Character.Collider.enabled = false;
        // Disable gravity
        rb.useGravity = false;

        anim.CrossFade("Death", 0.2f);

        if (!NetworkPlayer.Local.HasStateAuthority) return;
        if (team == NetworkPlayer.Team.Red) RoundManager.Instance.RedPlayersDies();
        else RoundManager.Instance.BluePlayersDies(); 
    }

    static void OnHPChanged(Changed<NetworkPlayer_Health> changed)
    {
        //Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.HP}");
    }

    static void OnStateChanged(Changed<NetworkPlayer_Health> changed)
    {
        Debug.Log($"{Time.time} OnStateChanged isDead {changed.Behaviour.isDead}");

        if (changed.Behaviour.isDead) changed.Behaviour.HandleDeath();
        else changed.Behaviour.HandleRespawn();
    }

    public float GetStartingHP()
    {
        return startingHP;
    }

    public void Heal(float amount)
    {
        HP = Mathf.Min(HP + amount, startingHP);
    }

    public void HandleRespawn()
    {
        // Disable input
        GetComponent<NetworkPlayer_InputController>().enabled = true;
        // Disable movement
        GetComponent<NetworkPlayer_Movement>().enabled = true;
        // Disable attack
        GetComponent<NetworkPlayer_Attack>().enabled = true;
        // Disable sphere collider
        GetComponent<SphereCollider>().enabled = true;
        // Disable gravity
        rb.useGravity = true;

        anim.CrossFade("Run", 0.2f);
    }

    public void Respawn()
    {
        isDead = false;
        HP = startingHP;
    }
}