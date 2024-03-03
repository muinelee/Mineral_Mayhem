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
    private Animator anim;
    private Rigidbody rb;

    [SerializeField] private float startingHP = 100;

    //Base dmg reduction multiplier 1 = normal damage
    [SerializeField] public float dmgReduction = 1.0f;

    // Start is called before the first frame update
    public override void Spawned()
    {
        if (HP == startingHP) isDead = false;
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    public override void OnHit(float x)
    {
        base.OnHit(x);
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

        Debug.Log($"{Time.time} {transform.name} took damage and has {HP} HP left");

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
        GetComponent<NetworkPlayer_InputController>().enabled = false;
        // Disable movement
        GetComponent<NetworkPlayer_Movement>().enabled = false;
        // Disable attack
        GetComponent<NetworkPlayer_Attack>().enabled = false;
        // Disable sphere collider
        GetComponent<SphereCollider>().enabled = false;
        // Disable gravity
        rb.useGravity = false;

        anim.CrossFade("Death", 0.2f);
    }

    static void OnHPChanged(Changed<NetworkPlayer_Health> changed)
    {
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.HP}");
    }

    static void OnStateChanged(Changed<NetworkPlayer_Health> changed)
    {
        Debug.Log($"{Time.time} OnStateChanged isDead {changed.Behaviour.isDead}");

        if (changed.Behaviour.isDead) changed.Behaviour.HandleDeath();
    }

    public float GetStartingHP()
    {
        return startingHP;
    }

    public void Heal(float amount)
    {
        HP = Mathf.Min(HP + amount, startingHP);
    }
}