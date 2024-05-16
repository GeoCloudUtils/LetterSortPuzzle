using Gameplay;
using System.Collections;
using System.Collections.Generic;
using UI.Base;
using UnityEngine;

namespace UI
{
    public class UIGameplayScreen : UIScreen
    {
        [SerializeField] private GameManager gameManager;

        public override void Show()
        {
            base.Show();
            if (gameManager != null)
            {
                if (!gameManager.Initialized)
                {
                    gameManager.Initialize();
                }
            }
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
