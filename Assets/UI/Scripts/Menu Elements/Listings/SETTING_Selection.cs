using TMPro;
using UnityEngine;

public class SETTING_Selection : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] string settingType;

    [SerializeField] string[] value = { "On", "Off" };

    [Header("Options")]
    [SerializeField] bool loop = true;
    int selected = 0;

    public void SetSettings()
    {
        if (settingType == "postProcessing")
        {
            selected = SettingsManager.postProcessing;
            UpdateSetting();
        }
        if (settingType == "windowed")
        {
            selected = SettingsManager.windowed;
            UpdateSetting();
        }
    }

    public void ResetSettings()
    {
        if (settingType == "postProcessing")
        {
            selected = 0;
            UpdateSetting();
        }
        if (settingType == "windowed")
        {
            selected = 1;
            UpdateSetting();
        }
    }

    public void Next()
    {
        if (selected < value.Length - 1)
        {
            selected++;
            UpdateSetting();
        }
        else if (loop)
        {
            selected = 0;
            UpdateSetting();
        }
    }
    public void Prev()
    {
        if (selected > 0)
        {
            selected--;
            UpdateSetting();
        }
        else if (loop)
        {
            selected = value.Length - 1;
            UpdateSetting();
        }
    }

    private void UpdateSetting()
    {
        for (int i = 0; i < value.Length; i++)
        {
            if (i == selected)
            {
                text.text = value[i];
            }
        }

        if (settingType != "") ChangeSetting.instance.ChangeSelection(selected, settingType);
    }
}
