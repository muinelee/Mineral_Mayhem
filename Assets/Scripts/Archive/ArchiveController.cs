using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ArchiveUIOverlay;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.InputSystem.XR;

public class ArchiveController : MonoBehaviour
{
    private float movementSpeed = 15f;
    private bool FPSMode = false;

    private float mouseSensitivity = 200f;
    [SerializeField] private float rotationSpeed = 10f;
    private float pitch = 0f;
    private float yaw = 0f;

    private Quaternion targetRotation;

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
        GetComponentInChildren<MeshRenderer>().enabled = !FPSMode;
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

        targetRotation = Quaternion.Euler(pitch, yaw, 0f);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
        if (FPSMode)
        {
            if (Input.GetKey(KeyCode.Q)) transform.position += Vector3.up * movementSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.E)) transform.position -= Vector3.up * movementSpeed * Time.deltaTime;
        }
    }
}
