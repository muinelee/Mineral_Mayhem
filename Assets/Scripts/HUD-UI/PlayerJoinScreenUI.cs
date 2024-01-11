using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerJoinScreenUI : MonoBehaviour
{
    public TMP_InputField playerName;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerName")) playerName.text = PlayerPrefs.GetString("PlayerName");
    }

    public void GoToGameScene()
    {
        if (playerName.text == null)
        {
            Debug.Log("A player name needs to be entered first");
            return;
        }

        PlayerPrefs.SetString("PlayerName", playerName.text);

        SceneManager.LoadScene("RichardCPhoton");
    }
}