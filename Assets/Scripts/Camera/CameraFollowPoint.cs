using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPoint : MonoBehaviour
{
    /*
        Attach to camera follow point in character to find Cinemachine virtual camera and use as the follow target

        Network Player Script will make reference to this game object and remove it from the player
    */

    private bool lookAheadMode = false;
    [SerializeField] private Transform target;

    [Header("Camera Offset properties")]
    [SerializeField] private float maxCameraOffset;
    [SerializeField] private float cameraOffsetTransitionTime;
    public void ToggleLookAheadCam()
    {
        lookAheadMode = !lookAheadMode;
    }

    public void PassCursorPosition(Vector3 cursorPosition)
    {
        cursorPosition.y = 0;

        if (!lookAheadMode) transform.position = Vector3.Lerp(transform.position, target.position, cameraOffsetTransitionTime);

        else if (lookAheadMode)
        {
            if (Vector3.Distance(target.position, cursorPosition) < maxCameraOffset) transform.position = Vector3.Lerp(transform.position, cursorPosition, cameraOffsetTransitionTime);
            else transform.position = Vector3.Lerp(transform.position, target.position + ((cursorPosition - target.position).normalized * maxCameraOffset), cameraOffsetTransitionTime);
        }
    }

    public void SetTarget(Transform player)
    {
        target = player;
    }
}
