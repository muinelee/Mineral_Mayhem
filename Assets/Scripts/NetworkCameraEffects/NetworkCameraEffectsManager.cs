using System.Collections;
using UnityEngine;
using Fusion;
using Cinemachine;
using System.Linq;

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
    [SerializeField]  private float camIntervalTime = 5.0f;
    public CameraTrack[] blueCinematicPositions;
    public CameraTrack[] redCinematicPositions;
    private bool isCameraMoving = false; 

    private bool isRedTeam;

    [SerializeField] private float cinematicTimerDuration = 10;
    private TickTimer cinematicTimer = TickTimer.None;

    void Start()
    {
        instance = this;
        cam = Camera.main.GetComponentInChildren<CinemachineBrain>();
        cinematicTimerDuration = 0f;
        foreach(CameraTrack track in blueCinematicPositions)
        {
            cinematicTimerDuration += track.duration;
        }
    }

    public void CameraHitEffect(float damage)
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

        if (Runner.IsServer) RoundManager.Instance.OnResetRound();
    } 

    public void SetPlayerCamera(Transform player)
    {
        topCameraPriority.Follow = player;
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
        if (victoryCameraPriority.Priority > 0) return;

        ResetCameraPriorities();
        teamCameraPriority.Priority = 100;
    }

    public void GoToRedCinematicCamera()
    {
        ResetCameraPriorities();
        redCinematicCameraPriority.Priority = 100;

        ControlCamera(redCinematicCameraPriority, false); 
    }

    public void GoToBlueCinematicCamera()
    {
        ResetCameraPriorities();
        blueCinematicCameraPriority.Priority = 100;

        ControlCamera(blueCinematicCameraPriority, true); 
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

    public void ControlCamera(CinemachineVirtualCamera cam, bool blueTeam)
    {
        if (isCameraMoving) return;
       
        if ((blueTeam && blueCinematicPositions.Length < 1) || (!blueTeam && redCinematicPositions.Length < 1))
        {
            Debug.LogError("CameraTrack component not found on CinemachineVirtualCamera.");
            return;
        }

        if (blueTeam) StartCoroutine(MoveCamera(cam, blueCinematicPositions));
        else if (!blueTeam) StartCoroutine(MoveCamera(cam, redCinematicPositions));
    }

    private IEnumerator MoveCamera(CinemachineVirtualCamera cam, CameraTrack[] cameraTracks)
    {
        isCameraMoving = true;

        float time = 0f;
        int index = 0;
        bool trackComplete = false;
        while (!trackComplete)
        {
            float t = time / cameraTracks[index].duration;

            cam.transform.position = Vector3.Lerp(
                cameraTracks[index].startPoint.position,
                cameraTracks[index].endPoint.position,
                t);

            cam.transform.rotation = Quaternion.Slerp(
                cameraTracks[index].startPoint.rotation,
                cameraTracks[index].endPoint.rotation,
                t);

            time += Time.deltaTime;

            if (time >= cameraTracks[index].duration)
            {
                time = 0f;
                index++;
                trackComplete = index >= cameraTracks.Length;
            }

            yield return null;
        }

        cam.transform.position = cameraTracks[index-1].endPoint.position;
        cam.transform.rotation = cameraTracks[index-1].endPoint.rotation;

        isCameraMoving = false;
    }

    #endregion

    #region <----- Team Camera ----->

    private void SetTeamCamera()
    {
        if (Runner.SessionInfo.MaxPlayers <= 2) return;

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
