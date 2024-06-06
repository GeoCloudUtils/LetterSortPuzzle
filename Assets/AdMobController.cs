using GoogleMobileAds.Api;
using System;
using UnityEngine;

[Serializable]
public class AdInfo
{
    public string bannedId;
    public string interstitialId;
    public string rewardedId;
}

public enum AdType
{
    Banner = 0,
    Interstitial = 1,
    Rewarded = 2
}

public class AdMobController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private bool _testMode = true;
    [Header("Android")]
    [SerializeField] private AdInfo _testInfoAndroid;
    [SerializeField] private AdInfo _prodInfoAndroid;
    [Header("iOS")]
    [SerializeField] private AdInfo _testInfoIOS;
    [SerializeField] private AdInfo _prodInfoIOS;
    private bool _initialized;
    private BannerView _bannerAd;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
            Debug.Log("[AdmobImpl] AdMob Initialized.");
            _initialized = true;
        });
        ShowBannerAd(BannerShown);
    }

    private void BannerShown(bool success, string id)
    {
        Debug.Log("Banner initialized!");
    }

    /// <summary>
    /// Show an Ad.
    /// </summary>
    /// <param name="type">Ad type.</param>
    /// <param name="statusCallback">Will return TRUE: Interstitial - when finished. Rewarded - if not skipped. Banner - if shown. </param>
    public void ShowAd(AdType type, Action<bool, string> statusCallback)
    {
        if (!_initialized)
        {
            string errorMsg = "AdMob is not initialized yet, Ad not loaded.";
            Debug.LogError($"[AdmobImpl] {errorMsg}");
            statusCallback?.Invoke(false, errorMsg);
            return;
        }

        switch (type)
        {
            case AdType.Banner: ShowBannerAd(statusCallback); break;
            case AdType.Interstitial: ShowInterstitialAd(statusCallback); break;
            case AdType.Rewarded: ShowRewardedAd(statusCallback); break;
            default:
                throw new Exception("Unsupported AdType!");
        }
    }

    /// <summary>
    /// Show banner ad. Status will be SUCCESS only if the banner shown.
    /// </summary>
    private void ShowBannerAd(Action<bool, string> statusCallback)
    {
        Debug.Log("[AdmobImpl] Loading banner...");

        string adId = GetAdId(AdType.Banner);

        // If we already have a banner, destroy the old one.
        DestroyAd(AdType.Banner);

        // Create a 320x50 banner at top of the screen
        _bannerAd = new BannerView(adId, AdSize.Banner, AdPosition.Bottom);

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        _bannerAd.LoadAd(adRequest);

        // Subscribe to events.
        // Raised when an ad is loaded into the banner view.
        _bannerAd.OnBannerAdLoaded += () =>
        {
            Debug.Log($"[AdmobImpl] Banner loaded and shown. Response: {_bannerAd.GetResponseInfo()}");
            statusCallback?.Invoke(true, null);
        };
        // Raised when an ad fails to load into the banner view.
        _bannerAd.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError($"[AdmobImpl] Banner NOT loaded. Error: {error}");
            statusCallback?.Invoke(false, error.ToString());
        };
        // Raised when the ad is estimated to have earned money.
        _bannerAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"[AdmobImpl] Banner paid. Sum:{adValue.Value}; Currency:{adValue.CurrencyCode}");
        };
    }

    /// <summary>
    /// Show interstitial ad. Status will be SUCCESS only when the ad finished.
    /// </summary>
    private void ShowInterstitialAd(Action<bool, string> statusCallback)
    {
        Debug.Log("[AdmobImpl] Loading InterstitialAd...");

        // Clean up the old ad before loading a new one.
        DestroyAd(AdType.Interstitial);

        string adId = GetAdId(AdType.Interstitial);

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(adId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError($"[AdmobImpl] InterstitialAd NOT loaded. Error: {error}");
                    statusCallback?.Invoke(false, error.ToString());
                    return;
                }

                _interstitialAd = ad;

                Debug.Log($"[AdmobImpl] InterstitialAd loaded. Response: {ad.GetResponseInfo()}");

                // Raised when the ad is estimated to have earned money.
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    Debug.Log($"[AdmobImpl] InterstitialAd paid. Sum:{adValue.Value}; Currency:{adValue.CurrencyCode}");
                };
                // Raised when an ad opened full screen content.
                ad.OnAdFullScreenContentOpened += () =>
                {
                    Debug.Log($"[AdmobImpl] InterstitialAd shown.");
                    statusCallback?.Invoke(true, null);
                };
                // Raised when the ad closed full screen content.
                ad.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log($"[AdmobImpl] InterstitialAd hidden.");
                };
                // Raised when the ad failed to open full screen content.
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    string errorMsg = $"InterstitialAd NOT shown. Reason: Failed to open Full Screen. Error:{error}";
                    Debug.LogError($"[AdmobImpl] {errorMsg}");
                    statusCallback?.Invoke(false, errorMsg);
                };

                bool isNull = ad == null;
                bool isReady = ad != null ? ad.CanShowAd() : false;
                if (!isNull && isReady)
                {
                    Debug.Log($"[AdmobImpl] InterstitialAd show started...");
                    ad.Show();
                }
                else
                {
                    string errorMsg = $"InterstitialAd NOT shown. Reason: Null or Not Ready yet. IsNull:{isNull}; IsReady:{isReady}";
                    Debug.LogError($"[AdmobImpl] {errorMsg}");
                    statusCallback?.Invoke(false, errorMsg);
                }

            });
    }

    /// <summary>
    /// Show rewarded ad. Status will be SUCCESS only if the user fully watched the Ad and received the reward.
    /// </summary>
    private void ShowRewardedAd(Action<bool, string> statusCallback)
    {
        // Clean up the old ad before loading a new one.
        DestroyAd(AdType.Rewarded);

        Debug.Log("[AdmobImpl] Loading RewardedAd...");

        string adId = GetAdId(AdType.Rewarded);

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(adId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError($"[AdmobImpl] RewardedAd NOT loaded. Error: {error}");
                    statusCallback?.Invoke(false, error.ToString());
                    return;
                }

                Debug.Log($"[AdmobImpl] RewardedAd loaded. Response: {ad.GetResponseInfo()}");

                _rewardedAd = ad;

                // Raised when the ad is estimated to have earned money.
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    Debug.Log($"[AdmobImpl] RewardedAd paid. Sum:{adValue.Value}; Currency:{adValue.CurrencyCode}");
                };
                // Raised when the ad failed to open full screen content.
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    string errorMsg = $"RewardedAd NOT shown. Reason: Failed to open Full Screen. Error:{error}";
                    Debug.LogError($"[AdmobImpl] {errorMsg}");
                    statusCallback?.Invoke(false, errorMsg);
                };

                bool isNull = ad == null;
                bool isReady = ad != null ? ad.CanShowAd() : false;
                if (!isNull && isReady)
                {
                    Debug.Log($"[AdmobImpl] RewardedAd show started...");
                    ad.Show((Reward reward) =>
                    {
                        Debug.Log($"[AdmobImpl] RewardedAd finished, user rewarded. Currency: {reward.Type}; Amount:{reward.Amount}");
                        statusCallback?.Invoke(true, null);
                    });
                }
                else
                {
                    string errorMsg = $"RewardedAd NOT shown. Reason: Null or Not Ready yet. IsNull:{isNull}; IsReady:{isReady}";
                    Debug.LogError($"[AdmobImpl] {errorMsg}");
                    statusCallback?.Invoke(false, errorMsg);
                }
            });
    }


    /// <summary>
    /// Get Ad Id by its type and needed platform.
    /// </summary>
    private string GetAdId(AdType type)
    {
        AdInfo info = null;
#if UNITY_ANDROID
        info = _testMode ? _testInfoAndroid : _prodInfoAndroid;
#elif UNITY_IOS
        info = _testMode? _testInfoIOS : _prodInfoIOS;
#else
        throw new Exception("Unsupported platform!");
#endif
        switch (type)
        {
            case AdType.Banner: return info.bannedId;
            case AdType.Interstitial: return info.interstitialId;
            case AdType.Rewarded: return info.rewardedId;
            default:
                throw new Exception("Unsupported AdType!");
        }
    }

    /// <summary>
    /// Destroys the banner.
    /// </summary>
    private void DestroyAd(AdType type)
    {
        switch (type)
        {
            case AdType.Banner:
                if (_bannerAd != null)
                {
                    _bannerAd.Destroy();
                    _bannerAd = null;
                    Debug.Log("[AdmobImpl] Banner destroyed.");
                }
                break;
            case AdType.Interstitial:
                if (_interstitialAd != null)
                {
                    _interstitialAd.Destroy();
                    _interstitialAd = null;
                }
                break;
            case AdType.Rewarded:
                if (_rewardedAd != null)
                {
                    _rewardedAd.Destroy();
                    _rewardedAd = null;
                }
                break;
            default:
                throw new Exception("Unsupported AdType.");
        }
    }
}
