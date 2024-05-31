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
    public event Action<bool> OnBlockEvent;
    public event Action<StatusEffect> OnStatusBeginEvent;
    public event Action<StatusEffect> OnStatusEndedEvent;
    public event Action OnCleanseEvent;
    public event Action OnPickupEvent;
    public event Action<float> OnEnergyChangeEvent;
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
    public override void OnBlock(bool isBlocking)
    {
        OnBlockEvent?.Invoke(isBlocking);
    }
    public override void OnStatusBegin(StatusEffect status)
    {
        OnStatusBeginEvent?.Invoke(status);
    }
    public override void OnStatusEnded(StatusEffect status)
    {
        OnStatusEndedEvent?.Invoke(status);
    }
    public override void OnCleanse()
    {
        OnCleanseEvent?.Invoke();
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
    public NetworkPlayer_Input Input { get; private set; }
    public NetworkPlayer_Movement Movement { get; private set; }
    public NetworkPlayer_Attack Attack { get; private set; }
    public StatusHandler StatusHandler { get; private set; }
    public NetworkPlayer_Health Health { get; private set; }
    public NetworkPlayer_Energy Energy { get; private set; }
    public NetworkPlayer_OnSpawnUI PlayerUI { get; private set; }
    public CharacterVisualHandler VisualHandler { get; private set; }

    # region Private Setter Functions
    public void SetAnimationLink(NetworkPlayer_AnimationLink characterComp)
    {
        Animator = characterComp;
    }
    public void SetInput(NetworkPlayer_Input characterComp)
    {
        Input = characterComp;
    }
    public void SetMovement(NetworkPlayer_Movement characterComp)
    {
        Movement = characterComp;
    }
    public void SetAttack(NetworkPlayer_Attack characterComp)
    {
        Attack = characterComp;
    }
    public void SetStatusHandler(StatusHandler characterComp)
    {
        StatusHandler = characterComp;
    }
    public void SetHealth(NetworkPlayer_Health characterComp)
    {
        Health = characterComp;
    }
    public void SetEnergy(NetworkPlayer_Energy characterComp)
    {
        Energy = characterComp;
    }
    public void SetPlayerUI(NetworkPlayer_OnSpawnUI characterComp)
    {
        PlayerUI = characterComp;
    }
    public void SetVisualHandler(CharacterVisualHandler characterComp)
    {
        VisualHandler = characterComp;
    }
    #endregion

    public NetworkPlayer.Team Team { get; private set; }
    public bool hasDespawned = false;
    public MeshRenderer TeamIndicator;

    public GameObject Shield;

    public static readonly List<CharacterEntity> Characters = new List<CharacterEntity>();

    public override void Spawned()
    {
        Rigidbody = GetComponent<NetworkRigidbody>();
        Collider = GetComponent<Collider>();

        if (!TeamIndicator) TeamIndicator = transform.Find("TeamIndicator").GetComponentInChildren<MeshRenderer>();
        if (Object.HasInputAuthority) RPC_SetTeam(NetworkPlayer.Local.team);
        
        var components = GetComponentsInChildren<CharacterComponent>();
        foreach (var component in components)
        {
            if (component == this) continue;
            component.Init(this);
        }

        if (Object.HasInputAuthority) NetworkCameraEffectsManager.instance.SetPlayerCamera(transform);
    }

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

        if (team == NetworkPlayer.Team.Red) this.TeamIndicator.material.color = Color.red;
        else this.TeamIndicator.material.color = Color.blue;
    }
}
