using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BottomButtonsHandler : MonoBehaviour
{
    [SerializeField] private LeanButton homeButton;
    [SerializeField] private LeanButton stepBackButton;
    [SerializeField] private LeanButton tipsButton;

    [SerializeField] private TMP_Text stepBackCountText;
    [SerializeField] private TMP_Text tipsCountText;

    public event Action OnTipsClick;
    public event Action OnStepBackClick;
    public event Action OnHomeClick;

    private void Start()
    {
        if (homeButton == null || stepBackButton == null || tipsButton == null)
        {
            Debug.LogError("One or more buttons are not assigned in the Inspector.");
            return;
        }

        homeButton.OnClick.AddListener(OnHomeButtonClick);
        stepBackButton.OnClick.AddListener(OnStepBackButtonClick);
        tipsButton.OnClick.AddListener(OnTipsButtonClick);
    }

    public void OnShow()
    {
        tipsCountText.SetText(GameDataManager.Instance.gameData.TipsCount.ToString());
        stepBackCountText.SetText(GameDataManager.Instance.gameData.StepBackPoints.ToString());
    }

    private void OnHomeButtonClick()
    {
        Debug.Log("Home button clicked");
        GoToHomeScreen();
    }

    private void OnStepBackButtonClick()
    {
        Debug.Log("Step Back button clicked");
        StepBack();
    }

    private void OnTipsButtonClick()
    {
        Debug.Log("Tips button clicked");
        ShowTips();
    }

    private void GoToHomeScreen()
    {
        Debug.Log("Navigating to home screen");
    }

    private void StepBack()
    {
        if (GameDataManager.Instance.gameData.StepBackPoints > 0)
        {
            GameDataManager.Instance.gameData.StepBackPoints--;
            GameDataManager.Instance.SaveGame();
            OnStepBackClick?.Invoke();
            Debug.Log("Performed a step back. Remaining step back points: " + GameDataManager.Instance.gameData.StepBackPoints);
        }
        else
        {
            Debug.Log("No step back points available.");
        }
    }

    private void ShowTips()
    {
        if (GameDataManager.Instance.gameData.TipsCount > 0)
        {
            GameDataManager.Instance.gameData.TipsCount--;
            GameDataManager.Instance.SaveGame();
            OnTipsClick?.Invoke();
            Debug.Log("Showing a tip. Remaining tips count: " + GameDataManager.Instance.gameData.TipsCount);
        }
        else
        {
            Debug.Log("No tips available.");
        }
    }
}
