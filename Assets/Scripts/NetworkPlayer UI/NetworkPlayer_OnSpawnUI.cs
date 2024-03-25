using Cinemachine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkPlayer_OnSpawnUI : NetworkBehaviour
{

    [Header("Player UI Elements")]
    [SerializeField] private NetworkPlayer_InGameUI playerUIPF;
    public NetworkPlayer_InGameUI playerUI;

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasInputAuthority)
        {
            // Link Cinemachine
            /*CinemachineVirtualCamera virtualCam = GameObject.FindWithTag("PlayerCamera").GetComponent<CinemachineVirtualCamera>();
            virtualCam.Follow = this.transform;
            virtualCam.LookAt = this.transform;

            GetComponent<NetworkPlayer_InputController>().SetCam(Camera.main);*/

            playerUI = Instantiate(playerUIPF, GameObject.FindGameObjectWithTag("UI Canvas").transform);

            // Local player health linked to player UI
            playerUI.SetPlayerHealth(GetComponent<NetworkPlayer_Health>());

            // Local player energy linked to player UI
            playerUI.SetPlayerEnergy(GetComponent<NetworkPlayer_Energy>());


            // Local player movement linked to player UI
            NetworkPlayer_Movement playerMovement = GetComponent<NetworkPlayer_Movement>();
            playerUI.SetPlayerMovement(playerMovement);
            playerUI.SetDash(playerMovement.GetDash());

            // Local player Attacks linked to player UI
            NetworkPlayer_Attack playerAttack = GetComponent<NetworkPlayer_Attack>();
            playerUI.SetPlayerAttack(playerAttack);
            playerUI.SetQAttack(playerAttack.GetQAttack());
            playerUI.SetEAttack(playerAttack.GetEAttack());
            playerUI.SetFAttack(playerAttack.GetFAttack());

            playerUI.PrimeUI();
        }
    }
}
