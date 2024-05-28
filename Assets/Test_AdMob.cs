using UnityEngine;
using UnityEngine.UI;

public class Test_AdMob : MonoBehaviour
{
    [SerializeField] private AdMobController _controller;
    [SerializeField] private Button _showBannerAd;
    [SerializeField] private Button _showInterstitialAd;
    [SerializeField] private Button _showRewardedAd;

    private void Start()
    {
        _showBannerAd.onClick.AddListener(OnShowBannerAdClicked);
        _showInterstitialAd.onClick.AddListener(OnShowInterstitialAdClicked);
        _showRewardedAd.onClick.AddListener(OnShowRewardedAdClicked);
    }

    private void OnShowRewardedAdClicked()
    {
        _controller.ShowAd(AdType.Rewarded, (bool success, string error) =>
        {
            Debug.Log($"[Test_AdMob] Rewarded Ad finished. Success:{success}; Error:{error}");
        });
    }

    private void OnShowInterstitialAdClicked()
    {
        _controller.ShowAd(AdType.Interstitial, (bool success, string error) =>
        {
            Debug.Log($"[Test_AdMob] Interstitial Ad finished. Success:{success}; Error:{error}");
        });
    }

    private void OnShowBannerAdClicked()
    {
        _controller.ShowAd(AdType.Banner, (bool success, string error) =>
        {
            Debug.Log($"[Test_AdMob] Banner Ad finished. Success:{success}; Error:{error}");
        });
    }
}
