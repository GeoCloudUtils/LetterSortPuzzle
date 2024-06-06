using DG.Tweening;
using Gameplay;
using Lean.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIGameplayScreen : UIScreen
    {
        [SerializeField] private TMP_Text debugText;

        [SerializeField] private AdMobController adMobController;

        [SerializeField] private BottomButtonsHandler bottomHandler;

        [SerializeField] private Backgrounds backgrounds;

        [SerializeField] private WinPanel winPanel;

        [SerializeField] private Image topBgImage;
        [SerializeField] private Image bodyIamge;

        [SerializeField] private UIMainScreenController mainScreen;

        [SerializeField] private GameplayManager gameManager;

        [SerializeField] private RectTransform firstWordLayout;
        [SerializeField] private RectTransform secondWordLayout;

        [SerializeField] private WordCell wordCellPrefab;

        [SerializeField] private List<WordCell> wordList;

        [SerializeField] private DOTweenAnimation rootTween;

        private bool levelIsInitialized = false;
        private bool canClick = true;

        public event Action MoveNext;
        private void Start()
        {
            bottomHandler.OnHomeClick += ShowHomeScreen;
            bottomHandler.OnHighlightClick += Highlight;
            bottomHandler.OnTipsClick += ShowTips;
            bottomHandler.OnAddClick += AddPipe;
            winPanel.OnMoveNext += HandleMoveNext;
        }

        private void HandleMoveNext()
        {
            MoveNext?.Invoke();
        }

        private void AddPipe()
        {
            adMobController.ShowAd(AdType.Rewarded, OnRewardedAddFinished);
        }

        private void OnRewardedAddFinished(bool success, string id)
        {
            if (success)
            {
                Debug.Log("Rewarded ad success!");
                _ = gameManager.AddPipe();
                bottomHandler.OnAddPipe();
            }
            else
            {
                Debug.Log("Rewarded ad failed! - " + id);
            }
        }

        private void ShowTips()
        {
            List<WordCell> unshownCells = wordList.FindAll(x => !x.IsShown);

            if (unshownCells.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, unshownCells.Count);
                WordCell randomUnshownCell = unshownCells[randomIndex];
                randomUnshownCell.Show();
            }
            else
            {
                // Handle the case when all cells are already shown
                Debug.Log("All cells are already shown.");
            }
        }

        private void Highlight()
        {

        }

        private void ShowHomeScreen()
        {
            if (!canClick) return;
            gameManager.canClick = false;
            canClick = false;
            rootTween.tween.SetEase(Ease.OutExpo).PlayBackwards();
            mainScreen.Show();
            HideAsync();
        }

        private void SetBackground()
        {
            string bgName = PlayerPrefs.GetString("BG", "bg1");
            BackgroundItem bgItem = backgrounds.GetBackgroundByName(bgName);
            bodyIamge.color = bgItem.bodyColor;
            topBgImage.sprite = bgItem.background;
        }

        private async void HideAsync()
        {
            await Task.Delay(500);
            Hide();
        }

        public void ShowWinScreen()
        {
            bottomHandler.gameObject.SetActive(false);
            winPanel.gameObject.SetActive(true);

            string tips = UnityEngine.Random.Range(1, 3).ToString();
            string removes = UnityEngine.Random.Range(1, 3).ToString();
            string highlights = UnityEngine.Random.Range(1, 3).ToString();

            winPanel.OnShow(tips, removes, highlights);
        }

        public async Task InitWordLayouts(string firstWord, string secondWord)
        {
            await CreateWordCell(firstWord, firstWordLayout);
            if (!string.IsNullOrEmpty(secondWord))
            {
                await CreateWordCell(secondWord, secondWordLayout);
            }
            levelIsInitialized = true;
            debugText.SetText(firstWord + "\n" + secondWord);
        }

        public void ShowWord(string word)
        {
            int halfCount = wordList.Count / 2;
            var layoutWord = new StringBuilder(halfCount);

            for (int i = 0; i < halfCount; i++)
            {
                layoutWord.Append(wordList[i].GetCellLetter());
            }

            IEnumerable<WordCell> cellsToShow;

            if (word == layoutWord.ToString())
            {
                cellsToShow = wordList.Take(halfCount);
            }
            else
            {
                cellsToShow = wordList.Skip(halfCount);
            }

            foreach (var cell in cellsToShow)
            {
                cell.Show();
            }
        }

        private async Task CreateWordCell(string word, RectTransform parent)
        {
            for (int i = 0; i < word.Length; i++)
            {
                WordCell wordCell = Instantiate(wordCellPrefab, parent);
                wordCell.transform.localScale = Vector3.one;
                wordCell.SetLetter(word[i].ToString());
                wordList.Add(wordCell);
            }
            await Task.Yield();
        }

        public async override void Show()
        {
            base.Show();
            rootTween.DORestart();
            if (gameManager != null)
            {
                if (!gameManager.Initialized)
                {
                    await gameManager.InitializeAsync();
                }
            }
            if (!levelIsInitialized)
            {
                await InitWordLayouts(gameManager.FirstWord, gameManager.SecondWord);
            }
            gameManager.canClick = true;
            canClick = true;
            SetBackground();
            bottomHandler.OnShow();
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
