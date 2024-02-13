using UnityEngine;
using UnityEngine.Audio;

public class ChangeSetting : MonoBehaviour
{
    public static ChangeSetting instance;

    [Header("Setting Effect References")]
    // Texture Quality is changed in game scene by TextureManager
    // Brightness Post-Processing is changed on start in game scene by PPManager
    [SerializeField] AudioMixer mixer;
    // Left Handed is changed on start in game scene by GameManager

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
        if (type == "fullScreen")
        {
            DataManager.fullScreen = selection;

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
