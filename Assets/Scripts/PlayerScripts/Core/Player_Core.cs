using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

[RequireComponent(typeof(Player_InputController), typeof(Player_Movement), typeof(Player_AttackController))]

public class Player_Core : MonoBehaviour
{
    [SerializeField] Player_StatsSO playerStats;

    [SerializeField] private float heroMaxHP;
    
    private bool isBlocking = false;
    private bool isPerfectBlock = false;

    private float statusEffectTimer;
    private float statusEffectDuration;

    private Rigidbody rb;
    [SerializeField] private CapsuleCollider cc1;
    [SerializeField] private CapsuleCollider cc2;
    [SerializeField] private GameObject floatingTextPrefab;
    private Player_InputController playerInput;

    // Start is called before the first frame update
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        playerInput = gameObject.GetComponent<Player_InputController>();
        if (!cc1) Debug.Log("Capsule Collider 1 not seen in Player_Core");
        if (!cc2) Debug.Log("Capsule Collider 2 not seen in Player_Core");

        //playerStats.maxHP = heroMaxHP;
       // playerStats.currentHP = heroMaxHP;
    }


    void Update()
    {
        if (statusEffectTimer > 0) statusEffectTimer -= Time.deltaTime;

        // ----- DEBUGGING -----
        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(10, 0, transform.position, STATUS_EFFECT.NONE, 0,0);
        }
    }


    public void TakeDamage(float damage, float knockback, Vector3 attackSource, STATUS_EFFECT statusEffect, float statusEffect_Value, float statusEffect_Duration)
    {
        // ----- DAMAGE STEP -----

        if (isPerfectBlock)
        {
            // Insert perfect block code
            return;
        }

        if (isBlocking) damage *= 0.2f;

        // Trigger floating text here.
        if (floatingTextPrefab)
        {
            ShowFloatingText(damage);
        }
        
        playerStats.currentHP -= damage;
        if (playerStats.currentHP <= 0)
        {
            Death();
        }

        Vector3 direction = (transform.position - attackSource).normalized;
        Knockback(knockback, direction);        
    }

    public void SetIsBlocking(bool b)
    {
        isBlocking = b;                                 // Use animation event to turn blocking on/off
    }

    public void SetIsPerfectBlocking(bool b)
    {
        isPerfectBlock = b;                             // Use animation event to turn perfectblocking on/off
    }

    private void ShowFloatingText(float damage)
    {
        var go = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform);
        go.GetComponent<TextMeshPro>().text = damage.ToString();
    }

    public void Knockback(float knockbackValue, Vector3 direction)
    {
        rb.AddForce(direction * knockbackValue, ForceMode.Impulse);
    }

    public void Death()
    {
        cc1.enabled = false;
        cc2.enabled = false;
        rb.isKinematic = true;
        playerInput.StartDeath();
        playerInput.enabled = false;
        Invoke("PlayerDieded", 5f);
    }

    void PlayerDieded()
    {
        SceneManager.LoadScene("GameOver");
    }
}
