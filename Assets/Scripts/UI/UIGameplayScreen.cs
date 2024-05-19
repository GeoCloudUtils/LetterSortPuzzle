using DG.Tweening;
using Gameplay;
using Lean.Gui;
using System.Threading.Tasks;
using UI.Base;
using UnityEngine;

namespace UI
{
    public class UIGameplayScreen : UIScreen
    {
        [SerializeField] private LeanButton homeButton;
        [SerializeField] private LeanButton stepBackButton;
        [SerializeField] private LeanButton tipsButton;

        [SerializeField] private UIMainScreenController mainScreen;

        [SerializeField] private GameManager gameManager;

        [SerializeField] private RectTransform firstWordLayout;
        [SerializeField] private RectTransform secondWordLayout;

        [SerializeField] private WordCell wordCellPrefab;

        [SerializeField] private DOTweenAnimation rootTween;

        private bool levelIsInitialized = false;
        private bool canClick = true;

        private void Start()
        {
            homeButton.OnClick.AddListener(Home);
            stepBackButton.OnClick.AddListener(GoOneStepBack);
            tipsButton.OnClick.AddListener(ShowTips);
        }

        private void ShowTips()
        {

        }

        private void GoOneStepBack()
        {

        }

        private void Home()
        {
            if (!canClick) return;
            gameManager.canClick = false;
            canClick = false;
            rootTween.tween.SetEase(Ease.OutExpo).PlayBackwards();
            mainScreen.Show();
            HideAsync();
        }

        private async void HideAsync()
        {
            await Task.Delay(500);
            Hide();
        }

        public async Task InitWordLayouts(string firstWord, string secondWord)
        {
            await CreateWordCell(firstWord, firstWordLayout);
            await CreateWordCell(secondWord, secondWordLayout);
            levelIsInitialized = true;
        }

        private async Task CreateWordCell(string word, RectTransform parent)
        {
            for (int i = 0; i < word.Length; i++)
            {
                WordCell wordCell = Instantiate(wordCellPrefab, parent);
                wordCell.transform.localScale = Vector3.one;
                wordCell.SetLetter(word[i].ToString(), false); //UnityEngine.Random.value > 0.5f);
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
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
