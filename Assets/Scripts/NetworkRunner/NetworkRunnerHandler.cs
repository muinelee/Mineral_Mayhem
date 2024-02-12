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

        Debug.Log($"Created a network runner at {SceneManager.GetActiveScene().name}");

        if (SceneManager.GetActiveScene().name == "RichardCPhoton")     // Change "RichardCPhoton" to game scene name in future
        {
            var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, "TestSession", NetworkInfoManager.instance.GetConnectionToken() ,NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
        }

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

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, string sessionName, byte[] connectionToken, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
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
            PlayerCount = 2,
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
            if (resumeNetworkObject.TryGetBehaviour<NetworkRigidbody>(out NetworkRigidbody playerRigidbody))
            {
                runner.Spawn(resumeNetworkObject, position: playerRigidbody.ReadPosition(), rotation: playerRigidbody.ReadRotation(), onBeforeSpawned: (runner, newNetworkObject) =>
                {
                    newNetworkObject.CopyStateFrom(resumeNetworkObject);
                
                    if (resumeNetworkObject.TryGetBehaviour<NetworkPlayer>(out NetworkPlayer oldNetworkPlayer))
                    {
                        // Store player token for reconnection
                        FindObjectOfType<CharacterSpawner>().AddPlayerToMap(oldNetworkPlayer.tokenID, newNetworkObject.GetComponent<NetworkPlayer>());
                    }
                    
                    if (resumeNetworkObject.TryGetBehaviour<NetworkPlayer_Health>(out NetworkPlayer_Health oldHealth))
                    {
                        NetworkPlayer_Health newHealth = newNetworkObject.GetComponent<NetworkPlayer_Health>();
                        newHealth.CopyStateFrom(oldHealth);
                    }
                });
            }
        }

        StartCoroutine(CleanUpHostMigrationCO());

        Debug.Log($"HostMigrationResume completed");
    }

    IEnumerator CleanUpHostMigrationCO()
    {
        yield return new WaitForSeconds(2f);

        FindObjectOfType<CharacterSpawner>().OnHostMigrationCleanup();
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
        Debug.Log($"Create session {sessionName} scene {sceneName} build index {SceneUtility.GetBuildIndexByScenePath($"_Scenes/{sceneName}")}");

        // Create game as a host
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Host, sessionName, NetworkInfoManager.instance.GetConnectionToken(), NetAddress.Any(), SceneUtility.GetBuildIndexByScenePath($"_Scenes/{sceneName}"), null);
    }

    public void JoinGame(SessionInfo sessionInfo)
    {
        // Join existing game as client
        // NOTE: When joining game, the scene argument will be passing the current active scene because it will be overriten by the host
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Client, sessionInfo.Name, NetworkInfoManager.instance.GetConnectionToken(), NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
    }
}