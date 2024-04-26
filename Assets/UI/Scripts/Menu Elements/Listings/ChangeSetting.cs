using UnityEngine;
using UnityEngine.Audio;

public class ChangeSetting : MonoBehaviour
{
    public static ChangeSetting instance;

    [Header("Setting Effect References")]
    // Brightness Post-Processing is changed on start in game scene by PPManager
    [SerializeField] AudioMixer mixer;

    //------------------------------------//

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ChangeSelection(int selection, string type)
    {
        if (type == "windowed")
        {
            DataManager.windowed = selection;

            // Setting Effect
        }
    }

    public void ChangeValue(float value, string type)
    {
        if (type == "brightness")
        {
            DataManager.brightness = value;

            // Setting Effect
        }
        else if (type == "contrast")
        {
            DataManager.contrast = value;

            // Setting Effect
        }
        else if (type == "saturation")
        {
            DataManager.saturation = value;

            // Setting Effect
        }
        else if (type == "volumeMaster")
        {
            DataManager.volumeMaster = value;

            // Setting Effect
            mixer.SetFloat("master", DataManager.volumeMaster);
        }
        else if (type == "volumeMusic")
        {
            DataManager.volumeMusic = value;

            // Setting Effect
            mixer.SetFloat("music", DataManager.volumeMusic * 80 - 80);
        }
        else if (type == "volumeSFX")
        {
            DataManager.volumeSFX = value;

            //Setting Effect
            mixer.SetFloat("sfx", DataManager.volumeSFX * 80 - 80);

        }
    }
}
