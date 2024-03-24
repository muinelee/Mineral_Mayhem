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
    private int redPlayers;
    private int bluePlayers;

    private int currentRound = 1;    
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
    
    public void RedPlayersDies(NetworkPlayer player)
    {
        //OnPlayerDeath?.Invoke(player); 
        Debug.Log("Red player died!");
    }

    public void BluePlayersDies(NetworkPlayer player)
    {
        //OnPlayerDeath?.Invoke(player);   
        Debug.Log("Blue player died!");
    }

    public void LoadNextRound()
    {
        // Resetting Timer
        roundStartTimer = TickTimer.None; 
        roundStartTimer = TickTimer.CreateFromSeconds(Runner, roundEndDuration);
        // Spawning/Setting players back into their own positions 
        // Resetting health and lives 
        // 10 second wait time for game to start for doors to open OR input to be enabled 
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
