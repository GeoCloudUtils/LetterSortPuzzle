using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using UI.Base;
using UnityEngine;

public class UISettingsScreen : UIScreen
{
    [SerializeField] private LeanWindow leanWindow;
    [SerializeField] private LeanToggle voiceToggle;
    [SerializeField] private LeanToggle sfxToggle;
    [SerializeField] private LeanButton rateUsButton;
    [SerializeField] private LeanButton buyNoAdsButton;
    [SerializeField] private LeanButton buyMeACoffeeButton;

    [SerializeField] private LeanButton closeButton;

    private void Start()
    {
        // Initialize the toggles based on current settings
        voiceToggle.On = GetVoiceSetting();
        sfxToggle.On = GetSFXSetting();

        closeButton.OnClick.AddListener(Hide);

        // Add listeners to handle toggle changes
        voiceToggle.OnOn.AddListener(delegate { OnVoiceToggleChanged(true); });
        voiceToggle.OnOff.AddListener(delegate { OnVoiceToggleChanged(false); });

        sfxToggle.OnOn.AddListener(delegate { OnSFXToggleChanged(true); });
        sfxToggle.OnOff.AddListener(delegate { OnSFXToggleChanged(false); });

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
        base.Hide();
        // Additional code to run when hiding the settings screen
        Debug.Log("Settings screen hidden");
    }

    public override void Show()
    {
        base.Show();
        leanWindow.TurnOn();
        // Additional code to run when showing the settings screen
        Debug.Log("Settings screen shown");
    }

    private void OnVoiceToggleChanged(bool isOn)
    {
        SetVoiceSetting(isOn);
        Debug.Log($"Voice setting changed: {isOn}");
    }

    private void OnSFXToggleChanged(bool isOn)
    {
        SetSFXSetting(isOn);
        Debug.Log($"SFX setting changed: {isOn}");
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

    private void SetVoiceSetting(bool isOn)
    {
        // Save the voice setting to PlayerPrefs or another settings storage
        PlayerPrefs.SetInt("VoiceSetting", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SetSFXSetting(bool isOn)
    {
        // Save the SFX setting to PlayerPrefs or another settings storage
        PlayerPrefs.SetInt("SFXSetting", isOn ? 1 : 0);
        PlayerPrefs.Save();
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
