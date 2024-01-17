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
    NetworkRunner networkRunner;
    /*
    private void Awake()
    {
        NetworkRunner networkRunnerInstance = FindAnyObjectByType<NetworkRunner>();

        if (networkRunnerInstance != null) networkRunner = networkRunnerInstance;
    }
    */

    void Start()
    {
        if (networkRunner) return;

        networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.name = "NetworkRunner";

        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, NetworkInfoManager.instance.GetConnectionToken() ,NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);

        Debug.Log($"Server NetworkRunner started.");
    }

    public void StartHostMigration(HostMigrationToken hostMigrationToken)
    {
        networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.name = "NetworkRunner - Migrated";

        var clientTask = InitializeHostMigration(networkRunner, hostMigrationToken, NetworkInfoManager.instance.GetConnectionToken());

        Debug.Log($"Host Migration started");
    }

    INetworkSceneManager GetSceneManager(NetworkRunner runner)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneManager == null) sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        return sceneManager;
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, byte[] connectionToken, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
    {
        var sceneManager = GetSceneManager(runner);

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            SessionName = "Test Room",
            Scene = scene,
            Initialized = initialized,
            SceneManager = sceneManager,
            ConnectionToken = connectionToken
        });
    }

    protected virtual Task InitializeHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken, byte[] connectionToken)
    {
        /* 
            hostMigrationToken has info from original scene manager:

            GameMode
            Address
            Scene
            SessionName
            CustomLobyName

            This info will be passed onto the new host
        */

        var sceneManager = GetSceneManager(networkRunner);

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            SceneManager = sceneManager,
            HostMigrationToken = hostMigrationToken, // holds info from previous host
            HostMigrationResume = HostMigrationResume, // will resume game once new host is determined
            ConnectionToken = connectionToken
        });
    }

    private void HostMigrationResume(NetworkRunner runner)
    {
        Debug.Log($"HostMigrationResume started");

        foreach (var resumeNetworkObject in runner.GetResumeSnapshotNetworkObjects())
        {
            if (resumeNetworkObject.TryGetBehaviour<NetworkRigidbody>(out var playerRigidbody))
            {
                runner.Spawn(resumeNetworkObject, position: playerRigidbody.ReadPosition(), rotation: playerRigidbody.ReadRotation(), onBeforeSpawned: (runner, newNetworkObject) =>
                {
                    newNetworkObject.CopyStateFrom(resumeNetworkObject);
                });
            }
        }

        Debug.Log($"HostMigrationResume completed");
    }
    /*
    public void OnJoinLobby()
    {
        var clientTask = JoinLobby();
    }

    private async Task JoinLobby()
    {
        Debug.Log("Join Lobby Started");

        string lobbyID = "Our Custom Lobby ID";

        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyID);

        if (!result.Ok) Debug.LogError($"Unable to join lobby {lobbyID}");
        else Debug.Log("Join Lobby OK");
    }

    public void CreateGame(string sessionName, string sceneName)
    {
        Debug.Log($"Created the session {sessionName} in the scene {sceneName}, with a build index of {SceneUtility.GetBuildIndexByScenePath($"scene/{sceneName}")}");

        InitializeNetworkRunner(networkRunner, GameMode.Host, sessionName, NetAddress.Any(), SceneUtility.GetBuildIndexByScenePath($"scene/{sceneName}"), null);
    }

    public void JoinGame(string sessionName, string sceneName)
    {
        Debug.Log($"Joining the session {sessionName}");

        InitializeNetworkRunner(networkRunner, GameMode.Client, sessionName, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
    }
    */
}