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

    public event Action OnTipsClick;
    public event Action OnHighlightClick;
    public event Action OnHomeClick;
    public event Action OnRemoveClick;
    public event Action OnAddClick;

    private int addedPipes = 0;

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
        addedPipes = 0;
    }

    public void SetState(LeanButton target, Func<int> getCount)
    {
        TMP_Text countText = target.transform.Find("Cap").GetComponentInChildren<TMP_Text>();
        countText.SetText(getCount().ToString());
        countText.color = getCount() > 0 ? Color.white : Color.red;
    }

    private void AddPipe()
    {
        addPipeButton.gameObject.SetActive(addedPipes < 1);
        OnAddClick?.Invoke();
        addedPipes++;
    }

    public void OnShow()
    {
        SetState(removeLetterButton, () => GameDataManager.Instance.gameData.Removes);
        SetState(tipsButton, () => GameDataManager.Instance.gameData.TipsCount);
        SetState(highlightButton, () => GameDataManager.Instance.gameData.Highlights);
    }

    private void RemoveLetter()
    {
        OnRemoveClick?.Invoke();
    }

    private void GoToHomeScreen()
    {
        OnHomeClick?.Invoke();
        Debug.Log("Navigating to home screen");
    }

    private void ExecuteAction(Func<int> getCount, Action setCount, Action onActionClick, LeanButton target, string actionName)
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
        TMP_Text countText = target.transform.Find("Cap").GetComponentInChildren<TMP_Text>();
        if (countText != null)
        {
            countText.SetText(getCount().ToString());
            SetState(target, () => getCount());
        }
    }

    private void Highlight()
    {
        ExecuteAction(
            () => GameDataManager.Instance.gameData.Highlights,
            () => GameDataManager.Instance.gameData.Highlights--,
            OnHighlightClick,
            highlightButton,
            "Highlight"
        );
    }

    private void ShowTips()
    {
        ExecuteAction(
            () => GameDataManager.Instance.gameData.TipsCount,
            () => GameDataManager.Instance.gameData.TipsCount--,
            OnTipsClick,
            tipsButton,
            "Tips"
        );
    }
}
