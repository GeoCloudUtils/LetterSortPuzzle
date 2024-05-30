using Lean.Gui;
using System;
using TMPro;
using UnityEngine;

public class BottomButtonsHandler : MonoBehaviour
{
    [SerializeField] private LeanButton homeButton;
    [SerializeField] private LeanButton removeLetterButton;
    [SerializeField] private LeanButton tipsButton;
    [SerializeField] private LeanButton highlightButton;
    [SerializeField] private LeanButton addPipeButton;

    [SerializeField] private TMP_Text stepBackCountText;
    [SerializeField] private TMP_Text tipsCountText;

    public event Action OnTipsClick;
    public event Action OnHighlightClick;
    public event Action OnHomeClick;
    public event Action OnRemoveClick;
    public event Action OnAddClick;

    private void Start()
    {
        if (homeButton == null || removeLetterButton == null || tipsButton == null)
        {
            Debug.LogError("One or more buttons are not assigned in the Inspector.");
            return;
        }

        homeButton.OnClick.AddListener(GoToHomeScreen);
        removeLetterButton.OnClick.AddListener(RemoveLetter);
        tipsButton.OnClick.AddListener(ShowTips);
        highlightButton.OnClick.AddListener(Highlight);
        addPipeButton.OnClick.AddListener(AddPipe);
    }

    private void AddPipe()
    {
        OnAddClick?.Invoke();
    }

    public void OnShow()
    {
        tipsCountText.SetText(GameDataManager.Instance.gameData.TipsCount.ToString());
        stepBackCountText.SetText(GameDataManager.Instance.gameData.HighlightsCount.ToString());
    }

    private void RemoveLetter()
    {
        OnRemoveClick?.Invoke();
    }

    private void GoToHomeScreen()
    {
        Debug.Log("Navigating to home screen");
    }

    private void ExecuteAction(Func<int> getCount, Action setCount, Action onActionClick, string actionName)
    {
        if (getCount() > 0)
        {
            setCount();
            GameDataManager.Instance.SaveGame();
            onActionClick?.Invoke();
            Debug.Log($"Performed {actionName}. Remaining {actionName.ToLower()} count: {getCount()}");
        }
        else
        {
            Debug.Log($"No {actionName.ToLower()} available.");
        }
    }

    private void Highlight()
    {
        ExecuteAction(
            () => GameDataManager.Instance.gameData.HighlightsCount,
            () => GameDataManager.Instance.gameData.HighlightsCount--,
            OnHighlightClick,
            "Highlight"
        );
    }

    private void ShowTips()
    {
        ExecuteAction(
            () => GameDataManager.Instance.gameData.TipsCount,
            () => GameDataManager.Instance.gameData.TipsCount--,
            OnTipsClick,
            "Tips"
        );
    }
}
