using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Fusion;
using TMPro;
using UnityEngine;

// Essential Components for player character base
[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(SphereCollider), typeof(Rigidbody), typeof(NetworkRigidbody))]
[RequireComponent(typeof(NetworkPlayer_InputController), typeof(NetworkPlayer_Movement))]
[RequireComponent(typeof(NetworkPlayer_Attack), typeof(NetworkPlayer_Energy), typeof(NetworkPlayer_Health))]
[RequireComponent(typeof(NetworkMecanimAnimator), typeof(HitboxRoot))]

/* 
    Notes when creating new character:
    
    Add player model as a child object
    Add a Floating Health Bar prefab as a child object
    Add hitbox component to model
*/

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; private set; }

    [SerializeField] private NetworkPlayer_WorldSpaceHUD floatingHealthBar;


    [Networked] public int tokenID { get; set; }        // Value is set when spawned by CharacterSpawner

    [Header("Camera Offset")]
    [SerializeField] private float cameraAngle;

    [Header("Username UI")]
    public TextMeshProUGUI playerNameTMP;
    [Networked(OnChanged = nameof(OnPlayerNameChanged))]
    public NetworkString<_16> playerName { get; private set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;

            Debug.Log("Spawned local player");

            floatingHealthBar.gameObject.SetActive(false);

            RPC_SetPlayerNames(PlayerPrefs.GetString("PlayerName"));

            Debug.Log("Set Player Name");

            GetComponent<NetworkPlayer_InputController>().SetCam(Camera.main);

            Debug.Log("Set Camera for local player");

            Camera.main.transform.rotation = Quaternion.Euler(cameraAngle, 0, 0);
            
            Debug.Log ($"Camera's new position is {Camera.main.transform.position}");
            
            CinemachineVirtualCamera virtualCam = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>();
            virtualCam.Follow = this.transform;
            virtualCam.LookAt= this.transform;

            Debug.Log("Camera made to target local player");


            NetworkPlayer_InGameUI playerUI = FindAnyObjectByType<NetworkPlayer_InGameUI>();

            playerUI.SetPlayerHealth(GetComponent<NetworkPlayer_Health>());

            Debug.Log("Local player health linked to player UI");


            playerUI.SetPlayerEnergy(GetComponent<NetworkPlayer_Energy>());

            Debug.Log("Local player energy linked to player UI");


            NetworkPlayer_Movement playerMovement = GetComponent<NetworkPlayer_Movement>();
            playerUI.SetPlayerMovement(playerMovement);
            playerUI.SetDash(playerMovement.GetDash());

            Debug.Log("Local player movement linked to player UI");


            NetworkPlayer_Attack playerAttack = GetComponent<NetworkPlayer_Attack>();
            playerUI.SetPlayerAttack(playerAttack);
            playerUI.SetQAttack(playerAttack.GetQAttack());
            playerUI.SetEAttack(playerAttack.GetEAttack());
            playerUI.SetFAttack(playerAttack.GetFAttack());

            playerUI.PrimeUI();

            Debug.Log("Local player Attacks linked to player UI");
        }

        else
        {
            Debug.Log("Spawned remote player");

            floatingHealthBar.gameObject.SetActive(true);
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    private static void OnPlayerNameChanged(Changed<NetworkPlayer> player)
    {
        Debug.Log($"{Time.time} Player name has been changed to {player.Behaviour.playerName}");

        player.Behaviour.OnPlayerNameChanged();
    }

    private void OnPlayerNameChanged()
    {
        Debug.Log($"Dispalyed name changed for player {gameObject.name} to {playerName}");

        playerNameTMP.text = playerName.ToString();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetPlayerNames(string newPlayerName, RpcInfo info = default)
    {
        Debug.Log($"[RPC] Setting the new player name to {newPlayerName}");
        this.playerName = newPlayerName;
    }

    private void OnDestroy()
    {
        if (floatingHealthBar) Destroy(floatingHealthBar.gameObject);
    }
}