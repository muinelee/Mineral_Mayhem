using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "New Attack Option")]
public class Attack_Attribute : ScriptableObject
{
    [Header("Attack Base Properties")]
    public string nameOfAttack;
    public string description;

    [Range(0,1)] public float attackAbilitySlowPercentage;
    public bool stopTurn;

    public float coolDown;
    public float cost;
    public float range;
    
    #region <----- universal variables to pass to prefab ----->
    
    public float damage;
    public float knockback;
    public Transform player;
    public STATUS_EFFECT statusEffect;
    public float statusEffectDuration;
    public float statusEffectValue;
    
    #endregion

    public bool canCharge;
    public float chargeMinTimer;
    public float chargeHoldDuration;

    public Vector3 offset;
    public bool canOffset;
    public float maxOffset;

    [SerializeField] public GameObject attackPrefab;
    [SerializeField] private AudioClip attackSFX;

    public void Activate(Transform attackPoint, Quaternion rotation)
    {
        if (attackPrefab)
        {
            GameObject instantiatedAttack = Instantiate(attackPrefab, attackPoint.position + offset, rotation);
        }
    }

    public void TakeChargeDuration(float holdDuration)
    {
        chargeHoldDuration = holdDuration;
    }
}