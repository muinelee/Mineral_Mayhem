using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


//add a timer to the game
public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public int amountKilled;
    public float timeRemaining;
    public bool timerIsRunning = false;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI amountKilledText;

    //testing purposes

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // Starts the timer automatically
        timerIsRunning = true;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                //decrease the time
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
                DisplayNPCKilled();
            }
            if (timeRemaining <= 0)
            {
                SceneManager.LoadScene("GameOver");  //works :)
                //stop the timer
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }

    //display the time
    void DisplayTime(float timeToDisplay)
    {
        //convert the time to minutes and seconds
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        //display the time
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void DisplayNPCKilled()
    {
        //get amountKilled from GameManager
        amountKilled = FindObjectOfType<GameManager>().amountKilled;
        //display the amountKilled
        amountKilledText.text = "You killed " + amountKilled + " enemies!";

    }
}