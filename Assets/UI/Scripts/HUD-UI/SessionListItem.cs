using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using System.Runtime.CompilerServices;
using System;

public class SessionListItem : MonoBehaviour
{
    public TextMeshProUGUI sessionNameText;
    public TextMeshProUGUI playerCountText;
    public Button joinButton;

    private SessionInfo sessionInfo;

    public event Action<SessionInfo> OnJoinSession;

    public void SetInformation(SessionInfo sessionInfo)
    {
        this.sessionInfo = sessionInfo;

        sessionNameText.text = sessionInfo.Name;
        playerCountText.text = $"{sessionInfo.PlayerCount.ToString()}/{sessionInfo.MaxPlayers.ToString()}";

        if (sessionInfo.PlayerCount >= sessionInfo.MaxPlayers) joinButton.gameObject.SetActive(false);
        else joinButton.gameObject.SetActive(true);
    }

    public void OnClick()
    {
        OnJoinSession?.Invoke(sessionInfo);
    }
}