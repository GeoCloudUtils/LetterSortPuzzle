using System.Text;
using UnityEngine;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int eprubeteCount;

        [SerializeField] private float[] xPositions;

        [SerializeField] private float letterBallSize = 75f;

        [SerializeField] private RectTransform parent;

        [SerializeField] private ClickableEprubete eprubetePrefab;

        [SerializeField] private LetterBall letterBallPrefab;

        [SerializeField] private WordsDictionary wordsDictionary;

        [SerializeField] private string word = "mercedes";

        [SerializeField] private ClickableEprubete selectedEprubete = null;

        [SerializeField] private LetterBall selectedLetter = null;

        [SerializeField] private bool isMoving = false;

        [SerializeField] private bool initialized = false;

        public bool Initialized { get => initialized; private set => initialized = value; }

        private void Awake()
        {
            wordsDictionary.LoadFromJSON(Difficulty.EASY);
        }

        public void Initialize()
        {
            initialized = true;
            word = wordsDictionary.GetWord();
            Debug.Log(word);
            CreateEprubete();
            SpawnBallLetters();
        }

        private void CreateEprubete()
        {
            int height = word.Length;
            for (int i = 0; i < xPositions.Length; i++)
            {
                ClickableEprubete eprubete = Instantiate(eprubetePrefab);
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

        private void MoveComplete(ClickableEprubete eprubete)
        {
            if (eprubete.GetVerticalString().Equals(word))
            {
                Debug.Log("MATCH WORD COMPLETE!");
                return;
            }
            isMoving = false;
        }

        private void HandleLetterSelect(LetterBall letterBall, ClickableEprubete eprubete)
        {
            if (isMoving)
            {
                return;
            }
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
                if (eprubete.IsFull())
                {
                    return;
                }
                else
                {
                    isMoving = true;
                    selectedEprubete.RemoveLetter(selectedLetter);
                    eprubete.DoLetterPath(selectedLetter);
                    selectedEprubete = null;
                    selectedLetter = null;
                }
            }
        }

        private void SpawnBallLetters()
        {
            char[] wordLetters = word.ToCharArray();

            string additionalLetters = GenerateRandomLetters(5);
            string allLetters = new string(wordLetters) + additionalLetters;

            for (int i = 0; i < allLetters.Length; i++)
            {
                LetterBall letterBall = Instantiate(letterBallPrefab);
                letterBall.SetSize(letterBallSize);

                int randomCellIndex;
                ClickableEprubete eprubete;
                do
                {
                    randomCellIndex = Random.Range(0, eprubeteCount);
                    eprubete = parent.transform.GetChild(randomCellIndex).GetComponent<ClickableEprubete>();
                } while (eprubete.IsFull());

                eprubete.AddLetterBall(letterBall);
                letterBall.SetText(allLetters[i].ToString());
            }
        }

        private string GenerateRandomLetters(int count)
        {
            string letters = "abcdefghijklmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < count; i++)
            {
                char randomChar = letters[Random.Range(0, letters.Length)];
                result.Append(randomChar);
            }
            return result.ToString();
        }
    }
}
