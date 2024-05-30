//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using static MoveCalculator;
//using Random = UnityEngine.Random;

//public class MoveCalculator2
//{
//    static string logs = "";
//    static MoveCalculator2()
//    {
//        Application.logMessageReceived += Application_logMessageReceived;
//    }

//    private static void Application_logMessageReceived(string condition, string stackTrace, LogType type)
//    {
//        logs += $"{condition}\n";
//        Debug.Log(logs);
//    }

//    public static List<MoveAction> Run(int numDisks=5)
//    {
//        logs = "";

//        List<Stack<int>> pegs = new List<Stack<int>>();

//        // Initialize pegs
//        for (int i = 0; i < 4; i++)
//        {
//            pegs.Add(new Stack<int>());
//        }

//        RandomizeDisks(pegs[0], numDisks); // Randomly distribute disks on the initial peg

//        // Display initial state of pegs
//        for (int i = 0; i < 4; i++)
//        {
//            Debug.Log("Initial Peg " + i + ": " + string.Join(" ", pegs[i].ToArray()));
//        }

//        // Specify source, auxiliary pegs, and destination peg
//        int source = 0;
//        int target = 3;
//        List<int> auxiliary = new List<int>() { 1, 2 };
//        Debug.Log($"Source:{source}; Target:{target}; Aux:{string.Join(",", auxiliary.ToArray())}");

//        // Solve the Towers of Hanoi problem
//        SolveHanoi(pegs, numDisks, source, target, auxiliary);

//        // Display final state of pegs
//        for (int i = 0; i < 4; i++)
//        {
//            Debug.Log("Peg " + i + ": " + string.Join(" ", pegs[i].ToArray()));
//        }

//        return null; // Placeholder, modify as needed
//    }

//    static void RandomizeDisks(Stack<int> peg, int numDisks)
//    {
//        List<int> shuffledDisks = new List<int>();
//        for (int i = numDisks; i >= 1; i--)
//        {
//            shuffledDisks.Add(i);
//        }
//        Shuffle(shuffledDisks);
//        foreach (int value in shuffledDisks)
//        {
//            peg.Push(value);
//        }
//    }

//    static void Shuffle(List<int> list)
//    {
//        int n = list.Count;
//        System.Random rng = new System.Random();
//        while (n > 1)
//        {
//            n--;
//            int k = rng.Next(n + 1);
//            int value = list[k];
//            list[k] = list[n];
//            list[n] = value;
//        }
//    }

//    static void SolveHanoi(List<Stack<int>> pegs, int n, int source, int target, List<int> auxiliary)
//    {
//        if (n == 1)
//        {
//            MoveDisk(pegs, source, target);
//            return;
//        }

//        SolveHanoi(pegs, n - 1, source, auxiliary[0], new List<int> { auxiliary[1], target });
//        MoveDisk(pegs, source, target);
//        SolveHanoi(pegs, n - 1, auxiliary[0], target, new List<int> { source, auxiliary[1] });
//    }

//    static void MoveDisk(List<Stack<int>> pegs, int fromPeg, int toPeg)
//    {
//        if (pegs[fromPeg].Count == 0)
//        {
//            throw new InvalidOperationException("Cannot move a disk from an empty peg.");
//        }

//        int disk = pegs[fromPeg].Pop();
//        pegs[toPeg].Push(disk);
//    }
//}


//public class HanoiMove
//{
//    public int NumDisks { get; }
//    public int Source { get; }
//    public int Target { get; }
//    public List<int> Auxiliary { get; }

//    public HanoiMove(int numDisks, int source, int target, List<int> auxiliary)
//    {
//        NumDisks = numDisks;
//        Source = source;
//        Target = target;
//        Auxiliary = auxiliary;
//    }
//}


