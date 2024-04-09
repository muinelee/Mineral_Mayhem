using UnityEngine;

public class NetworkInfoManager : MonoBehaviour
{
    // start instance
    public static NetworkInfoManager instance;

    byte[] connectionToken;

    private void Awake()
    {   
        if (connectionToken == null) SetConnectionToken(ConnectionTokenUtils.NewToken());
        Debug.Log($"Player connection token {ConnectionTokenUtils.HashToken(connectionToken)}");
    }

    public void SetConnectionToken(byte[] newConnectionToken)
    {
        connectionToken = newConnectionToken;
    }

    public byte[] GetConnectionToken()
    {
        return connectionToken;
    }
}