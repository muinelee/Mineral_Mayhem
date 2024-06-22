using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[CreateAssetMenu(fileName = "New Network Attack", menuName = "New Network Attack")]
public class SO_NetworkAttack : ScriptableObject
{
    [Header("Attack Prefab")]
    [SerializeField] protected NetworkObject attack;

    [Header("Voice Line")]
    [SerializeField] protected AudioClip[] voiceLine;

    [Header("Name and Description")]
    public string attackName;
    public string attackDescription;

    [Header("Attack Icon")]
    [SerializeField] protected Sprite attackIcon;
    [SerializeField] protected Sprite attackBackground;
    [SerializeField] protected Sprite attackBackgroundGrey;

    [Header("Ability Slow Percentage on activation")]
    [SerializeField, Range(0,1)] protected float abilitySlow;
    [SerializeField, Range(0,10)] protected float turnSlow;

    [Header("Cooldown Properties")]
    [SerializeField] private float coolDown;

    [Header("Allow Dash Cancel Property")]
    [SerializeField] private bool allowDashCancel = false;

    public NetworkObject GetAttackPrefab()
    {
        return attack;
    }
    public Sprite GetAttackIcon()
    {
        return attackIcon;
    }
    public Sprite GetAttackBackground()
    {
        return attackBackground;
    }
    public Sprite GetAttackBackgroundGrey()
    {
        return attackBackgroundGrey;
    }

    public float GetTurnSlow()
    {
        return turnSlow * 0.4f; //Slow factor of 0.4f for game feel
    }

    public float GetAbilitySlow()
    {
        return abilitySlow;
    }

    public float GetCoolDown()
    {
        return coolDown;
    }

    public AudioClip[] GetVoiceLine()
    {
        return voiceLine;
    }

    public bool GetAllowDashCancel()
    {
        return allowDashCancel;
    }
}