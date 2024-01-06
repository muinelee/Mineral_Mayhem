using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;

public class NetworkPlayer_PlayerNameUI : MonoBehaviour
{
    private Camera cam;
    private Transform playerTransform;
    private float yOffset;

    private void Start()
    {
        PrimeUI();
    }

    private void FixedUpdate()
    {
        MovePlayerNameUI();
    }

    private void PrimeUI()
    {
        cam = Camera.main;
        playerTransform = transform.parent.transform;
        yOffset = transform.localPosition.y;
        transform.parent = null;
    }

    private void MovePlayerNameUI()
    {
        Vector3 targetPosition = playerTransform.position + Vector3.up * yOffset;
        transform.position = targetPosition;
        transform.rotation = cam.transform.rotation;
    }
}
