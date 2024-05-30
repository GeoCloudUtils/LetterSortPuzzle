using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gameplay
{
    /// <summary>
    /// Main game manager
    /// Handles eprubete spawn
    /// Handles letters spawn
    /// </summary>
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private int eprubeteCount;

        [SerializeField] private float letterBallSize = 75f;

        [SerializeField] private RectTransform parent;

        [SerializeField] private ClickableEprubete eprubetePrefab;

        [SerializeField] private LetterBall letterBallPrefab;

        [SerializeField] private WordsDictionary wordsDictionary;

        private ClickableEprubete selectedEprubete = null;

        private LetterBall selectedLetter = null;

        [SerializeField] private bool isMoving = false;

        [SerializeField] private bool initialized = false;

        private List<ClickableEprubete> eprubeteList;

        public bool Initialized { get => initialized; private set => initialized = value; }

        public string FirstWord { get => firstWord; private set => firstWord = value; }
        public string SecondWord { get => secondWord; private set => secondWord = value; }

        private string firstWord;
        private string secondWord;

        public bool canClick = false;

        private void Awake()
        {
            wordsDictionary.LoadFromJSON(Difficulty.EASY);
            GameDataManager.Instance.LoadGame();
        }

        /// <summary>
        /// Initialize gameplay
        /// </summary>
        public async Task InitializeAsync()
        {
            initialized = true;
            string[] words = wordsDictionary.GetWords();
            FirstWord = words[0];
            SecondWord = words[1];
            eprubeteList = new List<ClickableEprubete>();
            await CreateEprubeteAsync(eprubeteCount);
            await SpawnBallLettersAsync();
        }

        /// <summary>
        /// Create eprubete at runtime
        /// </summary>
        private async Task CreateEprubeteAsync(int count)
        {
            int height = FirstWord.Length + 1;
            for (int i = 0; i < count; i++)
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
                eprubete.transform.localScale = Vector3.one;
                eprubete.OnSelect += HandleLetterSelect;
                eprubete.DispatchMoveComplete += MoveComplete;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
            await Task.Yield();
        }

        /// <summary>
        /// Ball move complete callback
        /// </summary>
        /// <param name="eprubete"></param>
        private void MoveComplete(ClickableEprubete eprubete)
        {
            if (IsComplete())
            {
                if (PlayerPrefs.GetInt("LEVEL_UP", 0) == 0)
                {
                    PlayerPrefs.SetInt("LEVEL_UP", 1);
                }
                SceneManager.LoadScene("Main");
                return;
            }
            string verticalString = eprubete.GetVerticalString();
            string reversedString = new string(verticalString.Reverse().ToArray());

            if (verticalString == FirstWord || verticalString == SecondWord || reversedString == FirstWord || reversedString == SecondWord)
            {
                eprubete.Lock();
            }
            else
            {
                foreach (ClickableEprubete ep in eprubeteList)
                {
                    ep.CheckEprubeteLetters(firstWord);
                }
            }
            isMoving = false;
        }


        /// <summary>
        /// Check for level complete
        /// </summary>
        /// <returns></returns>
        private bool IsComplete()
        {
            int count = eprubeteList.Count(eprubete =>
            eprubete.GetVerticalString() == FirstWord || eprubete.GetVerticalString() == SecondWord);
            return count > 1;
        }

        /// <summary>
        /// Letter selection callback
        /// </summary>
        /// <param name="letterBall"></param>
        /// <param name="eprubete"></param>
        private void HandleLetterSelect(LetterBall letterBall, ClickableEprubete eprubete)
        {
            if (isMoving || !canClick) return;

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

        public async Task AddPipe()
        {
            await CreateEprubeteAsync(1);
        }

        /// <summary>
        /// Spawn letters
        /// </summary>
        private async Task SpawnBallLettersAsync()
        {
            foreach (char letter in GetLetters())
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
            foreach (ClickableEprubete eprubete in eprubeteList)
            {
                eprubete.CheckEprubeteLetters(firstWord);
            }
            await Task.Yield();
        }

        public List<char> GetLetters()
        {
            List<char> lettersList = new List<char>(firstWord + secondWord);
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            List<char> shuffledAlphabet = alphabet.ToCharArray().OrderBy(x => UnityEngine.Random.value).ToList();
            int randomCount = UnityEngine.Random.Range(1, 3);
            for (int i = 0; i <= randomCount; i++)
            {
                lettersList.Add(shuffledAlphabet[i]);
            }
            return lettersList;
        }
    }
}
