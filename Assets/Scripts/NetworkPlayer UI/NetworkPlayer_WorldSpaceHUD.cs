using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class NetworkPlayer_WorldSpaceHUD : NetworkBehaviour
{
    private Transform playerTransform;
    private float yOffset;

    [SerializeField] private Slider nonLocalPlayerHealthBar;
    [SerializeField] private NetworkPlayer_Health playerHealth;

    public override void Spawned()
    {
        PrimeUI();
    }

    public override void FixedUpdateNetwork()
    {
        DisplayHUD(); 
    }

    private void PrimeUI()
    {
        // Set proper canvas rotation
        Camera cam = Camera.main;
        transform.rotation = cam.transform.rotation;
        
        // Detach from parent to prevent HUD from rotating with gameObject
        playerTransform = transform.parent.transform;
        yOffset = transform.localPosition.y;
        transform.parent = null;

        // Set Floating HealthBar properties
        if (Object.HasInputAuthority) nonLocalPlayerHealthBar.gameObject.SetActive(false);
        else nonLocalPlayerHealthBar.gameObject.SetActive(true);
    }

    private void DisplayHUD()
    {
        // Update HUD position and values
        transform.position = playerTransform.position + Vector3.up * yOffset;

        nonLocalPlayerHealthBar.value = playerHealth.HP / playerHealth.GetStartingHP();
    }
}
