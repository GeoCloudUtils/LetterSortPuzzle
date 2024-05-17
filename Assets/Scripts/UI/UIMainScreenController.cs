using Lean.Gui;
using TMPro;
using UI.Base;
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

        private void Start()
        {
            playButton.OnClick.AddListener(Play);
            rateButton.OnClick.AddListener(Rate);
            shareButton.OnClick.AddListener(Share);
            shopButton.OnClick.AddListener(OpenShop);
            settingsButton.OnClick.AddListener(OpenSettings);

            Show();
        }

        private void OpenSettings()
        {

        }

        private void OpenShop()
        {

        }

        private void Share()
        {

        }

        private void Rate()
        {

        }

        private void Play()
        {
            gameplayScreen.Show();
            this.Hide();
        }

        public override void Show()
        {
            base.Show();

            int level = PlayerPrefs.GetInt("LEVEL", 1);
            int crystals = PlayerPrefs.GetInt("CRYSTALS", 0);

            levelText.SetText(level.ToString());
            crystalsText.SetText(crystals.ToString());
            difficultyText.SetText("Difficulty: easy");
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
