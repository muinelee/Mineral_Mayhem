using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class PlayerJoinScreenUI : MonoBehaviour
{
    [Header("Lobby UI Panels")]
    [SerializeField] private GameObject playerDetailsPanel;
    [SerializeField] private GameObject sessionListPanel;
    [SerializeField] private GameObject createSessionPanel;
    [SerializeField] private GameObject statusPanel;

    [Header("Player Info")]
    public TMP_InputField playerName;

    [Header("New Game Info")]
    public TMP_InputField sessionName;

    private NetworkRunnerHandler networkRunnerHandler;
    private string[] roomAddress = new string[] { "RaeLeda/RaeLedaTrainingRoom", "RichardCPhoton" };
    private string map;

    private void Start()
    {
        //Testing
        if (PlayerPrefs.HasKey("PlayerName")) playerName.text = PlayerPrefs.GetString("PlayerName");

        networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
    }
    private void HideAllPanels()
    {
        playerDetailsPanel.SetActive(false);
        sessionListPanel.SetActive(false);
        createSessionPanel.SetActive(false);
        statusPanel.SetActive(false);
    }

    public void OnFindGameClicked()
    {
        if (playerName.text == null)
        {
            Debug.Log("A player name needs to be entered first");
            return;
        }

        PlayerPrefs.SetString("PlayerName", playerName.text);

        networkRunnerHandler.OnJoinLobby();

        HideAllPanels();
    
        sessionListPanel.SetActive(true);
        FindObjectOfType<SessionLobbyManager>(true).OnLookingForSession();
    }

    public void OnCreateNewGameClicked()
    {
        PlayerPrefs.SetString("PlayerName", playerName.text);
        HideAllPanels();
        createSessionPanel.SetActive(true);
        sessionName.text = PlayerPrefs.GetString("SessionName");
    }

    public void OnStartNewSessionClicked()
    {
        PlayerPrefs.SetString("SessionName", sessionName.text);
        networkRunnerHandler.CreateGame(sessionName.text, "RichardCPhoton");
        /*
        // Training Map
        if (map == roomAddress[0]) networkRunnerHandler.CreateGame(sessionName.text, map);

        // Arena Map
        else if (map == roomAddress[1] && networkRunnerHandler.GetRoomSize() > 1) networkRunnerHandler.CreateGame(sessionName.text, map);

        else return;
        // Display Status Panel
        HideAllPanels();
        statusPanel.SetActive(true);
        */
    }

    public void OnBackClicked()
    {
        HideAllPanels();

        playerDetailsPanel.SetActive(true);

        networkRunnerHandler.SetRoomSize(1);
        map = "";
    }

    public void OnJoiningServer()
    {
        HideAllPanels();

        statusPanel.SetActive(true);
    }

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
        Application.Quit();
    }
}