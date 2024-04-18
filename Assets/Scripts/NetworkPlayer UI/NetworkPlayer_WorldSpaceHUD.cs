using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Cinemachine;
using TMPro;

public class NetworkPlayer_WorldSpaceHUD : NetworkBehaviour
{
    private Transform playerTransform;
    private float yOffset;

    public Slider nonLocalPlayerHealthBar;
    public TextMeshProUGUI playerName;
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
        CinemachineVirtualCamera cam = Camera.main.GetComponentInChildren<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera;
        transform.rotation = cam.transform.rotation;
        
        // Detach from parent to prevent HUD from rotating with gameObject
        /*playerTransform = transform.parent.transform;
        yOffset = transform.localPosition.y;
        transform.SetParent(null);*/

        // Set Floating HealthBar properties
        if (Object.HasInputAuthority)
        {
            nonLocalPlayerHealthBar.gameObject.SetActive(false);
            RPC_SetPlayerName(NetworkPlayer.Local.playerName.ToString());
        }
        else nonLocalPlayerHealthBar.gameObject.SetActive(true);

    }

    private void DisplayHUD()
    {
        CinemachineVirtualCamera cam = Camera.main.GetComponentInChildren<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera;
        // Update HUD position and values

        nonLocalPlayerHealthBar.value = playerHealth.HP / playerHealth.GetStartingHP();

        transform.LookAt(cam.transform.rotation * Vector3.forward + transform.position, cam.transform.rotation * Vector3.up);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SetPlayerName(string name)
    {
        this.playerName.text = name;
    }
}
