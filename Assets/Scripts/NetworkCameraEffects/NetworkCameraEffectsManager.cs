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
    [SerializeField] private CinemachineVirtualCamera teamCameraPriority;

    [Header("Camera Tracking")]
    [SerializeField]  private int currentCamTrack = 0;
    [SerializeField]  private float camIntervalTime = 5.0f;
    private bool isCameraMoving = false; 
    //public CameraTrack[] camPositions; 

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

        if (Input.GetKeyDown(KeyCode.J))
        {
            GoToTeamCamera();
        }
    }

    void Start()
    {
        instance = this;
        cam = Camera.main.GetComponentInChildren<CinemachineBrain>();
    }

    public void CameraHitEffect(int damage)
    {
        if (damage < hitEffectThreshold || !Runner.IsServer) return;

        // Broadcast to all clients to hit stop and camera shake if damage is over a threshold
        RPC_CameraShake();
    }

    private void FixedUpdate()
    {
        if (!cinematicTimer.Expired(Runner)) return;

        cinematicTimer = TickTimer.None;
        SetTeamCamera();
        GoToTopCamera(); 
    } 

    #region <----- Camera Priority ----->
    public void GoToRedCamera()
    {
        ResetCameraPriorities();
        redCameraPriority.Priority = 100;
    }

    public void GoToBlueCamera()
    {
        ResetCameraPriorities();
        blueCameraPriority.Priority = 100;
    }

    public void GoToTopCamera()
    {
        ResetCameraPriorities();
        topCameraPriority.Priority = 100;
    }

    public void GoToVictoryCamera()
    {
        ResetCameraPriorities();
        victoryCameraPriority.Priority = 100;
    }

    public void GoToTeamCamera()
    {
        ResetCameraPriorities();
        teamCameraPriority.Priority = 100;
    }

    public void GoToRedCinematicCamera()
    {
        ResetCameraPriorities();
        redCinematicCameraPriority.Priority = 100;

        ControlCamera(redCinematicCameraPriority); 
    }

    public void GoToBlueCinematicCamera()
    {
        ResetCameraPriorities();
        blueCinematicCameraPriority.Priority = 100;

        ControlCamera(blueCinematicCameraPriority); 
    }

    private void ResetCameraPriorities()
    {
        redCameraPriority.Priority = 0;
        blueCameraPriority.Priority = 0;
        topCameraPriority.Priority = 0;
        victoryCameraPriority.Priority = 0;
        redCinematicCameraPriority.Priority = 0;
        blueCinematicCameraPriority.Priority = 0;
        teamCameraPriority.Priority = 0;
    }

    public void StartCinematic(NetworkPlayer player)
    {
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

    #region Camera Cinematic Tracking 

    public void ControlCamera(CinemachineVirtualCamera cam)
    {
        if (isCameraMoving) return;
       
        CameraTrack cameraTrack = cam.GetComponent<CameraTrack>();
        if (cameraTrack == null)
        {
            Debug.LogError("CameraTrack component not found on CinemachineVirtualCamera.");
            return;
        }

        StartCoroutine(MoveCamera(cam, cameraTrack));
    }

    private IEnumerator MoveCamera(CinemachineVirtualCamera cam, CameraTrack cameraTrack)
    {
        isCameraMoving = true;

        float elapsedTime = 0f;

        while (elapsedTime < cameraTrack.duration)
        {
            float t = elapsedTime / cameraTrack.duration;

            cam.transform.position = Vector3.Lerp(
                cameraTrack.startPoint.position,
                cameraTrack.endPoint.position,
                t);

            cam.transform.rotation = Quaternion.Slerp(
                cameraTrack.startPoint.rotation,
                cameraTrack.endPoint.rotation,
                t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = cameraTrack.endPoint.position;
        cam.transform.rotation = cameraTrack.endPoint.rotation;

        isCameraMoving = false;
    }

    #endregion

    #region <----- Team Camera ----->

    private void SetTeamCamera()
    {
        CharacterEntity[] characterEntities = FindObjectsOfType<CharacterEntity>();

        foreach (CharacterEntity character in characterEntities) 
        { 
            if (character.Team == NetworkPlayer.Local.team && !character.Object.HasInputAuthority)
            {
                teamCameraPriority.Follow = character.gameObject.transform;
            }
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
