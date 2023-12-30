using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "New Network Attack", menuName = "New Network Attack")]
public class SO_NetworkAttack : ScriptableObject
{
    [Header("Attack Prefab")]
    [SerializeField] private NetworkObject attack;

    [Header("Name and Description")]
    public string attackName;
    public string attackDescription;

    [Header("Cooldown Properties")]
    [SerializeField] private float coolDown;

    [Header("Attack Icon")]
    [SerializeField] private Sprite attackIcon;

    public NetworkObject GetAttackPrefab()
    {
        return attack;
    }

    public float GetCoolDown()
    {
        return coolDown;
    }

    public Sprite GetAttackIcon()
    {
        return attackIcon;
    }
}