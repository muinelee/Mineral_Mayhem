using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Cinemachine;
using TMPro;

public class NetworkPlayer_WorldSpaceHUD : NetworkBehaviour
{
    private Transform playerTransform;
    private float yOffset;

    public Image nonLocalPlayerHealthBar;
    public TextMeshProUGUI playerName;
    [SerializeField] private NetworkPlayer_Health playerHealth;

    [SerializeField] TMP_FontAsset fontBlue;
    [SerializeField] TMP_FontAsset fontRed;
    [SerializeField] TMP_FontAsset fontWhite;

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
            RPC_SetPlayerName(NetworkPlayer.Local.playerName.ToString(), NetworkPlayer.Local.team);
        }

    }

    private void DisplayHUD()
    {
        CinemachineVirtualCamera cam = Camera.main.GetComponentInChildren<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera;
        // Update HUD position and values

        nonLocalPlayerHealthBar.fillAmount = playerHealth.HP / playerHealth.GetStartingHP();

        transform.LookAt(cam.transform.rotation * Vector3.forward + transform.position, cam.transform.rotation * Vector3.up);
    }

    public void ShowFloatingHealthBar()
    {
        nonLocalPlayerHealthBar.gameObject.SetActive(true);
    }

    public void HideFloatingHealthBar()
    {
        nonLocalPlayerHealthBar.gameObject.SetActive(false);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SetPlayerName(string name, NetworkPlayer.Team team)
    {
        this.playerName.text = name;
        if (team == NetworkPlayer.Team.Red) this.playerName.font = fontRed;
        else if (team == NetworkPlayer.Team.Blue) this.playerName.font = fontBlue;
        else this.playerName.font = fontWhite;
    }
}
