using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SETTING_Slider : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Slider slider;

    [SerializeField] string settingType;
    float value;

    //--------------------------------------//

    private void Start()
    {
        if (settingType == "brightness")
        {
            slider.value = DataManager.brightness;
            UpdateSetting();
        }
        else if (settingType == "volumeMaster")
        {
            slider.value = DataManager.volumeMaster;
            UpdateSetting();
        }
        else if (settingType == "volumeMusic")
        {
            slider.value = DataManager.volumeMusic;
            UpdateSetting();
        }
        else if (settingType == "volumeSFX")
        {
            slider.value = DataManager.volumeSFX;
            UpdateSetting();
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
