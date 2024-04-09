using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Linq;

public class NetworkRunnerHandler : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    static NetworkRunner networkRunner = null;

    private int roomSize = 1;

    private void Awake()
    {
        NetworkRunner networkRunnerInScene = FindObjectOfType<NetworkRunner>();

        if (networkRunnerInScene != null) networkRunner = networkRunnerInScene;
    }

    void Start()
    {
        if (FindAnyObjectByType<NetworkRunner>()) return;

        networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.name = "NetworkRunner";

        //Debug.Log($"Created a network runner at {SceneManager.GetActiveScene().name}");

        if (SceneManager.GetActiveScene().name == "RichardCPhoton")     // Change "RichardCPhoton" to game scene name in future
        {
            var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, "TestSession", NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
        }

        //Debug.Log($"Server NetworkRunner started.");
    }

    INetworkSceneManager GetSceneManager(NetworkRunner runner)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneManager == null) sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        return sceneManager;
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, string sessionName, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
    {
        var sceneManager = GetSceneManager(runner);

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            SessionName = sessionName,
            CustomLobbyName = "OurLobbyID",
            Scene = scene,
            Initialized = initialized,
            SceneManager = sceneManager,
            PlayerCount = roomSize
        });
    }

    public void OnJoinLobby()
    {
        var clientTask = JoinLobby();
    }

    private async Task JoinLobby()
    {
        Debug.Log("Join Lobby started");

        string lobbyID = "OurLobbyID";

        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyID);

        if (!result.Ok) Debug.LogError($"Unable to join lobby {lobbyID}");

        else Debug.Log("Join Lobby ok");
    }

    public void CreateGame(string sessionName, string sceneName)
    {
        // Create game as a host
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Host, sessionName, NetAddress.Any(), SceneUtility.GetBuildIndexByScenePath($"_Scenes/{sceneName}"), null);
    }

    public void JoinGame(SessionInfo sessionInfo)
    {
        // Join existing game as client
        // NOTE: When joining game, the scene argument will be passing the current active scene because it will be overriten by the host
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Client, sessionInfo.Name, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
    }

    public int GetRoomSize()
    {
        return roomSize;
    }

    public void SetRoomSize(int size)
    {
        roomSize = size;
    }
}