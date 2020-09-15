using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;

public class SaveSystem : MonoBehaviour
{
    DataHolder dataHolder = new DataHolder();
    public Options options;

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

    public void SaveGame()
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

    }

    void SetData()
    {
        if (dataHolder != null)
        {
            options.SetQuality(dataHolder.quality);
            options.SetMusic(dataHolder.music);
            options.SetSoundEffects(dataHolder.soundEffects);
            options.SetSensitivity(dataHolder.sensitivity);
        }
    }

    private void OnApplicationQuit()
    {
        SavePreparations();
    }
}
