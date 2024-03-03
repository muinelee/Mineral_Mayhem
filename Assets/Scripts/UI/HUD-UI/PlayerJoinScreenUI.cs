using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
        HideAllPanels();

        createSessionPanel.SetActive(true);
    }

    public void OnStartNewSessionClicked()
    {
        if (networkRunnerHandler.GetGameMap() == NetworkRunnerHandler.GameMap.Undecided) return;

        networkRunnerHandler.CreateGame(sessionName.text, "RichardCPhoton");

        HideAllPanels();

        statusPanel.SetActive(true);
    }

    public void OnBackClicked()
    {
        HideAllPanels();

        playerDetailsPanel.SetActive(true);
    }

    public void OnJoiningServer()
    {
        HideAllPanels();

        statusPanel.SetActive(true);
    }
}