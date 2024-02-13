using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;
public class ReadyUpName : MonoBehaviour
{
    private NetworkPlayer player;

    [SerializeField] private Button kickButton;

    private void Awake()
    {
        if (FindAnyObjectByType<NetworkRunner>().IsServer) kickButton.gameObject.SetActive(true);
    }

    public void SetPlayer(NetworkPlayer player)
    {
        this.player = player;
        Debug.Log($"Player name is {player.playerName}");
        GetComponentInChildren<TextMeshProUGUI>().text = player.playerName.ToString();
        if (player.HasStateAuthority && player.HasInputAuthority) kickButton.gameObject.SetActive(false);
    }

    public NetworkPlayer GetPlayer()
    {
        return player;
    }

    public void KickPlayer()
    {
        ReadyUpManager.instance.KickPlayer(player);
        Destroy(gameObject);
    }
}