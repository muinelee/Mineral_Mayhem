using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RoomScreenUI : MonoBehaviour, IForceDisabledElement
{
    public Button readyUp;

    private static bool IsSubscribed;

    private void Awake()
    {
        //  GameManager.OnLobbyDetailsUpdated += UpdateDetails;     =>       Check UpdateDetails Below 

        // Can subscribe to NetworkPlayer.OnPlayerChanged when more relevant UI is implemented
    }

    void UpdateDetails(GameManager manager)
    {
        //  When any lobby info gets changed, this will get called automatically -> Therefore, add to it when relevant UI is implemented
    }

    public void Setup()
    {
        if (IsSubscribed) return;

        NetworkPlayer.OnPlayerJoined += AddPlayer;
        NetworkPlayer.OnPlayerLeave += RemovePlayer;
        NetworkPlayer.OnPlayerChanged += EnsurePlayersReady;

        readyUp.onClick.AddListener(ReadyUpListener);

        IsSubscribed = true;
    }

    public void OnDestruction()
    {
    }

    private void OnDestroy()
    {
        if (!IsSubscribed) return;

        NetworkPlayer.OnPlayerJoined -= AddPlayer;
        NetworkPlayer.OnPlayerLeave -= RemovePlayer;
        NetworkPlayer.OnPlayerChanged -= EnsurePlayersReady;

        IsSubscribed = false;
    }

    private void AddPlayer(NetworkPlayer player)
    {
        // Subscribe UI elements regarding to specific player here
    }

    private void RemovePlayer(NetworkPlayer player)
    {
        // Unsubscribe UI elements to specific player here
    }

    private void ReadyUpListener()
    {
        var local = NetworkPlayer.Local;
        Debug.Log($"local name: {local.name}");
        if (local && local.Object && local.Object.IsValid)
        {
            Debug.Log("About to ready up");
            local.RPC_ChangeReadyState(!local.IsReady);
        }
    }

    private void EnsurePlayersReady(NetworkPlayer player)
    {
        if (!NetworkPlayer.Local.IsLeader) return;

        Debug.LogWarning("Checking if all players are ready");

        if (IsAllReady())
        {
            Debug.LogWarning("We're doin it!");
            int scene = ResourceManager.Instance.arenas[GameManager.Instance.ArenaID].buildIndex;
            LevelManager.LoadScene(scene);
        }
    }

    private static bool IsAllReady() => NetworkPlayer.Players.Count > 0 && NetworkPlayer.Players.All(player => player.IsReady);
}
