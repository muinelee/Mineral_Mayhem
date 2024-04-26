using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SETTING_Slider : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Slider slider;

    [SerializeField] string settingType;
    public float value;

    //--------------------------------------//

    private void Start()
    {
        SetSettings();
    }

    public void SetSettings()
    {
        if (settingType == "volumeMaster")
        {
            slider.value = SettingsManager.volumeMaster;
            SliderValueChanged();
        }
        if (settingType == "volumeSFX")
        {
            Debug.Log("SFX: " + SettingsManager.volumeSFX);
            slider.value = SettingsManager.volumeSFX;
            SliderValueChanged();
        }
        if (settingType == "volumeMusic")
        {
            Debug.Log("Music: " + SettingsManager.volumeMusic);
            slider.value = SettingsManager.volumeMusic;
            SliderValueChanged();
        }
        if (settingType == "brightness")
        {
            slider.value = SettingsManager.brightness;
            SliderValueChanged();
        }
        if (settingType == "contrast")
        {
            slider.value = SettingsManager.contrast;
            SliderValueChanged();
        }
        if (settingType == "saturation")
        {
            slider.value = SettingsManager.saturation;
            SliderValueChanged();
        }
    }

    public void ResetSettings()
    {
        if (settingType == "volumeMaster")
        {
            slider.value = 1;
            SliderValueChanged();
        }
        if (settingType == "volumeSFX")
        {
            slider.value = 1;
            SliderValueChanged();
        }
        if (settingType == "volumeMusic")
        {
            slider.value = 1;
            SliderValueChanged();
        }
        if (settingType == "brightness")
        {
            slider.value = 0.5f;
            SliderValueChanged();
        }
        if (settingType == "contrast")
        {
            slider.value = 0.5f;
            SliderValueChanged();
        }
        if (settingType == "saturation")
        {
            slider.value = 0.5f;
            SliderValueChanged();
        }
    }

    public void SliderValueChanged()
    {
        value = slider.value;
        UpdateSetting();
    }

    private void UpdateSetting()
    {
        text.text = "  " + (value * 100).ToString("0") + "%";
        ChangeSetting.instance.ChangeValue(value, settingType);
    }
}
