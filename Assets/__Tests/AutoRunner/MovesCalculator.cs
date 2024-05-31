using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.XR;
using Random = UnityEngine.Random;

public class MoveAction
{
    public int from;
    public int to;
    public int disk;

    public MoveAction(int from, int to, int disk)
    {
        this.from = from;
        this.to = to;
        this.disk = disk;
    }

    public MoveAction() { }
}

public class MovesCalculator : MonoBehaviour
{
    [SerializeField] private bool _run = false;
    [SerializeField] private string _word;
    [SerializeField] private string _word2;
    [SerializeField] private int _pipesCount = 4;
    [SerializeField] private int _seed = 4324324;
    [SerializeField] private bool _autoSeed = true;
    [SerializeField] private string _randomLetters;
    private int _maxSlotsUsed = 0;
    private List<Stack<char>> Pipes;
    private int _target;
    private List<MoveAction> _moves;
    private int _frozenPipe = -1;
    private string _currWord;
    private bool _failed;

    private int MaxPipeCapacity => _word.Length + 
        (string.IsNullOrEmpty(_randomLetters)?0:_randomLetters.Length);

    public bool Failed => _failed;


    private void OnValidate()
    {
        if (_run)
        {
            _run = false;
            _frozenPipe = -1;
            _maxSlotsUsed = 0;


            if (_autoSeed)
            {
                System.Random rand = new System.Random();
                _seed = rand.Next(int.MaxValue);
            }

            Generate(_word+_word2, _randomLetters, _pipesCount, _seed);

            CalculateMoves(_word, _word2);
        }
    }

    /// <summary>
    /// Generate the pipes.
    /// </summary>
    public List<Stack<char>> Generate(string word, string randomLetters, int pipesCount, int seed)
    {
        try
        {
            Random.InitState(seed);
            _seed = seed;

            // Set.
            if (pipesCount < 3)
            {
                throw new Exception("Please set pipesCount >= 3!");
            }
            _randomLetters = randomLetters;
            _seed = seed;
            _pipesCount = pipesCount;
            _currWord = word;
            _maxSlotsUsed = 0;
            _frozenPipe = -1;
            _failed = false;

            // Add Letters to Pipes.
            Pipes = new List<Stack<char>>();
            for (int a = 0; a < _pipesCount; a++)
            {
                Pipes.Add(new Stack<char>());
            }
            for (int i = 0; i < _currWord.Length; i++)
            {
                Pipes[Random.Range(0, Pipes.Count)].Push(_currWord[i]);
            }

            // Add random letters.
            if (!string.IsNullOrEmpty(_randomLetters))
            {
                for (int i = 0; i < _randomLetters.Length; i++)
                {
                    Pipes[Random.Range(0, Pipes.Count)].Push(_randomLetters[i]);
                }
            }

            // Find best target.
            int bestTarget = 0;
            int bestCorrectLetters = 0;
            bool bestTargetFound = false;
            string revWord = ReverseString(_currWord);
            //Debug.Log($"Word:{_word}; RevWord:{revWord};");
            for (int a = 0; a < _pipesCount; a++)
            {
                var list = Pipes[a].ToList();
                int correctLetters = 0;
                //Debug.Log($"Pipe {a}. Pipe:{string.Join(",", list)}");
                for (int b = 0; b < list.Count; ++b)
                {
                    //Debug.Log($"PipeElem {b}. Elem:{list[b]}; WordElem:{_word[b]}; RevWordElem:{revWord[b]};");
                    if (list[b] == revWord[b])
                    {
                        ++correctLetters;
                    }
                    else
                        break;
                }
                if (correctLetters > 0 && bestCorrectLetters < correctLetters)
                {
                    bestCorrectLetters = correctLetters;
                    bestTarget = a;
                    bestTargetFound = true;
                }
            }

            // If best target not found, choose random.
            if (bestTargetFound)
                _target = bestTarget;
            else
                _target = Random.Range(0, _pipesCount);

            //Debug.Log($"Target:{_target}; BestTarget:{bestTarget}; BestLetters:{bestCorrectLetters}; BestTargetFound:{bestTargetFound};");

            //Debug.Log($"Initial State of Pipes: {PrintPipes()}");

            return Pipes;
        }
        catch (Exception e)
        {
            CheckFailed(e.Message);
            return null;
        }
    }

    public List<MoveAction> CalculateMoves(string word1, string word2)
    {
        try
        {
            Debug.Log($"---- CalculateMoves Started. Word1:{word1}; Word2:{word2}; ----");
            
            // Set.
            _word = word1;
            _word2 = word2;
            _moves = new List<MoveAction>();

            // Run Word 1.
            _currWord = _word;
            Run();

            //Debug.Log("--------------------------------");
            //Debug.Log("--------------------------------");

            // Freeze pipe with Word 1.
            _currWord = _word2;
            _frozenPipe = _target;
            do
            {
                _target = Random.Range(0, _pipesCount);
            }
            while (_frozenPipe == _target);

            // Run Word 2.
            Run();

            Debug.Log($"---- CalculateMoves Finished. TotalMoves:{_moves.Count}; Failed:{_failed}; Seed:{_seed}; Pipes:{PrintPipes()} ----");

            return _moves;

        }
        catch (Exception e)
        {
            CheckFailed(e.Message);
            return null;
        }
    }

    /// <summary>
    /// Calculate moves to solve the puzzle.
    /// </summary>
    private List<MoveAction> Run()
    {
        //Debug.Log("---- Run Start ----");

        if (Pipes == null)
        {
            CheckFailed("Use Generate() first!");
            return null;
        }

        // Phase 1: Move all letters from Target to other pipes.
        RunPhaseOne();

        // Phase 2: Move letters in correct order from other pipes to Target.
        RunPhaseTwo();

        //Debug.Log($"Final State of Pipes(moves={result.Count}): {PrintPipes()}");

        // Clear.
        // Pipes = null;
        // _target = -1;

        Debug.Log($"---- Run Finished. Moves:{_moves.Count}; Failed:{_failed}; Seed:{_seed}; Pipes:{PrintPipes()} ----");

        return _moves;
    }

    /// <summary>
    /// Phase 2: Move letters in correct order from other pipes to Target.
    /// </summary>
    private void RunPhaseTwo()
    {
        int iterations = 200;
        int searchLetterIdx = _currWord.Length - 1;
        while (searchLetterIdx >= 0 && iterations-- > 0)
        {
            for (int source = 0; source < _pipesCount; source++)
            {
                if (source == _target || source == _frozenPipe)
                    continue;

                List<int> aux = new List<int>();
                for (int a = 0; a < _pipesCount; a++)
                {
                    if (a != _target && a != source && a != _frozenPipe)
                        aux.Add(a);
                }
                searchLetterIdx = Process(source, _target, aux, searchLetterIdx);

                if (searchLetterIdx < 0)
                    break;
            }
        }
        if (iterations <= 0)
        {
            throw new Exception($"Max iterations occurred in Phase 2(Search).");
        }
    }

    /// <summary>
    /// Phase 1: Move all letters from Target to other pipes.
    /// </summary>
    private void RunPhaseOne()
    {
        List<int> aux = new List<int>();
        for (int a = 0; a < _pipesCount; a++)
        {
            if (a != _target && a != _frozenPipe)
                aux.Add(a);
        }
        //Debug.Log($"Target:{_target}; Aux:{string.Join(",", aux.ToArray())}");

        // Count nr. of correct letters to freeze in target.
        int countToSkip = 0;
        //var list = Pipes[_target].ToList();
        //int count = Pipes[_target].Count;
        //string revWord = ReverseString(_word);
        //for (int a=0; a < list.Count; ++a)
        //{
        //    Debug.Log($"Searching to Skip. PipeElem: {list[a]}; WordElem:{_word[a]}; RevWordElem:{ReverseString(_word)[a]};");
        //    if (list[a] == revWord[a])
        //        ++countToSkip;
        //    else
        //        break;
        //}

        int iterations = 200;
        while (Pipes[_target].Count > countToSkip && iterations-- > 0)
        {
            int src = _target;
            int dst = aux[Random.Range(0, aux.Count)];
            if (Pipes[_target].Count < MaxPipeCapacity)
                Move(src, dst, 1);
            else
                CheckFailed($"Destination {dst} capacity exceeded.");
        }

        if (iterations <= 0)
        {
            throw new Exception($"Max iterations occurred in Phase 1(TargetPipeClean).");
        }
        //Debug.Log($"Medium State of Pipes: {PrintPipes()}; CountToSkip:{countToSkip};");
    }

    /// <summary>
    /// Process a Pipe. Moves letters to Target if correct letter found, else move to an auxiliary Pipe.
    /// </summary>
    /// <param name="source">Pipe to be processed.</param>
    /// <param name="target">Target pipe where the correct letters should go.</param>
    /// <param name="aux">Auxiliary pipes if letters are incorrect.</param>
    /// <param name="searchLetterIdx">Current correct letter searching.</param>
    /// <returns>The new letter index. If progressed, a new index will be. If not, the same.</returns>
    private int Process(int source, int target, List<int> aux, int searchLetterIdx)
    {
        //Debug.Log($"Process. Source:{source}; Target:{_target}; LetterIdx:{searchLetterIdx}; Word:{_currWord}; Aux:{string.Join(",", aux.ToArray())}; Pipes:{PrintPipes()}");
        foreach (char letter in Pipes[source].ToArray())
        {
            char elem = Pipes[source].Peek();
            if (elem == _currWord[searchLetterIdx])
            {
                --searchLetterIdx;
                Move(source, target);
            }
            else
            {
                int iterations = 50;
                int auxIdx = 0;
                do
                {
                    auxIdx = aux[aux.Count == 1 ? 0 : Random.Range(0, aux.Count)];
                    if (Pipes[auxIdx].Count < MaxPipeCapacity)
                        break;
                }
                while (iterations-- > 0);
                Move(source, auxIdx);
            }

            if (searchLetterIdx < 0)
                break;
        }
        return searchLetterIdx;
    }

    /// <summary>
    /// Move a letter from a Source to Destination.
    /// </summary>
    private void Move(int src, int dst, int phase=2)
    {
        char elem = Pipes[src].Pop();
        Pipes[dst].Push(elem);
        _moves.Add(new MoveAction(src, dst, _currWord.IndexOf(elem)));
        //Debug.Log($"[Phase {phase}] Moved '{elem}' from {src} to {dst}; Moves:{_moves.Count}; Pipes:{PrintPipes()}");

        if (Pipes[dst].Count > MaxPipeCapacity)
        {
            CheckFailed($"Pipe {dst} capacity exceeded.");
        }

        if (Pipes[dst].Count > _maxSlotsUsed)
        {
            _maxSlotsUsed = Pipes[dst].Count;
        }
    }

    private string PrintPipes()
    {
        string result = "[";
        int idx = 0;
        foreach(var elem in Pipes)
        {
            result += $"Pipe {idx}: {string.Join(",", elem.ToArray())}, ";
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

    private string ReverseString(string Input)
    {
        char[] charArray = Input.ToCharArray();
        string reversedString = string.Empty;
        for (int i = charArray.Length - 1; i > -1; i--)
            reversedString += charArray[i];
        return reversedString;
    }

    private void CheckFailed(string msg)
    {
        Debug.LogError(msg);
        _failed = true;
    }
}

