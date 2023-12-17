using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Fusion;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; private set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;

            Debug.Log("Spawned local player");

            CinemachineVirtualCamera virtualCam = FindAnyObjectByType<CinemachineVirtualCamera>();
            virtualCam.Follow = this.transform;
            virtualCam.LookAt= this.transform;

            Debug.Log("Camera made to target local player");

            GetComponent<NetworkPlayer_InputController>().SetCam(FindAnyObjectByType<Camera>());

            Debug.Log("Set Camera for local player");


            // NetworkPlayer_Movement components

            NetworkPlayer_Movement playerMovement = GetComponent<NetworkPlayer_Movement>();

            playerMovement.SetNetworkRigidbody(GetComponent<NetworkRigidbody>());

            playerMovement.SetAnimator(GetComponentInChildren<Animator>());

            Debug.Log("Set Network Rigidbody and Animator for local player's movement script");
        }

        else
        {
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