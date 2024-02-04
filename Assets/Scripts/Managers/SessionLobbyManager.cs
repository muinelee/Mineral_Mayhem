using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class SessionLobbyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private SessionListItem sessionListItemPF;
    [SerializeField] private VerticalLayoutGroup sessionList;

    public void ClearList()
    {
        // Clear
        foreach (Transform sessionListItem in sessionList.transform)
        {
            Destroy(sessionListItem.gameObject);
        }

        // Hide the status text
        statusText.gameObject.SetActive(false);
    }

    public void AddToList(SessionInfo sessionInfo)
    {
        // Add a new item to the list
        SessionListItem addedSessionListItem = Instantiate(sessionListItemPF, sessionList.transform).GetComponent<SessionListItem>();

        addedSessionListItem.SetInformation(sessionInfo);

        addedSessionListItem.OnJoinSession += AddedSessionListItem_OnJoinSession;
    }

    private void AddedSessionListItem_OnJoinSession(SessionInfo obj)
    {
        Debug.Log("Added session to lobby list");
    }

    public void OnNoSessionsFound()
    {
        statusText.gameObject.SetActive(true);
        statusText.text = "No sessions found";
    }

    public void OnLookingForSession()
    {
        statusText.gameObject.SetActive(true);
        statusText.text = "Looking for gaem sessions";
    }
}
