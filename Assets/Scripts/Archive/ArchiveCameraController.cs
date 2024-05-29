using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class ArchiveCameraController : MonoBehaviour
{
    public static ArchiveCameraController Instance => Singleton<ArchiveCameraController>.Instance;

    public enum ArchiveCameraState
    {
        GameView,
        FirstPerson,
    }

    private ArchiveCameraState state;
    public ArchiveCameraState State
    {
        get { return state; }
        set
        {
            switch(value)
            {
                case ArchiveCameraState.GameView:
                    gameViewCamera.Priority = 10;
                    firstPersonViewCamera.Priority = 1;
                    break;
                case ArchiveCameraState.FirstPerson:
                    gameViewCamera.Priority = 1;
                    firstPersonViewCamera.Priority = 10;
                    break;
            }
            OnCameraStateChanged?.Invoke(value);
            state = value;
            return;
        }
    }

    public Action<ArchiveCameraState> OnCameraStateChanged;

    [Header("Virtual Cameras")]
    public CinemachineVirtualCamera gameViewCamera;
    public CinemachineVirtualCamera firstPersonViewCamera;

    private void Update()
    {
        CheckForFKeyPress();
    }

    private void CheckForFKeyPress()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            State = (State == ArchiveCameraState.GameView) ? ArchiveCameraState.FirstPerson : ArchiveCameraState.GameView;
        }
    }
}
