using Cinemachine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer_Input : CharacterComponent, INetworkRunnerCallbacks
{
    [Networked] public NetworkPlayer NetworkUser { get; set; }
    public bool characterHasBeenSelected = false;

    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        character.SetInput(this);
    }

    public override void Spawned()
    {
        base.Spawned();

        Runner.AddCallbacks(this);

        if (!Object.HasInputAuthority) return;

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

        if (Input.GetKey(KeyCode.Space)) userInput.Buttons |= NetworkInputStuff.ButtonDash;
        if (Input.GetKey(KeyCode.Q)) userInput.Buttons |= NetworkInputStuff.ButtonQ;
        if (Input.GetKey(KeyCode.E)) userInput.Buttons |= NetworkInputStuff.ButtonE;
        if (Input.GetKey(KeyCode.F)) userInput.Buttons |= NetworkInputStuff.ButtonF;
        if (Input.GetKey(KeyCode.Mouse0)) userInput.Buttons |= NetworkInputStuff.ButtonBasic;
        if (Input.GetKey(KeyCode.W)) userInput.Buttons |= NetworkInputStuff.ButtonW;
        if (Input.GetKey(KeyCode.A)) userInput.Buttons |= NetworkInputStuff.ButtonA;
        if (Input.GetKey(KeyCode.S)) userInput.Buttons |= NetworkInputStuff.ButtonS;
        if (Input.GetKey(KeyCode.D)) userInput.Buttons |= NetworkInputStuff.ButtonD;
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
