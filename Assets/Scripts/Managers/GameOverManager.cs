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

    [Header("GameOver Timer")]
    [SerializeField] private float gameOverScreenDuration;
    private TickTimer gameOverTimer = TickTimer.None;

    [Header("Victory Positions")]
    [SerializeField] private Transform[] victoryPositions;

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
        MoveWinners(NetworkPlayer.Team.Blue);
    }

    private void DisplayRedWins()
    {
        redWinImage.SetActive(true);
        MoveWinners(NetworkPlayer.Team.Red);
    }

    private void MoveWinners(NetworkPlayer.Team team)
    {
        CharacterEntity[] players = FindObjectsOfType<CharacterEntity>();

        int index = 0;

        foreach (CharacterEntity player in players)
        {
            if (player.Team == team)
            {
                player.transform.position = victoryPositions[index].position;
                player.transform.rotation = victoryPositions[index].rotation;

                index++;
            }
        }
    }

    public void DisplayWinners(bool isRedWins)
    {
        if (isRedWins) DisplayRedWins();
        else DisplayBlueWins();

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

        SceneManager.LoadScene(0);
    }
}