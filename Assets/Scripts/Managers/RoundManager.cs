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

    private void RedWinsRound()
    {
        Debug.Log($"Red wins round {currentRound}!"); 
    }

    private void BlueWinsRound()
    {
        Debug.Log($"Blue wins round {currentRound}!");
    }
    
    public void RedPlayersDies()
    {
        //OnPlayerDeath?.Invoke(player); 
        Debug.Log("Red player died!");
        redPlayersAlive--;  
        if (currentRound == 3) CheckMatchEnd();
        else CheckRoundEnd(); 
    }

    public void BluePlayersDies()
    {
        //OnPlayerDeath?.Invoke(player);   
        Debug.Log("Blue player died!");
        bluePlayersAlive--;
        if (currentRound == 3) CheckMatchEnd();
        else CheckRoundEnd();
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
    }
    private void CheckRoundEnd()
    {

    }

    public void CheckMatchEnd()
    {
        if (redRoundsWon > blueRoundsWon)
        {
            roundEndTimer = TickTimer.CreateFromSeconds(Runner, roundEndDuration);
            Debug.Log("Red Wins the game!"); 
        }
        else if (blueRoundsWon > redRoundsWon) 
        {
            roundEndTimer = TickTimer.CreateFromSeconds(Runner, roundEndDuration); 
            Debug.Log("Blue Wins the game!");
        }
        else
        {
            roundEndTimer = TickTimer.CreateFromSeconds(Runner, roundEndDuration);
            Debug.Log("Tie!");
        }
    }
}
