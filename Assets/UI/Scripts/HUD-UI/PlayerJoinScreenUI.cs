using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using UnityEngine.Events;

public class PlayerJoinScreenUI : MonoBehaviour
{
    private NetworkRunnerHandler networkRunnerHandler;
    private string[] roomAddress = new string[] { "RaeLeda/RaeLedaTrainingRoom", "RichardCPhoton" };
    private string map;

    public UnityEvent BackToMainMenu;

    private void Start()
    {
        networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
    }

    public void OnFindGameClicked()
    {
        networkRunnerHandler.OnJoinLobby();
        FindObjectOfType<SessionLobbyManager>(true).OnLookingForSession();
    }

    public void OnStartNewSessionClicked()
    {
        networkRunnerHandler.CreateGame(ClientInfo.LobbyName, "RichardCPhoton");
    }

    public void OnBackClicked()
    {
        networkRunnerHandler.SetRoomSize(1);
        map = "";
    }

    // Maybe hide panels on game joined?

    public void SetRoomSize(int size)
    {
        networkRunnerHandler.SetRoomSize(size);
    }

    public void OnTrainingClicked()
    {
        map = roomAddress[0];
    }

    public void OnQuitClicked()
    {
        BackToMainMenu?.Invoke();
    }
}