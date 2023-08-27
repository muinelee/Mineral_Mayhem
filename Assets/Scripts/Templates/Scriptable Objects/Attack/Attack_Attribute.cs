using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "New Attack Option")]
public class Attack_Attribute : ScriptableObject
{
    [Header("Attack Base Properties")]
    public float coolDown;
    public string nameOfAttack;
    public string description;

    /* status effect on pause
    [Header("Status Effect")]
    [SerializeField] private STATUS_EFFECT statusEffect;
    [SerializeField] private float statusEffect_Value;
    [SerializeField] private float statusEffect_Duration;
    */

    [SerializeField] private GameObject attackPrefab;

    public void Activate(Transform attackPoint, Quaternion rotation)
    {
        if (attackPrefab) Instantiate(attackPrefab, attackPoint.position, rotation);
    }
}