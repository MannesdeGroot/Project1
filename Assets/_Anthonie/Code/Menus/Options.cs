using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Options : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer;
    [Header("UI_Ellements")]
    public Slider soundEffectsSlider;
    public Slider musicSlider;
    public Slider sensitivitySlider;
    public Dropdown qualityDropdown;
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    [Header("Misc")]
    Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> resolutionStrings = new List<string>();

        int currentResInt = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionStrings.Add(resolutions[i].width + " x " + resolutions[i].height);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResInt = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionStrings);
        resolutionDropdown.value = currentResInt;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetSoundEffects(float volume)
    {
        audioMixer.SetFloat("Effects", volume);
        soundEffectsSlider.value = volume;
    }

    public void SetMusic(float volume)
    {
        audioMixer.SetFloat("Music", volume);
        musicSlider.value = volume;
    }

    public void SetQuality(int quality)
    {
        QualitySettings.SetQualityLevel(quality);
        qualityDropdown.value = quality;
        qualityDropdown.RefreshShownValue();
    }

    public void SetResolution(int res)
    {
        Screen.SetResolution(resolutions[res].width, resolutions[res].height, Screen.fullScreen);
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        fullscreenToggle.isOn = fullscreen;
    }

    public void SetSensitivity(float sensitivity)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if(players.Length != 0)
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i].GetComponent<PlayerController>().sensitivity = sensitivity;
            }
        }
        
        sensitivitySlider.value = sensitivity;

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
