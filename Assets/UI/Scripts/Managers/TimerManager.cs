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
        ResetTimer(0);
    }
    private void Update()
    {
        if (timerOn)
        {
            if (countUp && timer >= 0)
            {
                timer += Time.deltaTime;
            }
            else timer -= Time.deltaTime;

            UpdateTimerDisplay(timer);
        }
    }

    public void StopTimer()
    {
        timerOn = false;
        timer = 0f;

        UpdateTimerDisplay(timer);
    }

    public void ResetTimer(float startTime)
    {
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
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);

        string timeDisplay = string.Format("{00:00}{1:00}", minutes, seconds);
        minute1.text = timeDisplay[0].ToString();
        minute2.text = timeDisplay[0].ToString();
        second1.text = timeDisplay[0].ToString();
        second2.text = timeDisplay[0].ToString();
        Debug.Log(minutes + ":" + seconds);
    }
}
