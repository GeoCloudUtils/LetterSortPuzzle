using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// Letter ball logic
    /// </summary>
    public class LetterBall : MonoBehaviour
    {
        [SerializeField] private TMP_Text letterText;
        [SerializeField] private RectTransform rectTransform;

        [SerializeField] private ClickableEprubete lastEprupete;

        private bool isMoving = false;

        public bool IsMoving { get => isMoving; private set => isMoving = value; }

        /// <summary>
        /// Set letter
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            letterText.text = text;
        }

        /// <summary>
        /// Set ball size delta
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(float size)
        {
            rectTransform.sizeDelta = new Vector2(size, size);
        }

        /// <summary>
        /// Moving ball on select
        /// </summary>
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

        /// <summary>
        /// Detach from parent
        /// </summary>
        /// <param name="eprubete"></param>
        public void DetachParent(ClickableEprubete eprubete)
        {
            transform.SetParent(eprubete.transform);
            lastEprupete = eprubete;
        }
    }
}

