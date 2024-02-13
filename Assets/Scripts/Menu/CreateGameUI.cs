using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CreateGameUI : MonoBehaviour
{
    public TMP_InputField lobbyName;
    public Button confirmButton;

    private void Start()
    {
        lobbyName.onValueChanged.AddListener(x =>
        {
            ServerInfo.LobbyName = x;
            confirmButton.interactable = !string.IsNullOrEmpty(x);
        });
        lobbyName.text = ServerInfo.LobbyName = "Session" + Random.Range(0, 1000);

        ServerInfo.ArenaID = 0;         // We would customize hese 3 values based on additional UI functionality. For now - I am lazy to implement
        ServerInfo.GameMode = 0;
        ServerInfo.MaxUsers = 4;
    }

    private bool isLobbyValid;

    public void ValidateLobby()
    {
        isLobbyValid = string.IsNullOrEmpty(ServerInfo.LobbyName) == false;
    }

    public void TryFocusScreen(UIScreen screen)
    {
        if (isLobbyValid)
        {
            UIScreen.Focus(screen);
        }
    }

    public void TryCreateLobby(GameLauncher launcher)
    {
        if (isLobbyValid)
        {
            launcher.JoinOrCreateLobby();
            isLobbyValid = false;
        }
    }
}
