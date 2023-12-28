using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Netwokr Ult", menuName = "New Network Ult")]
public class SO_NetworkUlt : ScriptableObject
{
    [Header("Attack Prefab")]
    [SerializeField] private NetworkObject attack;

    [Header("Name and Description")]
    public string attackName;
    public string attackDescription;

    [Header("Attack Icon")]
    [SerializeField] private Sprite attackIcon;

    public NetworkObject GetAttackPrefab()
    {
        return attack;
    }

    public Sprite GetAttackIcon()
    {
        return attackIcon;
    }
}
