//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using Random = UnityEngine.Random;

//public class MoveCalculator //: MonoBehaviour
//{
//    public class MoveAction
//    {
//        public int from;
//        public int to;
//        public int disk;

//        public MoveAction(int from, int to, int disk)
//        {
//            this.from = from;
//            this.to = to;
//            this.disk = disk;
//        }

//        public MoveAction()
//        {
//        }
//    }

//    [SerializeField] private string _word = "TOY";
//    [SerializeField] private int _towersCount = 4;
//    [SerializeField] private bool _run;
//    public List<Stack<char>> Towers { get; private set; }

//    private void OnValidate()
//    {
//        if (_run)
//        {
//            _run = false;
//            Run(_word);
//        }
//    }

//    /// <summary>
//    /// 
//    /// * Solve towers of hanoi from an arbitrary position
//    /// * 
//    /// * @param diskPositions the current peg for each disk (0, 1, or 2) in increasing
//    /// *                      order of size.  This will be modified
//    /// * @param disksToMove  number of smallest disks to moves
//    /// * @param targetPeg target peg for disks to move
//    /// </summary>
//    /// <param name="diskPositions">Positions of disks. Disk 0: Tower M, Disk 1: Tower N, etc</param>
//    /// <param name="disksToMove">Number of smallest disks to moves.</param>
//    /// <param name="targetTower">Temp parameter.</param>
//    /// <param name="sumOfDisks">Sum of the disk indexes.</param>
//    /// <param name="actions"></param>
//    private void CalculateMoves(int[] diskPositions, int disksToMove, int targetTower, int sumOfDisks, List<MoveAction> actions)
//    {
//        for (int badDisk = disksToMove - 1; badDisk >= 0; --badDisk)
//        {
//            int currentTower = diskPositions[badDisk];
//            if (currentTower != targetTower)// Found the largest Element on the wrong tower.
//            {
//                // Sum of the disk indexes is sumOfDisks, so to find the other one:
//                int otherPeg = sumOfDisks - targetTower - currentTower;
//                // otherPeg = (15 - 3 - 1)

//                // Before we can move badElement, we have to get the smaller ones out of the way
//                CalculateMoves(diskPositions, badDisk, otherPeg, sumOfDisks, actions);

//                // Move.
//                diskPositions[badDisk] = targetTower;
//                Debug.Log("Move " + badDisk + " from " + currentTower + " to " + targetTower);
//                actions.Add(new MoveAction() { disk = badDisk, from = currentTower, to = targetTower });

//                // Now we can put the smaller ones in the right place.
//                CalculateMoves(diskPositions, badDisk, targetTower, sumOfDisks, actions);
//                break;
//            }
//        }
//    }

//    private void CalculateMoves(int[] disks, int target, List<MoveAction> actions)
//    {
//        int n = disks.Length;
//        // Calculate the next target rod for each of the disks
//        //  int target = 2; // The biggest disk should go to the rightmost rod
//        int[] targets = new int[n];
//        for (int a = n - 1; a >= 0; a--)
//        {
//            targets[a] = target;
//            if (disks[a] != target)
//            {
//                // To allow for this move, the smaller disk needs to get out of the way
//                target = _towersCount - target - disks[a];
//            }
//        }
//        int i = 0;
//        while (i < n)
//        { // Not yet solved?
//          // Find the disk that should move
//            for (i = 0; i < n; i++)
//            {
//                if (targets[i] != disks[i])
//                { // Found it
//                    target = targets[i]; // This and smaller disks should pile up here 
//                    Debug.LogFormat("move disk {0} from rod {1} to {2}\n", i, disks[i], target);
//                    actions.Add(new MoveAction() { disk = i, from = disks[i], to = target });
//                    disks[i] = target; // Make move
//                                       // Update the next targets of the smaller disks
//                    for (int j = i - 1; j >= 0; j--)
//                    {
//                        targets[j] = target;
//                        target = _towersCount - target - disks[j];
//                    }
//                    break;
//                }
//            }
//        }
//    }
//    public List<MoveAction> Run(string word)
//    {
//        return MoveCalculator2.Run();

//        return new List<MoveAction>() ;

//        char[] chars = word.ToCharArray();

//        // Create towers.
//        var towers = new List<Stack<char>>();
//        for (int a = 0; a < _towersCount; a++)
//        {
//            towers.Add(new Stack<char>());
//        }

//        // Add chars to towers.
//        List<int> charIndexes = new List<int>();
//        for (int a = 0; a < chars.Length; ++a)
//        {
//            int towerIdx = Random.Range(0, _towersCount);
//            charIndexes.Add(towerIdx);
//            towers[towerIdx].Push(chars[a]);
//        }
//        charIndexes.Reverse();

//        // Add fixed meta.
//        List<List<bool>> fixedLetters = new List<List<bool>>();
//        for (int a = 0; a < _towersCount; a++)
//        {
//            List<bool> list = new List<bool>();
//            fixedLetters.Add(list);
//            for (int b = 0; b < towers[a].Count; ++b)
//            {
//                list.Add(false);
//            }
//        }

//        // Print towers.
//        Print(towers);

//        // Find fixed.
//        int maxFixed = -1;
//        int fixedPlace = -1;
//        int fixedTower = -1;
//        for (int a = 0; a < _towersCount; a++)
//        {
//            if (towers[a].Count == 0)
//                continue;

//            int fixedCount = 0;
//            int placeIdx = 0;
//            int towerIdx = 0;
//            for (int b = 0; b < towers[a].Count; ++b)
//            {
//                if (towers[a].Reverse().ToList()[b] == word[b])
//                {
//                    Debug.Log($"[Test_CalcMoveCount] Found correct letter. Tower:{a}; Place: {b}; Letter:{word[b]}");
//                    fixedLetters[a][b] = true;
//                    placeIdx = b;
//                    towerIdx = a;
//                    ++fixedCount;
//                }
//                else
//                {
//                    break;
//                }
//            }
//            if (fixedCount != 0 && fixedCount > maxFixed)
//            {
//                fixedPlace = placeIdx;
//                fixedTower = towerIdx;
//                maxFixed = fixedCount;
//            }
//        }

//        Debug.Log($"[Test_CalcMoveCount] Max fixed. Tower:{fixedTower}; Place: {fixedPlace};");

//        Towers = new List<Stack<char>>();
//        foreach(var tower in towers)
//        {
//            Towers.Add(new Stack<char>(tower.ToList()));
//        }

//        List<MoveAction> result = new List<MoveAction>();
//        int sumOfDisks = (1+2+3+4);// (4 * (4 + 1) / 2);//charIndexes.Count * (charIndexes.Count + 1) / 2;
//        CalculateMoves(charIndexes.ToArray(), charIndexes.Count, Math.Max(0, fixedTower), sumOfDisks, result);
//        //CalculateMoves(charIndexes.ToArray(), Math.Max(0, fixedTower), result);
//        Print(towers, new MoveAction() { from = -1, to = -1 });
//        foreach (var elem in result)
//        {
//            if (elem.from < 0 || elem.to < 0)
//                continue;

//            Stack<char> from = towers[elem.from];
//            Stack<char> to = towers[elem.to];

//            // char elemValue = from[from.Count - 1];

//            char elemValue = from.Pop();
//            to.Push(elemValue);

//            // Print
//            Print(towers, elem);
//        }
//        Debug.Log($"[Test_CalcMoveCount] Word:{_word}; LettersCount:{_word.Length}; Total Moves: {result.Count}.");

//        return result;
//    }

//    private void Print(List<Stack<char>> towers, MoveAction elem = null)
//    {
//        string msgToPrint = "[";
//        for (int a = 0; a < _towersCount; a++)
//        {
//            var arr = towers[a].Reverse().ToArray();
//            msgToPrint += $"Tower {a}: {(towers[a].Count == 0 ? "--" : new string(arr))}, ";
//        }
//        msgToPrint = msgToPrint.Remove(msgToPrint.Length - 2);
//        msgToPrint += "]";
//        if (elem != null)
//            Debug.Log($"[Test_CalcMoveCount] Data:{elem.from}->{elem.to}; {msgToPrint}");
//        else
//            Debug.Log($"[Test_CalcMoveCount] {msgToPrint}");
//    }
//}
