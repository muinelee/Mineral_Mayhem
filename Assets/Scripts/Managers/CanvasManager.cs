using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class CanvasManager : MonoBehaviour
{
    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject settingsMenu;

    [Header("Buttons")]
    public Button startButton;
    public Button settingsButton;
    public Button backButton;
    public Button quitButton;

    [Header("Text")]
    public Text volSliderText;

    [Header("Slider")]
    public Slider volSlider;

    // Start is called before the first frame update
    void Start()
    {
        // Checks to see if these objects exist so that we avoid NULL references
        if (startButton)
            startButton.onClick.AddListener(StartGame);

        if (settingsButton)
            settingsButton.onClick.AddListener(ShowSettingsMenu);

        if (backButton)
            backButton.onClick.AddListener(ShowMainMenu);

        if (quitButton)
            quitButton.onClick.AddListener(Quit);

        if (volSlider)
        {
            // Passing an anonymous function with AddListner(value) =>
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
        // Need to load the main game scene
        // SceneManager.LoadScene("Game");
    }

    void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    void ShowSettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
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
        
        // Need to implement AudioManager, then uncomment next line
        // AudioManager.Instance.SetVolume(value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
