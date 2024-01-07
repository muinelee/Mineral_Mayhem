using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cinemachine;

public class NetworkCameraEffectsManager : NetworkBehaviour
{
    public static NetworkCameraEffectsManager instance;

    [Header("Camera Component")]
    [SerializeField] private CinemachineVirtualCamera cam;

    [Header("Hit Effect Trigger")]
    [SerializeField] private int hitEffectThreshold;

    [Header("Hit Stop Properties")]
    [SerializeField] private float hitStopDuration;

    [Header("Screen Shake Properties")]
    [SerializeField] private float screenShakeDuration;
    [SerializeField] private float screenShakeIntensity;

    [Header("Time Slow Properties")]
    private bool isTimeSlowed = false;

    void Start()
    {
        instance = this;
        cam = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    public void CameraHitEffect(int damage)
    {
        if (damage < hitEffectThreshold) return;

        // Broadcast to all clients to hit stop and camera shake if damage is over a threshold
        RPC_CameraShake();
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

    #region <----- Screen Shake ----->
    public void ScreenShake()
    {
        StartCoroutine(ApplyScreenShake());
    }

    IEnumerator ApplyScreenShake()
    {
        // Access Virtual Camera properties
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = screenShakeIntensity;

        yield return new WaitForSecondsRealtime(screenShakeDuration);

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
    }
    #endregion

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_CameraShake(RpcInfo info = default)
    {
        NetworkCameraEffectsManager.instance.HitStop();
    }
}
