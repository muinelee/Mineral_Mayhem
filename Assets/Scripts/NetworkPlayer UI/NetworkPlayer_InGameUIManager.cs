using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer_InGameUIManager : CharacterComponent
{
    [SerializeField] private NetworkPlayer_InGameUI playerUI;
    [SerializeField] private NetworkPlayer_WorldSpaceHUD floatingHealthBar;

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasInputAuthority)
        {
            Debug.Log("InGameUIManager");

            playerUI = FindObjectOfType<NetworkPlayer_InGameUI>();

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

        else
        {
            // Spawned remote player
            floatingHealthBar.nonLocalPlayerHealthBar.gameObject.SetActive(true);
        }
    }
}
