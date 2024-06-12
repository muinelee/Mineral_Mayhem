using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterSelect;

public class RoundManager : NetworkBehaviour
{
    public Dictionary<NetworkPlayer, Vector3> respawnPoints = new Dictionary<NetworkPlayer, Vector3>();

    //Resets the storm on round end
    public delegate void ResetStorm();
    //Starts the storm
    public delegate void StartStorm();

    private TickTimer gameStartTimer = TickTimer.None;
    private TickTimer roundStartTimer = TickTimer.None;  
    private TickTimer roundEndTimer = TickTimer.None;
    [SerializeField] private float gameStartDuration = 30f;
    [SerializeField] private float roundStartDuration = 10f; 
    [SerializeField] private float roundEndDuration = 10f; 
    private bool isRoundEnd;
    public int teammSize; 
    private int redPlayersAlive;
    private int bluePlayersAlive;

    public int currentRound = 0;    
    private int redRoundsWon = 0;
    private int blueRoundsWon = 0;
    public int maxRounds = 3;
    public static RoundManager Instance { get; private set; }
    //public static event Action<NetworkPlayer> OnPlayerDeath;

    public event Action MatchStartEvent;
    public event Action ResetRound;
    public event Action MatchEndEvent;

    //Public event for storm reset
    public static event ResetStorm resetStorm;
    //Start the storm
    public static event StartStorm startStorm;

    [Header("Round End Properties")]
    [SerializeField] private float matchEndDelay = 5;
    private TickTimer matchEndTimer = TickTimer.None;
    [SerializeField] AudioClip redTeamWins;
    [SerializeField] AudioClip blueTeamWins;

    public override void Spawned() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ResetRound += LoadRound;
        MatchEndEvent += MatchEnd;
    }
    
    public void RedPlayersDies()
    {
        //OnPlayerDeath?.Invoke(player); 
        //Debug.Log("Red player died!");
        redPlayersAlive--;

        if (redPlayersAlive != 0) return; 
        RoundEnd();
    }

    public void BluePlayersDies()
    {
        //OnPlayerDeath?.Invoke(player);   
        //Debug.Log("Blue player died!");
        bluePlayersAlive--;

        if (bluePlayersAlive != 0) return;
        RoundEnd();
    }

    public void LoadRound()
    {
        if (currentRound == maxRounds) return; 
        currentRound++;
        resetStorm?.Invoke();
        startStorm?.Invoke();

        // Spawning/Setting players back into their own positions 

        // Resetting health and lives 
        // 10 second wait time for game to start for doors to open OR input to be enabled 
        redPlayersAlive = teammSize; 
        bluePlayersAlive = teammSize;

        // Show Round UI
        //RPC_ShowRoundUI();

        StartCoroutine(StartCountDown());
    }

    public void RoundEnd() 
    {
        StartCoroutine(iRoundEnd());
    }
    private IEnumerator iRoundEnd()
    {
        yield return new WaitForSecondsRealtime(1f);

        // Checks which team has more players alive
        // Blueplayer and red playerdies already checks if all members on team dies 
        if (redPlayersAlive > bluePlayersAlive)
        {
            redRoundsWon++;
            RPC_UpdateRoundUIForClients(true);
        }

        else if (bluePlayersAlive > redPlayersAlive)
        {
            blueRoundsWon++;
            RPC_UpdateRoundUIForClients(false);
        }

        if (redRoundsWon == Mathf.CeilToInt((float)maxRounds / 2) || blueRoundsWon == Mathf.CeilToInt((float)maxRounds / 2))
        {
            matchEndTimer = TickTimer.CreateFromSeconds(Runner, matchEndDelay);
            yield break;
        }

        roundEndTimer = TickTimer.CreateFromSeconds(Runner, roundEndDuration);
    }

    public void MatchEnd()
    {
        if (redRoundsWon > blueRoundsWon)
        {
            RPC_DisplayGameOver(true);
            RPC_RedTeamWinsAnnouncer();
        }
        else if (blueRoundsWon > redRoundsWon)
        {
            RPC_DisplayGameOver(false);
            RPC_BlueTeamWinsAnnouncer();
        }

        else Debug.Log("Tie!");
        //RPC_HideRoundUI();
        RPC_GoToVictoryCamera();

        StartCoroutine(iMatchEnd());
    }
    private IEnumerator iMatchEnd()
    {
        yield return new WaitForSecondsRealtime(4f);

        RPC_CollapseRoundUI();

        yield return new WaitForSecondsRealtime(1.5f);

        RPC_HideRoundUI();
    }

    public void MatchStart()
    {
        NetworkCameraEffectsManager.instance.StartCinematic(NetworkPlayer.Local);
        MatchStartEvent?.Invoke();
        resetStorm?.Invoke();
        startStorm?.Invoke();
        NetworkPlayer_InGameUI.instance.HidePlayerUI();
        
        if (Runner.IsServer)
        {
            RPC_DisableControls(true);
            //RPC_ShowRoundUI();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (roundEndTimer.Expired(Runner))
        { 
            roundEndTimer = TickTimer.None;
            ResetRound?.Invoke();
        }

        if (matchEndTimer.Expired(Runner))
        {
            matchEndTimer = TickTimer.None;
            MatchEndEvent?.Invoke();
        }
    }

    public void OnResetRound()
    {
        ResetRound?.Invoke();
    }

    IEnumerator StartCountDown()
    {
        yield return 0;
        RPC_DisableControls(true);
        yield return new WaitForSeconds(2f);
        RPC_PlayCountdown();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayCountdown()
    {
        CountDownTimer.instance.StartCountDown();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateRoundUIForClients(bool isRedWin)
    {
        if (isRedWin) RoundUI.instance.RedWin();
        else RoundUI.instance.BlueWin();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_DisableControls(bool disable)
    {
        CharacterEntity[] characters = FindObjectsOfType<CharacterEntity>();
        foreach (CharacterEntity character in characters)
        {
            if (disable)
            {
                character.Health.DisableControls();
            }
            else
            {
                character.Health.EnableControls();
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_DisplayGameOver(bool isRedWins)
    {
        GameOverManager.Instance.DisplayWinners(isRedWins);
        NetworkPlayer_InGameUI.instance.enabled = false;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_GoToVictoryCamera()
    {
        NetworkCameraEffectsManager.instance.GoToVictoryCamera();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ShowRoundUI()
    {
        RoundUI.instance.ShowRoundUI();
        NetworkPlayer_InGameUI.instance.ShowPlayerUI();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_HideRoundUI()
    {
        RoundUI.instance.HideRoundUI();
        NetworkPlayer_InGameUI.instance.HidePlayerUI();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ExpandRoundUI()
    {
        RoundUI.instance.ExpandRoundUI();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_CollapseRoundUI()
    {
        RoundUI.instance.CollapseRoundUI();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_GrowRoundUI()
    {
        TimerManager.instance.StopTimer(false);
        RoundUI.instance.GrowRoundUI();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ShrinkRoundUI()
    {
        RoundUI.instance.ShrinkRoundUI();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_RedTeamWinsAnnouncer()
    {
        AudioManager.Instance.PlayAudioSFX(redTeamWins, transform.localPosition);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_BlueTeamWinsAnnouncer()
    {
        AudioManager.Instance.PlayAudioSFX(blueTeamWins, transform.localPosition);
    }
}
