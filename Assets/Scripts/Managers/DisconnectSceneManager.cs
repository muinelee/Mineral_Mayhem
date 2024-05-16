using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisconnectSceneManager : MonoBehaviour
{
    [SerializeField] private float timeToReturnToLobby;
    private float timer = 0;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > timeToReturnToLobby) ReturnToLobby();
    }

    public void ReturnToLobby()
    {
        SceneManager.LoadScene(0);
    }
}
