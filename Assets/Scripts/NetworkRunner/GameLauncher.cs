using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ConnectionStatus
{
    Disconnected,
    Connecting,
    Failed,
    Connected,
}

[RequireComponent(typeof(LevelManager))]
public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private GameManager gameManagerPrefab;
    [SerializeField] private NetworkPlayer networkPlayerPrefab;


    public static ConnectionStatus ConnectionStatus = ConnectionStatus.Disconnected;

    private GameMode Launcher_GameMode;
    private NetworkRunner Runner;
    private LevelManager LevelManager;

    private void Start()
    {
        LevelManager = GetComponent<LevelManager>();

        DontDestroyOnLoad(gameObject);
    
        // Launch Default Lobby Scene here
    }

    public void SetCreateLobby() => Launcher_GameMode = GameMode.Host;
    public void SetJoinLobby() => Launcher_GameMode = GameMode.Client;

    public void JoinOrCreateLobby()
    {
        SetConnectionStatus(ConnectionStatus.Connecting);

        if (Runner != null) LeaveSession();

        GameObject session = new GameObject("Session Runner");
        DontDestroyOnLoad(session);

        Runner = session.AddComponent<NetworkRunner>();
        Runner.ProvideInput = Launcher_GameMode != GameMode.Server;
        Runner.AddCallbacks(this);

        Debug.Log($"Created gameobject {session.name} - starting game");
        Runner.StartGame(new StartGameArgs
        {
            GameMode = Launcher_GameMode,
            SessionName = Launcher_GameMode == GameMode.Host ? ServerInfo.LobbyName : ClientInfo.LobbyName,
            SceneManager = LevelManager,
            PlayerCount = ServerInfo.MaxUsers,
            DisableClientSessionCreation = true
        });
    }

    private void SetConnectionStatus(ConnectionStatus status)
    {
        Debug.Log($"Setting connection status to {status}");
        ConnectionStatus = status;

        if (!Application.isPlaying) return;

        if (status == ConnectionStatus.Disconnected || status == ConnectionStatus.Failed)
        {
            //  Go back to lobby scene + Do appropriate UI stuff
        }
    }

    public void LeaveSession()
    {
        if (Runner != null) Runner.Shutdown();
        else SetConnectionStatus(ConnectionStatus.Disconnected);
    }




    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} joined!");
        if (runner.IsServer)
        {
            // Logic to spawn Game Manager
            if (Launcher_GameMode == GameMode.Host) runner.Spawn(gameManagerPrefab, Vector3.zero, Quaternion.identity);
            var networkPlayer = runner.Spawn(networkPlayerPrefab, Vector3.zero, Quaternion.identity, player);
            networkPlayer.GameState = NetworkPlayer.EnumGameState.Lobby;
        }
        SetConnectionStatus(ConnectionStatus.Connected);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // PICK UP HERE YOU FUCK
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        throw new NotImplementedException();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");
        SetConnectionStatus(ConnectionStatus.Connected);
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("Disconnected from server");
        LeaveSession();
        SetConnectionStatus(ConnectionStatus.Disconnected);
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        // Add logic to refuse a connection request here (For example, you're not in the correct scene), else accept
        request.Accept();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("Connect failed");
        LeaveSession();
        SetConnectionStatus(ConnectionStatus.Failed);
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }
}
