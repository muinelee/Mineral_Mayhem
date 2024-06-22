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
    [SerializeField] private CG_Fade redWinImage;
    [SerializeField] private CG_Fade blueWinImage;

    [SerializeField] private CG_Fade redWinRoundImage;
    [SerializeField] private CG_Fade blueWinRoundImage;

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
        blueWinImage.gameObject.SetActive(true);
        blueWinImage.FadeIn();
        MoveWinners(NetworkPlayer.Team.Blue);
    }
    private void DisplayRedWins()
    {
        redWinImage.gameObject.SetActive(true);
        redWinImage.FadeIn();
        MoveWinners(NetworkPlayer.Team.Red);
    }

    public void DisplayBlueWinsRound()
    {
        StartCoroutine(iDisplayBlueWinsRound());
    }
    private IEnumerator iDisplayBlueWinsRound()
    {
        blueWinRoundImage.gameObject.SetActive(true);
        blueWinRoundImage.FadeIn();

        yield return new WaitForSeconds(2.5f);

        blueWinRoundImage.FadeOut();
    }
    public void DisplayRedWinsRound()
    {
        StartCoroutine(iDisplayRedWinsRound());
    }
    private IEnumerator iDisplayRedWinsRound()
    {
        redWinRoundImage.gameObject.SetActive(true);
        redWinRoundImage.FadeIn();

        yield return new WaitForSeconds(2.5f);

        redWinRoundImage.FadeOut();
    }

    public void DisplayWinners(bool isRedWins)
    {
        if (isRedWins) DisplayRedWins();
        else DisplayBlueWins();

        gameOverTimer = TickTimer.CreateFromSeconds(Runner, gameOverScreenDuration);
        gameOver = true;

        FindAnyObjectByType<ShrinkingStorm>().gameObject.SetActive(false);
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
                
                if (Runner.SessionInfo.MaxPlayers > 2)
                {
                    player.transform.position = victoryPositionsTeam[index].position;
                    player.transform.rotation = victoryPositionsTeam[index].rotation;
                    index++;
                }

                else if (Runner.SessionInfo.MaxPlayers == 2)
                {
                    player.transform.position = victoryPositionSolo.position;
                    player.transform.rotation = victoryPositionSolo.rotation;
                }
            }

            else player.transform.position = Vector3.zero;
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

        if (gameOver) SceneManager.LoadScene("Main Menu");
        else SceneManager.LoadScene("PlayerDisconnected");
    }
}