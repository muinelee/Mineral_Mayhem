using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

[DefaultExecutionOrder(1)]
public class LST_Slider : MonoBehaviour
{
    public enum ListingType
    {
        Brightness,
        Master,
        Music,
        SFX,

    }
    public ListingType listingType;

    [SerializeField] AudioMixer mixer;

    [SerializeField] TextMeshProUGUI textValue;
    [SerializeField] Slider slider;

    public UnityEvent valueChanged;

    //---------------------------------//

    private void Start()
    {
        if (listingType == ListingType.Master)
        {
            slider.value = SettingsManager.volumeMaster > 0 ? Mathf.Log10(SettingsManager.volumeMaster) * 20 : -80;
        }
        else if (listingType == ListingType.Music)
        {
            slider.value = SettingsManager.volumeMusic > 0 ? Mathf.Log10(SettingsManager.volumeMusic) * 20 : -80;
        }
        else if (listingType == ListingType.SFX)
        {
            slider.value = SettingsManager.volumeSFX > 0 ? Mathf.Log10(SettingsManager.volumeSFX) * 20 : -80;
        }
    }

    public void UpdateValue()
    {
        if (textValue != null)
        {
            textValue.text = " " + (100 * slider.value).ToString("0") + "%";
        }

        if (listingType == ListingType.Master)
        {   
            if (slider.value <= -80)
            {
                SettingsManager.volumeMaster = 0;
            }
            else
            {
                SettingsManager.volumeMaster = Mathf.Pow(10, slider.value / 20);
            }
            mixer.SetFloat("master", SettingsManager.volumeMaster);
        }
        else if (listingType == ListingType.Music)
        {   
            if (slider.value <= -80)
            {
                SettingsManager.volumeMusic = 0;
            }
            else
            {
                SettingsManager.volumeMusic = Mathf.Pow(10, slider.value / 20);
            }
            mixer.SetFloat("music", SettingsManager.volumeMusic);
        }
        else if (listingType == ListingType.SFX)
        {   
            if (slider.value <= -80)
            {
                SettingsManager.volumeSFX = 0;
            }
            else
            {
                SettingsManager.volumeSFX = Mathf.Pow(10, slider.value / 20);
            }
            mixer.SetFloat("sfx", SettingsManager.volumeSFX);
        }

        valueChanged?.Invoke();
    }
}
