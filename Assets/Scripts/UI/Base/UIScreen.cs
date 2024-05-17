using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Base
{
    public abstract class UIScreen : MonoBehaviour
    {
        [SerializeField] private RectTransform root;
        public bool IsActive = false;
        public virtual void Show()
        {
            root.gameObject.SetActive(true);
            IsActive = true;
        }

        public virtual void Hide()
        {
            root.gameObject.SetActive(false);
            IsActive = false;
        }
    }
}
