using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JoinGameUI : MonoBehaviour
{
    public TMP_InputField lobbyName;

    private void OnEnable()
    {
        SetLobbyName(lobbyName.text);
    }

    private void Start()
    {
        lobbyName.onValueChanged.AddListener(SetLobbyName);
        lobbyName.text = ClientInfo.LobbyName;
    }

    private void SetLobbyName(string lobby)
    {
        ClientInfo.LobbyName = lobby;
    }
}
