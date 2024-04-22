using TMPro;
using UnityEngine;

public class ChangeName : MonoBehaviour
{
    [SerializeField] TMP_InputField inputFieldPlayerName;
    [SerializeField] TextMeshProUGUI textPlayerName;

    [SerializeField] TMP_InputField inputFieldSessionName;
    [SerializeField] TextMeshProUGUI textSessionName;

    //--------------------------------//

    private void Start()
    {
        textPlayerName.text = ClientInfo.Username;
        textSessionName.text = ClientInfo.LobbyName;
    }

    public void UpdatePlayerName()
    {
        ClientInfo.Username = inputFieldPlayerName.text;
        PlayerPrefs.SetString("PlayerName", ClientInfo.Username);
        Debug.Log("Player Name: " + ClientInfo.Username);
    }

    public void UpdateSessionName()
    {
        ClientInfo.LobbyName = inputFieldSessionName.text;
        PlayerPrefs.SetString("SessionName", ClientInfo.LobbyName);
        Debug.Log("Session Name: " + ClientInfo.LobbyName);
    }
}
