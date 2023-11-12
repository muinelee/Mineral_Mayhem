using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Entities.UI;


//add a timer to the game
public class UIManager : MonoBehaviour
{
    public int amountKilled;
    public float timeRemaining;
    public bool timerIsRunning = false;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI amountKilledText;

    //cooldown stuff used before, may use it again so commenting it out for now
    //public Image cooldownRadius;
    //public Text cooldownText;
    //public float cooldownTimer = 0.0f;
    //public float minValue = 0.0f;
    //public float maxValue = 10.0f;

    //cooldown stuff
    public Image imageCooldown;
    public Text cooldownText;
    public bool isCooldown = false;
    public float cooldownTimer = 0.0f;
    //cooldown time comes from player_attackController script in the function

    //testing purposes

    private void Start()
    {
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
        //get amountKilled from GameManager prefab
        amountKilled = FindObjectOfType<GameManager>().amountKilled;
        //display the amountKilled
        amountKilledText.text = "You killed " + amountKilled + " enemies!";

    }

    /*public void DisplayESpellCooldown(float eCooldownTime) { commenting out for now, may use later keep for now
        //display the cooldown
        //normalize the cooldown time, clamp it between 0 - 1 so it stops messing up the fill amount
        //not working currently, ignore for now
        //float normalizedRadius = Mathf.Clamp01((eCooldownTime - minValue) / (maxValue - minValue));
        cooldownRadius.fillAmount = eCooldownTime;

        cooldownText.text = eCooldownTime.ToString("F2");
    }*/

    public void DisplayESpellCooldown(float eCooldownTime) {
        //display cooldown, and count back to 0
        cooldownTimer += Time.deltaTime;
        cooldownTimer = Mathf.Clamp(cooldownTimer, 0.0f, eCooldownTime);
        cooldownText.text = cooldownTimer.ToString("F2");




        imageCooldown.fillAmount = eCooldownTime;
    }
}