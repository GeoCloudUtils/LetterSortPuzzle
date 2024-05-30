using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    private static GameDataManager _instance;
    public static GameDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameDataManager");
                _instance = go.AddComponent<GameDataManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private string dataPath;
    public GameData gameData;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            dataPath = Path.Combine(Application.persistentDataPath, "gameData.json");
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        string jsonData = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(dataPath, jsonData);
    }

    public void LoadGame()
    {
        Debug.Log("Loading data from: " + dataPath);
        if (File.Exists(dataPath))
        {
            string jsonData = File.ReadAllText(dataPath);
            gameData = JsonUtility.FromJson<GameData>(jsonData);
        }
        else
        {
            gameData = new GameData();
        }
    }

    public void ResetData()
    {
        if (File.Exists(dataPath))
        {
            File.Delete(dataPath);
        }
        gameData = new GameData();
    }

    public bool IsEprubeteOpen(string eprubeteName, int requiredLevel)
    {
        return gameData.Level >= requiredLevel && gameData.Eprubete.Contains(eprubeteName);
    }

    public bool IsBackgroundOpen(string backgroundName, int requiredLevel)
    {
        return gameData.Level >= requiredLevel && gameData.Backgrounds.Contains(backgroundName);
    }

    public void BuyEprubete(string eprubeteName)
    {
        if (!gameData.Eprubete.Contains(eprubeteName))
        {
            gameData.Eprubete.Add(eprubeteName);
            SaveGame();
        }
    }

    public void BuyBackground(string backgroundName)
    {
        if (!gameData.Backgrounds.Contains(backgroundName))
        {
            gameData.Backgrounds.Add(backgroundName);
            SaveGame();
        }
    }
}

[System.Serializable]
public class GameData
{
    public bool Music;
    public bool Sound;
    public bool NoAds;

    public int Level;
    public int TipsCount;
    public int Coins;
    public int Highlights;
    public int Removes;

    public string BackgroundName;

    public List<string> Eprubete;
    public List<string> Backgrounds;

    public GameData()
    {
        Music = true;
        Sound = true;
        NoAds = false;
        Level = 1;
        TipsCount = 8;
        Highlights = 1;
        Removes = 1;
        BackgroundName = "bg1";
        Coins = 1;
        Eprubete = new List<string>();
        Backgrounds = new List<string>();
    }
}

