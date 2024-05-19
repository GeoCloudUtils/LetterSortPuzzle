using Lean.Gui;
using TMPro;
using UI.Base;
using UnityEditor;
using UnityEngine;
namespace UI
{
    public class UIMainScreenController : UIScreen
    {
        [SerializeField] private LeanButton playButton;
        [SerializeField] private LeanButton rateButton;
        [SerializeField] private LeanButton shareButton;
        [SerializeField] private LeanButton shopButton;
        [SerializeField] private LeanButton settingsButton;

        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text crystalsText;
        [SerializeField] private TMP_Text difficultyText;

        [SerializeField] private UIGameplayScreen gameplayScreen;

        private bool canClick = true;

        private void Start()
        {
            playButton.OnClick.AddListener(Play);
            rateButton.OnClick.AddListener(Rate);
            shareButton.OnClick.AddListener(Share);
            shopButton.OnClick.AddListener(OpenShop);
            settingsButton.OnClick.AddListener(OpenSettings);
            if (SessionState.GetInt("LEVEL_UP", 0) == 0)
            {
                Show();
            }
            else
            {
                Play();
            }
        }

        private void OpenSettings()
        {
            if (!canClick) return;
            if (gameplayScreen.IsActive)
            {
                return;
            }
        }

        private void OpenShop()
        {
            if (!canClick) return;
            if (gameplayScreen.IsActive)
            {
                return;
            }
        }

        private void Share()
        {
            if (!canClick) return;
            if (gameplayScreen.IsActive)
            {
                return;
            }
        }

        private void Rate()
        {
            if (!canClick) return;
            if (gameplayScreen.IsActive)
            {
                return;
            }
        }

        private void Play()
        {
            if (!canClick) return;
            if (gameplayScreen.IsActive)
            {
                return;
            }
            canClick = false;
            gameplayScreen.Show();
        }

        public override void Show()
        {
            base.Show();

            int level = PlayerPrefs.GetInt("LEVEL", 1);
            int crystals = PlayerPrefs.GetInt("CRYSTALS", 0);

            levelText.SetText(level.ToString());
            crystalsText.SetText(crystals.ToString());
            difficultyText.SetText("Difficulty: easy");
            canClick = true;
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
