using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;

public class SaveSystem : MonoBehaviour
{
    DataHolder dataHolder = new DataHolder();
    public Options options;
    public HatSelect hatSelect;
    public PlayerController playerController;
    public string playerNameHolder;

    private void Start()
    {
        Load();
        SetData();
    }

    public void SavePreparations()
    {
        DataCollector();
        SaveGame();
        print(Application.persistentDataPath + "/" + "SaveData" + ".Xml");
    }

    void SaveGame()
    {

        var serializer = new XmlSerializer(typeof(DataHolder));
        var stream = new FileStream(Application.persistentDataPath + "/" + "SaveData" + ".Xml", FileMode.Create);
        serializer.Serialize(stream, dataHolder);
        stream.Close();
    }

    public void Load()
    {
        var serializer = new XmlSerializer(typeof(DataHolder));
        var stream = new FileStream(Application.persistentDataPath + "/" + "SaveData" + ".Xml", FileMode.Open);
        dataHolder = serializer.Deserialize(stream) as DataHolder;
        stream.Close();

    }

    void DataCollector()
    {
        dataHolder.quality = options.qualityDropdown.value;
        dataHolder.music = options.musicSlider.value;
        dataHolder.soundEffects = options.soundEffectsSlider.value;
        dataHolder.sensitivity = options.sensitivitySlider.value;
        dataHolder.fullscreen = options.fullscreenToggle.isOn;
        if(options.nameInput != null)
        {
            dataHolder.name = options.nameInput.text;
        }
        else
        {
            dataHolder.name = playerNameHolder;
        }
        if (hatSelect != null)
        {
            dataHolder.hatSelected = hatSelect.currentHatSelected;
        }
    }

    void SetData()
    {
        if (dataHolder != null)
        {
            options.SetQuality(dataHolder.quality);
            options.SetMusic(dataHolder.music);
            options.SetSoundEffects(dataHolder.soundEffects);
            options.SetSensitivity(dataHolder.sensitivity);
            options.SetFullscreen(dataHolder.fullscreen);
            if(options.nameInput != null)
            {
                options.SetName(dataHolder.name);
            }
            else
            {
                playerNameHolder = dataHolder.name;
            }
            if (hatSelect != null)
            {
                hatSelect.currentHatSelected = dataHolder.hatSelected;
                hatSelect.SetHat();
            }
            if(playerController != null)
            {
                playerController.currentHat = dataHolder.hatSelected;
            }
        }
    }

    private void OnApplicationQuit()
    {
        SavePreparations();
    }
}
