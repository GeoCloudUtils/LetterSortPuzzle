using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public enum Difficulty
{
    EASY = 0,
    MEDIUM = 1,
    HARD = 2
}

[CreateAssetMenu(fileName = "Words", menuName = "ScriptableObjects/CreateWordsDictionary", order = 1)]
public class WordsDictionary : ScriptableObject
{
    public WordsCollection[] WordAssets;

    public List<string> words;

    public void LoadFromJSON(Difficulty difficulty)
    {
        words = new List<string>();
        string json = WordAssets.Where(e => e.Difficulty == difficulty).FirstOrDefault().WordCollection.text;

        WordList wordList = JsonConvert.DeserializeObject<WordList>(json);
        words = wordList.words;
    }

    public string GetWord()
    {
        int next = PlayerPrefs.GetInt("NEXT", 0);
        if (next > words.Count - 1)
        {
            Debug.Log("1");
            PlayerPrefs.SetInt("NEXT", 0);
            LoadFromJSON(PlayerPrefs.GetInt("DIFFICULTY", 0) == 0 ? Difficulty.MEDIUM : Difficulty.HARD);
        }
        return words[next];
    }
}

[System.Serializable]
public class WordsCollection
{
    public Difficulty Difficulty;
    public TextAsset WordCollection;
}

[System.Serializable]
public class WordList
{
    public List<string> words;
}
