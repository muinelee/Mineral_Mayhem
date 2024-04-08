using UnityEngine;

public static class ServerInfo
{
    //  Can add more info as needed (for example, if we have more than one arena -> ArenaID)

    public const int UserCapacity = 8; //   Hard limit for photon, it seems

    public static string LobbyName;
    public static int GameMode
    {
        get => PlayerPrefs.GetInt("Server_GameMode", 0);
        set => PlayerPrefs.SetInt("Server_GameMode", value);
    }

    public static int ArenaID
    {
        get => PlayerPrefs.GetInt("Server_ArenaID", 0);
        set => PlayerPrefs.SetInt("Server_ArenaID", value);
    }

    public static int MaxUsers
    {
        get => PlayerPrefs.GetInt("Server_MaxUsers", 0);
        set => PlayerPrefs.SetInt("Server_MaxUsers", Mathf.Clamp(value, 1, UserCapacity));
    }
}
