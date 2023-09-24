using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEffectManager : MonoBehaviour
{
    public static CombatEffectManager instance;

    private void Awake() 
    {
        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else Destroy(gameObject);
    }

    [Header("Effect Durations")]

    [SerializeField] private float hitStopDuration;
    [SerializeField] private float timeSlowDuration;
    private bool isTimeSlowed;

    public void HitStop()
    {
        StartCoroutine(ApplyHitStop());
    }

    IEnumerator ApplyHitStop()
    {
        if (!isTimeSlowed)
        {
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(hitStopDuration);
            Time.timeScale = 1;
        }
    }

    public void TimeSlow()
    {
        StartCoroutine(ApplyTimeSlow());
    }

    IEnumerator ApplyTimeSlow()
    {
        Time.timeScale = 0.3f;
        isTimeSlowed = true;
        yield return new WaitForSecondsRealtime(timeSlowDuration);
        isTimeSlowed = false;
        Time.timeScale = 1;
    }
}
