using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{

    private string dataDirPath = "";

    public FileDataHandler(string dataDirPath)
    {
        this.dataDirPath = dataDirPath;
    }

    public GameData Load(string saveGame)
    {

        string fullPath = Path.Combine(dataDirPath, saveGame + ".game");
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

            } catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }

        } else
        {

            Debug.LogWarning("The file " + fullPath + " doesn't exist!");
        }

        Debug.Log(fullPath);

        return loadedData;
    }

    public void Save(string saveName, GameData data)
    {
        //':' creates a folder
        saveName = saveName.Replace(":", "_");
        string fullPath = Path.Combine(dataDirPath, saveName + ".game");
        Debug.Log(fullPath);

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

        } catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }

    }

}
