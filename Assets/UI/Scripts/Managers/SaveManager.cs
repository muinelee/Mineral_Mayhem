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
        SaveData();
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("Windowed", DataManager.windowed);

        PlayerPrefs.SetFloat("VolumeMaster", DataManager.volumeMaster);
        PlayerPrefs.SetFloat("VolumeMusic", DataManager.volumeMusic);
        PlayerPrefs.SetFloat("VolumeSFX", DataManager.volumeSFX);

        PlayerPrefs.SetFloat("Brightness", DataManager.brightness);
        PlayerPrefs.SetFloat("Contrast", DataManager.contrast);
        PlayerPrefs.SetFloat("Saturation", DataManager.saturation);
    }

    public void LoadData()
    {
        DataManager.windowed = PlayerPrefs.GetInt("Windowed");
        DataManager.volumeMaster = PlayerPrefs.GetFloat("VolumeMaster");
        DataManager.volumeMusic = PlayerPrefs.GetFloat("VolumeMusic");
        DataManager.volumeSFX = PlayerPrefs.GetFloat("volumeSFX");
    }

    public void ClearData()
    {
        DataManager.windowed = 1;
        DataManager.volumeMaster = 1f;
        DataManager.volumeMusic = 0.8f;
        DataManager.volumeSFX = 0.75f;

        SaveData();
    }
}
