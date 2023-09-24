using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPoint : MonoBehaviour
{
    /*
        Attach to camera follow point in character to find Cinemachine virtual camera and use as the follow target
    */

    private bool lookAheadMode = false;

    [Header("Camera Offset properties")]
    [SerializeField] private float maxCameraOffset;
    [SerializeField] private float cameraOffsetTransitionTime;

    public void ToggleLookAheadCam()
    {
        lookAheadMode = !lookAheadMode;
    }

    public void PassCursorPosition(Vector3 cursorPosition)
    {
        if (!lookAheadMode) transform.position = Vector3.Lerp(transform.position, transform.parent.position, 0.01f);

        else if (lookAheadMode)
        {
            if (Vector3.Distance(transform.parent.position, cursorPosition) < maxCameraOffset) transform.position = Vector3.Lerp(transform.position, cursorPosition, cameraOffsetTransitionTime);
            else transform.position = Vector3.Lerp(transform.position, transform.parent.position + (cursorPosition - transform.parent.position).normalized * maxCameraOffset, cameraOffsetTransitionTime);
        }
    }
}
