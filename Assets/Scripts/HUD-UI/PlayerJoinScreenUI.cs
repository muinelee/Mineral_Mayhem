using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerJoinScreenUI : MonoBehaviour
{
    [Header("Network Runner")]
    [SerializeField] NetworkRunnerHandler networkRunnerHandler;

    [Header("Lobby UI Panels")]
    [SerializeField] GameObject playerDetailsPanel;
    [SerializeField] GameObject sessionListPanel;
    [SerializeField] GameObject createSessionPanel;
    [SerializeField] GameObject statusPanel;

    [Header("Player Info")]
    public TMP_InputField playerName;

    [Header("New Game Info")]
    public TMP_InputField sessionName;

    private void Start()
    {
        if (PlayerPrefs.HasKey("PlayerName")) playerName.text = PlayerPrefs.GetString("PlayerName");
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

        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.OnJoinLobby();
    
        HideAllPanels();
        sessionListPanel.SetActive(true);
    }

    public void OnCreateNewGameClicked()
    {
        HideAllPanels();

        createSessionPanel.SetActive(true);
    }

    public void OnStartNewSessionClicked()
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.CreateGame(sessionName.text, "RichardCPhoton");

        HideAllPanels();

        statusPanel.SetActive(true);
    }
}