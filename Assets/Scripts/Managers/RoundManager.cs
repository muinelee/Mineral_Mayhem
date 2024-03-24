using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RoundManager : NetworkBehaviour
{
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
    private int redRoundsWon;
    private int blueRoundsWon;
    public int maxRounds = 3;
    public static RoundManager Instance { get; private set; }
    //public static event Action<NetworkPlayer> OnPlayerDeath;

    public event Action ResetRound;

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
        if (currentRound == maxRounds)
        {
            MatchEnd();
            return; 
        }
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
            Debug.Log("Red Wins the round!");
            RPC_UpdateRoundUIForClients(true);

        }
        else if (bluePlayersAlive > redPlayersAlive)
        {
            Debug.Log("Blue Wins the round!");
            RPC_UpdateRoundUIForClients(false);
        }

        if (currentRound != maxRounds) roundEndTimer = TickTimer.CreateFromSeconds(Runner, roundEndDuration);
    }

    public void MatchEnd()
    {
        if (redRoundsWon > blueRoundsWon)
        {
            Debug.Log("Red Wins the game!");
            RPC_UpdateRoundUIForClients(true); 
        }
        else if (blueRoundsWon > redRoundsWon) 
        {
            Debug.Log("Blue Wins the game!");
            RPC_UpdateRoundUIForClients(false); 
        }
        else
        {
            roundEndTimer = TickTimer.CreateFromSeconds(Runner, roundEndDuration);
            Debug.Log("Tie!");
        }
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
}
