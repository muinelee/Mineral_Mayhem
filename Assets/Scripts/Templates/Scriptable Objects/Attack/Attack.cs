using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Attack : NetworkBehaviour
{
    [SerializeField] private AudioClip attackSFX;

    public float attackDamage;
    public float attackKnockback;
    public Transform playerTransform;
    public STATUS_EFFECT attackStatusEffect;
    public float attackStatusEffectDuration;
    public float attackStatusEffectValue;

    public float holdDuration = 0;
    [SerializeField] private float attackLifetime;

    protected virtual void OnEnable()
    {
        AudioManager.Instance.PlayAudioSFX(attackSFX);
        Invoke("AttackComplete", attackLifetime);        
    }

    protected virtual void DealDamage(GameObject other)
    {
        if (other.CompareTag("NPC")) other.GetComponent<NPC_Core>().TakeDamage(attackDamage, attackKnockback, playerTransform.position, STATUS_EFFECT.NONE, 0, 0);
        else if(other.CompareTag("Player")) other.GetComponent<Player_Core>().TakeDamage(attackDamage, attackKnockback, playerTransform.position, STATUS_EFFECT.NONE, 0, 0);
    }

    [ServerRpc]
    protected void AttackCompleteServerRpc()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }
}
