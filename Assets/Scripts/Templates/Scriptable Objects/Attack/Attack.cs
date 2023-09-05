using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    public float attackDamage;
    public float attackKnockback;
    public Transform playerTransform;
    public STATUS_EFFECT attackStatusEffect;
    public float attackStatusEffectDuration;
    public float attackStatusEffectValue;

    public float holdDuration = 0;
}
