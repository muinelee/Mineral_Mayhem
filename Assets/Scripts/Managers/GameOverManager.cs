using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Fusion;

public class GameOverManager : NetworkBehaviour
{
    public static GameOverManager Instance { get; set; }

    [Header("Text Images")]
    [SerializeField] private GameObject redWinImage;
    [SerializeField] private GameObject blueWinImage;

    [Header("Buttons")]
    [SerializeField] private Button rematchButton;
    [SerializeField] private Button returnToLobbyButton;

    [Header("GameOver Timer")]
    [SerializeField] private float gameOverScreenDuration;
    private TickTimer gameOverTimer = TickTimer.None;

    private void Awake()
    {
        Instance = this;
    }

    public override void FixedUpdateNetwork()
    {
        if (gameOverTimer.Expired(Runner)) ReturnToLobby();
    }

    private void DisplayBlueWins()
    {
        blueWinImage.SetActive(true);
    }

    private void DisplayRedWins()
    {
        redWinImage.SetActive(true);
    }

    public void DisplayWinners(bool isRedWins)
    {
        if (isRedWins) DisplayRedWins();
        else DisplayBlueWins();

        rematchButton.gameObject.SetActive(true);
        returnToLobbyButton.gameObject.SetActive(true);

        gameOverTimer = TickTimer.CreateFromSeconds(Runner, gameOverScreenDuration);
    }

    public void ReturnToLobby()
    {
        if (!Runner.IsServer) return;

        gameOverTimer = TickTimer.None;

        NetworkRunner runner = FindAnyObjectByType<NetworkRunner>();

        foreach (NetworkPlayer player in NetworkPlayer.Players)
        {
            if (player.Object.HasStateAuthority) continue;
            runner.Disconnect(player.Object.InputAuthority);
        }

        NetworkPlayer.Players.Clear();

        runner.Shutdown();

        SceneManager.LoadScene(1);
    }
}