using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class SaveManager : MonoBehaviour
{
    private void Start()
    {
        if (PlayerPrefs.HasKey("PostProcessing"))
            LoadData();
        else
            ChangeSetting.instance.ResetSettings();
    }
    private void OnDisable()
    {
        //SaveData();
    }

    public void SaveData()
    {
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
        if (PlayerPrefs.HasKey("PostProcessing"))
        {
            SettingsManager.postProcessing = PlayerPrefs.GetInt("PostProcessing");

            SettingsManager.volumeMaster = PlayerPrefs.GetFloat("VolumeMaster");
            SettingsManager.volumeMusic = PlayerPrefs.GetFloat("VolumeMusic");
            SettingsManager.volumeSFX = PlayerPrefs.GetFloat("VolumeSFX");

            SettingsManager.brightness = PlayerPrefs.GetFloat("Brightness");
            SettingsManager.contrast = PlayerPrefs.GetFloat("Contrast");
            SettingsManager.saturation = PlayerPrefs.GetFloat("Saturation");

            ChangeSetting.instance.SetSettings();
        }
        else
            ChangeSetting.instance.ResetSettings();
    }

    [ContextMenu("Delete All PlayerPrefs Entries")]
    public void DeletePrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
