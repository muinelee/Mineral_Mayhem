using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

public class CharacterSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Placeholder player prefab")]
    public NetworkPlayer playerPrefab;

    [Header("Session Lobby Manager")]
    [SerializeField] private SessionLobbyManager sessionLobbyManager;

    // Dictionary for holding player UserIDs
    private Dictionary<int, NetworkPlayer> mapTokenIDWithNetworkPlayer = new Dictionary<int, NetworkPlayer>();

    private string[] roomAddress = new string[] { "RaeLeda/RaeLedaTrainingRoom", "RichardCPhoton" };

    public void AddPlayerToMap(int token, NetworkPlayer player)
    {
        mapTokenIDWithNetworkPlayer.Add(token, player);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)                          // Spawns player in scene
    {
        if (!roomAddress.Contains(SceneManager.GetActiveScene().name)) return;

        if (runner.IsServer)
        {
            NetworkPlayer newPlayer = runner.Spawn(playerPrefab, transform.position, Quaternion.identity, player);
        }
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("I got disconnected from server");

        NetworkPlayer.Players.Clear();
        FindAnyObjectByType<NetworkRunner>().Shutdown();
        SceneManager.LoadScene("RichardCPlayerLobby");
    }

    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        NetworkPlayer.Players.Clear();
        await FindAnyObjectByType<NetworkRunner>().Shutdown();
        SceneManager.LoadScene("RichardCPlayerLobby");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player Left");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        if (sessionLobbyManager == null) return;

        if (sessionList.Count == 0)
        {
            Debug.Log("No sessions running");
            sessionLobbyManager.OnNoSessionsFound();
        }

        else
        {
            sessionLobbyManager.ClearList();

            foreach (SessionInfo sessionInfo in sessionList)
            {
                sessionLobbyManager.AddToList(sessionInfo);

                Debug.Log($"Found session {sessionInfo.Name} with a player count of {sessionInfo.PlayerCount}");
            }    
        }
    }

    public void OnHostMigrationCleanup()
    {
        Debug.Log("Spawner OnHostMigrationCleanup started");

        foreach (KeyValuePair<int, NetworkPlayer> entry in mapTokenIDWithNetworkPlayer)
        {
            NetworkObject entryNetworkObject = entry.Value.GetComponent<NetworkObject>();

            if (entryNetworkObject.InputAuthority.IsNone)
            {
                Debug.Log($"{Time.time} {entry.Value.playerName} disconnected. Despawning.");
                entryNetworkObject.Runner.Despawn(entryNetworkObject);
            }
        }

        Debug.Log("Spawner OnHostMigrationCleanup completed");
    }    

    #region More Runner Callbacks
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
    }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {  
    }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {  
    }
    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }
    public void OnSceneLoadStart(NetworkRunner runner)
    {  
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
    #endregion
}