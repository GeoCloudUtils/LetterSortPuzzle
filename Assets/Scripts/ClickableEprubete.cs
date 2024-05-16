using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ClickableEprubete : MonoBehaviour
{
    [SerializeField] private Button eprubeteButton;

    [SerializeField] private List<Transform> cells;

    [SerializeField] private List<LetterBall> letterBalls;

    public event Action<LetterBall, ClickableEprubete> OnSelect;

    public event Action DispatchMoveComplete;

    private void Awake()
    {
        eprubeteButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        letterBalls ??= new List<LetterBall>();
        LetterBall selectedBall = letterBalls.Count > 0 ? letterBalls.Last() : null;
        OnSelect?.Invoke(selectedBall, this);
    }

    public void RemoveLetter(LetterBall letter)
    {
        letterBalls.Remove(letter);
    }

    public void AddCell(Transform cell)
    {
        cells ??= new List<Transform>();
        cells.Add(cell);
    }

    public void AddLetterBall(LetterBall letterBall)
    {
        for (int i = cells.Count - 1; i >= 0; i--)
        {
            if (cells[i].childCount == 0)
            {
                letterBall.transform.position = cells[i].position;
                letterBall.transform.SetParent(cells[i]);

                letterBalls.Add(letterBall);
                return;
            }
        }
        Debug.LogWarning("No empty cell found to move the letter ball to.");
    }

    public void DoLetterPath(LetterBall letterBall)
    {
        Transform targetCell = null;
        for (int i = cells.Count - 1; i >= 0; i--)
        {
            if (cells[i].childCount == 0)
            {
                targetCell = cells[i];
                break;
            }
        }
        float eprubeteX = GetUpCell().position.x;
        letterBall.transform.DOMoveX(eprubeteX, 0.25f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            letterBall.transform.SetParent(targetCell);
            letterBall.transform.DOLocalMove(Vector3.zero, 0.25f).SetEase(Ease.OutExpo).OnComplete(() => DispatchMoveComplete?.Invoke());
            letterBalls.Add(letterBall);
        });
    }

    public bool IsFull()
    {
        for (int i = cells.Count - 1; i >= 0; i--)
        {
            if (cells[i].childCount == 0)
            {
                return false;
            }
        }
        return true;
    }

    public Transform GetUpCell()
    {
        return cells[0].transform;
    }
}
