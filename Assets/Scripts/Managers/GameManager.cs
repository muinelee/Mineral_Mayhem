using UnityEngine;
using Fusion;
using System;

public class GameManager : NetworkBehaviour
{
    public static event Action<GameManager> OnLobbyDetailsUpdated;

    public static Arena CurrentArena { get; private set; }
    public static bool IsPlaying => CurrentArena != null;
    public static GameManager Instance { get; private set; }
    public string ArenaName => ResourceManager.Instance.arenas[ArenaID].arenaName;
    public string ModeName => ResourceManager.Instance.gameModes[GameModeID].modeName;

    [Networked(OnChanged = nameof(LobbyDetailsCallback))] public NetworkString<_16> LobbyName { get; set; }
    [Networked(OnChanged = nameof(LobbyDetailsCallback))] public int ArenaID { get; set; }
    [Networked(OnChanged = nameof(LobbyDetailsCallback))] public int GameModeID { get; set; }
    [Networked(OnChanged = nameof(LobbyDetailsCallback))] public int MaxUsers { get; set; }

    private static void LobbyDetailsCallback(Changed<GameManager> changed)
    {
        OnLobbyDetailsUpdated?.Invoke(changed.Behaviour);
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void Spawned()
    {
        base.Spawned();
        
        if (Object.HasStateAuthority)
        {
            LobbyName = ServerInfo.LobbyName;
            ArenaID = ServerInfo.ArenaID;
            GameModeID = ServerInfo.GameMode;
            MaxUsers = ServerInfo.MaxUsers;
        }
    }

    public static void SetArena(Arena arena)
    {
        CurrentArena = arena;
    }
}
