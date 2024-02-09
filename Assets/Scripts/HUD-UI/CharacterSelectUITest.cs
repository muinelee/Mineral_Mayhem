using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class CharacterSelectUITest : NetworkBehaviour
{
    [SerializeField] private Button startGameButton;

    // Start is called before the first frame update
    void Start()
    {
        if (Object.HasStateAuthority) startGameButton.gameObject.SetActive(false);
    }

    public void OnReadyUp()
    {
        NetworkPlayer.Local.isReady = true;
        Debug.Log("Player is ready: " + NetworkPlayer.Local.isReady);
    }
}
