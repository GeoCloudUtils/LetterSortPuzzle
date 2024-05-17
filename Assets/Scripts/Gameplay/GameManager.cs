using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// Main game manager
    /// Handles eprubete spawn
    /// Handles letters spawn
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int eprubeteCount;

        [SerializeField] private float[] xPositions;

        [SerializeField] private float letterBallSize = 75f;

        [SerializeField] private RectTransform parent;

        [SerializeField] private ClickableEprubete eprubetePrefab;

        [SerializeField] private LetterBall letterBallPrefab;

        [SerializeField] private WordsDictionary wordsDictionary;

        [SerializeField] private ClickableEprubete selectedEprubete = null;

        [SerializeField] private LetterBall selectedLetter = null;

        [SerializeField] private bool isMoving = false;

        [SerializeField] private bool initialized = false;

        private List<ClickableEprubete> eprubeteList;

        public bool Initialized { get => initialized; private set => initialized = value; }

        private string word_1;
        private string word_2;

        private void Awake()
        {
            wordsDictionary.LoadFromJSON(Difficulty.EASY);
        }

        /// <summary>
        /// Initialize gameplay
        /// </summary>
        public void Initialize()
        {
            initialized = true;
            string[] words = wordsDictionary.GetWords();
            word_1 = words[0];
            word_2 = words[1];
            eprubeteList = new List<ClickableEprubete>();
            CreateEprubete();
            SpawnBallLetters();
        }

        /// <summary>
        /// Create eprubete at runtime
        /// </summary>
        private void CreateEprubete()
        {
            int height = word_1.Length;
            for (int i = 0; i < xPositions.Length; i++)
            {
                ClickableEprubete eprubete = Instantiate(eprubetePrefab);
                eprubeteList.Add(eprubete);
                for (int j = 0; j < height; j++)
                {
                    GameObject cell = new GameObject("letterCell");
                    cell.AddComponent<RectTransform>();
                    cell.transform.SetParent(eprubete.transform);
                    eprubete.AddCell(cell.transform);
                }
                eprubete.transform.SetParent(parent.transform);
                Vector3 pos = Vector3.zero;
                pos.x = xPositions[i];
                eprubete.transform.localPosition = pos;
                eprubete.OnSelect += HandleLetterSelect;
                eprubete.DispatchMoveComplete += MoveComplete;
            }
        }

        /// <summary>
        /// Ball move complete callback
        /// </summary>
        /// <param name="eprubete"></param>
        private void MoveComplete(ClickableEprubete eprubete)
        {
            if (IsComplete())
            {
                Debug.Log("Level Complete!");
                return;
            }
            isMoving = false;
        }

        private bool IsComplete()
        {
            int count = eprubeteList.Count(eprubete =>
            eprubete.GetVerticalString() == word_1 || eprubete.GetVerticalString() == word_2);
            return count > 1;
        }

        /// <summary>
        /// Letter selection callback
        /// </summary>
        /// <param name="letterBall"></param>
        /// <param name="eprubete"></param>
        private void HandleLetterSelect(LetterBall letterBall, ClickableEprubete eprubete)
        {
            if (isMoving) return;

            if (selectedEprubete == null)
            {
                if (letterBall != null)
                {
                    letterBall.DetachParent(eprubete);
                    letterBall.MoveUp();
                    selectedLetter = letterBall;
                    selectedEprubete = eprubete;
                }
            }
            else
            {
                if (!eprubete.IsFull())
                {
                    isMoving = true;
                    selectedEprubete.RemoveLetter(selectedLetter);
                    eprubete.DoLetterPath(selectedLetter);
                    selectedEprubete = null;
                    selectedLetter = null;
                }
            }
        }

        /// <summary>
        /// Spawn letters
        /// </summary>
        private void SpawnBallLetters()
        {
            string allLetters = word_1 + word_2;

            foreach (char letter in allLetters)
            {
                LetterBall letterBall = Instantiate(letterBallPrefab);
                letterBall.SetSize(letterBallSize);

                ClickableEprubete eprubete;
                do
                {
                    int randomCellIndex = Random.Range(0, eprubeteCount);
                    eprubete = parent.transform.GetChild(randomCellIndex).GetComponent<ClickableEprubete>();
                } while (eprubete.IsFull());

                eprubete.AddLetterBall(letterBall);
                letterBall.SetText(letter.ToString());
            }
        }

    }
}
