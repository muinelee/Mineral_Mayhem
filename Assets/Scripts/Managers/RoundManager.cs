using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : NetworkBehaviour
{
    public Dictionary<NetworkPlayer, Vector3> respawnPoints = new Dictionary<NetworkPlayer, Vector3>();

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
        Debug.Log("Red player died!");
        redPlayersAlive--;

        if (redPlayersAlive != 0) return; 
        RoundEnd();
    }

    public void BluePlayersDies()
    {
        //OnPlayerDeath?.Invoke(player);   
        Debug.Log("Blue player died!");
        bluePlayersAlive--;

        if (bluePlayersAlive != 0) return;
        RoundEnd();
    }

    public void LoadRound()
    {
        if (currentRound == maxRounds) return; 
        currentRound++;
        // Round start based on if its round 1, then its 30s, if not, 10s 
        float startDuration = (currentRound == 1) ? gameStartDuration : roundStartDuration;
        roundStartTimer = TickTimer.CreateFromSeconds(Runner, startDuration);
        // Spawning/Setting players back into their own positions 

        // Resetting health and lives 
        // 10 second wait time for game to start for doors to open OR input to be enabled 
        redPlayersAlive = teammSize; 
        bluePlayersAlive = teammSize; 
    }

    private void RoundEnd()
    {
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
            MatchEndEvent?.Invoke();
            return;
        }

        roundEndTimer = TickTimer.CreateFromSeconds(Runner, roundEndDuration);
    }

    public void MatchEnd()
    {
        if (redRoundsWon > blueRoundsWon) RPC_DisplayGameOver(true);
        else if (blueRoundsWon > redRoundsWon) RPC_DisplayGameOver(false);
        else Debug.Log("Tie!");
    }

    public void MatchStart()
    {
        MatchStartEvent?.Invoke();
    }

    public override void FixedUpdateNetwork()
    {
        if (roundEndTimer.Expired(Runner))
        {
            roundEndTimer = TickTimer.None; 
            ResetRound?.Invoke();
        }
    } 

    public void OnResetRound()
    {
        ResetRound?.Invoke();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateRoundUIForClients(bool isRedWin)
    {
        if (isRedWin) RoundUI.instance.RedWin();
        else RoundUI.instance.BlueWin();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_DisplayGameOver(bool isRedWins)
    {
        GameOverManager.Instance.DisplayWinners(isRedWins);
        NetworkPlayer_InGameUI.instance.enabled = false;
    }
}
