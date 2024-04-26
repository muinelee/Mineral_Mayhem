using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

[DefaultExecutionOrder(1)]
public class ChangeSetting : MonoBehaviour
{
    public static ChangeSetting instance;

    [Header("Setting Effect References")]
    // Brightness Post-Processing is changed on start in game scene by PPManager
    [SerializeField] GameObject postProcessingGameObject;
    [SerializeField] AudioMixer mixer;
    [SerializeField] Volume PP_Brightness;
    [SerializeField] Volume PP_Contrast;
    [SerializeField] Volume PP_Saturation;

    [Header("Settings Listings References")]
    [SerializeField] SETTING_Selection S_Windowed;
    [SerializeField] SETTING_Selection S_PostProcessing;
    [SerializeField] SETTING_Slider S_Master;
    [SerializeField] SETTING_Slider S_SFX;
    [SerializeField] SETTING_Slider S_Music;
    [SerializeField] SETTING_Slider S_Brightness;
    [SerializeField] SETTING_Slider S_Contrast;
    [SerializeField] SETTING_Slider S_Saturation;

    //------------------------------------//

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        SetSettings();
    }

    public void SetSettings()
    {
        if (SettingsManager.postProcessing == 0) postProcessingGameObject.SetActive(true);
        else if (SettingsManager.postProcessing == 1) postProcessingGameObject.SetActive(false);
        //Set Windowed Setting
        mixer.SetFloat("master", SettingsManager.volumeMaster * 80 - 80);
        mixer.SetFloat("music", SettingsManager.volumeMusic * 80 - 80);
        mixer.SetFloat("sfx", SettingsManager.volumeSFX * 80 - 80);

        PP_Brightness.weight = SettingsManager.brightness;
        PP_Contrast.weight = SettingsManager.contrast;
        PP_Saturation.weight = SettingsManager.saturation;

        S_Windowed.SetSettings();
        S_PostProcessing.SetSettings();
        S_Master.SetSettings();
        S_SFX.SetSettings();
        S_Music.SetSettings();
        S_Brightness.SetSettings();
        S_Contrast.SetSettings();
        S_Saturation.SetSettings();

        Debug.Log("Settings Set");
    }

    public void ResetSettings()
    {
        //Set Windowed Setting
        postProcessingGameObject.SetActive(true);
        mixer.SetFloat("master", 1 * 80 - 80);
        mixer.SetFloat("music", 1 * 80 - 80);
        mixer.SetFloat("sfx", 1 * 80 - 80);

        PP_Brightness.weight = 0.5f;
        PP_Contrast.weight = 0.5f;
        PP_Saturation.weight = 0.5f;

        S_Windowed.SetSettings();
        S_PostProcessing.ResetSettings();
        S_Master.ResetSettings();
        S_SFX.ResetSettings();
        S_Music.ResetSettings();
        S_Brightness.ResetSettings();
        S_Contrast.ResetSettings();
        S_Saturation.ResetSettings();

        Debug.Log("Settings Reset");
    }

    public void ChangeSelection(int selection, string type)
    {
        if (type == "windowed")
        {
            SettingsManager.windowed = selection;

            // Setting Effect

        }
        if (type == "postProcessing")
        {
            SettingsManager.postProcessing = selection;

            // Setting Effect
            if (selection == 0) postProcessingGameObject.SetActive(true);
            else if (selection == 1) postProcessingGameObject.SetActive(false);
        }
    }

    public void ChangeValue(float value, string type)
    {
        if (type == "brightness")
        {
            SettingsManager.brightness = value;

            // Setting Effect
            PP_Brightness.weight = value;
        }
        else if (type == "contrast")
        {
            SettingsManager.contrast = value;

            // Setting Effect
            PP_Contrast.weight = value;
        }
        else if (type == "saturation")
        {
            SettingsManager.saturation = value;

            // Setting Effect
            PP_Saturation.weight = value;
        }
        else if (type == "volumeMaster")
        {
            SettingsManager.volumeMaster = value;

            // Setting Effect
            mixer.SetFloat("master", SettingsManager.volumeMaster * 80 - 80);
        }
        else if (type == "volumeMusic")
        {
            SettingsManager.volumeMusic = value;

            // Setting Effect
            mixer.SetFloat("music", SettingsManager.volumeMusic * 80 - 80);
        }
        else if (type == "volumeSFX")
        {
            SettingsManager.volumeSFX = value;

            //Setting Effect
            mixer.SetFloat("sfx", SettingsManager.volumeSFX * 80 - 80);

        }
    }
}
