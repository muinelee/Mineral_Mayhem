using Cinemachine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer_Input : CharacterComponent, INetworkRunnerCallbacks
{
    [Networked] public NetworkPlayer NetworkUser { get; set; }
    public bool CharacterSelected = false;
    public LayerMask layerMask;

    public float camScrollSpeed = 8f;
    public float minCamDistance = 30f;
    public float maxCamDistance = 60f;

    public event Action<float> OnCameraDistanceChange;

    private float cameraDistance = 48f;
    public float CameraDistance
    {
        get { return cameraDistance; }
        set
        {
            if (value == cameraDistance) return;
            cameraDistance = Mathf.Clamp(value, minCamDistance, maxCamDistance);
            OnCameraDistanceChange?.Invoke(cameraDistance);
        }
    }
    public event Action<bool> OnCameraLockSwapped;
    private bool cameraLockOnPlayer = true;
    public bool CameraLockOnPlayer
    {
        get { return cameraLockOnPlayer; }
        set
        {
            if (cameraLockOnPlayer == value) return;
            cameraLockOnPlayer = value;
            OnCameraLockSwapped?.Invoke(value);
        }
    }

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
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);

        Runner.RemoveCallbacks(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2)) CameraLockOnPlayer = !CameraLockOnPlayer;
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        CameraDistance += scrollInput * - camScrollSpeed;
    }

    public NetworkInputData GetInput()
    {
        var userInput = new NetworkInputData();
        if (!CharacterSelected) return userInput;

        if (Input.GetKey(KeyCode.W)) userInput.Buttons |= NetworkInputData.ButtonW;
        if (Input.GetKey(KeyCode.A)) userInput.Buttons |= NetworkInputData.ButtonA;
        if (Input.GetKey(KeyCode.S)) userInput.Buttons |= NetworkInputData.ButtonS;
        if (Input.GetKey(KeyCode.D)) userInput.Buttons |= NetworkInputData.ButtonD;

        if (Input.GetKey(KeyCode.F)) userInput.Buttons |= NetworkInputData.ButtonF;
        else if (Input.GetKey(KeyCode.Q)) userInput.Buttons |= NetworkInputData.ButtonQ;
        else if (Input.GetKey(KeyCode.E)) userInput.Buttons |= NetworkInputData.ButtonE;
        else if (Input.GetKey(KeyCode.Space)) userInput.Buttons |= NetworkInputData.ButtonDash;
        else if (Input.GetKey(KeyCode.Mouse0)) userInput.Buttons |= NetworkInputData.ButtonBasic;
        
        if (Input.GetKey(KeyCode.Mouse1)) userInput.Buttons |= NetworkInputData.ButtonBlock;
        
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit, Mathf.Infinity, layerMask))
            userInput.cursorLocation = new Vector2(raycastHit.point.x, raycastHit.point.z);

        return userInput;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!Object.HasInputAuthority) return;
        NetworkInputData userInput = GetInput();
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
