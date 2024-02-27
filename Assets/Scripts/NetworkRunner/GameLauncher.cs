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
        // Defining Logic for this script - Creates the runner, and assigns this scripts callbacks to it on the spot.

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
        //  Defining Logic as well - when session is over, Runner.Shutdown() will disable all NetworkBehaviours in scene.
        if (Runner != null) Runner.Shutdown();
        else SetConnectionStatus(ConnectionStatus.Disconnected);
    }

    #region INetworkRunnerCallbacks
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
        Debug.Log($"{player.PlayerId} diconnected");

        NetworkPlayer.RemovePlayer(runner, player);

        SetConnectionStatus(ConnectionStatus);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        SetConnectionStatus(ConnectionStatus.Disconnected);

        NetworkPlayer.Players.Clear();

        if (Runner) Destroy(Runner.gameObject);

        //  IF we use object pooling later, would be a place to reset those here


        Runner = null;
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

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }
    #endregion
}
