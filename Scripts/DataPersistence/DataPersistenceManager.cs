using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class DataPersistenceManager : MonoBehaviour
{

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance { get; private set; }



    private void Awake()
    {
       if (instance != null)
        {
            Debug.LogError("Found more than one Persistence Manager in the scene.");
        }
        instance = this;

        this.gameData = new GameData();
    }

    private void Start()
    {
        //right now this is only the GameManager. If other dataPersistenceObjects are created,
        //this should get moved to the SaveGame method
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        this.dataHandler = new FileDataHandler(Application.persistentDataPath);
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }
    public void LoadGame(string saveName)
    {
        this.gameData = dataHandler.Load(saveName);

        if (this.gameData == null)
        {
            Debug.Log("No data was found. Abort loading.");
            return;
        }

        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        //Debug.Log("loaded " + gameData.allGemüslis.Count);
    }
    public void SaveGame(string saveName)
    {
        gameData = new GameData();

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }


        dataHandler.Save(saveName, gameData);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public string GetAllSaveFileNamesAsString()
    {

        string toReturn = "";
        string[] files = System.IO.Directory.GetFiles(Application.persistentDataPath);

        foreach (string f in files)
        {
            FileInfo fi = new FileInfo(f);
            toReturn += fi.Name.ToString() + ", ";   
        }

        toReturn.Remove(toReturn.Length - 1);
        toReturn.Remove(toReturn.Length - 1);

        return toReturn;
    }
}
