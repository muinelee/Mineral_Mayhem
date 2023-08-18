using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class CanvasManager : MonoBehaviour
{
    [Header("Main Menu")]
    public GameObject mainMenu;
    public GameObject compendium;
    public GameObject settingsMenu;

    [Header("Buttons")]
    public Button startButton;
    public Button trainingRoomButton;
    public Button compendiumButton;
    public Button settingsButton;
    public Button mainMenuButton;
    public Button quitButton;

    [Header("Slider")]
    public Slider volSlider;

    [Header("Text")]
    public Text volSliderText;

    // Start is called before the first frame update
    void Start()
    {
        // Checks to see if these objects exist so that we avoid NULL references
        if (startButton)
        {
            startButton.onClick.AddListener(StartGame);
        }
        if (trainingRoomButton)
        {
            trainingRoomButton.onClick.AddListener(TrainingRoom);
        }
        if (compendiumButton)
        {
            compendiumButton.onClick.AddListener(Compendium);
        }
        if (settingsButton)
        {
            settingsButton.onClick.AddListener(ShowSettingsMenu);
        }
        if (mainMenuButton)
        {
            mainMenuButton.onClick.AddListener(ShowMainMenu);
        }
        if (quitButton)
        { 
        quitButton.onClick.AddListener(Quit);
        }

        if (volSlider)
        {
            volSlider.onValueChanged.AddListener((value) => OnSliderValueChanged(value));

            if (volSliderText)
            { 
                volSliderText.text = volSlider.value.ToString();
            }

            OnSliderValueChanged(volSlider.value);
        }
    }

    private void StartGame()
    {
        Debug.Log("Start Game Logic Not Yet Implemented");
        //SceneManager.LoadScene("Game");
    }

    void TrainingRoom()
    {
        SceneManager.LoadScene("Test Room");
    }

    void Compendium()
    {
        Debug.Log("Compendium Scene Logic Not Yet Implemented");
        //mainMenu.SetActive(false);
        //compendium.SetActive(true);
    }

    void ShowSettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    void OnSliderValueChanged(float value)
    {
        float step = 0.1f;
        value = Mathf.Round(value / step) * step;
        volSliderText.text = (value * 100).ToString();
        
        // Need to implement AudioManager then apply logic
        //AudioManager.Instance.SetVolume(value);
    }
}
