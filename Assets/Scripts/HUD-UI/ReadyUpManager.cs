using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;

public class ReadyUpManager : NetworkBehaviour
{
    [SerializeField] private Button readyUp;
    [SerializeField] private TextMeshProUGUI readyUpText;
    [SerializeField] private Button startGame;

    private bool isReady = false;

    [SerializeField] private TextMeshProUGUI[] playerList;

    public override void Spawned()
    {
        if (Object.HasStateAuthority) startGame.gameObject.SetActive(true);
    }

    public void OnReadyUp()
    {
        NetworkRunner runner = FindAnyObjectByType<NetworkRunner>();

        this.Object.AssignInputAuthority(NetworkPlayer.Local.playerRef);

        Debug.Log(this.Object.InputAuthority);

        RPC_ReadyUp();
    }

    public void OnStartGame()
    {
        if (!Object.HasStateAuthority) return;

        NetworkRunner runner = FindAnyObjectByType<NetworkRunner>();

        if (runner.IsServer)
        {
            foreach (TextMeshProUGUI playerName in playerList)
            {
                if (playerName.text == "Player")
                {
                    Debug.Log("There is an empty slot");
                    return;
                }

                else Debug.Log($"{playerName.text} is ready");
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_ReadyUp(RpcInfo info = default)
    {
        this.readyUpText.text = "This is working now";
    }
}