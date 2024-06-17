using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TimerManager : MonoBehaviour
{
    public static TimerManager instance;

    public float timer;

    [SerializeField] TextMeshProUGUI minute1;
    [SerializeField] TextMeshProUGUI minute2;
    [SerializeField] TextMeshProUGUI second1;
    [SerializeField] TextMeshProUGUI second2;

    private bool countUp = false;
    [SerializeField] private bool timerOn = false;
    private bool timerEnded = false;

    //-----------------------------------//

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        //ResetTimer(0);
    }
    private void Update()
    {
        if (timerOn)
        {
            if (!timerEnded && timer <= 0 && countUp == false)
            {
                timerEnded = true;
                StopTimer(true);
                return;
            }

            if (countUp && timer >= 0)
            {
                timer += Time.deltaTime;
            }
            else timer -= Time.deltaTime;

            UpdateTimerDisplay(timer);
        }
    }

    public void StopTimer(bool setZero)
    {
        timerOn = false;
        if (setZero) timer = 0f;

        UpdateTimerDisplay(timer);
    }

    public void ResetTimer(float startTime)
    {
        timerEnded = false;
        timerOn = true;

        timer = startTime;

        UpdateTimerDisplay(timer);
        
        if (startTime == 0)
        {
            countUp = true;
        }
        else countUp= false;
    }

    private void UpdateTimerDisplay(float time)
    {
        if (gameObject.activeSelf == true)
        {
            float minutes = Mathf.FloorToInt(time / 60);
            float seconds = Mathf.FloorToInt(time % 60);

            string timeDisplay = string.Format("{00:00}{1:00}", minutes, seconds);
            minute1.text = timeDisplay[0].ToString();
            minute2.text = timeDisplay[1].ToString();
            second1.text = timeDisplay[2].ToString();
            second2.text = timeDisplay[3].ToString();
            if (minutes < 10)
            {
                minute1.gameObject.SetActive(false);
            }
            else
            {
                minute1.gameObject.SetActive(true);
            }
        }
    }
}
