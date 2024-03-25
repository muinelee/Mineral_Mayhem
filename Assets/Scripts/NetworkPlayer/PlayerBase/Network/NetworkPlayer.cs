using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Essential Components for player character base
//[RequireComponent(typeof(SphereCollider), typeof(Rigidbody), typeof(NetworkRigidbody))]
//[RequireComponent(typeof(NetworkObject))]
/*[RequireComponent(typeof(SphereCollider), typeof(Rigidbody), typeof(NetworkRigidbody))]
[RequireComponent(typeof(NetworkPlayer_InputController), typeof(NetworkPlayer_Movement))]
[RequireComponent(typeof(NetworkPlayer_Attack), typeof(NetworkPlayer_Energy), typeof(NetworkPlayer_Health))]
[RequireComponent(typeof(NetworkMecanimAnimator), typeof(HitboxRoot))]*/

/* 
    Notes when creating new character:
    
    Add player model as a child object
    Add a Floating Health Bar prefab as a child object
    Add hitbox component to model
*/

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public enum EnumGameState
    {
        Lobby,
        GameReady,
        CharacterSelection,
        Cutscene,
    }

    public static readonly List<NetworkPlayer> Players = new List<NetworkPlayer>();

    public static event Action<NetworkPlayer> OnPlayerJoined;
    public static event Action<NetworkPlayer> OnPlayerChanged;
    public static event Action<NetworkPlayer> OnPlayerLeave;
    public static NetworkPlayer Local { get; private set; }

    [Networked] public NetworkPlayer_InputController Avatar { get; set; }
    [Networked] public EnumGameState GameState { get; set; }
    [Networked] public int CharacterID { get; set; }

    public bool IsLeader => Object != null && Object.IsValid && Object.HasStateAuthority;

    [SerializeField] private ReadyUpManager readyUpUIPF;

    [Networked] public NetworkBool isReady { get; set; } = false;

    public enum Team { Undecided, Red, Blue};
    [Networked] public Team team { get; set; } = Team.Undecided;

    [Networked] public int tokenID { get; set; }        // Value is set when spawned by CharacterSpawner

    [Header("Team Properties")]

    [Header("Camera Offset")]
    [SerializeField] private float cameraAngle;

    [Header("Username UI")]
    public TextMeshProUGUI playerNameTMP;
    [Networked(OnChanged = nameof(OnStateChanged))] public NetworkBool IsReady {  get; set; }
    [Networked(OnChanged = nameof(OnPlayerNameChanged))] public NetworkString<_16> playerName { get; private set; }

    public override void Spawned()
    {
        // *** Must take a look at decoupling this later - can be much smaller, when removing most physical parts of the character.avatar from the player ***
        base.Spawned();

        if (Object.HasInputAuthority)
        {
            Local = this;

            OnPlayerChanged?.Invoke(this);
            RPC_SetPlayerStats(ClientInfo.Username, ClientInfo.CharacterID);

            playerName = PlayerPrefs.GetString("PlayerName");
            RPC_SetPlayerNames(PlayerPrefs.GetString("PlayerName"));

            ReadyUpManager readyUpUI = Instantiate(readyUpUIPF, GameObject.FindGameObjectWithTag("UI Canvas").transform);
            readyUpUI.PrimeReadyUpUI(this);
            RPC_JoinUndecided();
        }

        Players.Add(this);
        OnPlayerJoined?.Invoke(this);
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority) Runner.Despawn(Object);
    }

    private static void OnPlayerNameChanged(Changed<NetworkPlayer> player)
    {
        player.Behaviour.OnPlayerNameChanged();
    }

    private void OnPlayerNameChanged()
    {
        //playerNameTMP.text = playerName.ToString();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetPlayerNames(string newPlayerName, RpcInfo info = default)
    {
        this.playerName = newPlayerName;
    }

    #region <----- Ready Up functions ----->

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_ReadyUp()
    {
        ReadyUpManager.instance.ReadyUp(this);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_UnReadyUp()
    {
        ReadyUpManager.instance.UnReadyUp(this);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_JoinBlueTeam()
    {
        ReadyUpManager.instance.JoinBlueTeam(this);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_JoinRedTeam()
    {
        ReadyUpManager.instance.JoinRedTeam(this);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_JoinUndecided()
    {
        ReadyUpManager.instance.JoinUndecided(this);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_StartGame()
    {
        ReadyUpManager.instance.StartGame();
    }

    #endregion

    public static void RemovePlayer(NetworkRunner runner, PlayerRef p)
    {
        var roomPlayer = Players.FirstOrDefault(x => x.Object.InputAuthority == p);
        // Despawns the avatar controlled character
        if (roomPlayer != null) runner.Despawn(roomPlayer.Avatar.Object);

        // Despawns the network player
        Players.Remove(roomPlayer);
        runner.Despawn(roomPlayer.Object);
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority, InvokeResim = true)]
    private void RPC_SetPlayerStats(NetworkString<_16> username, int charID)
    {
        playerName = username;
        CharacterID = charID;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_ChangeReadyState(NetworkBool state)
    {
        Debug.Log($"Setting {Object.Name} ready state to {state}");
        IsReady = state;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetCharacterID(int newID)
    {
        CharacterID = newID;
    }

    private static void OnStateChanged(Changed<NetworkPlayer> changed) => OnPlayerChanged?.Invoke(changed.Behaviour);

    public CharacterEntity SpawnCharacter(CharacterEntity character, PlayerRef player)
    {
        return Runner.Spawn(character, Vector3.zero, Quaternion.identity, player);
    }
}