using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    /// <summary>
    /// Eprubete logic
    /// </summary>
    public class ClickableEprubete : MonoBehaviour
    {
        [SerializeField] private Button eprubeteButton;

        [SerializeField] private List<Transform> cells;

        [SerializeField] private List<LetterBall> letterBalls;

        public event Action<LetterBall, ClickableEprubete> OnSelect;

        public event Action<ClickableEprubete> DispatchMoveComplete;

        private bool isLocked = false;

        public bool monitor = false;

        private string firstWord;
        private string secondWord;

        private void Awake()
        {
            eprubeteButton.onClick.AddListener(OnClick);
        }

        /// <summary>
        /// Handles eprubete click callback
        /// </summary>
        private void OnClick()
        {
            if (isLocked) return;
            letterBalls ??= new List<LetterBall>();
            LetterBall selectedBall = letterBalls.Count > 0 ? letterBalls.Last() : null;
            OnSelect?.Invoke(selectedBall, this);
        }

        /// <summary>
        /// Handles letter remove callback
        /// </summary>
        /// <param name="letter"></param>
        public void RemoveLetter(LetterBall letter)
        {
            letterBalls.Remove(letter);
        }

        /// <summary>
        /// Add cell
        /// </summary>
        /// <param name="cell"></param>
        public void AddCell(Transform cell)
        {
            cells ??= new List<Transform>();
            cells.Add(cell);
        }

        public void SaveWords(string firstWord, string secondWord)
        {
            this.firstWord = firstWord;
            this.secondWord = secondWord;
        }

        private void CheckPlaces()
        {
            if (!monitor)
            {
                return;
            }
            foreach (LetterBall letterBall in letterBalls)
            {
                letterBall.SetActive(false);
            }
            for (int i = firstWord.Length - 1; i >= 0; i--)
            {
                for (int j = letterBalls.Count - 1; j >= 0; j--)
                {
                    if (letterBalls[j].Letter == firstWord[i].ToString())
                    {
                        letterBalls[j].SetActive(true);
                    }
                }
            }
        }

        public bool CheckLetterPositions(string word, List<char> letters)
        {
            // Check if the length of the word matches the number of provided letters
            if (word.Length != letters.Count)
            {
                Debug.LogError("Number of letters provided does not match the length of the word.");
                return false;
            }

            // Iterate through each letter and compare it with the corresponding letter in the word
            for (int i = 0; i < word.Length; i++)
            {
                if (word[i] != letters[i])
                {
                    // If any letter doesn't match, return false
                    return false;
                }
            }

            // If all letters match their corresponding positions, return true
            return true;
        }

        /// <summary>
        /// Adding letter to eprubete cell
        /// </summary>
        /// <param name="letterBall"></param>
        public void AddLetterBall(LetterBall letterBall)
        {
            letterBall.SetActive(false);
            for (int i = cells.Count - 1; i >= 0; i--)
            {
                if (cells[i].childCount == 0)
                {
                    letterBall.transform.position = cells[i].position;
                    letterBall.transform.SetParent(cells[i]);
                    letterBall.transform.localScale = Vector3.one;
                    letterBalls.Add(letterBall);
                    return;
                }
            }

            Debug.LogWarning("No empty cell found to move the letter ball to.");
        }

        /// <summary>
        /// Ball move
        /// </summary>
        /// <param name="letterBall"></param>
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
                letterBall.transform.DOLocalMove(Vector3.zero, 0.25f).SetEase(Ease.OutExpo).OnComplete(() => DispatchMoveComplete?.Invoke(this));
                letterBalls.Add(letterBall);
                CheckPlaces();
            });
        }

        public void Lock()
        {
            isLocked = true;
        }

        /// <summary>
        /// Check's if eprubete is full
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Check eprubete vertical string
        /// </summary>
        /// <returns></returns>
        public string GetVerticalString()
        {
            string verticalString = "";
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].GetComponentInChildren<TMP_Text>(true))
                {
                    string s = cells[i].transform.GetComponentInChildren<TMP_Text>().text;
                    verticalString += s;
                }
            }
            return verticalString;
        }

        /// <summary>
        /// Get top cell of eprubete
        /// </summary>
        /// <returns></returns>
        public Transform GetUpCell()
        {
            return cells[0].transform;
        }
    }
}
