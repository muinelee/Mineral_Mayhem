using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;


public class ArchiveUIOverlay : MonoBehaviour
{
    public enum ArchiveUIState
    {
        Active,
        Hidden,
    }

    private ArchiveUIState state = ArchiveUIState.Active;
    public ArchiveUIState State
    {
        get { return state; }
        set
        {
            if (value == state) return;
            MainPanel.SetActive(value == ArchiveUIState.Active);
            HiddenPanel.SetActive(value == ArchiveUIState.Hidden);
            state = value;
            return;
        }
    }

    [Header("UI Panels")]
    [SerializeField] GameObject MainPanel;
    [SerializeField] GameObject HiddenPanel;

    [Header("Relocate Controller Section Info")]
    [SerializeField] List<Transform> teleportPositions;
    [SerializeField] List<Button> teleportButtons;

    [Header("Controller Speed Section Info")]
    [SerializeField] Slider speedSlider;
    [SerializeField] TextMeshProUGUI speedText;

    [Header("Camera Sensitivity Section Info")]
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] TextMeshProUGUI sensitivityText;

    [Header("Hide Panel Button")]
    [SerializeField] Button hamburgerMenuButton;

    private ArchiveController controller;

    private void Start()
    {
        controller = FindAnyObjectByType<ArchiveController>();

        if (teleportButtons.Count > 0 && controller)
        {
            for (int i = 0; i < teleportButtons.Count; i++)
            {
                int index = i;
                teleportButtons[index].onClick.AddListener(() => RelocateController(teleportPositions[index]));
            }
        }
        if (hamburgerMenuButton)
        {
            hamburgerMenuButton.onClick.AddListener(SwapOverlay);
        }
        if (speedSlider && speedText && controller)
        {
            speedSlider.onValueChanged.AddListener(UpdateSpeedText);
            speedSlider.onValueChanged.AddListener(controller.SetControllerSpeed);
            UpdateSpeedText(speedSlider.value);
            controller.SetControllerSpeed(speedSlider.value);
        }
        if (sensitivitySlider && sensitivityText && controller)
        {
            sensitivitySlider.onValueChanged.AddListener(UpdateSensitivityText);
            sensitivitySlider.onValueChanged.AddListener(controller.SetFPSensitivity);
            UpdateSensitivityText(sensitivitySlider.value);
            controller.SetFPSensitivity(sensitivitySlider.value);
        }
    }

    public void Update()
    {
        CheckForHKeyPress();
        CheckForEscKeyPress();
    }

    private void CheckForHKeyPress()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            SwapOverlay();
        }
    }

    private void CheckForEscKeyPress()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void SwapOverlay()
    {
        State = (State == ArchiveUIState.Active) ? ArchiveUIState.Hidden : ArchiveUIState.Active;
    }

    private void RelocateController(Transform location)
    {
        controller.transform.position = location.position;
    }

    private void UpdateSpeedText(float value)
    {
        speedText.text = ((int)value).ToString();
    }

    private void UpdateSensitivityText(float value)
    {
        sensitivityText.text = ((int)value).ToString();
    }
}
