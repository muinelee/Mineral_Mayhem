using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Network Dash", menuName = "New Network Dash")]
public class SO_NetworkDash : ScriptableObject
{
    [Header("Name and Description")]
    public string dashName;
    public string dashDescription;

    [Header("Dash properties")]
    [SerializeField] private bool canSteer;
    [SerializeField] private float dashValue;
    [SerializeField] private float dashDuration;
    [SerializeField] private float coolDown;

    [Header("Dash Icon")]
    [SerializeField] private Sprite dashIcon;
    [SerializeField] private Sprite dashBackground;
    [SerializeField] private Sprite dashBackgroundGrey;

    [Header("Dash Sound")]
    [SerializeField] private AudioClip[] dashSounds;

    public bool GetCanSteer()
    {
        return canSteer;
    }

    public float GetDashValue()
    {
        return dashValue;
    }

    public float GetDashDuration()
    {
        return dashDuration;
    }

    public float GetCoolDown()
    {
        return coolDown;
    }

    public Sprite GetDashIcon()
    {
        return dashIcon;
    }
    public Sprite GetDashBackground()
    {
        return dashBackground;
    }
    public Sprite GetDashBackgroundGrey()
    {
        return dashBackgroundGrey;
    }

    public AudioClip[] GetDashSounds()
    {
        return dashSounds;
    }
}
