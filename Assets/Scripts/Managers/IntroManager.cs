using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private float holdTime = 2.0f;
    private float currentHoldTime = 0.0f;
    [SerializeField] private Slider holdTimeSlider;

    private const string IntroSeenKey = "IntroSeen";

    private void Start()
    {
        if (PlayerPrefs.HasKey(IntroSeenKey) && PlayerPrefs.GetInt(IntroSeenKey) == 1)
        {
            gameObject.SetActive(false);
            return;
        }

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
        PlayerPrefs.SetInt(IntroSeenKey, 1);  
        PlayerPrefs.Save(); 
        SceneManager.LoadScene("Main Menu"); 
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= EndVideo;
        }
    }
}
