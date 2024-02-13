using UnityEngine;

public static class ClientInfo
{
    public static string Username
    {
        get => PlayerPrefs.GetString("Client_Username", string.Empty);
        set => PlayerPrefs.GetString("Client_Username", value);
    }

    public static int CharacterID
    {
        get => PlayerPrefs.GetInt("Client_CharacterID", 0);
        set => PlayerPrefs.GetInt("Client_CharacterID", value);
    }

    public static string LobbyName
    {
        get => PlayerPrefs.GetString("Client_LastLobbyName", "");
        set => PlayerPrefs.GetString("Client_LastLobbyName", value);
    }
}