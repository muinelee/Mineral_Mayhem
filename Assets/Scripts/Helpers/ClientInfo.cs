using UnityEngine;

public static class ClientInfo
{
    public static string Username
    {
        get => PlayerPrefs.GetString("Client_Username", "Player Name");
        set => PlayerPrefs.SetString("Client_Username", value);
    }

    public static int CharacterID
    {
        get => PlayerPrefs.GetInt("Client_CharacterID", 0);
        set => PlayerPrefs.SetInt("Client_CharacterID", value);
    }

    public static string LobbyName
    {
        get => PlayerPrefs.GetString("Client_LastLobbyName", "Session Name");
        set => PlayerPrefs.SetString("Client_LastLobbyName", value);
    }
}