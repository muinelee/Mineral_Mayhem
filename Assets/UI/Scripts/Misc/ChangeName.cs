using TMPro;
using UnityEngine;

public class ChangeName : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI textPlayerName;

    //--------------------------------//

    private void Start()
    {
        textPlayerName.text = ClientInfo.Username;
    }

    public void UpdatePlayerName()
    {
        ClientInfo.Username = inputField.text;
        Debug.Log("Player Name: " + ClientInfo.Username);
    }
}
