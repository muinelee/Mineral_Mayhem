using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "New Attack Option")]
public class Attack_Attribute : ScriptableObject
{
    [Header("Attack Base Properties")]
    public string nameOfAttack;
    public string description;

    public float coolDown;
    public float cost;
    public float range;
    public float damage;

    public Vector3 offset;
    public bool canOffset;
    public float maxOffset;

    /* working on status effects on pause
    [Header("Status Effect")]
    [SerializeField] private STATUS_EFFECT statusEffect;
    [SerializeField] private float statusEffect_Value;
    [SerializeField] private float statusEffect_Duration;
    */

    [SerializeField] private GameObject attackPrefab;

    public void Activate(Transform attackPoint, Quaternion rotation)
    {
        Debug.Log("This attack is activated");
        if (attackPrefab) Instantiate(attackPrefab, attackPoint.position + offset, rotation);
    }
}