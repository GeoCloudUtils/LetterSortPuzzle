#if UNITY_EDITOR
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AutoRunner : MonoBehaviour
{
    [Serializable]
    public class AutoRunnerWordInfo
    {
        public string word1;
        public string word2;
        public int seed;
    }

    [Header("File for Words with Seeds")]
    [SerializeField] private string _savedSeeds;

    [Header("Auto Solve")]
    [SerializeField] private Button _autoSolveButton;
    [SerializeField] private bool _autoSolve = false;


    [Header("Auto Change Word")]
    [SerializeField] private Button _autoChangeWordButton;
    [SerializeField] private bool _autoChangeWord = false;

    [Header("Current Word")]
    [SerializeField] private Button _regenerate;
    [SerializeField] private Button _toNextWord;
    [SerializeField] private Button _runNextMove;


    [SerializeField] private TMP_Text _seedText;
    [SerializeField] private TMP_Text _currWordsText;
    [SerializeField] private TMP_Text _progressText;
    [SerializeField] private float _speed;
    //[SerializeField] private string _word;
    //[SerializeField] private string _word2;
    [SerializeField] private string _randomLetters;
    [SerializeField] private int _pipes;
    [SerializeField] private AutoRunnerLetter _slotPrefab;
    [SerializeField] private List<RectTransform> _tubes;
    [SerializeField] private MovesCalculator _moveCalculator;
    [SerializeField] public WordsDictionary _wordsDict;
    [SerializeField] public List<AutoRunnerWordInfo> _words;
    private List<AutoRunnerLetter> _spawnedLetters = new List<AutoRunnerLetter>();
    private bool _running = false;
    private System.Random _rand = new System.Random();
    private int _seed = -1;
    private float _timeSinceLastMove = 1f;
    private List<MoveAction> _moves;
    private int _currMoveIndex;
    private int _currWordIndex;
    private AutoRunnerWordInfo _currWord;
    private Dictionary<int, Stack<AutoRunnerLetter>> _slots = new Dictionary<int, Stack<AutoRunnerLetter>>();
    private bool _failed;

    void Start()
    {
        _regenerate.onClick.AddListener(OnRegenerateClicked);
        _autoSolveButton.onClick.AddListener(OnAutoSolveClicked);
        _runNextMove.onClick.AddListener(OnRunNextClicked);

        _autoChangeWordButton.onClick.AddListener(OnAutoChangeWordClicked);
        _toNextWord.onClick.AddListener(OnToNextWordClicked);

        _autoChangeWordButton.GetComponentInChildren<TMP_Text>().text = $"Auto Change Word: {(_autoChangeWord ? "ON" : "OFF")}";
        _autoSolveButton.GetComponentInChildren<TMP_Text>().text = $"Auto Solve: {(_autoSolve ? "ON" : "OFF")}";

        if (!string.IsNullOrEmpty(_savedSeeds))
        {
            _words = Load(_savedSeeds);
        }
        else
        {
            _words = new List<AutoRunnerWordInfo>();
            List<string> words = _wordsDict.words;
            for (int a = 0; a < words.Count; a+=2)
            {
                _words.Add(new AutoRunnerWordInfo()
                {
                    seed = -1,
                    word1 = words[a],
                    word2 = words[a + 1],
                });
            }
        }
    }

    private void OnToNextWordClicked()
    {
        NextWord();
    }

    private void OnAutoChangeWordClicked()
    {
        _autoChangeWord = !_autoChangeWord;
        _autoChangeWordButton.GetComponentInChildren<TMP_Text>().text = $"Auto Change Word: {(_autoChangeWord ? "ON" : "OFF")}";
    }

    private void OnRegenerateClicked()
    {
        Generate();
    }

    private void OnRunNextClicked()
    {
        NextMove();
    }

    private void OnAutoSolveClicked()
    {
        _autoSolve = !_autoSolve;
        _autoSolveButton.GetComponentInChildren<TMP_Text>().text = $"Auto Solve: {(_autoSolve?"ON":"OFF")}";
    }

    private void Generate()
    {
        _running = true;
        _moves = null;

        _currWord = _words[_currWordIndex];
        
        // Set seed.
        if(_currWord.seed > 0)
        {
            _seed  = _currWord.seed;
        }
        else
        {
            _seed = _rand.Next(int.MaxValue);
            _currWord.seed = _seed;
        }
        Random.InitState(_seed);
        _seedText.text = $"Seed:{_seed}";

        // Remove slots.
        //foreach(var pair in _slots)
        //{
        //    foreach(var elem in pair.Value)
        //    {
        //        Destroy(elem.gameObject);
        //    }
        //}
        ClearChildren(_tubes[0]);
        ClearChildren(_tubes[1]);
        ClearChildren(_tubes[2]);
        ClearChildren(_tubes[3]);
        _slots.Clear();
        Debug.Log($"Slots:{_slots.Count}; Slot1:{_tubes[0].childCount}; Slot2:{_tubes[1].childCount}; Slot3:{_tubes[2].childCount}; Slot4:{_tubes[3].childCount}");

        List<Stack<char>> pipes = _moveCalculator.Generate(_currWord.word1 + _currWord.word2, _randomLetters, _pipes, _seed);

        for (int a = 0; a < pipes.Count; a++)
        {
            CreateSlots(pipes[a], _tubes[a], a);
        }

        _currWordsText.text = $"Curr Words: {_currWord.word1}, {_currWord.word2}";
        _progressText.text = $"Progress: {_currWordIndex}/{_words.Count}";

        _moves = _moveCalculator.CalculateMoves(_currWord.word1, _currWord.word2);

        _currMoveIndex = -1;
    }

    private void ClearChildren(RectTransform parent)
    {
    //    while(parent.childCount != 0)
    //    {
    //        Destroy(parent.GetChild(0).gameObject);
    //    }

        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateSlots(Stack<char> tower, RectTransform parent, int index)
    {
        Stack<AutoRunnerLetter> slots = new Stack<AutoRunnerLetter>();
        var list = tower.ToList();
        list.Reverse();
        foreach (var elem in list)
        {
            AutoRunnerLetter slot = Instantiate(_slotPrefab, parent);
            slot.SetLetter(elem.ToString());
            slots.Push(slot);
        }
        _slots[index] = slots;
    }

    private void NextMove()
    {
        if (!_running)
        {
            Debug.LogError("Generate first!");
            return;
        }
        else if (_moves == null)
        {
            Debug.LogError("Moves are NULL!");
            return;
        }
        else if (_currMoveIndex >= _moves.Count)
        {
            Debug.LogError("Already Finished!");
            return;
        }


        ++_currMoveIndex;
        if( _currMoveIndex < _moves.Count)
        {
            MoveAction action = _moves[_currMoveIndex];
            MoveUI(action);
        }
    }

    private void MoveUI(MoveAction action)
    {
        try
        {
            int from = action.from;
            int to = action.to;
            Debug.Log($"[MoveUI] Before Move from {from} to {to}. MoveIdx:{_currMoveIndex}; Pipes:{PrintPipes()}");
            var elem = _slots[from].Pop();
            _slots[to].Push(elem);
            elem.transform.SetParent(_tubes[to]);
            elem.transform.SetAsFirstSibling();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception occurred. Msg: " + ex.Message);
            _failed = true;

            RetryWord();
        }
    }

    private string PrintPipes()
    {
        string result = "[";
        int idx = 0;
        foreach (var elem in _slots)
        {
            string list = "";
            foreach (var elem2 in elem.Value)
            {
                list += $"{elem2.text.text}, ";
            }
            if (elem.Value.Count > 0)
                list = list.Remove(list.Length - 2);
            result += $"Pipe {elem.Key}: {list}, ";
            ++idx;
            //if(elem.Count > MaxPipeCapacity)
            //{
            //    Debug.LogError($"Pipe {idx-1} capacity exceeded.");
            //}
        }
        result = result.Remove(result.Length - 2);
        result += "]";
        return result;
    }

    void Update()
    {
        if (!_running || _moves == null || _currMoveIndex >= _moves.Count)
            return;

        float delta = Time.deltaTime * _speed;
        _timeSinceLastMove += delta;
        if (_timeSinceLastMove >= 1f && _autoSolve)
        {
            _timeSinceLastMove -= 1f;
            NextMove();

            if(_currMoveIndex >= _moves.Count && _autoChangeWord)
            {
                NextWord();
            }
        }
    }

    private void NextWord()
    {
        bool running = _running;
        _running = false;
        _moves = null;

        if (_moveCalculator.Failed || _failed)
        {
            RetryWord();
        }
        else
        {
            _failed = false;
            ++_currWordIndex;
            if (_currWordIndex < _words.Count)
            {
                _currWord = _words[_currWordIndex];

                Generate();

                _currMoveIndex = -1;

                _running = running;
            }
            else
            {
                Debug.Log("All words finished!");

                Save();
            }
        }
    }

    private void Save()
    {
        DateTime now = DateTime.Now;
        string fileName = $"GeneratedSeeds_{now.Year}_{now.Month}_{now.Day}-{now.Hour}_{now.Minute}_{now.Second}.txt";
        string file = $@"{Application.dataPath}/__Tests/AutoRunner/Seeds/{fileName}";
        if (!File.Exists(file))
        {
            File.Create(file).Close();
        }
        File.WriteAllText(file, JsonConvert.SerializeObject(_words, Formatting.Indented));
        AssetDatabase.Refresh();
    }

    private List<AutoRunnerWordInfo> Load(string fileName)
    {
        DateTime now = DateTime.Now;
        string file = $@"{Application.dataPath}/__Tests/AutoRunner/Seeds/{fileName}.txt";
        if (!File.Exists(file))
        {
            throw new Exception($"Can't Load! File not exists: {file}");
        }
        string json = File.ReadAllText(file);
        return JsonConvert.DeserializeObject<List<AutoRunnerWordInfo>>(json);
    }

    private void RetryWord()
    {
        _currWord.seed = -1;

        Debug.Log($"This word failed, retrying... CalcFailed:{_moveCalculator.Failed}; RunnerFailed:{_failed}");
        Generate();

        _currMoveIndex = -1;

        _running = true;
    }
}
#endif
