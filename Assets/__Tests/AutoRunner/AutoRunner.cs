using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AutoRunner : MonoBehaviour
{
    [SerializeField] private Button _regenerate;
    [SerializeField] private Button _autoRunToEnd;
    [SerializeField] private Button _runNext;
    [SerializeField] private Button _stopAutoRun;
    [SerializeField] private TMP_Text _seedText;
    [SerializeField] private float _speed;
    [SerializeField] private string _word;
    [SerializeField] private int _pipes;
    [SerializeField] private AutoRunnerLetter _slotPrefab;
    [SerializeField] private List<RectTransform> _tubes;
    [SerializeField] private MovesCalculator _moveCalculator;
    private List<AutoRunnerLetter> _spawnedLetters = new List<AutoRunnerLetter>();
    private bool _running = false;
    private System.Random _rand = new System.Random();
    private int _seed = -1;
    private float _timeSinceLastMove = 1f;
    private List<MoveAction> _moves;
    private int _currMoveIndex;
    private bool _autoRun = false;

    private Dictionary<int, Stack<AutoRunnerLetter>> _slots = new Dictionary<int, Stack<AutoRunnerLetter>>();

    void Start()
    {
        _regenerate.onClick.AddListener(OnRegenerateClicked);
        _autoRunToEnd.onClick.AddListener(OnAutoRunToEndClicked);
        _runNext.onClick.AddListener(OnRunNextClicked);
        _stopAutoRun.onClick.AddListener(OnStopAutoRunClicked);
    }

    private void OnRegenerateClicked()
    {
        Generate(true);
    }

    private void OnRunNextClicked()
    {
        NextMove();
    }

    private void OnStopAutoRunClicked()
    {
        _autoRun = false;
    }

    private void OnAutoRunToEndClicked()
    {
        _autoRun = true;
    }

    private void Generate(bool newSeed)
    {
        _running = true;
        _moves = null;
        if (newSeed || _seed == -1)
        {
            _seed = _rand.Next(int.MaxValue);
            Random.InitState(_seed);
            _seedText.text = $"Seed:{_seed}";
        }

        // Remove slots.
        foreach(var pair in _slots)
        {
            foreach(var elem in pair.Value)
            {
                Destroy(elem.gameObject);
            }
        }

        List<Stack<char>> pipes = _moveCalculator.Generate(_word, _pipes);

        _slots = new Dictionary<int, Stack<AutoRunnerLetter>>();
        for (int a = 0; a < pipes.Count; a++)
        {
            CreateSlots(pipes[a], _tubes[a], a);
        }

        _moves = _moveCalculator.CalculateMoves();

        _currMoveIndex = -1;
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
        int from = action.from;
        int to = action.to;
        var elem = _slots[from].Pop();
        _slots[to].Push(elem);
        elem.transform.SetParent(_tubes[to]);
        elem.transform.SetAsFirstSibling();
    }

    void Update()
    {
        if (!_running || _moves == null || _currMoveIndex >= _moves.Count)
            return;

        float delta = Time.deltaTime * _speed;
        _timeSinceLastMove += delta;
        if (_timeSinceLastMove >= 1f && _autoRun)
        {
            _timeSinceLastMove -= 1f;
            NextMove();
        }
    }
}
