using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;

public class ReadyUpManager : MonoBehaviour
{
    public static ReadyUpManager instance { get; set; }

    [SerializeField] private Button readyUp;
    [SerializeField] private Button unReadyUp;
    [SerializeField] private TextMeshProUGUI readyUpText;
    [SerializeField] private Button startGame;

    [Header("Blue Team properties")]
    [SerializeField] private List<NetworkPlayer> blueTeamList;
    [SerializeField] private VerticalLayoutGroup blueTeamLayoutGroup;

    [Header("Red Team properties")]
    [SerializeField] private List<NetworkPlayer> redTeamList;
    [SerializeField] private VerticalLayoutGroup redTeamLayoutGroup;

    [Header("Undecided Team properties")]
    [SerializeField] private List<NetworkPlayer> undecidedTeamList;
    [SerializeField] private VerticalLayoutGroup undecidedTeamLayoutGroup;

    [Header("Item List Prefabs")]
    [SerializeField] private ReadyUpName playerTeamDisplayPF;

    [Header("Round Manager")]
    [SerializeField] private RoundManager roundManagerPF;

    // Keep track of which player is at team list
    private Dictionary<NetworkPlayer, ReadyUpName> playerTeamDisplayPair = new Dictionary<NetworkPlayer, ReadyUpName>();

    private void Awake()
    {
        instance = this;
    }

    public void OnStartGame()
    {
        if (!NetworkPlayer.Local.HasStateAuthority) return;

        //Check to see if there are enough players
        NetworkRunner runner = FindAnyObjectByType<NetworkRunner>();

        if (runner.SessionInfo.PlayerCount != runner.SessionInfo.MaxPlayers) return;

        if (blueTeamList.Count != redTeamList.Count) return;

        for (int i = 0; i < runner.SessionInfo.MaxPlayers / 2; i++)
        {
            if (!blueTeamList[i].isReady || !redTeamList[i].isReady) return;
        }

        runner.Spawn(roundManagerPF, Vector3.zero, Quaternion.identity);

        RoundManager.Instance.teammSize = blueTeamList.Count;
        RoundManager.Instance.OnResetRound();

        NetworkPlayer.Local.RPC_StartGame();
    }

    public void PrimeReadyUpUI(NetworkPlayer player)
    {
        if (player.HasStateAuthority) startGame.gameObject.SetActive(true);

        NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();
        foreach (NetworkPlayer netPlayer in players)
        {
            // Display team colors if players not ready
            if (netPlayer.team == NetworkPlayer.Team.Blue) JoinBlueTeam(netPlayer);
            else if (netPlayer.team == NetworkPlayer.Team.Red) JoinRedTeam(netPlayer);
            else if (netPlayer.team == NetworkPlayer.Team.Undecided && netPlayer != NetworkPlayer.Local) JoinUndecided(netPlayer);

            if (netPlayer.isReady)
            {
                playerTeamDisplayPair[netPlayer].GetComponent<Image>().color = Color.green;
                ReadyUp(netPlayer);
            }
        }
    }

    #region <----- Ready Up Functionality ----->

    public void OnReadyUp()
    {
        if (NetworkPlayer.Local.team == NetworkPlayer.Team.Undecided) return;

        NetworkPlayer.Local.RPC_ReadyUp();
        unReadyUp.gameObject.SetActive(true);
    }

    public void OnUnreadyUp()
    {
        NetworkPlayer.Local.RPC_UnReadyUp();
        unReadyUp.gameObject.SetActive(false);
    }

    public void ReadyUp(NetworkPlayer player)
    {
        if (!playerTeamDisplayPair.ContainsKey(player)) return;

        if (undecidedTeamList.Contains(player)) return;

        playerTeamDisplayPair[player].GetComponent<Image>().color = Color.green;

        player.isReady = true;
    }

    public void UnReadyUp(NetworkPlayer player)
    {
        if (player.team == NetworkPlayer.Team.Blue) playerTeamDisplayPair[player].GetComponent<Image>().color = Color.blue;
        else if (player.team == NetworkPlayer.Team.Red) playerTeamDisplayPair[player].GetComponent<Image>().color = Color.red;

        player.isReady = false;
    }

    #endregion

    #region <----- Join Functionality ----->

    public void OnJoinBlueTeam()
    {
        NetworkPlayer.Local.RPC_JoinBlueTeam();
        unReadyUp.gameObject.SetActive(false);
    }

    public void OnJoinRedTeam()
    {
        NetworkPlayer.Local.RPC_JoinRedTeam();
        unReadyUp.gameObject.SetActive(false);
    }

    public void JoinBlueTeam(NetworkPlayer player)
    {
        // Manage player in team lists
        if (blueTeamList.Contains(player)) return;
        if (redTeamList.Contains(player)) redTeamList.Remove(player);
        if (undecidedTeamList.Contains(player)) undecidedTeamList.Remove(player);

        player.isReady = false;

        // Create player name display
        ReadyUpName readyUpName = Instantiate(playerTeamDisplayPF, blueTeamLayoutGroup.transform);
        readyUpName.SetPlayer(player);

        readyUpName.GetComponent<Image>().color = Color.blue;

        // Destroy player display name if one is already present in the other team list
        if (playerTeamDisplayPair.ContainsKey(player)) Destroy(playerTeamDisplayPair[player].gameObject);
        playerTeamDisplayPair[player] = readyUpName;

        // Add player to the blue team
        blueTeamList.Add(player);
        player.team = NetworkPlayer.Team.Blue;
    }

    public void JoinRedTeam(NetworkPlayer player)
    {
        // Manager player in team lists
        if (redTeamList.Contains(player)) return;
        if (blueTeamList.Contains(player)) blueTeamList.Remove(player);
        if (undecidedTeamList.Contains(player)) undecidedTeamList.Remove(player);

        player.isReady = false;

        // Create player name display in UI
        ReadyUpName readyUpName = Instantiate(playerTeamDisplayPF, redTeamLayoutGroup.transform);
        readyUpName.SetPlayer(player);

        readyUpName.GetComponent<Image>().color = Color.red;

        // Destroy player display name if one is already present in the other team list
        if (playerTeamDisplayPair.ContainsKey(player)) Destroy(playerTeamDisplayPair[player].gameObject);
        playerTeamDisplayPair[player] = readyUpName;

        // Add playre to the red team
        redTeamList.Add(player);
        player.team = NetworkPlayer.Team.Red;
    }

    public void StartGame()
    {
        FindAnyObjectByType<CharacterSelect>().ActivateCharacterSelect();
        this.gameObject.SetActive(false);
    }

    public void JoinUndecided(NetworkPlayer player)
    {
        ReadyUpName readyUpName = Instantiate(playerTeamDisplayPF, undecidedTeamLayoutGroup.transform);
        readyUpName.SetPlayer(player);
        readyUpName.GetComponent<Image>().color = Color.grey;

        playerTeamDisplayPair[player] = readyUpName;

        undecidedTeamList.Add(player);
    }

    #endregion

    public void KickPlayer(NetworkPlayer player)
    {
        if (blueTeamList.Contains(player)) blueTeamList.Remove(player);
        if (redTeamList.Contains(player)) redTeamList.Remove(player);
        if (undecidedTeamList.Contains(player)) undecidedTeamList.Remove(player);

        NetworkRunner runner = FindAnyObjectByType<NetworkRunner>();
        PlayerRef playerAuthority = player.GetComponent<NetworkObject>().InputAuthority;

        NetworkPlayer.RemovePlayer(runner, playerAuthority);
        runner.Disconnect(playerAuthority);

        Destroy(playerTeamDisplayPair[player].gameObject);
        playerTeamDisplayPair.Remove(player);
    }

    public void PlayerLeft()
    {
        NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();

        foreach (NetworkPlayer player in players)
        {
            if (playerTeamDisplayPair.ContainsKey(player)) continue;

            // Destroy players not in game
            Destroy(playerTeamDisplayPair[player].gameObject);
            playerTeamDisplayPair.Remove(player);
        }
    }

    public int GetIndex(NetworkPlayer player)
    {
        if (player.team == NetworkPlayer.Team.Blue)
        {
            return blueTeamList.IndexOf(player);
        }
        else if (player.team == NetworkPlayer.Team.Red)
        {
            return redTeamList.IndexOf(player);
        }
        return 0;
    }
}