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

    private int currentRound = 0;    
    private int redRoundsWon;
    private int blueRoundsWon;
    private int maxRounds = 3;
    public static RoundManager Instance { get; private set; }
    //public static event Action<NetworkPlayer> OnPlayerDeath;
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
    }
    
    public void RedPlayersDies()
    {
        //OnPlayerDeath?.Invoke(player); 
        Debug.Log("Red player died!");
        redPlayersAlive--;

        if (redPlayersAlive != 0) return; 
        if (currentRound == 3) MatchEnd();
        else RoundEnd();
        
    }

    public void BluePlayersDies()
    {
        //OnPlayerDeath?.Invoke(player);   
        Debug.Log("Blue player died!");
        bluePlayersAlive--;

        if (bluePlayersAlive != 0) return;
        if (currentRound == 3) MatchEnd();
        else RoundEnd();
    }

    public void LoadRound()
    {
        currentRound++;
        // Resetting Timer
        roundStartTimer = TickTimer.None;
        // Round start based on if its round 1, then its 30s, if not, 15s 
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
            RPC_UpdateCharacterRoundUIForClients(true);

        }
        else if (bluePlayersAlive > redPlayersAlive)
        {
            Debug.Log("Blue Wins the round!");
            RPC_UpdateCharacterRoundUIForClients(false);
        }

        LoadRound(); 
    }

    public void MatchEnd()
    {
        if (redRoundsWon > blueRoundsWon)
        {
            roundEndTimer = TickTimer.CreateFromSeconds(Runner, roundEndDuration);
            Debug.Log("Red Wins the game!");
            RPC_UpdateCharacterRoundUIForClients(true); 
        }
        else if (blueRoundsWon > redRoundsWon) 
        {
            roundEndTimer = TickTimer.CreateFromSeconds(Runner, roundEndDuration); 
            Debug.Log("Blue Wins the game!");
            RPC_UpdateCharacterRoundUIForClients(false); 
        }
        else
        {
            roundEndTimer = TickTimer.CreateFromSeconds(Runner, roundEndDuration);
            Debug.Log("Tie!");
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateCharacterRoundUIForClients(bool isRedWin)
    {
        if (isRedWin) RoundUI.instance.RedWin(); 
        else RoundUI.instance.BlueWin(); 
    } 
}
