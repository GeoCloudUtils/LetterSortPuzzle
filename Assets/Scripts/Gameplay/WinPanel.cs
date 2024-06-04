using Lean.Gui;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class WinPanel : MonoBehaviour
{
    [SerializeField] private float waitDuration = 2.0f;

    [SerializeField] private AdMobController adMobController;

    [SerializeField] private LeanButton getDoubleButton;
    [SerializeField] private LeanButton nextButton;

    [SerializeField] private TMP_Text tipsCountText;
    [SerializeField] private TMP_Text removesCountText;
    [SerializeField] private TMP_Text highlightCountText;
    [SerializeField] private TMP_Text timeText;

    public event Action OnMoveNext;

    private void Start()
    {
        getDoubleButton.OnClick.AddListener(GetDoubleRewards);
        nextButton.OnClick.AddListener(MoveNext);
    }

    private void MoveNext()
    {
        SaveData();
        OnMoveNext?.Invoke();
    }

    private void GetDoubleRewards()
    {
        adMobController.ShowAd(AdType.Rewarded, OnRewardedAdSucceed);
    }

    private void OnRewardedAdSucceed(bool succes, string id)
    {
        if (succes)
        {
            Debug.Log("Get X2 reward!");

            string tips = (int.Parse(tipsCountText.text) + 1).ToString();
            string removes = (int.Parse(removesCountText.text) + 1).ToString();
            string highlights = (int.Parse(highlightCountText.text) + 1).ToString();

            OnShow(tips, removes, highlights);
            StartCoroutine(OnContinue());
        }
        else
        {
            Debug.LogFormat("Something went wrong! Failed to get X2 reward! Succes: {0} ID: {1}", succes, id);
        }
    }

    public void OnShow(string tips, string removes, string highlights, string time = null)
    {
        tipsCountText.text = $"+{tips}";
        removesCountText.text = $"+{removes}";
        highlightCountText.text = $"+{highlights}";
        if (!string.IsNullOrEmpty(time))
        {
            timeText.text = time;
        }
    }

    private IEnumerator OnContinue()
    {
        yield return new WaitForSeconds(waitDuration);
        SaveData();
        OnMoveNext?.Invoke();
    }

    private void SaveData()
    {
        GameData data = GameDataManager.Instance.gameData;
        data.TipsCount += int.Parse(tipsCountText.text);
        data.Highlights += int.Parse(highlightCountText.text);
        data.WordsOpen += int.Parse(removesCountText.text);
        data.Level += 1;
        GameDataManager.Instance.SaveGame();
    }
}
