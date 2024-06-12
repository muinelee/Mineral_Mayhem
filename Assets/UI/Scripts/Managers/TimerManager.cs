using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TimerManager : MonoBehaviour
{
    public static TimerManager instance;

    private float timer;

    [SerializeField] TextMeshProUGUI minute1;
    [SerializeField] TextMeshProUGUI minute2;
    [SerializeField] TextMeshProUGUI second1;
    [SerializeField] TextMeshProUGUI second2;

    private bool countUp = false;

    //-----------------------------------//

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Update()
    {
        if (countUp)
        {
            timer += Time.deltaTime;
        }

        UpdateTimerDisplay(timer);
    }

    public void StopTimer()
    {
        timer = 0f;
    }

    public void ResetTimer(float startTime)
    {
        timer = startTime;
        
        if (startTime == 0)
        {
            countUp = true;
        }
        else
            countUp= false;
    }

    private void UpdateTimerDisplay(float time)
    {

    }
}
