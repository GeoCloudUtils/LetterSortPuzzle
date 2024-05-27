using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

public class UIShopScreen : UIScreen
{
    [SerializeField] private LeanButton turnsTab;
    [SerializeField] private LeanButton bgsTab;
    [SerializeField] private LeanButton coinsTab;

    [SerializeField] private LeanButton closeButton;

    [SerializeField] private ShopViewElement[] shopElements;

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color defaultColor;

    LeanButton[] tabs;

    private LeanButton lastTab = null;

    public event Action OnClose;

    private void Start()
    {
        turnsTab.OnClick.AddListener(ShowTurnsContent);
        bgsTab.OnClick.AddListener(ShowBgsContent);
        coinsTab.OnClick.AddListener(ShowCoinsContent);

        closeButton.OnClick.AddListener(Close);

        tabs = new LeanButton[] { turnsTab, bgsTab, coinsTab };
        foreach (ShopViewElement shopElem in shopElements)
        {
            shopElem.Init();
        }
        ShowTurnsContent();
    }

    private void Close()
    {
        OnClose?.Invoke();
        Hide();
    }

    private void ShowCoinsContent()
    {
        ActivateTab(coinsTab);
    }

    private void ShowBgsContent()
    {
        ActivateTab(bgsTab);
    }

    private void ShowTurnsContent()
    {
        ActivateTab(turnsTab);
    }

    private void ActivateTab(LeanButton target)
    {
        if (lastTab != null)
        {
            if (lastTab == target)
            {
                return;
            }
        }
        lastTab = target;
        ResetAllTabs();
        target.transform.Find("Cap").GetComponent<Image>().color = selectedColor;
        ShowShopElement(System.Array.IndexOf(tabs, target));
    }

    private void ShowShopElement(int index)
    {
        foreach (ShopViewElement elem in shopElements)
        {
            elem.gameObject.SetActive(false);
        }
        shopElements[index].gameObject.SetActive(true);
    }

    private void ResetAllTabs()
    {
        foreach (LeanButton tab in tabs)
        {
            tab.transform.Find("Cap").GetComponent<Image>().color = defaultColor;
        }
    }

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }
}
