using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cinemachine;

public class NetworkCameraEffectsManager : NetworkBehaviour
{
    public static NetworkCameraEffectsManager instance;

    [Header("Camera Component")]
    [SerializeField] private CinemachineBrain cam;

    [Header("Hit Effect Trigger")]
    [SerializeField] private int hitEffectThreshold;

    [Header("Hit Stop Properties")]
    [SerializeField] private float hitStopDuration;

    [Header("Screen Shake Properties")]
    [SerializeField] private float screenShakeDuration;
    [SerializeField] private float screenShakeIntensity;

    [Header("Time Slow Properties")]
    private bool isTimeSlowed = false;

    [Header("Camera Priority")]
    [SerializeField] private CinemachineVirtualCamera redCameraPriority;
    [SerializeField] private CinemachineVirtualCamera blueCameraPriority;
    [SerializeField] private CinemachineVirtualCamera topCameraPriority;
    [SerializeField] private CinemachineVirtualCamera victoryCameraPriority;
    [SerializeField] private CinemachineVirtualCamera redCinematicCameraPriority;
    [SerializeField] private CinemachineVirtualCamera blueCinematicCameraPriority;

    private bool isRedTeam;

    [SerializeField] private float cinematicTimerDuration = 10;
    private TickTimer cinematicTimer = TickTimer.None;

    // Update Method is for testing. Remove/Move/Replace when done and logic for player's team has been implemented
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            // Go to Team Red Camera
            RPC_CameraPriority(isRedTeam = true);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            // Go to Team Blue Camera
            RPC_CameraPriority(isRedTeam = false);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            // Go to Player Camera (Top-Down View)
            GoToTopCamera();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Go to Player Camera (Top-Down View)
            GoToVictoryCamera();
        }
    }

    void Start()
    {
        instance = this;
        cam = Camera.main.GetComponentInChildren<CinemachineBrain>();
    }

    public void CameraHitEffect(int damage)
    {
        if (damage < hitEffectThreshold) return;

        // Broadcast to all clients to hit stop and camera shake if damage is over a threshold
        RPC_CameraShake();
    }

    private void FixedUpdate()
    {
        if (!cinematicTimer.Expired(Runner)) return;

        cinematicTimer = TickTimer.None;
        GoToTopCamera(); 
    } 

    #region <----- Camera Priority ----->
    public void GoToRedCamera()
    {
        redCameraPriority.Priority = 100;
        blueCameraPriority.Priority = 10;
        topCameraPriority.Priority = 10;
        victoryCameraPriority.Priority = 10;
    }

    public void GoToBlueCamera()
    {
        redCameraPriority.Priority = 10;
        blueCameraPriority.Priority = 100;
        topCameraPriority.Priority = 10;
        victoryCameraPriority.Priority = 10;
    }

    public void GoToTopCamera()
    {
        redCameraPriority.Priority = 10;
        blueCameraPriority.Priority = 10;
        topCameraPriority.Priority = 100;
        victoryCameraPriority.Priority = 10;
    }

    public void GoToVictoryCamera()
    {
        redCameraPriority.Priority = 10;
        blueCameraPriority.Priority = 10;
        topCameraPriority.Priority = 10;
        victoryCameraPriority.Priority = 100;
    }

    public void GoToRedCinematicCamera()
    {
        redCameraPriority.Priority = 0;
        blueCameraPriority.Priority = 0;
        topCameraPriority.Priority = 0;
        victoryCameraPriority.Priority = 0;
        blueCinematicCameraPriority.Priority = 0;  

        redCinematicCameraPriority.Priority = 100;
    }

    public void GoToBlueCinematicCamera()
    {
        redCameraPriority.Priority = 0;
        blueCameraPriority.Priority = 0;
        topCameraPriority.Priority = 0;
        victoryCameraPriority.Priority = 0;
        redCinematicCameraPriority.Priority = 0;

        blueCinematicCameraPriority.Priority = 100; 
    }

    public void StartCinematic(NetworkPlayer player)
    {
        Debug.Log("Checking for player's team"); 
        if (player.team == NetworkPlayer.Team.Red)
        {
            GoToRedCinematicCamera();
        }
        else if (player.team == NetworkPlayer.Team.Blue)
        {
            GoToBlueCinematicCamera();
        }
        cinematicTimer = TickTimer.CreateFromSeconds(Runner, cinematicTimerDuration);  
    } 

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_CameraPriority(bool isRedTeam)
    {
        if (isRedTeam) GoToRedCamera();
        else GoToBlueCamera();
    }
    #endregion

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
        CinemachineVirtualCamera virtualCam = cam.ActiveVirtualCamera as CinemachineVirtualCamera;
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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
