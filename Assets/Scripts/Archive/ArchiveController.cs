using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchiveController : MonoBehaviour
{
    private float movementSpeed = 1f;
    private bool FPSMode = false;

    private float mouseSensitivity = 200f;
    private float pitch = 0f;
    private float yaw = 0f;


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
        HandleRotation();
        HandleMovement();
    }

    public void SetControllerSpeed(float speed)
    {
        movementSpeed = speed;
    }

    public void SetFPSensitivity(float value)
    {
        mouseSensitivity = value;
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

    private void HandleRotation()
    {
        if (!FPSMode) return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;

        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }

    private void HandleMovement()
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
}
