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
    private string[] roomAddress = new string[] { "TrainingRoom", "RichardCPhoton" };
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
        if (networkRunnerHandler.GetRoomSize() <= 1) return;
        CreateGame(roomAddress[1]); 
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
        SetRoomSize(1);
        networkRunnerHandler.CreateGame("TrainingRoom", roomAddress[0]);
    }

    public void OnQuitClicked()
    {
        BackToMainMenu?.Invoke();
    }

    private void CreateGame(string game)
    {
        networkRunnerHandler.CreateGame(ClientInfo.LobbyName, game);
    }
}