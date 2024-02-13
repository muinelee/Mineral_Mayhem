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
    private static UIManager instance;
    public int amountKilled;
    public float timeRemaining;
    public bool timerIsRunning = false;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI amountKilledText;

    //cooldown stuff
    public Image EimageCooldown;
    public Image QimageCooldown;
    public Text EcooldownText;
    public Text QcooldownText;
    //cooldown time comes from player_attackController script in the function

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
                spellCooldown();
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

    public void spellCooldown()
    {
        //find the player
        GameObject player = GameObject.FindGameObjectWithTag("Participant");

        if (player != null)
        {
            //reference the player attack controller
            Player_AttackController playerAttackController = player.GetComponent<Player_AttackController>();
            //find spell cooldown
            float eSpellCooldownTimer = playerAttackController.eAttackTimer;
            float eSpellCooldown = playerAttackController.eAttack.coolDown;
            float qSpellCooldown = playerAttackController.qAttackTimer;
            float qSpellCooldownTimer = playerAttackController.qAttack.coolDown;
            //send cooldown to uimanager
            DisplayESpellCooldown(eSpellCooldown, eSpellCooldownTimer);
            DisplayQSpellCooldown(qSpellCooldown, qSpellCooldownTimer);

            if (player == null)
            {
                Debug.Log("Player not found");
            }
        }
    }
    public void DisplayESpellCooldown(float eSpellCooldown, float eSpellCooldownTimer)
    {
        //display the cooldown
        EcooldownText.text = (eSpellCooldown - eSpellCooldownTimer).ToString("F2");
        if (eSpellCooldown - eSpellCooldownTimer <= 0)
        {
            EcooldownText.text = "";
            EimageCooldown.fillAmount = 0;
        }
        else
        {
            EimageCooldown.fillAmount = (eSpellCooldown - eSpellCooldownTimer) / eSpellCooldown;
        }
    }

    public void DisplayQSpellCooldown(float qSpellCooldown, float qSpellCooldownTimer)
    {
        //display the cooldown
        QcooldownText.text = (qSpellCooldown - qSpellCooldownTimer).ToString("F2");
        if (qSpellCooldown - qSpellCooldownTimer <= 0)
        {
            QcooldownText.text = "";
            QimageCooldown.fillAmount = 0;
        }
        else
        {
            QimageCooldown.fillAmount = (qSpellCooldown - qSpellCooldownTimer) / qSpellCooldown;
        }
    }
}