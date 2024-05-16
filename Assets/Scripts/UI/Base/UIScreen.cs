using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Base
{
    public abstract class UIScreen : MonoBehaviour
    {
        [SerializeField] private RectTransform root;

        public virtual void Show()
        {
            root.gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            root.gameObject.SetActive(false);
        }
    }
}
