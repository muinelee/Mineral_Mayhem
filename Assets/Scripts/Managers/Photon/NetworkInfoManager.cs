using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Called the GameManager in the networking series in the research channel for the capstone

    First mentioned in the 'Host Migration' video
 */

public class NetworkInfoManager : MonoBehaviour
{
    // start instance
    public static NetworkInfoManager instance;

    byte[] connectionToken;

    private void Awake()
    {
        CreateInstance();
        if (connectionToken == null) SetConnectionToken(ConnectionTokenUtils.NewToken());
    }

    public void SetConnectionToken(byte[] newConnectionToken)
    {
        connectionToken = newConnectionToken;
    }

    public byte[] GetConnectionToken()
    {
        return connectionToken;
    }

    private void CreateInstance()
    {
        if (!instance) instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}