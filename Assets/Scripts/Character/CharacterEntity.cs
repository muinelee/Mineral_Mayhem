using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEntity : CharacterComponent
{
    public static event Action<CharacterEntity> OnCharacterSpawned;
    public static event Action<CharacterEntity> OnCharacterDespawned;

    public event Action<float> OnHitEvent;
    public event Action<float> OnHealEvent;
    public event Action<StatusEffect> OnStatusBeginEvent;
    public event Action<StatusEffect> OnStatusEndedEvent;
    public event Action OnPickupEvent;
    public event Action OnCharacterDeathEvent;
    public event Action OnRoundEndEvent;


    // *** Important - can set all character components to be derived from CharacterComponent -> Allows a simple initialization on Awake
    public NetworkPlayer_AnimationLink Animator { get; private set; }
    //  Player Camera at some point
    public NetworkPlayer_InputController Controller { get; private set; }
    //  Might split the input from controller - will be important later 
    //  reference to personal UI here
    public NetworkRigidbody Rigidbody { get; private set; }

    public bool hasDespawned = false;

    private void Awake()
    {
        Animator = GetComponentInChildren<NetworkPlayer_AnimationLink>();
        Controller = GetComponent<NetworkPlayer_InputController>();
        Rigidbody = GetComponent<NetworkRigidbody>();



        // *** If all components do this instead, allows for very reader friendly method of initialization
        var components = GetComponentsInChildren<CharacterComponent>();
        foreach (var component in components) component.Init(this);
    }

    public static readonly List<CharacterEntity> Characters = new List<CharacterEntity>();

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        Characters.Remove(this);
        hasDespawned = true;
        OnCharacterDespawned?.Invoke(this);
    }

    private void OnDestroy()
    {
        Characters.Remove(this);
        if(!hasDespawned)
        {
            OnCharacterDespawned?.Invoke(this);
        }
    }
}
