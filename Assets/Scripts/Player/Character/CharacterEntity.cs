using Fusion;
using System;
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

    #region Exposed Delegate Function Calls
    public override void OnHit(float x)
    {
        OnHitEvent?.Invoke(x);
    }
    public override void OnHeal(float x)
    {
        OnHealEvent?.Invoke(x);
    }
    public override void OnStatusBegin(StatusEffect status)
    {
        OnStatusBeginEvent?.Invoke(status);
    }
    public override void OnStatusEnded(StatusEffect status)
    {
        OnStatusEndedEvent?.Invoke(status);
    }

    public override void OnCharacterDeath()
    {
        OnCharacterDeathEvent?.Invoke();
    }
    public override void OnRoundEnd()
    {
        OnRoundEndEvent?.Invoke();
    }
    public override void OnPickup()
    {
        OnPickupEvent?.Invoke();
    }
    #endregion

    // *** Important - can set all character components to be derived from CharacterComponent -> Allows a simple initialization on Awake
    public NetworkRigidbody Rigidbody { get; private set; }
    public Collider Collider { get; private set; }
    public NetworkPlayer_AnimationLink Animator { get; private set; }
    public NetworkPlayer_InputController Controller { get; private set; }
    public NetworkPlayer_Movement Movement { get; private set; }
    public NetworkPlayer_Attack Attack { get; private set; }
    public StatusHandler StatusHandler { get; private set; }
    public NetworkPlayer_Health Health { get; private set; }
    public NetworkPlayer_OnSpawnUI PlayerUI { get; private set; }
    public NetworkPlayer.Team Team { get; private set; }
    public bool hasDespawned = false;
    public SpriteRenderer TeamIndicator;

    public override void Spawned()
    {
        Rigidbody = GetComponent<NetworkRigidbody>();
        Collider = GetComponent<Collider>();
        Animator = GetComponentInChildren<NetworkPlayer_AnimationLink>();
        Controller = GetComponent<NetworkPlayer_InputController>();
        StatusHandler = GetComponent<StatusHandler>();
        Movement = GetComponent<NetworkPlayer_Movement>();
        Attack = GetComponent<NetworkPlayer_Attack>();
        Health = GetComponent<NetworkPlayer_Health>();
        PlayerUI = GetComponent<NetworkPlayer_OnSpawnUI>();
        TeamIndicator = GetComponentInChildren<SpriteRenderer>();
        if (Object.HasInputAuthority) RPC_SetTeam(NetworkPlayer.Local.team);

        // *** If all components do this instead, allows for very reader friendly method of initialization
        var components = GetComponentsInChildren<CharacterComponent>();
        foreach (var component in components)
        {
            if (component == this) continue;
            component.Init(this);
        }
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

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SetTeam(NetworkPlayer.Team team)
    {
        this.Team = team;

        if (team == NetworkPlayer.Team.Red) this.TeamIndicator.color = Color.red;
        else this.TeamIndicator.color = Color.blue;
    }
}
