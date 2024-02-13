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
        PlayerPrefs.SetInt("FullScreen", DataManager.fullScreen);

        PlayerPrefs.SetFloat("VolumeMaster", DataManager.volumeMaster);
        PlayerPrefs.SetFloat("VolumeMusic", DataManager.volumeMusic);
        PlayerPrefs.SetFloat("VolumeSFX", DataManager.volumeSFX);
    }

    public void LoadData()
    {
        DataManager.fullScreen = PlayerPrefs.GetInt("FullScreen");
        DataManager.volumeMaster = PlayerPrefs.GetFloat("VolumeMaster");
        DataManager.volumeMusic = PlayerPrefs.GetFloat("VolumeMusic");
        DataManager.volumeSFX = PlayerPrefs.GetFloat("volumeSFX");
    }

    public void ClearData()
    {
        DataManager.fullScreen = 1;
        DataManager.volumeMaster = 1f;
        DataManager.volumeMusic = 0.8f;
        DataManager.volumeSFX = 0.75f;

        SaveData();
    }
}
