using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchiveController : MonoBehaviour
{
    private float movementSpeed = 1f;
    private bool FPSMode = false;

    private void Start()
    {
        if (ArchiveCameraController.Instance)
        {
            ArchiveCameraController.Instance.OnCameraStateChanged += ChangeControlScheme;
            ArchiveCameraController.Instance.State = ArchiveCameraController.ArchiveCameraState.GameView;
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (FPSMode) transform.position += transform.forward * movementSpeed * Time.deltaTime;
            else transform.position += Vector3.forward * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (FPSMode) transform.position -= transform.forward * movementSpeed * Time.deltaTime;
            else transform.position -= Vector3.forward * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (FPSMode) transform.position += transform.right * movementSpeed * Time.deltaTime;
            else transform.position += Vector3.right * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (FPSMode) transform.position -= transform.right * movementSpeed * Time.deltaTime;
            else transform.position -= Vector3.right * movementSpeed * Time.deltaTime;
        }
    }

    public void SetControllerSpeed(float speed)
    {
        movementSpeed = speed;
    }

    public void ChangeControlScheme(ArchiveCameraController.ArchiveCameraState state)
    {
        FPSMode = state == ArchiveCameraController.ArchiveCameraState.FirstPerson;
        if (!FPSMode)
        {
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
            transform.rotation = Quaternion.identity;
        }
    }
}
