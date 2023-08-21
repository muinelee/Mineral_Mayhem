using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_ : MonoBehaviour
{
    [Header("Attack Base Properties")]
    [SerializeField] private float damage;
    [SerializeField] private float coolDowwn;
    [SerializeField] private float knockback;

    [Header("Status Effect")]
    [SerializeField] private STATUS_EFFECT statusEffect;
    [SerializeField] private float statusEffect_Value;
    [SerializeField] private float statusEffect_Duration;
}