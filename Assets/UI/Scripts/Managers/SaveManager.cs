using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class SaveManager : MonoBehaviour
{
    private void Awake()
    {
        LoadData();
    }
    private void OnDisable()
    {
        //SaveData();
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("Windowed", SettingsManager.windowed);
        PlayerPrefs.SetInt("PostProcessing", SettingsManager.postProcessing);

        PlayerPrefs.SetFloat("VolumeMaster", SettingsManager.volumeMaster);
        PlayerPrefs.SetFloat("VolumeMusic", SettingsManager.volumeMusic);
        PlayerPrefs.SetFloat("VolumeSFX", SettingsManager.volumeSFX);

        PlayerPrefs.SetFloat("Brightness", SettingsManager.brightness);
        PlayerPrefs.SetFloat("Contrast", SettingsManager.contrast);
        PlayerPrefs.SetFloat("Saturation", SettingsManager.saturation);
    }

    public void LoadData()
    {
        SettingsManager.windowed = PlayerPrefs.GetInt("Windowed");
        SettingsManager.postProcessing = PlayerPrefs.GetInt("PostProcessing");

        SettingsManager.volumeMaster = PlayerPrefs.GetFloat("VolumeMaster");
        SettingsManager.volumeMusic = PlayerPrefs.GetFloat("VolumeMusic");
        SettingsManager.volumeSFX = PlayerPrefs.GetFloat("VolumeSFX");

        SettingsManager.brightness = PlayerPrefs.GetFloat("Brightness");
        SettingsManager.contrast = PlayerPrefs.GetFloat("Contrast");
        SettingsManager.saturation = PlayerPrefs.GetFloat("Saturation");

        ChangeSetting.instance.SetSettings();
    }
}
