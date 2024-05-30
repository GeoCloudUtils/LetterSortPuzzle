using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using UI.Base;
using UnityEngine;

public class UISettingsScreen : UIScreen
{
    [SerializeField] private LeanWindow leanWindow;
    [SerializeField] private LeanToggle soundToggle;
    [SerializeField] private LeanToggle musicToggle;
    [SerializeField] private LeanButton rateUsButton;
    [SerializeField] private LeanButton buyNoAdsButton;
    [SerializeField] private LeanButton buyMeACoffeeButton;

    [SerializeField] private LeanButton closeButton;

    private void Start()
    {
        // Initialize the toggles based on current settings
        soundToggle.On = GetVoiceSetting();
        musicToggle.On = GetSFXSetting();

        closeButton.OnClick.AddListener(Hide);

        leanWindow.OnOff.AddListener(Hide);

        // Add listeners to handle button clicks
        rateUsButton.OnClick.AddListener(OnRateUsButtonClicked);
        buyNoAdsButton.OnClick.AddListener(OnBuyNoAdsButtonClicked);
        buyMeACoffeeButton.OnClick.AddListener(BuyCoffee);
    }

    private void BuyCoffee()
    {
        //to do
    }

    public override void Hide()
    {
        leanWindow.TurnOff();
        GameDataManager.Instance.gameData.Sound =soundToggle.On;
         GameDataManager.Instance.gameData.Music = musicToggle.On;
        GameDataManager.Instance.SaveGame();
        base.Hide();
        // Additional code to run when hiding the settings screen
        Debug.Log("Settings screen hidden");
    }

    public override void Show()
    {
        base.Show();
        leanWindow.TurnOn();
        soundToggle.On = GameDataManager.Instance.gameData.Sound;
        musicToggle.On = GameDataManager.Instance.gameData.Music;
        Debug.Log("Settings screen shown");
    }

    private void OnRateUsButtonClicked()
    {
        // Open the app store or rating page
        Debug.Log("Rate Us button clicked");
        OpenRateUsPage();
    }

    private void OnBuyNoAdsButtonClicked()
    {
        // Initiate the in-app purchase process
        Debug.Log("Buy No Ads button clicked");
        BuyNoAds();
    }

    private bool GetVoiceSetting()
    {
        // Retrieve the voice setting from PlayerPrefs or another settings storage
        return PlayerPrefs.GetInt("VoiceSetting", 1) == 1;
    }

    private bool GetSFXSetting()
    {
        // Retrieve the SFX setting from PlayerPrefs or another settings storage
        return PlayerPrefs.GetInt("SFXSetting", 1) == 1;
    }

    private void OpenRateUsPage()
    {
        // Logic to open the rate us page in the app store
        Application.OpenURL("https://yourappstorelink.com");
    }

    private void BuyNoAds()
    {
        // Logic to initiate the in-app purchase process
        Debug.Log("Initiating in-app purchase for No Ads");
        // Your in-app purchase code here
    }
}
