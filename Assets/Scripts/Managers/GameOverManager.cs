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
    [SerializeField] private Transform[] victoryPositionsTeam;
    [SerializeField] private Transform victoryPositionSolo;

    public bool gameOver = false;

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

    public void DisplayWinners(bool isRedWins)
    {
        if (isRedWins) DisplayRedWins();
        else DisplayBlueWins();

        gameOverTimer = TickTimer.CreateFromSeconds(Runner, gameOverScreenDuration);
        gameOver = true;
    }

    private void MoveWinners(NetworkPlayer.Team team)
    {
        CharacterEntity[] players = FindObjectsOfType<CharacterEntity>();

        int index = 0;

        foreach (CharacterEntity player in players)
        {
            if (player.Team == team)
            {
                player.Health.DisableControls();

                player.Rigidbody.Rigidbody.velocity = Vector3.zero;

                player.Animator.anim.Play("Victory");
                player.Animator.anim.Play("Victory", 2);

                player.gameObject.GetComponentInChildren<NetworkPlayer_WorldSpaceHUD>().HideFloatingHealthBar();

                player.transform.rotation = victoryPositionSolo.rotation;
                
                if (Runner.SessionInfo.MaxPlayers > 2)
                {
                    player.transform.position = victoryPositionsTeam[index].position;

                    index++;
                }

                else if (Runner.SessionInfo.MaxPlayers == 2)
                {
                    player.transform.position = victoryPositionSolo.position;
                }
            }
        }
    }

    public void ReturnToLobby()
    {
        if (!Runner.IsServer) return;

        gameOverTimer = TickTimer.None;

        NetworkRunner runner = FindAnyObjectByType<NetworkRunner>();

        int num = 0;

        foreach (NetworkPlayer player in NetworkPlayer.Players)
        {
            Debug.Log($"this ran {num}");
            num++;
            if (player.Object.HasInputAuthority) continue;
            runner.Disconnect(player.Object.InputAuthority);
        }

        NetworkPlayer.Players.Clear();

        runner.Shutdown();

        if (gameOver) SceneManager.LoadScene(0);
        else SceneManager.LoadScene(5);
    }
}