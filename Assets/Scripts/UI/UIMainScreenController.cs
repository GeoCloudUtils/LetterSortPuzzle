using Lean.Gui;
using System;
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
        [SerializeField] private TMP_Text coinsText;
        [SerializeField] private TMP_Text difficultyText;

        [SerializeField] private UIGameplayScreen gameplayScreen;
        [SerializeField] private UIShopScreen shopScreen;
        [SerializeField] private UISettingsScreen settingScreen;

        private bool canClick = true;

        public int coins;

        private void Start()
        {
            playButton.OnClick.AddListener(Play);
            rateButton.OnClick.AddListener(Rate);
            shareButton.OnClick.AddListener(Share);
            shopButton.OnClick.AddListener(OpenShop);
            settingsButton.OnClick.AddListener(OpenSettings);
            shopScreen.OnClose += HandleShopScreenClose;

            PlayerPrefs.SetInt("COINS", coins);
            if (PlayerPrefs.GetInt("LEVEL_UP", 0) == 0)
            {
                Show();
            }
            else
            {
                Play();
            }
        }

        private void HandleShopScreenClose()
        {
            Show();
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetInt("LEVEL_UP", 0);
        }

        private void OpenSettings()
        {
            if (!canClick) return;
            if (gameplayScreen.IsActive)
            {
                return;
            }
            settingScreen.Show();
        }

        private void OpenShop()
        {
            if (!canClick) return;
            if (gameplayScreen.IsActive)
            {
                return;
            }
            shopScreen.Show();
            Hide();
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
            int coins = PlayerPrefs.GetInt("COINS", 0);

            levelText.SetText("Lvl:" + level.ToString());
            coinsText.SetText(coins.ToString());
            difficultyText.SetText("Difficulty: easy");
            canClick = true;
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
