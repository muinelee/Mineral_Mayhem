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
    public CharacterEntity character;
    public NetworkRunner networkRunner;

    [Header("Session Lobby Manager")]
    [SerializeField] private SessionLobbyManager sessionLobbyManager;

    // Dictionary for holding player UserIDs
    private Dictionary<int, NetworkPlayer> mapTokenIDWithNetworkPlayer = new Dictionary<int, NetworkPlayer>();

    [Header("Components for UI")]
    private NetworkPlayer_InputController playerInputController;


    private string[] roomAddress = new string[] { "RaeLeda/RaeLedaTrainingRoom", "RichardCPhoton" };

    private int GetPlayerGUID(NetworkRunner runner, PlayerRef player)
    {
        // if it is the local player, directly get GUID using the local instance of the NetworkInfoManager
        if (runner.LocalPlayer == player) return ConnectionTokenUtils.HashToken(NetworkInfoManager.instance.GetConnectionToken());
        
        // else if it the non local player, get GUIDI using the built in GetPlayerConnectionToken from Fusion
        else
        {
            byte[] token = runner.GetPlayerConnectionToken(player);

            if (token != null) return ConnectionTokenUtils.HashToken(token);

            // if token is null, a faulty player was passed
            Debug.LogError($"Invalid token was passed to GetPlayerToken() function");
            return 0;
        }
    }

    public void AddPlayerToMap(int token, NetworkPlayer player)
    {
        mapTokenIDWithNetworkPlayer.Add(token, player);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)                          // Spawns player in scene
    {
        if (!roomAddress.Contains(SceneManager.GetActiveScene().name)) return;

        if (runner.IsServer)
        {
            // Get the player's token
            int playerToken = GetPlayerGUID(runner, player);


                NetworkPlayer newPlayer = runner.Spawn(playerPrefab, transform.position, Quaternion.identity, player);

                if (character) runner.Spawn(character, transform.position, Quaternion.identity, player);

                newPlayer.tokenID = playerToken;
                mapTokenIDWithNetworkPlayer[playerToken] = newPlayer;
        }
    }

    public void SpawnCharacter()
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (playerInputController == null) return; 
        
        // playerInputController is set from NetworkPlayer_InputController when character is spawned
        input.Set(playerInputController.GetNetworkInput());
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

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        FindAnyObjectByType<NetworkRunner>().Shutdown();
        SceneManager.LoadScene("RichardCPlayerLobby");
    }

    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("Migrating Host");

        await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);

        // Find Network Runner Handler and start the host migration
        //FindObjectOfType<NetworkRunnerHandler>().StartHostMigration(hostMigrationToken);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player Left");
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

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

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

    public void SetInputController(NetworkPlayer_InputController controller)
    {
        playerInputController = controller;
    }
}
