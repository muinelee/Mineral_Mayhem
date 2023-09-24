using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPC_Core : MonoBehaviour
{
    [SerializeField] public float currentHP;
    [SerializeField] public float heroMaxHP;

    //to watch for death counter
    public event Action OnDeath;

    private Rigidbody rb;
    private Animator anim;
    private NPC_AINavigation npcAINav;
    [SerializeField] private CapsuleCollider cc;
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private AudioClip hitSFX;

    // Start is called before the first frame update
    void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!cc) cc = GetComponent<CapsuleCollider>();
        if (!anim) anim = GetComponentInChildren<Animator>();
        if (!npcAINav) npcAINav = GetComponent<NPC_AINavigation>();
        //npcStats.currentHP = npcStats.maxHP;
    }

    public void Awake()
    {
        currentHP = heroMaxHP;
    }

    public void TakeDamage(float damage, float knockback, Vector3 attackSource, STATUS_EFFECT statuseffect, float statusEffect_Value, float statusEffect_Duration)
    {
        AudioManager.Instance.PlayAudioSFX(hitSFX);
        currentHP -= damage;
        anim.CrossFade("DummyPushed", 0.3f);

        // Trigger floating text here.
        if (floatingTextPrefab)
        {
            ShowFloatingText(damage);
        }

        if (currentHP <= 0)
        {
            Death();            
        }

        if (CombatEffectManager.instance && damage > 12) CombatEffectManager.instance.HitStop();
        Vector3 direction = (transform.position - attackSource).normalized;
        Knockback(knockback, direction);
        if (knockback != 0) npcAINav.ApplyHitStun();
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

    public void ResetAnimation()
    {
        anim.CrossFade("DummyIdle", 0.3f);
    }

    public void Death()
    {
        if (CombatEffectManager.instance) CombatEffectManager.instance.TimeSlow();
        OnDeath?.Invoke();
        Debug.Log("Death has been invoked");
        Debug.Log("NPC has Dieded");
        cc.enabled = false;
        rb.isKinematic = true;
        anim.CrossFade("DummyDied", 0.3f);
        npcAINav.DeathActivated();
        Destroy(gameObject, 3f);
    }
}
