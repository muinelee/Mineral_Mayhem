using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using UnityEngine.Events;

public class PlayerJoinScreenUI : MonoBehaviour
{

    [Header("New Game Info")]
    public TMP_InputField sessionName;

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

    public void OnCreateNewGameClicked()
    {
        sessionName.text = PlayerPrefs.GetString("SessionName");
    }

    public void OnStartNewSessionClicked()
    {
        PlayerPrefs.SetString("SessionName", sessionName.text);
        networkRunnerHandler.CreateGame(sessionName.text, "RichardCPhoton");
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

    public void OnArenaClicked()
    {
        map = roomAddress[1];
    }

    public void OnQuitClicked()
    {
        BackToMainMenu?.Invoke();
    }
}