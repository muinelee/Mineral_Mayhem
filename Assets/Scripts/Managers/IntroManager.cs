using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class IntroManager : MonoBehaviour
{
    private VideoPlayer videoPlayer; 
    private float holdTime = 3.0f; 
    private float currentHoldTime = 0.0f;

    private void Start()
    {
        videoPlayer = GameObject.Find("IntroVideo").GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += EndVideo; 
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            currentHoldTime += Time.deltaTime;
        }
        else
        {
            currentHoldTime = 0.0f;
        }

        if (currentHoldTime >= holdTime) EndVideo(videoPlayer); 
    }

    private void EndVideo(VideoPlayer vp)
    {
        SceneManager.LoadScene("Main Menu"); 
    }

    private void OnDestroy()
    {
        videoPlayer.loopPointReached -= EndVideo; 
    }
}
