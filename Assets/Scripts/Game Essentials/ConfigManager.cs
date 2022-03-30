using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConfigManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider fxVolumeSlider;

    [Header("Controls Settings")]
    [SerializeField] Button leftiesButton;
    [SerializeField] Button rightiesButton;
    [Space]
    [SerializeField] Sprite leftEnabled;
    [SerializeField] Sprite leftDisabled;
    [SerializeField] Sprite rightEnabled;
    [SerializeField] Sprite rightDisabled;
    [Space]
    [SerializeField] EventSystem mainEventSystem;

    public void LoadSettigns()
    {
        //Setea el Master Volume
        if (!PlayerPrefs.HasKey("MasterVolume"))
        {
            PlayerPrefs.SetFloat("MasterVolume", 1f);
        }
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");

        //Setea el Music Volume
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1f);
        }
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");

        //Setea el Fx Volume
        if (!PlayerPrefs.HasKey("FxVolume"))
        {
            PlayerPrefs.SetFloat("FxVolume", 1f);
        }
        fxVolumeSlider.value = PlayerPrefs.GetFloat("FxVolume");

        //Ajusta el volumen de los sonidos en función de los ajustes establecidos
        AudioManager.instance.ModifyAudioVolumes();

        //Setea el Control
        if (!PlayerPrefs.HasKey("IsRightHanded"))
        {
            PlayerPrefs.SetInt("IsRightHanded", 0);
        }
        SetHand(PlayerPrefs.GetInt("IsRightHanded"));
    }

    public void SetHand(int value)
    {
        switch (value)
        {
            case 0:
                //Carga el feedback Visual
                leftiesButton.image.sprite = leftDisabled;
                rightiesButton.image.sprite = rightEnabled;
                //Deselecciona los botones
                mainEventSystem.SetSelectedGameObject(null);
                //Setea el valor de los controles
                PlayerPrefs.SetInt("IsRightHanded", 0);
                break;

            case 1:
                //Carga el feedback Visual
                leftiesButton.image.sprite = leftEnabled;
                rightiesButton.image.sprite = rightDisabled;
                //Deselecciona los botones
                mainEventSystem.SetSelectedGameObject(null);
                //Setea el valor de los controles
                PlayerPrefs.SetInt("IsRightHanded", 1);
                break;

            default:
                break;
        }
    }

    public void SetMasterVolume()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        
        AudioManager.instance.ModifyAudioVolumes();
    }

    public void SetMusicVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);

        AudioManager.instance.ModifyAudioVolumes();
    }

    public void SetFxVolume()
    {
        PlayerPrefs.SetFloat("FxVolume", fxVolumeSlider.value);

       AudioManager.instance.ModifyAudioVolumes();
    }

    public void EraseData()
    {
        SaveSystem.EraseData();
    }
}