using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    [SerializeField] private int _pipesCount = 4;
    private List<Stack<char>> Pipes;
    private int _target;
    private List<MoveAction> _moves;

    private void OnValidate()
    {
        if (_run)
        {
            _run = false;
            Generate(_word, _pipesCount);
            CalculateMoves();
        }
    }

    /// <summary>
    /// Generate the pipes.
    /// </summary>
    public List<Stack<char>> Generate(string word, int pipesCount)
    {
        // Set.
        if (pipesCount < 3)
        {
            throw new Exception("Please set pipesCount >= 3!");
        }
        _pipesCount = pipesCount;
        _word = word;

        // Add Letters to Pipes.
        Pipes = new List<Stack<char>>();
        for (int a = 0; a < _pipesCount; a++)
        {
            Pipes.Add(new Stack<char>());
        }
        for (int i = 0; i < _word.Length; i++)
        {
            Pipes[Random.Range(0, Pipes.Count)].Push(_word[i]);
        }

        // Find best target.
        int bestTarget = 0;
        int bestCorrectLetters = 0;
        bool bestTargetFound = false;
        string revWord = ReverseString(_word);
        //Debug.Log($"Word:{_word}; RevWord:{revWord};");
        for (int a = 0; a < _pipesCount; a++)
        {
            var list = Pipes[a].ToList();
            int correctLetters = 0;
            //Debug.Log($"Pipe {a}. Pipe:{string.Join(",", list)}");
            for (int b =0; b < list.Count; ++b)
            {
                //Debug.Log($"PipeElem {b}. Elem:{list[b]}; WordElem:{_word[b]}; RevWordElem:{revWord[b]};");
                if (list[b] == revWord[b])
                {
                    ++correctLetters;
                }
                else
                    break;
            }
            if(correctLetters > 0 && bestCorrectLetters < correctLetters)
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

        Debug.Log($"Initial State of Pipes: {PrintPipes()}");

        return Pipes;
    }

    /// <summary>
    /// Calculate moves to solve the puzzle.
    /// </summary>
    public List<MoveAction> CalculateMoves()
    {
        if (Pipes == null)
        {
            throw new Exception("Use Generate() first!");
        }

        _moves = new List<MoveAction>();

        // Phase 1: Move all letters from Target to other pipes.
        RunPhaseOne();

        // Phase 2: Move letters in correct order from other pipes to Target.
        RunPhaseTwo();

        //Debug.Log($"Final State of Pipes(moves={result.Count}): {PrintPipes()}");

        // Clear.
        Pipes = null;
        _target = -1;

        return _moves;
    }

    /// <summary>
    /// Phase 2: Move letters in correct order from other pipes to Target.
    /// </summary>
    private void RunPhaseTwo()
    {
        int iterations = 200;
        int searchLetterIdx = _word.Length - 1;
        while (searchLetterIdx >= 0 && iterations-- > 0)
        {
            for (int source = 0; source < _pipesCount; source++)
            {
                if (source == _target)
                    continue;

                List<int> aux = new List<int>();
                for (int a = 0; a < _pipesCount; a++)
                {
                    if (a != _target && a != source)
                        aux.Add(a);
                }
                searchLetterIdx = Process(source, _target, aux, searchLetterIdx);
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
            if (a != _target)
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
            Move(src, dst);
        }

        if (iterations <= 0)
        {
            throw new Exception($"Max iterations occurred in Phase 1(TargetPipeClean).");
        }
        Debug.Log($"Medium State of Pipes: {PrintPipes()}; CountToSkip:{countToSkip};");
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
        //Debug.Log($"Process. Source:{source}; Target:{_target}; Aux:{string.Join(",", aux.ToArray())}; Pipes:{PrintPipes()}");
        foreach (char letter in Pipes[source].ToArray())
        {
            char elem = Pipes[source].Peek();
            if (elem == _word[searchLetterIdx])
            {
                --searchLetterIdx;
                Move(source, target);
            }
            else
            {
                int auxIdx = aux[aux.Count == 1 ? 0 : Random.Range(0, aux.Count)];
                Move(source, auxIdx);
            }
        }
        return searchLetterIdx;
    }

    /// <summary>
    /// Move a letter from a Source to Destination.
    /// </summary>
    private void Move(int src, int dst)
    {
        char elem = Pipes[src].Pop();
        Pipes[dst].Push(elem);
        _moves.Add(new MoveAction(src, dst, _word.IndexOf(elem)));
        Debug.Log($"Moved '{elem}' from {src} to {dst}; Pipes:{PrintPipes()}");
    }

    private string PrintPipes()
    {
        string result = "[";
        int idx = 0;
        foreach(var elem in Pipes)
        {
            result += $"Pipe {idx}: {string.Join(",", elem.ToArray())}, ";
            ++idx;
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
}

