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
            slider.value = (SettingsManager.volumeMaster + 30) / 30;
        }
        else if (listingType == ListingType.Music)
        {
            slider.value = (SettingsManager.volumeMusic + 30) / 30;
        }
        else if (listingType == ListingType.SFX)
        {
            slider.value = (SettingsManager.volumeSFX + 30) / 30;
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
            SettingsManager.volumeMaster = slider.value * 30 - 30;
            mixer.SetFloat("master", SettingsManager.volumeMaster);
        }
        else if (listingType == ListingType.Music)
        {
            SettingsManager.volumeMusic = slider.value * 30 - 30;
            mixer.SetFloat("music", SettingsManager.volumeMusic);
        }
        else if (listingType == ListingType.SFX)
        {
            SettingsManager.volumeSFX = slider.value * 30 - 30;
            mixer.SetFloat("sfx", SettingsManager.volumeSFX);
        }

        valueChanged?.Invoke();
    }
}
