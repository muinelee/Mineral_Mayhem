using Cinemachine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class NetworkPlayer_Input : CharacterComponent, INetworkRunnerCallbacks
{
    [Networked] public NetworkPlayer NetworkUser { get; set; }

    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        character.SetInput(this);
    }

    public override void Spawned()
    {
        base.Spawned();

        Runner.AddCallbacks(this);

        CinemachineVirtualCamera virtualCam = GameObject.FindWithTag("PlayerCamera").GetComponent<CinemachineVirtualCamera>();
        virtualCam.Follow = this.transform;
        virtualCam.LookAt = this.transform;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);

        Runner.RemoveCallbacks(this);
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var userInput = new NetworkInputStuff();

        if (Input.GetKeyDown(KeyCode.Space)) userInput.Buttons |= NetworkInputStuff.ButtonDash;
        if (Input.GetKeyDown(KeyCode.Q)) userInput.Buttons |= NetworkInputStuff.ButtonQ;
        if (Input.GetKeyDown(KeyCode.E)) userInput.Buttons |= NetworkInputStuff.ButtonE;
        if (Input.GetKeyDown(KeyCode.F)) userInput.Buttons |= NetworkInputStuff.ButtonF;
        if (Input.GetKeyDown(KeyCode.Mouse0)) userInput.Buttons |= NetworkInputStuff.ButtonBasic;
        if (Input.GetKeyDown(KeyCode.W)) userInput.Buttons |= NetworkInputStuff.ButtonW;
        if (Input.GetKeyDown(KeyCode.A)) userInput.Buttons |= NetworkInputStuff.ButtonA;
        if (Input.GetKeyDown(KeyCode.S)) userInput.Buttons |= NetworkInputStuff.ButtonS;
        if (Input.GetKeyDown(KeyCode.D)) userInput.Buttons |= NetworkInputStuff.ButtonD;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit))
            userInput.cursorLocation = new Vector2(raycastHit.point.x, raycastHit.point.z);

        input.Set(userInput);
    }

    #region More Runner Callbacks
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
    }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
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
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
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
