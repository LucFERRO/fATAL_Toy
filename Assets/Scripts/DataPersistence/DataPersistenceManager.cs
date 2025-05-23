using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects;

    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("DataPersistenceManager already exists in the scene.");
        }

        instance = this;
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        NewGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
            dataPersistenceObjects = FindAllDataPersistenceObjects();
            Debug.Log("Loading last save.");
            LoadGame();
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            SaveGame();
            Debug.Log("Saving game.");
        }
    }

    public void NewGame()
    {
        this.gameData = new GameData();
        Debug.Log("New game started.");
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();
        if (gameData == null)
        {
            Debug.Log("No game data found. Starting a new game.");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Loaded game data: " + gameData.currentNumberOfRolls);
    }

    public void SaveGame()
    {
        if (dataHandler == null)
        {
            Debug.LogError("DataHandler is not initialized. Cannot save game.");
            return;
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        Debug.Log("Saved game data: " + gameData.currentNumberOfRolls);

        dataHandler.Save(gameData);
    }

    //private void OnApplicationQuit()
    //{
    //    SaveGame();
    //}

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
