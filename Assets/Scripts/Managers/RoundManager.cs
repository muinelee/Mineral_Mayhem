using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
    public Player_Core player;
    public int maxRounds = 3;
    private int currentRound = 1;

    void Start()
    {
        StartRound();
    }

    void Update()
    {
        // Check if the round is over
        if (IsRoundOver())
        {
            EndRound();
        }
    }

    void StartRound()
    {
        Debug.Log("Start of Round " + currentRound);
        // Reset player health at the beginning of each round
        player.ResetHealth();
    }

    bool IsRoundOver()
    {
        // Check if the player is eliminated
        return player != null && player.IsDead();
    }

    void EndRound()
    {
        Debug.Log("End of Round " + currentRound);

        // Display round results or winner
        if (player != null && player.IsDead())
        {
            Debug.Log("Player lost the round!");
        }
        else
        {
            Debug.Log("Player won the round!");
        }

        // Increment the round counter
        currentRound++;

        // Check if the game is over
        if (currentRound > maxRounds)
        {
            Debug.Log("Game Over");
            // Implement game over logic, e.g., show winner, restart game, etc.
            RestartGame();
        }
        else
        {
            // Start the next round after a delay
            Invoke("StartRound", 2f);
        }
    }

    void RestartGame()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}

