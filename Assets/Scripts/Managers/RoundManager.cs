using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
    public static RoundManager instance;

    public Player_Core player;
    private int maxRounds = 3;
    private int currentRound = 1;
    public MatchStartText matchStartText;

    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        StartRound();
    }

    void Update()
    {
        if (IsRoundOver())
        {
            EndRound();
        }
    }


    void StartRound()
    {
        Debug.Log("Start of Round " + currentRound);

        if (matchStartText != null)
        {
            matchStartText.DisplayMatchStartText("Round " + currentRound);
        }

        player.ResetHealth();
    }

    bool IsRoundOver()
    {
        return player != null && player.IsDead();
    }

    void EndRound()
    {
        Debug.Log("End of Round " + currentRound);

        if (player != null && player.IsDead())
        {
            Debug.Log("Player lost the round!");
        }
        else
        {
            Debug.Log("Player won the round!");
        }

        currentRound = currentRound + 1;

        if (currentRound > maxRounds)
        {
            Debug.Log("Game Over");
        }
        else
        {
            Invoke("StartRound", 2f);
            RestartGame();
        }
    }

    void RestartGame()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}