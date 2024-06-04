using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using Unity.VisualScripting;

public class IntroManager : MonoBehaviour
{
    private VideoPlayer videoPlayer; 
    private float holdTime = 2.0f; 
    private float currentHoldTime = 0.0f;
    [SerializeField] private Slider holdTimeSlider; 

    private void Start()
    {
        videoPlayer = GameObject.Find("IntroVideo").GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += EndVideo;

        if (holdTimeSlider != null)
        {
            holdTimeSlider.minValue = 0;
            holdTimeSlider.maxValue = 1; 
        }
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

        float holdTimePercentage = Mathf.Clamp01(currentHoldTime / holdTime);

        if (holdTimeSlider != null) holdTimeSlider.value = holdTimePercentage;
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
