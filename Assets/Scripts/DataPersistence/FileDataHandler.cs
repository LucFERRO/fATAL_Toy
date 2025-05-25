using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class FileDataHandler
{
    public string dataDirPath = "";
    public string dataFileName = "";

    public FileDataHandler(string dataDirPath)
    {
        this.dataDirPath = dataDirPath.Trim();
        //this.dataFileName = dataFileName.Trim();
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data from {fullPath}: {e.Message}");
            }
        }
        return loadedData;
    }

    public void Save(GameData data, string sessionId)
    {
        string dataFileName = $"gameData_{sessionId}.json";

        string relativePath = Path.Combine("SaveData", "Save", dataFileName);
        string fullPath = Path.Combine(Application.dataPath, "..", relativePath);
        //Debug.Log(fullPath);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = JsonUtility.ToJson(data, true);
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save data to {fullPath}: {e.Message}");
        }
    }

    public List<string> GetAllSaveFiles()
    {
        List<string> saveFiles = new List<string>();

        if (Directory.Exists(dataDirPath))
        {
            string[] files = Directory.GetFiles(dataDirPath, "Save_*.json");
            foreach (string file in files)
            {
                saveFiles.Add(Path.GetFileName(file));
            }
        }
        else
        {
            Debug.LogWarning($"Save directory does not exist: {dataDirPath}");
        }

        return saveFiles;
    }
}
