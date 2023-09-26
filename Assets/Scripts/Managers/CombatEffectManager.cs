using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CombatEffectManager : MonoBehaviour
{
    public static CombatEffectManager instance;
    
    [Header("Hit Stop Properties")]
    [SerializeField] private float hitStopDuration;

    [Header("Time Slow Properties")]
    [SerializeField] private float timeSlowDuration;
    private bool isTimeSlowed;

    [Header("Screen Shake Properties")]
    [SerializeField] private float screenShakeDuration;
    [SerializeField] private float screenShakeIntensity;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Awake() 
    {
        // Create Instance
        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else Destroy(gameObject);

        // Get Cinemachine Virtual Camera for Screen Shake
        cinemachineVirtualCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
    }

#region <----- Hit Stop ----->
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
            ScreenShake();
        }
    }
#endregion

#region <----- Time Slow ----->
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
#endregion

#region <----- Screen Shake ----->
    public void ScreenShake()
    {
        StartCoroutine(ApplyScreenShake());
    }

    IEnumerator ApplyScreenShake()
    {
        // Access Virtual Camera properties
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = screenShakeIntensity;

        yield return new WaitForSecondsRealtime(screenShakeDuration);

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
    }
#endregion
}