using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugCameraTrack : MonoBehaviour
{
    [SerializeField] Button blueBtn;
    [SerializeField] Button redBtn;

    private void Start()
    {
        if (blueBtn)
        {
            blueBtn.onClick.AddListener(StartBlueCameraTrack);
        }
        if (redBtn)
        {
            redBtn.onClick.AddListener(StartRedCameraTrack);
        }
    }

    public void StartBlueCameraTrack()
    {
        NetworkCameraEffectsManager.instance.GoToBlueCinematicCamera();
    }
    public void StartRedCameraTrack()
    {
        NetworkCameraEffectsManager.instance.GoToRedCinematicCamera();
    }
}
