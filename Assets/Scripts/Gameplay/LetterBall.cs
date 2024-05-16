using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class LetterBall : MonoBehaviour
    {
        [SerializeField] private TMP_Text letterText;
        [SerializeField] private RectTransform rectTransform;

        [SerializeField] private ClickableEprubete lastEprupete;

        private bool isMoving = false;

        public bool IsMoving { get => isMoving; private set => isMoving = value; }

        public void SetText(string text)
        {
            letterText.text = text;
        }

        public void SetSize(float size)
        {
            rectTransform.sizeDelta = new Vector2(size, size);
        }

        public void MoveUp()
        {
            if (IsMoving)
            {
                return;
            }
            IsMoving = true;
            Vector3 p = new Vector3(lastEprupete.transform.position.x, lastEprupete.GetUpCell().position.y + 200f, 0);
            rectTransform.DOMoveY(p.y, 0.25f).SetEase(Ease.OutExpo).OnComplete(() =>
            {
                IsMoving = false;
            });
        }

        public void DetachParent(ClickableEprubete eprubete)
        {
            transform.SetParent(eprubete.transform);
            lastEprupete = eprubete;
        }
    }
}

