using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Fusion;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; private set; }

    public static bool isLocal { get; private set; }
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            isLocal = true;

            Debug.Log("Spawned local player");


            CinemachineVirtualCamera cam = FindAnyObjectByType<CinemachineVirtualCamera>();
            CameraFollowPoint camFollowPoint = GetComponentInChildren<CameraFollowPoint>();
            cam.LookAt = camFollowPoint.transform;
            cam.Follow = camFollowPoint.transform;

            camFollowPoint.SetTarget(transform);
            camFollowPoint.transform.parent = null;
    
            NetworkPlayer_InputController playerInput = GetComponent<NetworkPlayer_InputController>();
            playerInput.camFollowPoint = camFollowPoint;

            Debug.Log("Camera made to target local player");
        }
        else
        {
            isLocal = false;
            Debug.Log("Spawned remote player");
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }
}