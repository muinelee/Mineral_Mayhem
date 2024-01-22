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
            slider.value = (DataManager.masterVolume + 30) / 30;
        }
        else if (listingType == ListingType.Music)
        {
            slider.value = (DataManager.musicVolume + 30) / 30;
        }
        else if (listingType == ListingType.SFX)
        {
            slider.value = (DataManager.sfxVolume + 30) / 30;
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
            DataManager.masterVolume = slider.value * 30 - 30;
            mixer.SetFloat("master", DataManager.masterVolume);
        }
        else if (listingType == ListingType.Music)
        {
            DataManager.musicVolume = slider.value * 30 - 30;
            mixer.SetFloat("music", DataManager.musicVolume);
        }
        else if (listingType == ListingType.SFX)
        {
            DataManager.sfxVolume = slider.value * 30 - 30;
            mixer.SetFloat("sfx", DataManager.sfxVolume);
        }

        valueChanged?.Invoke();
    }
}
