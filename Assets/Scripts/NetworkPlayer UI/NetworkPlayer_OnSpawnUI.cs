using Cinemachine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkPlayer_OnSpawnUI : CharacterComponent
{

    [Header("Player UI Elements")]
    [SerializeField] private NetworkPlayer_InGameUI playerUIPF;
    public NetworkPlayer_InGameUI playerUI;

    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        character.SetPlayerUI(this);
    }

    public override void Spawned()
    {
        if (!Object.HasInputAuthority) return;

        if (RoundManager.Instance)
        {
            //RoundManager.Instance.MatchStartEvent += SpawnPlayerUI;
        }
    }

    public void SpawnPlayerUI()
    {
        if (Object.HasInputAuthority)
        {
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

    private void OnDestroy()
    {
        if (RoundManager.Instance)
        {
            //RoundManager.Instance.MatchStartEvent -= SpawnPlayerUI;
        }
    }
}
